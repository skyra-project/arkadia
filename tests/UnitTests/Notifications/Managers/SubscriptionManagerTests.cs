using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
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
	}
}
