using System;
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
			// NOTE: do not remove .Date - for some reason, something strange is happening without it.
			// the date given BACK from postgres is a few milliseconds off (?)
			var expiresAt = DateTime.Now.Date; 
			var guildIds = new[] { "id1", "id2" };
			var alreadySeenIds = new[] { "id1", "id2" };

			// act
			
			await context.YoutubeSubscriptions.AddAsync(new YoutubeSubscription
			{
				Id = id,
				ChannelTitle = channelTitle,
				ExpiresAt = expiresAt,
				GuildIds = guildIds,
				AlreadySeenIds = alreadySeenIds
			});

			await context.SaveChangesAsync();

			var response = await repository.GetSubscriptionByIdOrDefaultAsync(id);

			// assert

			Assert.That(response, Is.Not.Null);
			Assert.That(response!.Id, Is.EqualTo(id));
			Assert.That(response.ChannelTitle, Is.EqualTo(channelTitle));
			Assert.That(response.ExpiresAt, Is.EqualTo(expiresAt));
			Assert.That(response.GuildIds, Is.EqualTo(guildIds));
			Assert.That(response.AlreadySeenIds, Is.EqualTo(alreadySeenIds));
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
	}
}
