using System;
using System.Collections.Generic;
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
using Remora.Results;

namespace Notifications.Managers
{
	public class SubscriptionManager
	{
		private readonly IBrowsingContext _browsingContext;
		private readonly ILogger<SubscriptionManager> _logger;
		private readonly PubSubClient _pubSubClient;
		private readonly Timer _resubTimer;

		private Dictionary<string, DateTime> _resubscribeTimes = new Dictionary<string, DateTime>();

		public SubscriptionManager(PubSubClient pubSubClient, ILogger<SubscriptionManager> logger, IBrowsingContext browsingContext)
		{
			_pubSubClient = pubSubClient;
			_logger = logger;
			_browsingContext = browsingContext;

			var timerInterval = int.Parse(Environment.GetEnvironmentVariable("RESUB_TIMER_INTERVAL") ?? "60");

			_resubTimer = new Timer(1000 * timerInterval);
			_resubTimer.Elapsed += ResubcriptionTimerOnElapsed;
		}

		private async void ResubcriptionTimerOnElapsed(object _, ElapsedEventArgs args)
		{
			var cloned = new Dictionary<string, DateTime>(_resubscribeTimes);
			foreach (var (channelId, value) in cloned)
			{
				if (DateTime.Now.AddMinutes(-10) >= value)
				{
					_logger.LogInformation("Resubscribing to channel {ChannelId}", channelId);
					var result = await _pubSubClient.SubscribeAsync(channelId);
					
					if (!result.IsSuccess)
					{
						_logger.LogInformation("No longer attempting to resubscribe to channel: {Channel}", channelId);
						_resubscribeTimes.Remove(channelId);
						return;
					}

					var resubTime = DateTime.Now.AddDays(5);
					_resubscribeTimes[channelId] = resubTime;
					
					using var database = new ArkadiaDbContext();

					var subscription = await database.YoutubeSubscriptions.FindAsync(channelId);

					if (subscription is null)
					{
						_logger.LogError("Subscription with ID {Id} is null when attempting to resubscribed due to lease", channelId);
						return;
					}

					subscription.ExpiresAt = resubTime;
					await database.SaveChangesAsync();
				}
			}
		}

		public Task StartAsync()
		{
			using var database = new ArkadiaDbContext();
			var currentSubscriptions = database.YoutubeSubscriptions;
			

			_resubscribeTimes = currentSubscriptions.ToDictionary(sub => sub.Id, sub => sub.ExpiresAt);
			_resubTimer.Start();
			return Task.CompletedTask;
		}

		public async Task<Result> UpdateSubscriptionSettingsAsync(string guildId, string? uploadChannel = null, string? uploadMessage = null, string? liveChannel = null, string? liveMessage = null)
		{

			if (uploadChannel is null && uploadMessage is null && liveChannel is null && liveMessage is null)
			{
				_logger.LogError("UpdateSubscriptionSettingsAsync() called with all null parameters");
				return Result.FromError(new AllParametersNullError());
			}
			
			using var database = new ArkadiaDbContext();
			var guild = await database.Guilds.FindAsync(guildId);

			if (guild is null)
			{
				var entry = await database.Guilds.AddAsync(new Guild
				{
					Id = guildId
				});
				guild = entry.Entity;
			}

			guild.YoutubeUploadNotificationChannel = uploadChannel ?? guild.YoutubeUploadNotificationChannel;
			guild.YoutubeUploadNotificationMessage = uploadMessage ?? guild.YoutubeUploadNotificationMessage;
			guild.YoutubeUploadLiveChannel = liveChannel ?? guild.YoutubeUploadLiveChannel;
			guild.YoutubeUploadLiveMessage = liveMessage ?? guild.YoutubeUploadLiveMessage;

			await database.SaveChangesAsync();
			
			return Result.FromSuccess();
		}

		public async Task<Result<bool>> IsSubscribedAsync(string guildId, string youtubeChannelUrl)
		{
			var (youtubeChannelId, _) = await GetChannelInfoAsync(youtubeChannelUrl);

			if (youtubeChannelId is null)
			{
				return Result<bool>.FromError(new ChannelInfoRetrievalError());
			}

			using var database = new ArkadiaDbContext();

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogError("Subscription with ID {Id} was not found when checking if guild {GuildId} was subscribed", youtubeChannelId, guildId);
				return Result<bool>.FromError(new NullSubscriptionError());
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
			
			using var database = new ArkadiaDbContext();

			var guild = await database.Guilds.FindAsync(guildId);

			// ReSharper disable once MergeSequentialChecks
			if (guild is null || guild.YoutubeUploadLiveChannel is null || guild.YoutubeUploadLiveMessage is null
				|| guild.YoutubeUploadNotificationChannel is null || guild.YoutubeUploadNotificationMessage is null)
			{
				return Result.FromError(new UnconfiguredError());
			}
			
			var (youtubeChannelId, youtubeChannelTitle) = await GetChannelInfoAsync(youtubeChannelUrl);

			if (youtubeChannelId is null || youtubeChannelTitle is null)
			{
				return Result.FromError(new ChannelInfoRetrievalError());
			}

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is not null)
			{
				// we already have an active subscription, just need to add this guild to the sub
				
				var currentlySubscribedGuilds = subscription.GuildIds;
				var newSubscribedGuilds = new string[currentlySubscribedGuilds.Length + 1];
				currentlySubscribedGuilds.CopyTo(newSubscribedGuilds, 0);
				newSubscribedGuilds[currentlySubscribedGuilds.Length] = guildId;
				
				_logger.LogInformation("Added a subscription to {YoutubeChannelId} for {GuildId}", youtubeChannelId, guildId);
			}
			else
			{
				// we don't have any subscriptions, so let's make a new one
				var subscriptionResult = await _pubSubClient.SubscribeAsync(youtubeChannelId);

				if (!subscriptionResult.IsSuccess)
				{
					return Result.FromError(subscriptionResult.Error);
				}

				var nowPlusFiveDays = DateTime.Now.AddDays(5);

				await database.YoutubeSubscriptions.AddAsync(new YoutubeSubscription
				{
					Id = youtubeChannelId,
					AlreadySeenIds = Array.Empty<string>(),
					ChannelTitle = youtubeChannelTitle,
					ExpiresAt = nowPlusFiveDays,
					GuildIds = new[] { guildId }
				});

				_resubscribeTimes[youtubeChannelId] = nowPlusFiveDays;
			}

