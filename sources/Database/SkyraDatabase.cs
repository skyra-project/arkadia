using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Results;
using Database;
using Database.Models;
using Database.Models.Entities;
using Shared.Results;

namespace Database
{
	public class SkyraDatabase : IDatabase, IDisposable, IAsyncDisposable
	{
		private readonly SkyraDbContext _context;
		private readonly ILogger<SkyraDatabase> _logger;
		private readonly Random _random = new Random();

		public SkyraDatabase(SkyraDbContext context, ILogger<SkyraDatabase> logger)
		{
			_context = context;
			_logger = logger;
		}

		/// <inheritdoc />
		public async ValueTask DisposeAsync()
		{
			await _context.DisposeAsync();
		}


		public async Task<Result<Guild?>> GetGuildAsync(string guildId)
		{
			try
			{
				var guild = await _context.Guilds.FindAsync(guildId);
				return Result<Guild?>.FromSuccess(guild);
			}
			catch (Exception e)
			{
				return HandleException<Guild?>(e);
			}
		}

		public Task<Result> AddYoutubeSubscriptionAsync(string channelId, string channelTitle, string guildId)
		{
			return AddYoutubeSubscriptionAsync(channelId, channelTitle, guildId, default);
		}

		public async Task<Result> AddYoutubeSubscriptionAsync(string channelId, string channelName, string guildId, DateTime expiresAt)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(channelId);

			if (subscription is null)
			{
				subscription = new YoutubeSubscription
				{
					ExpiresAt = expiresAt,
					Id = channelId,
					ChannelTitle = channelName,
					GuildIds = new[] {guildId},
					AlreadySeenIds = Array.Empty<string>()
				};

				await _context.YoutubeSubscriptions.AddAsync(subscription);
				return Result.FromSuccess();
			}

			var guilds = subscription.GuildIds;

			// TODO: should this return an error?
			if (guilds.Contains(guildId))
			{
				return Result.FromSuccess();
			}


			subscription.GuildIds = subscription.GuildIds.Concat(new[] {guildId}).ToArray();

			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<Result> UpdateYoutubeSubscriptionSettingsAsync(string guildId, string? message, string? channel)
		{
			var guild = await _context.Guilds.FindAsync(guildId);
			if (guild is null)
			{
				if (message is null || channel is null)
				{
					_logger.LogError("UpdateYoutubeSubscriptionSettings was called with either a null channel ({Channel}), or message ({Message})", channel, message);
					return Result.FromError();
				}

				_context.Guilds.Add(new Guild
				{
					YoutubeNotificationChannel = channel,
					YoutubeNotificationMessage = message,
					Id = guildId
				});
				await _context.SaveChangesAsync();
				return Result.FromSuccess();
			}

			guild.YoutubeNotificationChannel = channel ?? guild.YoutubeNotificationChannel;
			guild.YoutubeNotificationMessage = message ?? guild.YoutubeNotificationMessage;
			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<Result<YoutubeSubscription[]>> GetSubscriptionsAsync()
		{
			var subscriptions = _context.YoutubeSubscriptions;

			if (await subscriptions.AnyAsync())
			{
				return Result<YoutubeSubscription[]>.FromSuccess(subscriptions.ToArray());
			}
			return Result<YoutubeSubscription[]>.FromSuccess(Array.Empty<YoutubeSubscription>());
		}

		public async Task<Result<YoutubeSubscription[]>> GetSubscriptionsAsync(string guildId)
		{
			var subscriptions = _context.YoutubeSubscriptions.Where(sub => sub.GuildIds.Contains(guildId));
			if (await subscriptions.AnyAsync())
			{
				return Result<YoutubeSubscription[]>.FromSuccess(subscriptions.ToArray());
			}

			return Result<YoutubeSubscription[]>.FromSuccess(Array.Empty<YoutubeSubscription>());
		}

		public async Task<Result<YoutubeSubscription>> GetSubscriptionAsync(string channelId)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(channelId);
			if (subscription is null)
			{
				return Result<YoutubeSubscription>.FromError();
			}

			return Result<YoutubeSubscription>.FromSuccess(subscription);
		}

		public async Task<Result> AddSeenVideoAsync(string channelId, string videoId)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(channelId);
			subscription.AlreadySeenIds = subscription.AlreadySeenIds.Concat(new[] {videoId}).ToArray();
			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<Result<bool>> SubscriptionExistsAsync(string channelId)
		{
			return Result<bool>.FromSuccess(await _context.YoutubeSubscriptions.FindAsync(channelId) != null);
		}

		public async Task<Result> RemoveSubscriptionAsync(string channelId, string guildId)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(channelId);
			if (subscription is null || !subscription.GuildIds.Contains(guildId))
			{
				return Result.FromError();
			}

			if (!subscription.GuildIds.Any())
			{
				_context.YoutubeSubscriptions.Remove(subscription);
			}
			else
			{
				subscription.GuildIds = subscription.GuildIds.Where(id => id != guildId).ToArray();
			}

			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<Result<(string, string)[]>> ExecuteSqlAsync(string query)
		{
			await using var command = _context.Database.GetDbConnection().CreateCommand();
			command.CommandText = query;

			await _context.Database.OpenConnectionAsync();

			await using var result = await command.ExecuteReaderAsync();

			var results = new List<(string, string)>();
			while (await result.ReadAsync())
			{
				for (var i = 0; i < result.VisibleFieldCount; i++)
				{
					results.Add((result.GetName(i), result.GetValue(i).ToString()!));
				}
			}

			return Result<(string, string)[]>.FromSuccess(results.ToArray());
		}

		public async Task<Result> UpdateSubscriptionTimerAsync(string key, DateTime resubTime)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(key);

			if (subscription is null)
			{
				return Result.FromError("No subscription found.");
			}

			subscription.ExpiresAt = resubTime;
			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<Result> UpdateChannelNameAsync(string id, string name)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(id);
			if(subscription.ChannelTitle == name) return Result.FromSuccess();

			subscription.ChannelTitle = name;
			await _context.SaveChangesAsync();
			return Result.FromSuccess();
		}

		public async Task<bool> IsSubscribedAsync(string guildId, string channelId)
		{
			var subscription = await _context.YoutubeSubscriptions.FindAsync(channelId);
			return subscription is not null && subscription.GuildIds.Contains(guildId);
		}

		public async Task<Result> UnsubscribeFromAllAsync(string guildId)
		{
			var subscriptions = _context.YoutubeSubscriptions
				.Where(subscription => subscription.GuildIds.Contains(guildId));

			foreach (var sub in subscriptions)
			{
				await RemoveSubscriptionAsync(sub.Id, guildId);
			}

			return Result.FromSuccess();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_context.Dispose();
		}

		private void PrintException(Exception exception)
		{
			_logger.LogCritical("Received Error: {Error}", exception);
		}

		private Result HandleException(Exception exception)
		{
			PrintException(exception);
			return Result.FromError(exception);
		}

		private Result<T> HandleException<T>(Exception exception)
		{
			PrintException(exception);
			return Result<T>.FromError(exception);
		}
	}
}
