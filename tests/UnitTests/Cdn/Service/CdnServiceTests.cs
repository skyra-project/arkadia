using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Services;
using CdnService = Cdn.Services.CdnService;

namespace UnitTests.Cdn.Service
{
	[TestFixture]
	[Parallelizable]
	public class CdnServiceTests
	{
		
		[OneTimeSetUp]
		public void Setup() => Environment.SetEnvironmentVariable("BASE_ASSET_LOCATION", "/assets");
		
		[Test]
		public async Task CdnService_Get_ShouldReturnItem_WhenExists()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";

			var content = Encoding.UTF8.GetBytes("hello there");

			var query = new GetRequest
			{
				Name = entryName
			};
			
			// act

			var entity = await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);
			var id = entity.Id;

			var path = $"assets/{id}";
			fileSystem.Directory.CreateDirectory("assets");
			await fileSystem.File.WriteAllBytesAsync(path, content);

			var returnValue = await service.Get(query, null!);
			
			// assert
			
			Assert.AreEqual(CdnResult.Ok, returnValue.Result);
			Assert.AreEqual(content, returnValue.Content.ToByteArray());
		}
		
		[Test]
		public async Task CdnService_Get_ShouldReturnDoesNotExist_WhenDoesNotExist()
		{
			
			// arrange

			Environment.SetEnvironmentVariable("BASE_ASSET_LOCATION", "/assets");

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";

			var query = new GetRequest
			{
				Name = entryName
			};
			
			// act
			
			var returnValue = await service.Get(query, null!);
			
			// assert
			
			Assert.AreEqual(CdnResult.DoesNotExist, returnValue.Result);
		}
		
		[Test]
		public async Task CdnService_Get_ShouldReturnError_WhenExists_ButFileDoesNot()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";

			var query = new GetRequest
			{
				Name = entryName
			};
			
			// act

			var entity = await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);
			var id = entity.Id;

			var returnValue = await service.Get(query, null!);
			
			// assert
			
			Assert.AreEqual(CdnResult.Error, returnValue.Result);
		}
		
		[Test]
		public async Task CdnService_Upsert_ShouldEditItem_WhenExists()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";
			const string contentType = "test/new";

			var oldContent = Encoding.UTF8.GetBytes("hello there");
			var newContent = Encoding.UTF8.GetBytes("general kenobi");

			var upsertQuery = new UpsertRequest
			{
				Name = entryName,
				Content = ByteString.CopyFrom(newContent),
				ContentType = contentType
			};

			var getQuery = new GetRequest
			{
				Name = entryName
			};
			
			// act
			
			// insert the first entry
			await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);
			
			var entity = await repository.GetEntryByNameOrDefaultAsync(entryName);

			// write the first entry to the 'file system'
			
			var id = entity!.Id;
			var path = $"assets/{id}";
			
			fileSystem.Directory.CreateDirectory("assets");
			await fileSystem.File.WriteAllBytesAsync(path, newContent);

			// perform the queries
			
			var upsertResult = await service.Upsert(upsertQuery, null!);
			var getResult = await service.Get(getQuery, null!);
	
			// assert

			Assert.AreEqual(CdnResult.Ok, upsertResult.Result);
			Assert.AreEqual(CdnResult.Ok, getResult.Result);
			Assert.AreEqual(newContent, getResult.Content.ToByteArray());
		}
		
		[Test]
		public async Task CdnService_Delete_ShouldDelete_WhenExists()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";

			var content = Encoding.UTF8.GetBytes("hello there");

			var query = new DeleteRequest
			{
				Name = entryName
			};
			
			// act

			var entity = await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);
			var id = entity.Id;

			var path = $"assets/{id}";
			fileSystem.Directory.CreateDirectory("assets");
			await fileSystem.File.WriteAllBytesAsync(path, content);

			var returnValue = await service.Delete(query, null!);
			var existsOnDisk = fileSystem.File.Exists(path);
			var existsOnDb = await repository.GetEntryByNameOrDefaultAsync(entryName) is not null;
			
			// assert
			
			Assert.AreEqual(CdnResult.Ok, returnValue.Result);
			Assert.IsFalse(existsOnDisk);
			Assert.IsFalse(existsOnDb);
		}
		
		[Test]
		public async Task CdnService_Delete_ShouldReturnDoesNotExist_WhenDoesNotExistInDatabase()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			
			const string entryName = "test1";

			var query = new DeleteRequest
			{
				Name = entryName
			};
			
			// act

			var returnValue = await service.Delete(query, null!);

			// assert
			
			Assert.AreEqual(CdnResult.DoesNotExist, returnValue.Result);
		}
		
		[Test]
		public async Task CdnService_Delete_ShouldReturnError_WhenFileDoesNotExist()
		{
			
			// arrange

			await using var repository = new MockCdnRepository();
			
			var factory = new MockCdnRepositoryFactory(repository);
			var fileSystem = new MockFileSystem();
			var service = new CdnService(new NullLogger<CdnService>(), fileSystem, factory);
			

			const string entryName = "test1";

			var query = new DeleteRequest
			{
				Name = entryName
			};
			
			// act

			var _ = await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);

			var returnValue = await service.Delete(query, null!);

			// assert
			
			Assert.AreEqual(CdnResult.Error, returnValue.Result);
		}
	}
}
