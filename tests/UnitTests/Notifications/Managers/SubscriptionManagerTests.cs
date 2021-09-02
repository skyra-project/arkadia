using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Text;
using Database.Models.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Notifications.Clients;
using Notifications.Errors;
using Notifications.Managers;
using Notifications.Repositories;
using NUnit.Framework;
using UnitTests.Notifications.Mocks;

namespace UnitTests.Notifications.Managers
{
	[TestFixture]
	public class SubscriptionManagerTests
	{
		[Test]
		public async Task SubscriptionManager_StartAsync_SetsUpManagerCorrectly()
		{
			
			// arrange

			var mockRepo = new MockNotificationRepository();

			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				new MockNotificationRepositoryFactory(mockRepo),
				null!,
				null!);

			const string id = "test";
			var expiresAt = DateTime.Now;
			var guildIds = new[] { "test1", "test2" };
			const string channelTitle = "cooltitle";
			
			// act


			await mockRepo.AddSubscriptionAsync(id, expiresAt, guildIds, channelTitle);

			await manager.StartAsync();

			// assert
			
			Assert.That(manager.ResubTimer.Enabled, Is.True);
			Assert.That(manager.ResubscribeTimes, Has.Exactly(1).Items);
			Assert.That(manager.ResubscribeTimes.Keys.First(), Is.EqualTo(id));
			Assert.That(manager.ResubscribeTimes.Values.First(), Is.EqualTo(expiresAt));
			
			manager.ResubTimer.Stop();
			manager.ResubTimer.Dispose();
		}
		
		[Test]
		public async Task SubscriptionManager_UpdateSubscriptionSettings_ReturnsErrorWithNoParameters()
		{
			
			// arrange

			var mockRepo = new MockNotificationRepository();

			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				new MockNotificationRepositoryFactory(mockRepo),
				null!,
				null!);

			const string id = "test";

			// act

			var result = await manager.UpdateSubscriptionSettingsAsync(id);

			// assert

			Assert.That(result.IsSuccess, Is.False);
			
