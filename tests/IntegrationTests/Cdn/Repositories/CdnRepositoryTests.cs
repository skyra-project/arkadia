using System;
using System.Threading.Tasks;
using Cdn.Repositories;
using Database;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace IntegrationTests.Cdn.Repositories
{
	[TestFixture]
	public class CdnRepositoryTests
	{

		[SetUp]
		public async Task Setup()
		{
			await using var context = new ArkadiaDbContext();

			foreach (var entry in context.CdnEntries) context.Remove(entry);

			await context.SaveChangesAsync();
		}

		[Test]
		public async Task CdnRepository_GetEntryByNameOrDefaultAsync_ReturnsNull_WhenItemDoesNotExist()
		{
			// arrange

			await using var repo = new CdnRepository(new NullLogger<CdnRepository>());

			// act

			var response = await repo.GetEntryByNameOrDefaultAsync("test");

			// assert

			Assert.IsNull(response);
		}

		[Test]
		public async Task CdnRepository_GetEntryByNameOrDefaultAsync_ReturnsItem_WhenItemDoesExist()
		{
			// arrange

			await using var repo = new CdnRepository(new NullLogger<CdnRepository>());

			// act

			const string name = "test";
			const string etag = "foobar";
			const string contentType = "test/unit";
			var updated = DateTime.Now;

			await repo.UpsertEntryAsync(name, contentType, etag, updated);

			var response = await repo.GetEntryByNameOrDefaultAsync("test");

			// assert

			Assert.IsNotNull(response);
			Assert.That(response!.Name, Is.EqualTo(name));
			Assert.That(response.ETag, Is.EqualTo(etag));
			Assert.That(response.ContentType, Is.EqualTo(contentType));
			Assert.That(response.LastModifiedAt, Is.EqualTo(updated));
		}

		[Test]
		public async Task CdnRepository_UpsertEntryAsync_InsertsNew_WhenDoesNotExist()
		{
			// arrange

			await using var repo = new CdnRepository(new NullLogger<CdnRepository>());

			// act

			const string name = "test";
			const string etag = "foobar";
			const string contentType = "test/unit";
			var updated = DateTime.Now;

			var response = await repo.UpsertEntryAsync(name, contentType, etag, updated);


			// assert

			Assert.IsNotNull(response);
			Assert.That(response.Name, Is.EqualTo(name));
			Assert.That(response.ETag, Is.EqualTo(etag));
			Assert.That(response.ContentType, Is.EqualTo(contentType));
			Assert.That(response.LastModifiedAt, Is.EqualTo(updated));
		}

		[Test]
		public async Task CdnRepository_UpsertEntryAsync_Modifies_WhenDoesExist()
		{
			// arrange

			await using var repo = new CdnRepository(new NullLogger<CdnRepository>());

			// act

			const string name = "test";
			const string etag = "foobar";
			const string contentType = "test/unit";
			var updated = DateTime.Now;

			const string newETag = "hellothere";
			const string newContentType = "test/new";
			var newUpdated = DateTime.Now.AddDays(1);

			var firstResponse = await repo.UpsertEntryAsync(name, contentType, etag, updated);
			var secondResponse = await repo.UpsertEntryAsync(name, newContentType, newETag, newUpdated);

			// assert

			Assert.IsNotNull(firstResponse);
			Assert.IsNotNull(secondResponse);
			Assert.That(secondResponse.Id, Is.EqualTo(firstResponse.Id));
			Assert.That(secondResponse.ETag, Is.EqualTo(newETag));
			Assert.That(secondResponse.ContentType, Is.EqualTo(newContentType));
			Assert.That(secondResponse.LastModifiedAt, Is.EqualTo(newUpdated));
		}

		[Test]
		public async Task CdnRepository_Delete_DeletesItem_WhenDoesExist()
		{
			// arrange

			await using var repo = new CdnRepository(new NullLogger<CdnRepository>());

			const string name = "test";
			const string etag = "foobar";
			const string contentType = "test/unit";
			var updated = DateTime.Now;

			// act

			await repo.UpsertEntryAsync(name, contentType, etag, updated);
			var deleteResponse = await repo.DeleteEntryAsync(name);
			var getResponse = await repo.GetEntryByNameOrDefaultAsync(name);

			Assert.IsNotNull(deleteResponse);
			Assert.IsNull(getResponse);
		}
	}
}
