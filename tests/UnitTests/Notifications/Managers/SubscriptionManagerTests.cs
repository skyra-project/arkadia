using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Notifications.Clients;
using Notifications.Managers;
using NUnit.Framework;

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
				new BrowsingContext(),
				new MockNotificationRepositoryFactory(mockRepo));

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
				null!,
				new MockNotificationRepositoryFactory(mockRepo));

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
				null!,
				new MockNotificationRepositoryFactory(mockRepo));

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
				null!,
				new MockNotificationRepositoryFactory(mockRepo));

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
	}
}