			await database.SaveChangesAsync();
			
			return Result.FromSuccess();
		}

		public async Task<Result> UnsubscribeAsync(string youtubeChannelUrl, string guildId)
		{
			var (youtubeChannelId, _) = await GetChannelInfoAsync(youtubeChannelUrl);

			if (youtubeChannelId is null)
			{
				return Result.FromError(new ChannelInfoRetrievalError());
			}

			return await RemoveSubscriptionAsync(guildId, youtubeChannelId);
		}

		private async Task<Result> RemoveSubscriptionAsync(string guildId, string youtubeChannelId)
		{
			using var database = new ArkadiaDbContext();

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is not null)
			{
				// we already have an active subscription, just need to remove this guild from the sub

				if (subscription.GuildIds.Length > 1)
				{
					subscription.GuildIds = subscription.GuildIds.Where(id => id != guildId).ToArray();
					_logger.LogInformation("removed a subscription to {ChannelId} for {GuildId}", youtubeChannelId, guildId);
				}
				else
				{
					database.YoutubeSubscriptions.Remove(subscription);
					await _pubSubClient.UnsubscribeAsync(youtubeChannelId);
					_logger.LogInformation("Removing subscription to channel {ChannelId} entirely as no guild are subscribed to it currently", youtubeChannelId);
				}

				await database.SaveChangesAsync();
			}
			else
			{
				_logger.LogWarning("Subscription with id {Id} does not exist, yet an attempt was made to unsubscribe from it", youtubeChannelId);
				return Result.FromSuccess();
			}

			return Result.FromSuccess();
		}

		public async Task<Result> UnsubscribeFromAllAsync(string guildId)
		{
			using var database = new ArkadiaDbContext();

			var subscriptions = database.YoutubeSubscriptions.Where(subscription => subscription.GuildIds.Contains(guildId));

			var exists = await subscriptions.AnyAsync();

			if (!exists)
			{
				_logger.LogWarning("Guild {GuildId} tried to unsubscribe from all but had no subscriptions to begin with", guildId);
				return Result.FromSuccess();
			}

			foreach (var subscription in subscriptions)
			{
				await UnsubscribeAsync(guildId, subscription.Id);
			}
			
			return Result.FromSuccess();
		}

		public async Task<YoutubeSubscription?> GetSubscriptionAsync(string youtubeChannelId)
		{
			using var database = new ArkadiaDbContext();

			return await database.YoutubeSubscriptions.FindAsync(youtubeChannelId);
		}

		public async Task AddSeenVideoAsync(string youtubeChannelId, string videoId)
		{
			using var database = new ArkadiaDbContext();

			var subscription = await database.YoutubeSubscriptions.FindAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogWarning("Subscription to channel with ID {ChannelId} not found when trying to add seen video with ID {VideoId}", youtubeChannelId, videoId);
				return;
			}

			var currentIdArray = subscription.AlreadySeenIds;
			var newIdArray = new string[currentIdArray.Length + 1];
			currentIdArray.CopyTo(newIdArray, 0);
			newIdArray[currentIdArray.Length] = videoId;

			await database.SaveChangesAsync();
		}

		public async Task UpdateChannelNameAsync(string youtubeChannelTitle, string youtubeChannelId)
		{
			using var database = new ArkadiaDbContext();

			var subscription = await GetSubscriptionAsync(youtubeChannelId);

			if (subscription is null)
			{
				_logger.LogWarning("Could not find subscription for channel ID {Id} with name {Name} when trying to update the name.", youtubeChannelId, youtubeChannelTitle);
				return;
			}

			if (subscription.ChannelTitle == youtubeChannelTitle)
			{
				_logger.LogTrace("Not updating channel name with id {Id} as it's the same ({Name})", youtubeChannelId, youtubeChannelTitle);
				return;
			}

			subscription.ChannelTitle = youtubeChannelTitle;

			await database.SaveChangesAsync();
		}

		public async Task<YoutubeSubscription[]> GetAllSubscriptionsAsync(string guildId)
		{
			using var database = new ArkadiaDbContext();

			return await database.YoutubeSubscriptions.ToArrayAsync();
		}

		private async Task<(string?, string?)> GetChannelInfoAsync(string channelUrl)
		{
			var document = await _browsingContext.OpenAsync(channelUrl);
			if (document.StatusCode != HttpStatusCode.OK)
			{
				_logger.LogError("Did not recieve OK response from youtube for channel url of {Url} - instead received {Status}", channelUrl, document.StatusCode);
				return (null, null);
			}

			var cell = document.QuerySelector("meta[itemprop='channelId']") as IHtmlMetaElement;

			if (cell is null)
			{
				_logger.LogError("Could not find <meta> tag for the channel-id for url {Url}", channelUrl);
				return (null, null);
			}

			var name = document.QuerySelector("meta[property='og:title']").Attributes["content"].Value;

			if (name is null)
			{
				_logger.LogError("Could not find 'og:title' tag for url {Url}", channelUrl);
				return (null, null);
			}

			return (cell.Content, name);
		}
	}

}
