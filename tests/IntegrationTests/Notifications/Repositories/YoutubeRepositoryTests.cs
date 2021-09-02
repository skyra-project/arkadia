using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Notifications.Factories;
using Notifications.Repositories;
using NUnit.Framework;

namespace IntegrationTests.Notifications.Repositories
{
	[TestFixture]
	public class YoutubeRepositoryTests
	{
		[SetUp]
		public async Task Setup()
		{
			await using var context = new ArkadiaDbContext();

			foreach (var entry in context.YoutubeSubscriptions)
			{
				context.Remove(entry);
			}
			
			foreach (var entry in context.Guilds)
			{
				context.Remove(entry);
			}

			await context.SaveChangesAsync();
		}

		[Test]
		public async Task YoutubeRepository_GetSubscriptionByIdOrDefaultAsync_ReturnsNull_WhenEntryDoesNotExist()
		{
			
			// arrange

			var repository = new YoutubeRepository();
			const string name = "test";

			// act

			var response = await repository.GetSubscriptionByIdOrDefaultAsync(name);

			// assert
			
			Assert.That(response, Is.Null);
		}

		[Test]
		public async Task YoutubeRepository_GetSubscriptionByIdOrDefaultAsync_ReturnsItem_WhenExists()
		{
			// arrange

			var repository = new YoutubeRepository();
			
			await using var context = new ArkadiaDbContext();

			const string id = "test1";
			const string channelTitle = "coolTitle";
			var expiresAt = DateTime.Now;
			var guildIds = new[] { "id1", "id2" };
			var alreadySeenIds = new[] { "id1", "id2" };
			
			var expected = new YoutubeSubscription
			{
				Id = id,
				ChannelTitle = channelTitle,
				ExpiresAt = expiresAt,
				GuildIds = guildIds,
				AlreadySeenIds = alreadySeenIds
			};

			// act

			var entity = GetDefaultSubscription(id, channelTitle, expiresAt, guildIds, alreadySeenIds);
			await context.YoutubeSubscriptions.AddAsync(entity);

			await context.SaveChangesAsync();

			var response = await repository.GetSubscriptionByIdOrDefaultAsync(id);

			// assert

			Assert.That(response, Is.Not.Null);
			Assert.That(response, Is.EqualTo(expected).Using(new YoutubeSubscriptionComparer()));
		}

		[Test]
		public async Task YoutubeRepository_ModifyExpiryDateAsync_ModifiesCorrectly()
		{
			// arrange

			var repository = new YoutubeRepository();

			await using var context = new ArkadiaDbContext();

			const string id = "test1";
			const string channelTitle = "coolTitle";
			var expiresAt = DateTime.UtcNow; 
			var guildIds = new[] { "id1", "id2" };
			var alreadySeenIds = new[] { "id1", "id2" };

			var newExpiresAt = DateTime.UtcNow.AddDays(1);

			// act

			var entity = GetDefaultSubscription(id, channelTitle, expiresAt, guildIds, alreadySeenIds);
			await context.YoutubeSubscriptions.AddAsync(entity);

			await context.SaveChangesAsync();

			await repository.ModifyExpiryAsync(id, newExpiresAt);

			var newEntity = await repository.GetSubscriptionByIdOrDefaultAsync(id);
			
			Assert.That(newEntity!.ExpiresAt, Is.EqualTo(newExpiresAt));
		}

		[Test]
		public void YoutubeRepository_GetSubscriptions_ReturnsEmpty_WhenNoSubscriptionsExist()
		{
			
			// arrange
			
			var repository = new YoutubeRepository();
			
			// act

			var subscriptions = repository.GetSubscriptions();
			
			// assert
			
			Assert.That(subscriptions, Is.Empty);
		}
		
		[Test]
		public async Task YoutubeRepository_GetSubscriptions_ReturnsItems_WhenSubscriptionsExist()
		{
			
			// arrange
			
			var repository = new YoutubeRepository();

			using var context = new ArkadiaDbContext();

			var items = new YoutubeSubscription[]
			{
				new YoutubeSubscription
				{
					Id = "1",
					ChannelTitle = "cooltitle",
					ExpiresAt = DateTime.UtcNow.AddDays(10),
					GuildIds = new[] { "guild1" }
				},
				new YoutubeSubscription
				{
					Id = "2",
					ChannelTitle = "coolertitle",
					ExpiresAt = DateTime.UtcNow.AddDays(3),
					GuildIds = new[] { "guild1" }
				}
			};
			
			// act

			foreach (var sub in items)
			{
				await repository.AddSubscriptionAsync(sub.Id, sub.ExpiresAt, sub.GuildIds[0], sub.ChannelTitle);
			}

			var subscriptions = repository.GetSubscriptions().ToArray();

			// assert
			
			Assert.That(subscriptions, Is.Not.Empty);
			Assert.That(subscriptions, Is.EquivalentTo(items).Using(new YoutubeSubscriptionComparer()));
		}

