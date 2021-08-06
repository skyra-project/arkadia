using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Services;
using CdnService = Cdn.Services.CdnService;

namespace UnitTests.Cdn.Service
{
	[TestFixture]
	public class CdnServiceTests
	{
		[Test]
		public async Task CdnService_Get_ShouldReturnItem_WhenExists()
		{
			
			// arrange

			Environment.SetEnvironmentVariable("BASE_ASSET_LOCATION", "/assets");

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

			var entity = await repository.UpsertEntryAsync(entryName, "test/default", "test_tag", DateTime.Now);
			var id = entity.Id;

			var returnValue = await service.Get(query, null!);
			
			// assert
			
			Assert.AreEqual(CdnResult.Error, returnValue.Result);
		}
	}
}
