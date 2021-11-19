using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using AngleSharp;
using AngleSharp.Html.Dom;
using Database;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.Clients;
using Notifications.Errors;
using Notifications.Factories;
using Notifications.Repositories;
using Remora.Results;

namespace Notifications.Managers
{
	public class SubscriptionManager
	{
		private readonly ILogger<SubscriptionManager> _logger;
		private readonly IPubSubClient _pubSubClient;
		private readonly IYoutubeRepositoryFactory _repositoryFactory;
		private readonly IChannelInfoRepository _channelInfoRepository;
		private readonly IDateTimeRepository _dateTimeRepository;

		public Timer ResubTimer { get; }
		public Dictionary<string, DateTime> ResubscribeTimes { get; private set; } = new Dictionary<string, DateTime>();

		public SubscriptionManager(IPubSubClient pubSubClient, ILogger<SubscriptionManager> logger, IYoutubeRepositoryFactory repositoryFactory, IChannelInfoRepository channelInfoRepository, IDateTimeRepository dateTimeRepository)
		{
			_pubSubClient = pubSubClient;
			_logger = logger;
			_repositoryFactory = repositoryFactory;
			_channelInfoRepository = channelInfoRepository;
			_dateTimeRepository = dateTimeRepository;

			var timerInterval = int.Parse(Environment.GetEnvironmentVariable("RESUB_TIMER_INTERVAL") ?? "60");

			ResubTimer = new Timer(1000 * timerInterval);
			ResubTimer.Elapsed += ResubcriptionTimerOnElapsed;
		}

		[ExcludeFromCodeCoverage(Justification = "Timer based methods are very difficult to test.")]
		private async void ResubcriptionTimerOnElapsed(object _, ElapsedEventArgs args)
		{
			var cloned = new Dictionary<string, DateTime>(ResubscribeTimes);
			foreach (var (channelId, value) in cloned)
			{
				if (DateTime.Now.AddMinutes(-10) >= value)
				{
					_logger.LogInformation("Resubscribing to channel {ChannelId}", channelId);
					var result = await _pubSubClient.SubscribeAsync(channelId);

					if (!result.IsSuccess)
					{
						_logger.LogInformation("No longer attempting to resubscribe to channel: {Channel}", channelId);
						ResubscribeTimes.Remove(channelId);
						return;
					}

					var resubTime = DateTime.Now.AddDays(5);
					ResubscribeTimes[channelId] = resubTime;

					await using var repository = _repositoryFactory.GetRepository();

					var subscription = await repository.GetSubscriptionByIdOrDefaultAsync(channelId);

					if (subscription is null)
					{
						_logger.LogError("Subscription with ID {Id} is null when attempting to resubscribed due to lease", channelId);
						return;
					}

					await repository.ModifyExpiryAsync(channelId, resubTime);
				}
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Too simple to require a test.")]
		public async Task StartAsync()
		{
			await using var repository = _repositoryFactory.GetRepository();
			var currentSubscriptions = repository.GetSubscriptions();
			
			ResubscribeTimes = currentSubscriptions.ToDictionary(sub => sub.Id, sub => sub.ExpiresAt);
			ResubTimer.Start();
		}

		public async Task<Result> UpdateSubscriptionSettingsAsync(string guildId, string? uploadChannel = null, string? uploadMessage = null, string? liveChannel = null,
			string? liveMessage = null)
		{
			if (uploadChannel is null && uploadMessage is null && liveChannel is null && liveMessage is null)
			{
				_logger.LogError("UpdateSubscriptionSettingsAsync() called with all null parameters");
				return Result.FromError(new AllParametersNullError());
			}

			await using var repo = _repositoryFactory.GetRepository();

			await repo.UpsertGuildAsync(guildId, uploadChannel, uploadMessage, liveChannel, liveMessage);

			return Result.FromSuccess();
		}

		public async Task<Result<bool>> IsSubscribedAsync(string guildId, string youtubeChannelUrl)
		{
			var (youtubeChannelId, _) = await _channelInfoRepository.GetChannelInfoAsync(youtubeChannelUrl);

			if (youtubeChannelId is null)
			{
				return Result<bool>.FromError(new ChannelInfoRetrievalError());
			}

			await using var repo = _repositoryFactory.GetRepository();

			var subscription = await repo.GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogError("Subscription with ID {Id} was not found when checking if guild {GuildId} was subscribed", youtubeChannelId, guildId);
				return Result<bool>.FromSuccess(false);
			}

			var isSubscribed = subscription.GuildIds.Contains(guildId);

			return Result<bool>.FromSuccess(isSubscribed);
		}

		public async Task<Result> SubscribeAsync(string youtubeChannelUrl, string guildId)
		{
			var isAlreadySubscribed = await IsSubscribedAsync(guildId, youtubeChannelUrl);

			if (!isAlreadySubscribed.IsSuccess)
			{
				return Result.FromError(isAlreadySubscribed.Error);
			}

			if (isAlreadySubscribed.Entity)
			{
				return Result.FromSuccess();
			}

			await using var repo = _repositoryFactory.GetRepository();

			var guild = await repo.GetGuildByIdOrDefaultAsync(guildId);

			// ReSharper disable once MergeSequentialChecks
			if (guild is null || guild.YoutubeUploadLiveChannel is null || guild.YoutubeUploadLiveMessage is null
				|| guild.YoutubeUploadNotificationChannel is null || guild.YoutubeUploadNotificationMessage is null)
			{
				return Result.FromError(new UnconfiguredError());
			}

			var (youtubeChannelId, youtubeChannelTitle) = await _channelInfoRepository.GetChannelInfoAsync(youtubeChannelUrl);
			

			var subscription = await GetSubscriptionAsync(youtubeChannelId!);

			if (subscription is not null)
			{
				// we already have an active subscription, just need to add this guild to the sub

				// no need to check result here, as the above call to IsSubscribedAsync checks if the subscription exists,
				// which is the only time this can fail anyway.
				await repo.AddGuildToSubscriptionAsync(youtubeChannelId!, guildId);

				_logger.LogInformation("Added a subscription to {YoutubeChannelId} for {GuildId}", youtubeChannelId, guildId);
			}
			else
			{
				// we don't have any subscriptions, so let's make a new one
				var subscriptionResult = await _pubSubClient.SubscribeAsync(youtubeChannelId!);

				if (!subscriptionResult.IsSuccess)
				{
					return Result.FromError(subscriptionResult.Error);
				}

				var nowPlusFiveDays = _dateTimeRepository.GetTime().AddDays(5);
				
				await repo.AddSubscriptionAsync(youtubeChannelId!, nowPlusFiveDays, guildId, youtubeChannelTitle!);

				ResubscribeTimes[youtubeChannelId!] = nowPlusFiveDays;
			}

			return Result.FromSuccess();
		}

		public async Task<Result> UnsubscribeAsync(string youtubeChannelUrl, string guildId)
		{
			var (youtubeChannelId, _) = await _channelInfoRepository.GetChannelInfoAsync(youtubeChannelUrl);

			if (youtubeChannelId is null)
			{
				return Result.FromError(new ChannelInfoRetrievalError());
			}

			return await RemoveSubscriptionAsync(guildId, youtubeChannelId);
		}

		private async Task<Result> RemoveSubscriptionAsync(string guildId, string youtubeChannelId)
		{
			await using var repo = _repositoryFactory.GetRepository();

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is not null)
			{
				// we already have an active subscription, just need to remove this guild from the sub

				if (subscription.GuildIds.Length > 1)
				{
					var result = await repo.RemoveGuildFromSubscriptionAsync(youtubeChannelId, guildId);

					if (!result.IsSuccess)
					{
						return Result.FromError(result.Error);
					}
					
					_logger.LogInformation("removed a subscription to {ChannelId} for {GuildId}", youtubeChannelId, guildId);
				}
				else
				{
					// no need to check result here, as the only error that can occur is the subscription being null,
					// which is already handled by the GetSubscriptionAsync() call above.
					await repo.RemoveSubscriptionAsync(youtubeChannelId);

					await _pubSubClient.UnsubscribeAsync(youtubeChannelId);
					_logger.LogInformation("Removing subscription to channel {ChannelId} entirely as no guild are subscribed to it currently", youtubeChannelId);
				}
			}
			else
			{
				_logger.LogWarning("Subscription with id {Id} does not exist, yet an attempt was made to unsubscribe from it", youtubeChannelId);
				return Result.FromError(new NullSubscriptionError());
			}

			return Result.FromSuccess();
		}

