using System;
using System.IO;
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

namespace Cdn.Services
{
	public class CdnService : global::Services.CdnService.CdnServiceBase
	{
		private readonly string _baseAssetLocation;
		private readonly ILogger<CdnService> _logger;

		public CdnService(ILogger<CdnService> logger)
		{
			_logger = logger;
			_baseAssetLocation = Environment.GetEnvironmentVariable("BASE_ASSET_LOCATION")
								?? throw new EnvironmentVariableMissingException("BASE_ASSET_LOCATION");
		}

		public override async Task<CdnFileResponse> Get(GetRequest request, ServerCallContext _)
		{
			await using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstOrDefaultAsync(entry => entry.Name == request.Name);

			if (cdnEntry is null)
			{
				return new CdnFileResponse
				{
					Result = CdnResult.DoesNotExist
				};
			}

			var path = Path.Join(_baseAssetLocation, cdnEntry.Id.ToString());
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

		public override async Task<CdnResponse> Upsert(UpsertRequest request, ServerCallContext _)
		{
			await using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstOrDefaultAsync(entry => entry.Name == request.Name);

			var content = request.Content.ToByteArray();

			var eTag = GetETag(content);

			if (cdnEntry is null) // insert
			{
				var entryEntity = await context.CdnEntries.AddAsync(new CdnEntry
				{
					Name = request.Name,
					ContentType = request.ContentType,
					ETag = eTag,
					LastModifiedAt = DateTime.Now
				});

				cdnEntry = entryEntity.Entity;

				_logger.LogTrace("Creating new entry with name {Name}", request.Name);
			}
			else // update
			{
				cdnEntry.ContentType = request.ContentType;
				cdnEntry.LastModifiedAt = DateTime.Now;
				cdnEntry.ETag = eTag;

				_logger.LogTrace("Updating entry with name {Name}", request.Name);
			}

			var path = GetPath(cdnEntry.Id);

			await context.SaveChangesAsync();

			await File.WriteAllBytesAsync(path, content);

			return new CdnResponse
			{
				Result = CdnResult.Ok
			};
		}

		public override async Task<CdnResponse> Delete(DeleteRequest request, ServerCallContext _)
		{
			await using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstOrDefaultAsync(entry => entry.Name == request.Name);

			if (cdnEntry is null)
			{
				_logger.LogInformation("Attempting to delete entry with name {Name}, but it does not exist in the database", request.Name);

				return new CdnResponse
				{
					Result = CdnResult.DoesNotExist
				};
			}

			var path = GetPath(cdnEntry.Id);

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

		private string GetPath(long id)
		{
			return Path.Join(_baseAssetLocation, id.ToString());
		}

		private string GetETag(byte[] content)
		{
			using var md5 = MD5.Create();

			var etagBytes = md5.ComputeHash(content);
			var etagString = BitConverter.ToString(etagBytes).Replace("-", "");

			return etagString;
		}
	}
}
