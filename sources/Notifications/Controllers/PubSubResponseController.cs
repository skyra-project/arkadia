using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notifications.Clients;
using Notifications.Managers;
using Notifications.Models;

namespace Notifications.Controllers
{
	[ApiController]
	[Route("notifications")]
	[ExcludeFromCodeCoverage(Justification = "Not mocking the entire Youtube API to test.")]
	public class PubSubResponseController : ControllerBase
	{
		private readonly YoutubeApiClient _apiClient;
		private readonly RequestCache _cache;
		private readonly ILogger<PubSubResponseController> _logger;
		private readonly ConcurrentQueue<Notification> _notificationQueue;
		private readonly DateTime _startupTime = DateTime.Now;
		private readonly SubscriptionManager _subscriptionManager;
		
		public PubSubResponseController(ILogger<PubSubResponseController> logger, RequestCache cache, ConcurrentQueue<Notification> notificationQueue,
			SubscriptionManager subscriptionManager, YoutubeApiClient apiClient)
		{
			_logger = logger;
			_cache = cache;
			_notificationQueue = notificationQueue;
			_subscriptionManager = subscriptionManager;
			_apiClient = apiClient;
		}

		[HttpGet]
		public async Task Authenticate()
		{
			var (topic, mode, challenge, channelId, isSubscription) = StringValues();

			_logger.LogInformation("Received Authentication request with challenge {Challenge} for topic {Topic} ({Mode})",
				challenge, topic, mode);

			if (!_cache.GetRequest(channelId, isSubscription))
			{
				_logger.LogCritical("No request found with channel_id {ChannelId} and mode {Mode} from IP: {Ip}", channelId, mode, Request.Host.Host);
				Response.StatusCode = 404;
				return;
			}

			Response.StatusCode = 200;
			await Response.Body.WriteAsync(Encoding.ASCII.GetBytes(challenge));

			(string topic, string mode, string challenge, string channelId, bool isSubscription) StringValues()
			{
				var stringValues = Request.Query["hub.topic"];
				var mode = Request.Query["hub.mode"];
				var challenge = Request.Query["hub.challenge"];
				var queryParams = HttpUtility.ParseQueryString(new Uri(stringValues).Query);

				var id = queryParams["channel_id"]!;
				var isSubscription = mode == "subscribe";
				return (stringValues, mode, challenge, id, isSubscription);
			}
		}

		[HttpPost]
		public async Task<IActionResult> Notify()
		{
			using var database = new ArkadiaDbContext();
			var elements = await GetElementsAsync();

			var xElements = elements.ToArray();
			var entryElement = xElements.FirstOrDefault(element => element.Name.LocalName == "entry");

			if (entryElement is null) // it's a delete, they have no 'entry' element, ignore
			{
				var atby = xElements
					.First(element => element.Name.LocalName == "deleted-entry")
					.Elements()
					.First(element => element.Name.LocalName == "by");

				var url = atby
					.Elements()
					.First(element => element.Name.LocalName == "uri").Value;

				_logger.LogInformation("Ignoring deleted video from channel: {Id}", url);
				return Ok();
			}

			var (published, channelId, title, videoId) = GetMetadata(entryElement);

			if (published is null || channelId is null || title is null || videoId is null)
			{
				_logger.LogError("One or more metadata elements were null: " +
								"\n  Published: {Published}" +
								"\n  Channel ID: {ChannelId}" +
								"\n  VideoTitle: {VideoTitle}" +
								"\n  Video ID: {VideoId}",
					published, channelId, title, videoId);
				return Ok();
			}

			var publishedTime = published.Value;

			// if it was published more then 10 minutes ago, we ignore it, as youtube likes to send videos from hours ago
			if (publishedTime.AddMinutes(10) <= _startupTime)
			{
				_logger.LogInformation("Ignoring uploaded video that was uploaded at {PublishedTime} with ID of {Id}", published, videoId);
				return Ok();
			}

			var subscription = await _subscriptionManager.GetSubscriptionAsync(channelId);

			if (subscription is null)
			{
				// we don't actually have a subscription, but youtube is sending notifications for that channel anyway
				_logger.LogInformation("Ignoring event with video ID of {VideoId} because it was not found as a subscription in the database", videoId);
				return Ok();
			}

			var ids = subscription.AlreadySeenIds;

			if (ids.Contains(videoId))
			{
				// we have already posted this video, so let's ignore it, as youtube likes to send duplicates
				_logger.LogInformation("Ignoring event with video ID of {VideoId} because it's already been seen in the database", videoId);
				return Ok();
			}

			// at this point, we can be pretty sure that we've covered all bases.
			// thank you, google, for making me suffer.

			await _subscriptionManager.AddSeenVideoAsync(channelId, videoId);

			var channelName = GetChannelName(entryElement);

			if (channelName is null)
			{
				_logger.LogWarning("Channel name element was null: {Entry}", entryElement.ToString());
				return Ok();
			}


			await _subscriptionManager.UpdateChannelNameAsync(channelName, channelId);

			var thumbnailUrl = $"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg";

			var isLive = await _apiClient.IsVideoLiveAsync(videoId);

			if (!isLive.IsSuccess) _logger.LogError("Youtube API Client did not succeed in querying videos: {IsLiveError}", isLive.Error.Message);

			_logger.LogTrace("Enqueuing video with ID {Id}", videoId);

			_notificationQueue.Enqueue(new Notification
			{
				ChannelId = channelId,
				Title = title,
				VideoId = videoId,
				ThumbnailUrl = thumbnailUrl,
				PublishedAt = publishedTime,
				ChannelName = channelName,
				Type = isLive.Entity ? UploadType.Live : UploadType.Upload
			});

			return Ok();
		}

		private static string? GetChannelName(XElement entry)
		{
			var channelName = entry
				.Elements()
				.FirstOrDefault(element => element.Name.LocalName == "author")?
				.Elements()
				.FirstOrDefault(element => element.Name.LocalName == "name")?
				.Value;

			return channelName;
		}

		private static (DateTime? published, string? channelId, string? title, string? videoId) GetMetadata(XElement entry)
		{
			var published = DateTime.Parse(entry.Elements().First(element => element.Name.LocalName == "published").Value);
			var channelId = entry.Elements().First(element => element.Name.LocalName == "channelId").Value;
			var title = entry.Elements().First(element => element.Name.LocalName == "title").Value;
			var videoId = entry.Elements().First(element => element.Name.LocalName == "videoId").Value;
			return (published, channelId, title, videoId);
		}

		private async Task<IEnumerable<XElement>> GetElementsAsync()
		{
			var reader = new StreamReader(Request.Body);
			var text = await reader.ReadToEndAsync();
			var document = XElement.Parse(text);
			var elements = document.Elements();
			return elements;
		}
	}
}
