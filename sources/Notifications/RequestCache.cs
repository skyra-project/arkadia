﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Notifications
{
	public class RequestCache
	{
		private readonly ILogger<RequestCache> _logger;
		private readonly Dictionary<string, bool> _requests = new();

		public RequestCache(ILogger<RequestCache> logger)
		{
			_logger = logger;
		}

		public bool GetRequest(string channelId, bool isSubscription, bool remove = true)
		{
			var isCorrect = _requests.TryGetValue(channelId, out var subscription) && subscription == isSubscription;

			if (!isCorrect) _logger.LogCritical("request with channel-id {Id} was not found in the request cache", channelId);

			if (remove) RemoveRequest(channelId);

			return isCorrect;
		}

		public void AddRequest(string channelId, bool isSubscription)
		{
			_requests[channelId] = isSubscription;
		}

		public void RemoveRequest(string channelId)
		{
			_requests.Remove(channelId);
		}
	}
}