		[Test]
		public async Task YoutubeRepository_AddSubscription_AddsSubscriptionCorrectly()
		{
			
			// arrange

			await using var repo = new YoutubeRepository();

			const string id = "test";
			const string channelTitle = "deez";
			var expiryTime = DateTime.UtcNow.AddMinutes(100);
			var guildId = "541738403230777351";

			var expected = new YoutubeSubscription
			{
				Id = id,
				ChannelTitle = channelTitle,
				ExpiresAt = expiryTime,
				GuildIds = new []{ guildId }
			};
			
			// act

			await repo.AddSubscriptionAsync(id, expiryTime, guildId, channelTitle);

			var result = repo.GetSubscriptions().ToArray();
			
			// assert
			
			Assert.That(result, Has.Exactly(1).Items);
			Assert.That(result.First(), Is.EqualTo(expected).Using(new YoutubeSubscriptionComparer()));
		}
		
		[Test]
		public async Task YoutubeRepository_GetGuildByIdOrDefaultAsync_ReturnsNull_WhenEntryDoesNotExist()
		{
			
			// arrange

			var repository = new YoutubeRepository();
			const string name = "test";

			// act

			var response = await repository.GetGuildByIdOrDefaultAsync(name);

			// assert
			
			Assert.That(response, Is.Null);
		}

		[Test]
		public async Task YoutubeRepository_GetGuildByIdOrDefaultAsync_ReturnsItem_WhenExists()
		{
			// arrange

			var repository = new YoutubeRepository();


			await using var context = new ArkadiaDbContext();

			var guild = CreateDefaultGuild();

			// act
			
			await context.Guilds.AddAsync(guild);

			await context.SaveChangesAsync();

			var response = await repository.GetGuildByIdOrDefaultAsync(guild.Id);

			// assert

			Assert.That(response, Is.Not.Null);
			Assert.That(response!.Id, Is.EqualTo(guild.Id));
			Assert.That(response.YoutubeUploadLiveChannel, Is.EqualTo(guild.YoutubeUploadLiveChannel));
		}

		private Guild CreateDefaultGuild()
		{
			return new Guild
			{
				Id = "1",
				YoutubeUploadNotificationChannel = "test",
				YoutubeUploadNotificationMessage = "test",
				YoutubeUploadLiveChannel = "test",
				YoutubeUploadLiveMessage = "test"
			};
		}
		
		private YoutubeSubscription GetDefaultSubscription(string id, string channelTitle, DateTime expiresAt, string[] guildIds, string[] alreadySeenIds)
		{
			return new YoutubeSubscription
			{
				Id = id,
				ChannelTitle = channelTitle,
				ExpiresAt = expiresAt,
				GuildIds = guildIds,
				AlreadySeenIds = alreadySeenIds
			};
		}
		
		private class YoutubeSubscriptionComparer : IEqualityComparer<YoutubeSubscription>
		{

			public bool Equals(YoutubeSubscription? x, YoutubeSubscription? y)
			{
				if (x is null || y is null) return false;
				return x.Id == y.Id
						&& x.ChannelTitle == y.ChannelTitle
						&& x.ExpiresAt.Equals(y.ExpiresAt)
						&& x.GuildIds.SequenceEqual(y.GuildIds)
						&& x.AlreadySeenIds.SequenceEqual(y.AlreadySeenIds);
			}

			public int GetHashCode(YoutubeSubscription obj)
			{
				var hash = new HashCode();

				foreach (var guildId in obj.GuildIds)
				{
					hash.Add(guildId);
				}

				foreach (var alreadySeenId in obj.AlreadySeenIds)
				{
					hash.Add(alreadySeenId);
				}
				
				hash.Add(obj.Id);
				hash.Add(obj.ChannelTitle);
				hash.Add(obj.ExpiresAt);

				return hash.ToHashCode();
			}
		}
	}
}
