using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services;
using Shared;
using CdnServiceBase = Services.CdnService.CdnServiceBase;

namespace Cdn.Services
{
	public class CdnService : CdnServiceBase
	{
		private ILogger<CdnService> _logger;
		private readonly string BaseAssetLocation;

		public CdnService(ILogger<CdnService> logger)
		{
			_logger = logger;
			BaseAssetLocation = Environment.GetEnvironmentVariable("BASE_ASSET_LOCATION") 
								?? throw new EnvironmentVariableMissingException("BASE_ASSET_LOCATION");
		}

		public override async Task<CdnFileResponse> Get(GetRequest request, ServerCallContext _)
		{
			using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstAsync(entry => entry.Name == request.Name);

			if (cdnEntry is null)
			{
				return new CdnFileResponse
				{
					Result = CdnResult.DoesNotExist
				};
			}

			var path = Path.Join(BaseAssetLocation, cdnEntry.Id.ToString());
			var exists = File.Exists(path);

			if (!exists)
			{
				
				_logger.LogCritical("File with path {Path} was requested and found in the database, but does not exist", path);
				
				return new CdnFileResponse
				{
					Result = CdnResult.Error
				};
			}

			var stream = File.OpenRead(path);

			return new CdnFileResponse
			{
				Result = CdnResult.Ok,
				Content = await ByteString.FromStreamAsync(stream)
			};
		}

		public override async Task<CdnResponse> Create(CreateRequest request, ServerCallContext _)
		{
			using var context = new ArkadiaDbContext();

			var cdnEntryExists = await context.CdnEntries.AnyAsync(entry => entry.Name == request.Name);

			if (cdnEntryExists)
			{
				_logger.LogInformation("Attempted to Create() with entry name {Name} but that already exists", request.Name);
				return new CdnResponse
				{
					Result = CdnResult.Duplicate
				};
			}

			using var md5 = MD5.Create();

			var etagBytes = md5.ComputeHash(request.Content.ToByteArray());
			var etagString = BitConverter.ToString(etagBytes).Replace("-", "");
			
			var newEntry = new CdnEntry
			{
				Name = request.Name,
				ContentType = request.ContentType,
				LastModifiedAt = DateTime.Now, 
				ETag = etagString
			};

			var entity = await context.CdnEntries.AddAsync(newEntry);

			var path = Path.Join(BaseAssetLocation, entity.Entity.Id.ToString());

			await File.WriteAllBytesAsync(path, request.Content.ToByteArray());
			
			await context.SaveChangesAsync();
			
			return new CdnResponse
			{
				Result = CdnResult.Ok
			};
		}

		public override async Task<CdnResponse> Edit(EditRequest request, ServerCallContext _)
		{
			using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstAsync(entry => entry.Name == request.Name);

			if (cdnEntry is null)
			{
				return new CdnResponse
				{
					Result = CdnResult.DoesNotExist
				};
			}

			var path = Path.Join(BaseAssetLocation, cdnEntry.Id.ToString());

			var exists = File.Exists(path);

			if (!exists)
			{
				
				_logger.LogCritical("File with path {Path} does not exist, when attempting to update contents", path);
				
				return new CdnResponse
				{
					Result = CdnResult.Error
				};
			}

			await File.WriteAllBytesAsync(path, request.Content.ToByteArray());
			
			cdnEntry.LastModifiedAt = DateTime.Now;

			await context.SaveChangesAsync();

			return new CdnResponse
			{
				Result = CdnResult.Ok
			};
		}

		public override async Task<CdnResponse> Delete(DeleteRequest request, ServerCallContext _)
		{
			using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstAsync(entry => entry.Name == request.Name);

			if (cdnEntry is null)
			{
				_logger.LogInformation("Attempting to delete entry with name {Name}, but it does not exist in the database", request.Name);

				return new CdnResponse
				{
					Result = CdnResult.DoesNotExist
				};
			}

			var path = Path.Join(BaseAssetLocation, cdnEntry.Id.ToString());

			var exists = File.Exists(path);

			if (!exists)
			{
				_logger.LogCritical("Attempting to delete entry with name {Name} but the file does not exist", request.Name);

				return new CdnResponse
				{
					Result = CdnResult.Error
				};
			}
			
			File.Delete(path);

			context.CdnEntries.Remove(cdnEntry);
			await context.SaveChangesAsync();
			
			return new CdnResponse
			{
				Result = CdnResult.Ok
			};
		}
	}
}