using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Notifications.Extensions;
using Notifications.Factories;
using Notifications.Managers;
using Notifications.Models;
using Services;
using Shared.Extensions;
using YoutubeServiceBase = Services.YoutubeSubscription.YoutubeSubscriptionBase;

namespace Notifications.Services
{
	public class YoutubeService : YoutubeServiceBase
	{
		private readonly ILogger<YoutubeService> _logger;
		private readonly ConcurrentQueue<Notification> _notificationQueue;
		private readonly SubscriptionManager _subscriptionManager;
		private readonly IYoutubeRepositoryFactory _repositoryFactory;

		public YoutubeService(ConcurrentQueue<Notification> notificationQueue, ILogger<YoutubeService> logger, SubscriptionManager subscriptionManager, IYoutubeRepositoryFactory repositoryFactory)
		{
			_notificationQueue = notificationQueue;
			_logger = logger;
			_subscriptionManager = subscriptionManager;
			_repositoryFactory = repositoryFactory;
		}

		[DoesNotReturn]
		[ExcludeFromCodeCoverage(Justification = "Infinite loop.")]
		public override async Task NotificationStream(Empty request, IServerStreamWriter<UploadNotification> responseStream, ServerCallContext _)
		{
			while (true)
			{
				if (!_notificationQueue.TryDequeue(out var notification))
				{
					continue;
				}

				_logger.LogInformation("Sending notification {@Notification}", notification);

				await using var factory = _repositoryFactory.GetRepository();

				var subscription = await factory.GetSubscriptionByIdOrDefaultAsync(notification.ChannelId);

				if (subscription is null)
				{
					_logger.LogError("Subscription with ID {Id} not found in the database while trying to dispatch notification for video {VideoId}", notification.ChannelId,
						notification.VideoId);
					continue;
				}

				var guildInformation = new GuildInformation[subscription.GuildIds.Length];

				for (var index = 0; index < subscription.GuildIds.Length; index++)
				{
					var guildId = subscription.GuildIds[index];
					var guild = await factory.GetGuildByIdOrDefaultAsync(guildId);

					if (guild is null)
					{
						_logger.LogError("Guild with ID {Id} was not found in the database while trying to dispatch notification for video {VideoId}", guildId, notification.VideoId);
						continue;
					}

					var isLive = notification.Type == UploadType.Live;

					var discordChannelId = isLive
						? guild.YoutubeUploadLiveChannel!
						: guild.YoutubeUploadNotificationChannel!;

					var discordMessageContent = isLive
						? guild.YoutubeUploadLiveMessage!
						: guild.YoutubeUploadNotificationMessage!;

					guildInformation[index] = new GuildInformation
					{
						GuildId = guildId,
						ChannelId = discordChannelId,
						Message = discordMessageContent
					};
				}

				var videoUrlBuilder = new UriBuilder("https://www.youtube.com");
				videoUrlBuilder.AddQueryParameter("watch", notification.VideoId);
				var videoUrl = videoUrlBuilder.Uri.ToString();

				var uploadNotification = new UploadNotification
				{
					Video = new Video
					{
						Title = notification.Title,
						Url = videoUrl,
						Time = notification.PublishedAt.ToTimestamp(),
						ChannelInfo = new ChannelInformation
						{
							ChannelName = notification.ChannelName,
							ChannelUrl = GetChannelUrl(notification.ChannelId)
						}
					}
				};

				uploadNotification.GuildInfo.AddRange(guildInformation);

				await responseStream.WriteAsync(uploadNotification);

				var guildsToString = string.Join(',', subscription.GuildIds);
				_logger.LogInformation("Send notification for {VideoTitle} ({VideoId}) to guilds [{GuildIds}]", notification.Title, notification.VideoId, guildsToString);
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> Subscribe(SubscriptionRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.SubscribeAsync(request.ChannelUrl, request.GuildId);

			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> Unsubscribe(SubscriptionRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UnsubscribeAsync(request.ChannelUrl, request.GuildId);

			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> SetDiscordUploadChannel(DiscordChannelRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UpdateSubscriptionSettingsAsync(request.GuildId, request.ChannelId);
			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> SetDiscordUploadMessage(DiscordMessageRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UpdateSubscriptionSettingsAsync(request.GuildId, uploadMessage: request.Content);
			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> SetDiscordLiveChannel(DiscordChannelRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UpdateSubscriptionSettingsAsync(request.GuildId, liveChannel: request.ChannelId);
			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> SetDiscordLiveMessage(DiscordMessageRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UpdateSubscriptionSettingsAsync(request.GuildId, liveMessage: request.Content);
			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<YoutubeServiceResponse> RemoveAllSubscriptions(RemoveAllRequest request, ServerCallContext _)
		{
			var managerResponse = await _subscriptionManager.UnsubscribeFromAllAsync(request.GuildId);
			return managerResponse.AsYoutubeServiceResponse();
		}

		[ExcludeFromCodeCoverage(Justification = "Tested elsewhere.")]
		public override async Task<SubscriptionListResponse> GetSubscriptions(SubscriptionListRequest request, ServerCallContext _)
		{
			var subscriptions = _subscriptionManager.GetAllSubscriptionsAsync(request.GuildId);

			var response = new SubscriptionListResponse();

			var channelInformation = subscriptions.Select(subcription => new ChannelInformation
			{
				ChannelName = subcription.ChannelTitle,
				ChannelUrl = GetChannelUrl(subcription.Id)
			});

			response.Info.AddRange(channelInformation);
			return response;
		}

		[ExcludeFromCodeCoverage(Justification = "Unnecessary to test.")]
		private static string GetChannelUrl(string youtubeChannelId)
		{
			return $"https://www.youtube.com/channel/{youtubeChannelId}";
		}
	}
}
