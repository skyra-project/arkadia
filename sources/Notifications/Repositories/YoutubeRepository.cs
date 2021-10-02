using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Cdn.Repositories;
using Database;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.Errors;
using Remora.Results;

namespace Notifications.Repositories
{
	public class YoutubeRepository : IYoutubeRepository
	{
		private readonly ArkadiaDbContext _context = new ArkadiaDbContext();

		[ExcludeFromCodeCoverage]
		public ValueTask DisposeAsync()
		{
			return _context.DisposeAsync();
		}

		public ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id)
		{
			return _context.YoutubeSubscriptions.FindAsync(id);
		}

		public ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id)
		{
			return _context.Guilds.FindAsync(id);
		}

		public async Task ModifyExpiryAsync(string id, DateTime newTime)
		{
			var subscription = await GetSubscriptionByIdOrDefaultAsync(id);
			
			// leave this nullability warning, it is expected for the caller to check if it exists first
			// if not, we'd like to throw anyway.
			subscription!.ExpiresAt = newTime;

			await _context.SaveChangesAsync();

		}

		public IEnumerable<YoutubeSubscription> GetSubscriptions()
		{
			return _context.YoutubeSubscriptions;
		}

		public async Task AddSubscriptionAsync(string id, DateTime expiresAt, string guildId, string channelTitle)
		{
			await _context.YoutubeSubscriptions.AddAsync(new YoutubeSubscription
			{
				Id = id,
				ExpiresAt = expiresAt,
				GuildIds = new []{ guildId },
				ChannelTitle = channelTitle
			});

			await _context.SaveChangesAsync();
		}

		public async ValueTask<Guild> UpsertGuildAsync(string id, string? uploadChannel, string? uploadMessage, string? liveChannel, string? liveMessage)
		{

			var guild = await _context.Guilds.FindAsync(id);
			
			if (guild is null)
			{
				guild = new Guild
				{
					Id = id
				};

				await _context.Guilds.AddAsync(guild);
			}

			guild.YoutubeUploadNotificationChannel = uploadChannel ?? guild.YoutubeUploadNotificationChannel;
			guild.YoutubeUploadNotificationMessage = uploadMessage ?? guild.YoutubeUploadNotificationMessage;
			guild.YoutubeUploadLiveChannel = liveChannel ?? guild.YoutubeUploadLiveChannel;
			guild.YoutubeUploadLiveMessage = liveMessage ?? guild.YoutubeUploadLiveMessage;

			await _context.SaveChangesAsync();

			return guild;
		}

		public async Task<Result> AddGuildToSubscriptionAsync(string youtubeChannelId, string guildId)
		{
			await using var context = new ArkadiaDbContext();

			var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

			if (subscription is null)
			{
				return Result.FromError(new MissingSubscriptionError());
			}

			var currentlySubscribedGuilds = subscription.GuildIds;
			var newSubscribedGuilds = new string[currentlySubscribedGuilds.Length + 1];
			currentlySubscribedGuilds.CopyTo(newSubscribedGuilds, 0);
			newSubscribedGuilds[currentlySubscribedGuilds.Length] = guildId;

			subscription.GuildIds = newSubscribedGuilds;

			await _context.SaveChangesAsync();
			
			return Result.FromSuccess();
		}

		public async Task<Result> RemoveGuildFromSubscriptionAsync(string youtubeChannelId, string guildId)
		{
			var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

			if (subscription is null)
			{
				return Result.FromError(new NullSubscriptionError());
			}

			var containsGuild = subscription.GuildIds.Contains(guildId);

			if (!containsGuild)
			{
				return Result.FromError(new MissingSubscriptionError());
			}

			subscription.GuildIds = subscription.GuildIds.Where(id => id != guildId).ToArray();
			await _context.SaveChangesAsync();
			
			return Result.FromSuccess();
		}

		public async Task<Result> RemoveSubscriptionAsync(string youtubeChannelId)
		{
			var subscription = await GetSubscriptionByIdOrDefaultAsync(youtubeChannelId);

			if (subscription is null)
			{
				return Result.FromError(new NullSubscriptionError());
			}

			_context.YoutubeSubscriptions.Remove(subscription);
			await _context.SaveChangesAsync();
			
			return Result.FromSuccess();
		}

		public async Task<(bool, IEnumerable<YoutubeSubscription>)> TryGetSubscriptionsAsync(string guildId)
		{
			var subscriptions = _context.YoutubeSubscriptions;
			
			var guildSubscriptions = subscriptions.Where(subscription => subscription.GuildIds.Contains(guildId));

			var exists = await subscriptions.AnyAsync();
			
			return (exists, subscriptions);
		}

		public async Task UpdateSeenVideosAsync(string youtubeChannelId, string[] seenVideos)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(youtubeChannelId);

			subscription.AlreadySeenIds = seenVideos;

			await _context.SaveChangesAsync();
		}

	}
}
