using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Results;
using Database.Models;
using Database.Models.Entities;
using Shared.Results;

namespace Database
{
	public interface IDatabase
	{
		Task<Result> AddYoutubeSubscriptionAsync(string channelId, string channelTitle, string guildId);
		Task<Result> AddYoutubeSubscriptionAsync(string channelId, string channelName, string guildId, DateTime expiresAt);
		Task<Result> UpdateYoutubeSubscriptionSettingsAsync(string guildId, string? message, string? channel);
		Task<Result<YoutubeSubscription[]>> GetSubscriptionsAsync(string guildId);
		Task<Result<YoutubeSubscription[]>> GetSubscriptionsAsync();
		Task<Result<YoutubeSubscription>> GetSubscriptionAsync(string channelId);
		Task<Result> AddSeenVideoAsync(string channelId, string videoId);
		Task<Result<bool>> SubscriptionExistsAsync(string channelId);
		Task<Result> RemoveSubscriptionAsync(string channelId, string guildId);
		Task<Result<(string, string)[]>> ExecuteSqlAsync(string query);
		Task<Result> UpdateSubscriptionTimerAsync(string key, DateTime resubTime);
		Task<Result> UpdateChannelNameAsync(string id, string name);
		Task<bool> IsSubscribedAsync(string guildId, string channelId);
		Task<Result> UnsubscribeFromAllAsync(string guildId);
	}
}