		public async Task<Result> UnsubscribeFromAllAsync(string guildId)
		{
			await using var repo = _repositoryFactory.GetRepository();

			var (exists, subscriptions) = await repo.TryGetSubscriptionsAsync(guildId);

			if (!exists)
			{
				_logger.LogWarning("Guild {GuildId} tried to unsubscribe from all but had no subscriptions to begin with", guildId);
				return Result.FromSuccess();
			}

			// important: do not remove .ToArray() - collection gets modified during iteration.
			foreach (var subscription in subscriptions.ToArray())
			{
				var unsubscribeResult = await UnsubscribeAsync(guildId, subscription.Id);
				
				if (!unsubscribeResult.IsSuccess)
				{
					return Result.FromError(unsubscribeResult.Error);	
				}
			}

			return Result.FromSuccess();
		}

		public ValueTask<YoutubeSubscription?> GetSubscriptionAsync(string youtubeChannelId)
		{
			var repository = _repositoryFactory.GetRepository();

			return repository.GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);
		}

		public async Task AddSeenVideoAsync(string youtubeChannelId, string videoId)
		{
			await using var repo = _repositoryFactory.GetRepository();

			var subscription = await repo.GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogWarning("Subscription to channel with ID {ChannelId} not found when trying to add seen video with ID {VideoId}", youtubeChannelId, videoId);
				return;
			}

			var currentIdArray = subscription.AlreadySeenIds;
			var newIdArray = new string[currentIdArray.Length + 1];
			currentIdArray.CopyTo(newIdArray, 0);
			newIdArray[currentIdArray.Length] = videoId;

			await repo.UpdateSeenVideosAsync(youtubeChannelId, newIdArray);
		}

		public async Task UpdateChannelNameAsync(string youtubeChannelTitle, string youtubeChannelId)
		{
			var repository = _repositoryFactory.GetRepository();

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogWarning("Could not find subscription for channel ID {Id} with name {Name} when trying to update the name", youtubeChannelId, youtubeChannelTitle);
				return;
			}

			if (subscription.ChannelTitle == youtubeChannelTitle)
			{
				_logger.LogTrace("Not updating channel name with id {Id} as it's the same ({Name})", youtubeChannelId, youtubeChannelTitle);
				return;
			}

			subscription.ChannelTitle = youtubeChannelTitle;
		}

		public IEnumerable<YoutubeSubscription> GetAllSubscriptionsAsync(string guildId)
		{
			var repo = _repositoryFactory.GetRepository();

			return repo.GetSubscriptions()
				.Where(subscription => subscription.GuildIds.Contains(guildId));
		}
	}

}
