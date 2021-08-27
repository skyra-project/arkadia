using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Cdn.Repositories;
using Database;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

		public async Task AddSubscriptionAsync(string id, DateTime expiresAt, string[] guildIds, string channelTitle)
		{
			await _context.YoutubeSubscriptions.AddAsync(new YoutubeSubscription
			{
				Id = id,
				ExpiresAt = expiresAt,
				GuildIds = guildIds,
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
	}
}
