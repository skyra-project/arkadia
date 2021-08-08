using System.Threading.Tasks;
using Database;
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
			
			Assert.IsNull(response);
		}
	}
}
