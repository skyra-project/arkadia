using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Results;

namespace Notifications.Clients
{
	public class PubSubClient
	{
		private readonly RequestCache _cache;
		private readonly HttpClient _httpClient;
		private readonly ILogger<PubSubClient> _logger;
		private readonly string _callbackUrl;
		private readonly string _pubSubUrl;

		public PubSubClient(RequestCache cache, HttpClient httpClient, ILogger<PubSubClient> logger)
		{
			_cache = cache;
			_httpClient = httpClient;
			_logger = logger;
			_pubSubUrl = Environment.GetEnvironmentVariable("PUBSUB_URL") ?? "https://pubsubhubbub.appspot.com/";
			_callbackUrl = Environment.GetEnvironmentVariable("CALLBACK_URL") ?? throw new ArgumentException("The environement variable 'CALLBACK_URL' must be set.");
		}

		public Task<Result> SubscribeAsync(string channelId)
		{
			return SendRequestAsync(channelId, true);
		}

		public Task<Result> UnsubscribeAsync(string channelId)
		{
			return SendRequestAsync(channelId, false);
		}

		private async Task<Result> SendRequestAsync(string channelId, bool isSubscription)
		{
			var formBody = BuildForm(channelId, isSubscription);
			_cache.AddRequest(channelId, isSubscription);

			var status = await _httpClient.PostAsync(_pubSubUrl, formBody);

			if (status.IsSuccessStatusCode)
			{
				return Result.FromSuccess();
			}

			_logger.LogError("Subscription request to pubsubhubbub failed: {Error}", await status.Content.ReadAsStringAsync());
			
			_cache.RemoveRequest(channelId);
			
			return Result.FromError(new PubSubHubBubError());
		}

		private FormUrlEncodedContent BuildForm(string channelId, bool isSubscription)
		{
			var collection = new List<KeyValuePair<string?, string?>>();
			collection.Add(new KeyValuePair<string?, string?>("hub.callback", _callbackUrl));
			collection.Add(new KeyValuePair<string?, string?>("hub.mode", isSubscription ? "subscribe" : "unsubscribe"));
			collection.Add(new KeyValuePair<string?, string?>("hub.topic", $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={channelId}"));

			var options = new FormUrlEncodedContent(collection);
			return options;
		}
		
		public class PubSubHubBubError : IResultError
		{
			public string Message => "";
		}
	}
}
