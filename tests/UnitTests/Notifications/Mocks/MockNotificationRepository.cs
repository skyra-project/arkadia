using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models.Entities;
using Notifications.Errors;
using Notifications.Repositories;
using Remora.Results;

namespace UnitTests.Notifications.Mocks;

public class MockNotificationRepository : IYoutubeRepository
{
	private readonly List<Guild> _guildEntries = new();

	private readonly List<YoutubeSubscription> _youtubeEntries = new();

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id)
	{
		var entry = _youtubeEntries.FirstOrDefault(entry => entry.Id == id);
		return ValueTask.FromResult(entry);
	}

	public ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id)
	{
		var entry = _guildEntries.FirstOrDefault(entry => entry.Id == id);
		return ValueTask.FromResult(entry);
	}

	public async Task ModifyExpiryAsync(string id, DateTime newTime)
	{
		var entry = await GetSubscriptionByIdOrDefaultAsync(id);

		if (entry is null) throw new ArgumentException(nameof(entry));

		entry.ExpiresAt = newTime;
	}

	public IEnumerable<YoutubeSubscription> GetSubscriptions()
	{
		return _youtubeEntries;
	}

	public Task AddSubscriptionAsync(string id, DateTime expiresAt, string guildId, string channelTitle)
	{
		_youtubeEntries.Add(new YoutubeSubscription
		{
			Id = id,
			ExpiresAt = expiresAt,
			GuildIds = new[] { guildId },
			ChannelTitle = channelTitle,
			AlreadySeenIds = Array.Empty<string>()
		});

		return Task.CompletedTask;
	}

	public ValueTask<Guild> UpsertGuildAsync(string id, string? uploadChannel, string? uploadMessage,
		string? liveChannel, string? liveMessage)
	{
		var guild = _guildEntries.FirstOrDefault(entry => entry.Id == id);

		if (guild is null)
		{
			guild = new Guild
			{
				Id = id
			};
			_guildEntries.Add(guild);
		}

		guild.YoutubeUploadNotificationChannel = uploadChannel ?? guild.YoutubeUploadNotificationChannel;
		guild.YoutubeUploadNotificationMessage = uploadMessage ?? guild.YoutubeUploadNotificationMessage;
		guild.YoutubeUploadLiveChannel = liveChannel ?? guild.YoutubeUploadLiveChannel;
		guild.YoutubeUploadLiveMessage = liveMessage ?? guild.YoutubeUploadLiveMessage;

		return ValueTask.FromResult(guild);
	}

	public async Task<Result> AddGuildToSubscriptionAsync(string youtubeChannelId, string guildId)
	{
		var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

		if (subscription is null) return Result.FromError(new MissingSubscriptionError());

		var currentlySubscribedGuilds = subscription.GuildIds;
		var newSubscribedGuilds = new string[currentlySubscribedGuilds.Length + 1];
		currentlySubscribedGuilds.CopyTo(newSubscribedGuilds, 0);
		newSubscribedGuilds[currentlySubscribedGuilds.Length] = guildId;

		subscription.GuildIds = newSubscribedGuilds;

		return Result.FromSuccess();
	}

	public async Task<Result> RemoveGuildFromSubscriptionAsync(string youtubeChannelId, string guildId)
	{
		var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

		if (subscription is null) return Result.FromError(new MissingSubscriptionError());

		var containsGuild = subscription.GuildIds.Contains(guildId);

		if (!containsGuild) return Result.FromError(new MissingSubscriptionError());

		subscription.GuildIds = subscription.GuildIds.Where(id => id != guildId).ToArray();

		return Result.FromSuccess();
	}

	public async Task<Result> RemoveSubscriptionAsync(string youtubeChannelId)
	{
		var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

		if (subscription is null) return Result.FromError(new MissingSubscriptionError());

		_youtubeEntries.Remove(subscription);

		return Result.FromSuccess();
	}

	public Task<(bool, IEnumerable<YoutubeSubscription>)> TryGetSubscriptionsAsync(string guildId)
	{
		var entries = _youtubeEntries.Where(entry => entry.GuildIds.Contains(guildId));

		return Task.FromResult((entries.Any(), entries));
	}

	public Task UpdateSeenVideosAsync(string youtubeChannelId, string[] seenVideos)
	{
		var entry = _youtubeEntries.First(entry => entry.Id == youtubeChannelId);
		entry.AlreadySeenIds = seenVideos;
		return Task.CompletedTask;
	}

	public Task AddSubscriptionAsync(string id, DateTime expiresAt, string[] guildIds, string channelTitle)
	{
		_youtubeEntries.Add(new YoutubeSubscription
		{
			Id = id,
			ExpiresAt = expiresAt,
			GuildIds = guildIds,
			ChannelTitle = channelTitle
		});
		return Task.CompletedTask;
	}
}
