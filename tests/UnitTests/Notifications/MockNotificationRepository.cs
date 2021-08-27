using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models.Entities;
using Notifications.Repositories;

namespace UnitTests.Notifications
{
	public class MockNotificationRepository : IYoutubeRepository
	{
		
		private readonly List<YoutubeSubscription> _youtubeEntries = new List<YoutubeSubscription>();
		private readonly List<Guild> _guildEntries = new List<Guild>();
		
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

		public ValueTask<Guild> UpsertGuildAsync(string id, string? uploadChannel, string? uploadMessage, string? liveChannel, string? liveMessage)
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
	}
}
