﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models.Entities;
using Remora.Results;

namespace Notifications.Repositories;

public interface IYoutubeRepository : IAsyncDisposable
{
	ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id);
	ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id);
	Task ModifyExpiryAsync(string id, DateTime newTime);
	IEnumerable<YoutubeSubscription> GetSubscriptions();
	Task AddSubscriptionAsync(string id, DateTime expiresAt, string guildId, string channelTitle);

	ValueTask<Guild> UpsertGuildAsync(string id, string? uploadChannel, string? uploadMessage, string? liveChannel,
		string? liveMessage);

	Task<Result> AddGuildToSubscriptionAsync(string youtubeChannelId, string guildId);
	Task<Result> RemoveGuildFromSubscriptionAsync(string youtubeChannelId, string guildId);
	Task<Result> RemoveSubscriptionAsync(string youtubeChannelId);
	Task<(bool, IEnumerable<YoutubeSubscription>)> TryGetSubscriptionsAsync(string guildId);
	Task UpdateSeenVideosAsync(string youtubeChannelId, string[] seenVideos);
}