			manager.ResubTimer.Stop();
			manager.ResubTimer.Dispose();
		}
		
		[Test]
		public async Task SubscriptionManager_UpdateSubscriptionSettings_UpdatesCorrectly_WhenGuildAlreadyExists()
		{
			
			// arrange

			var mockRepo = new MockNotificationRepository();

			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				new MockNotificationRepositoryFactory(mockRepo),
				null!,
				null!);

			const string id = "test";
			const string uploadMessage = "hello!";
			
			// act


			await mockRepo.UpsertGuildAsync(id, null, null, null, null);

			await manager.UpdateSubscriptionSettingsAsync(id, uploadMessage: uploadMessage);

			// assert

			var guild = await mockRepo.GetGuildByIdOrDefaultAsync(id);

			Assert.That(guild!.YoutubeUploadNotificationMessage, Is.EqualTo(uploadMessage));
			
			manager.ResubTimer.Stop();
			manager.ResubTimer.Dispose();
		}
		
		[Test]
		public async Task SubscriptionManager_UpdateSubscriptionSettings_InsertsCorrectly_WhenGuildDoesNotExist()
		{
			
			// arrange

			var mockRepo = new MockNotificationRepository();

			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				new MockNotificationRepositoryFactory(mockRepo),
				null!,
				null!);

			const string id = "test";
			const string uploadMessage = "hello!";
			
			// act

			var firstGuild = await mockRepo.GetGuildByIdOrDefaultAsync(id);

			await mockRepo.UpsertGuildAsync(id, null, null, null, null);

			await manager.UpdateSubscriptionSettingsAsync(id, uploadMessage: uploadMessage);

			var secondGuild = await mockRepo.GetGuildByIdOrDefaultAsync(id);
			
			// assert

			Assert.That(firstGuild, Is.Null);
			Assert.That(secondGuild, Is.Not.Null);
			Assert.That(secondGuild!.YoutubeUploadNotificationMessage, Is.EqualTo(uploadMessage));
			
			manager.ResubTimer.Stop();
			manager.ResubTimer.Dispose();
		}

		[Test]
		public async Task SubscriptionManager_IsSubscribed_ReturnsChannelInfoRetrievalError_WhenRetrievalFails()
		{
			// arrange

			var nullChannelInfoRepo = new MockNullReturningChannelInfoRepository();
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				null!,
				nullChannelInfoRepo,
				null!);
			
			// act

			var subscriptionExistsResult = await manager.IsSubscribedAsync("1", "notaurl");
			
			// assert
			
			Assert.That(subscriptionExistsResult.IsSuccess, Is.False);
			Assert.That(subscriptionExistsResult.Error, Is.InstanceOf<ChannelInfoRetrievalError>());

		}

		[Test]
		public async Task SubscriptionManager_IsSubscribed_ReturnsTrue_WhenSubscriptionExists()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";
			
			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), new[] { guildId }, channelTitle);

			var subscriptionExistsResult = await manager.IsSubscribedAsync(guildId, "notaurl");
			
			// assert
			
			Assert.That(subscriptionExistsResult.IsSuccess, Is.True);
			Assert.That(subscriptionExistsResult.Entity, Is.True);

		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsSuccess_WhenSubscriptionExists()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";
			
			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), new[] { guildId }, channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.True);

		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsError_WhenIsSubscribedFails()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockNullReturningChannelInfoRepository();
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), new[] { guildId }, channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);

		}

		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsUnconfiguredError_WhenGuildIsNotPresent()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
		}

		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsUnconfiguredError_WhenGuildHasNoUploadChannel()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);

			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);

			// act

			await notificationRepo.UpsertGuildAsync(guildId, null, "test", "1", "test");
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);

			// assert

			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<UnconfiguredError>());
		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsUnconfiguredError_WhenGuildHasNoUploadMessage()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", null, "1", "test");
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<UnconfiguredError>());
		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsUnconfiguredError_WhenGuildHasNoLiveChannel()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", null, "test");
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync("youtubeChannelId", guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<UnconfiguredError>());
		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsUnconfiguredError_WhenGuildHasNoLiveMessage()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", "1", null);
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<UnconfiguredError>());
		}

		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsChannelInfoRetrievalError_WhenChannelInfoNotFound()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle, 1);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", "1", "test");
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<ChannelInfoRetrievalError>());
		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_AddsGuildToSubscription_WhenGuildIsNotAlreadyInSubscriptions()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);
			
			var manager = new SubscriptionManager(null!,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", "1", "test");
			await notificationRepo.AddSubscriptionAsync(youtubeChannelId, DateTime.Now.AddDays(10), Array.Empty<string>(), channelTitle);

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.True);
			Assert.That(notificationRepo.GetSubscriptions(), Has.Exactly(1).Property("GuildIds").Contains(guildId));
		}

		[Test]
		public async Task SubscriptionManager_Subscribe_ReturnsPubSubHubBubError_WhenClientFails()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);

			var mockFailingPubSubClient = new MockFailingPubSubHubClient();
			
			var manager = new SubscriptionManager(mockFailingPubSubClient,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				null!);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", "1", "test");

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);
			
			// assert
			
			Assert.That(subscriptionResult.IsSuccess, Is.False);
			Assert.That(subscriptionResult.Error, Is.InstanceOf<PubSubHubBubError>());
		}
		
		[Test]
		public async Task SubscriptionManager_Subscribe_UpdatesDataCorrectly()
		{
			// arrange

			const string youtubeChannelId = "123";
			const string guildId = "1";
			const string channelTitle = "generalKenobi";

			var channelInfoRepo = new MockFakeChannelInfoRepository(youtubeChannelId, channelTitle);
			var notificationRepo = new MockNotificationRepository();
			var mockNotificationFactory = new MockNotificationRepositoryFactory(notificationRepo);

			var now = DateTime.UtcNow;
			var fiveDays = now.AddDays(5);
			
			var mockDateTimeRepo = new MockDateTimeRepository(now);
			
			var mockPubSubClient = new MockPubSubClient();
			
			var manager = new SubscriptionManager(mockPubSubClient,
				new NullLogger<SubscriptionManager>(),
				mockNotificationFactory,
				channelInfoRepo,
				mockDateTimeRepo);
			
			// act

			await notificationRepo.UpsertGuildAsync(guildId, "1", "test", "1", "test");

			var subscriptionResult = await manager.SubscribeAsync(youtubeChannelId, guildId);

			var expected = new YoutubeSubscription
			{
				ExpiresAt = fiveDays,
				AlreadySeenIds = Array.Empty<string>(),
				ChannelTitle = channelTitle,
				Id = youtubeChannelId,
				GuildIds = new[] { guildId }
			};
			
			// assert

			Assert.That(subscriptionResult.IsSuccess, Is.True);
			Assert.That(manager.ResubscribeTimes, Does.ContainKey(youtubeChannelId).WithValue(fiveDays));
			Assert.That(notificationRepo.GetSubscriptions(), Has.Exactly(1).EqualTo(expected).Using(new YoutubeSubscriptionComparer()));
		}
	}

	internal class YoutubeSubscriptionComparer : IEqualityComparer<YoutubeSubscription>
	{

		public bool Equals(YoutubeSubscription x, YoutubeSubscription y)
		{
			if (ReferenceEquals(x, y)) return true;
			return x.Id == y.Id 
					&& x.AlreadySeenIds.Equals(y.AlreadySeenIds) 
					&& x.ExpiresAt.Equals(y.ExpiresAt) 
					&& x.GuildIds.SequenceEqual(y.GuildIds) 
					&& x.ChannelTitle == y.ChannelTitle;
		}

		public int GetHashCode(YoutubeSubscription obj)
		{
			return HashCode.Combine(obj.Id, obj.AlreadySeenIds, obj.ExpiresAt, obj.GuildIds, obj.ChannelTitle);
		}
	}
}
