using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cdn.Factories;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Services;
using Shared;

namespace Cdn.Services
{
	public class CdnService : global::Services.CdnService.CdnServiceBase
	{
		private readonly string _baseAssetLocation;
		private readonly ILogger<CdnService> _logger;
		private readonly IFileSystem _fileSystem;
		private readonly ICdnRepositoryFactory _repositoryFactory;

		public CdnService(ILogger<CdnService> logger, IFileSystem fileSystem, ICdnRepositoryFactory repositoryFactory)
		{
			_logger = logger;
			_fileSystem = fileSystem;
			_repositoryFactory = repositoryFactory;
			_baseAssetLocation = Environment.GetEnvironmentVariable("BASE_ASSET_LOCATION")
								?? throw new EnvironmentVariableMissingException("BASE_ASSET_LOCATION");
		}

		public override async Task<CdnFileResponse> Get(GetRequest request, ServerCallContext _)
		{
			await using var repository = _repositoryFactory.GetRepository();

			var cdnEntry = await repository.GetEntryByNameOrDefaultAsync(request.Name);

			if (cdnEntry is null)
			{
				return DoesNotExistFile();
			}

			var path = Path.Join(_baseAssetLocation, cdnEntry.Id.ToString());
			var exists = _fileSystem.File.Exists(path);

			if (!exists)
			{
				_logger.LogCritical("File with path {Path} was requested and found in the database, but does not exist", path);

				return ErrorFile();
			}

			var stream = _fileSystem.File.OpenRead(path);

			return OkFile(stream);
		}

		public override async Task<CdnResponse> Upsert(UpsertRequest request, ServerCallContext _)
		{
			await using var factory = _repositoryFactory.GetRepository();

			var content = request.Content.ToByteArray();

			var eTag = GetETag(content);
			
			var cdnEntry = await factory.UpsertEntryAsync(request.Name, request.ContentType, eTag, DateTime.Now);
			
			var path = GetPath(cdnEntry.Id);

			await _fileSystem.File.WriteAllBytesAsync(path, content);

			return Ok();
		}

		public override async Task<CdnResponse> Delete(DeleteRequest request, ServerCallContext _)
		{
			await using var repository = _repositoryFactory.GetRepository();

			var cdnEntry = await repository.DeleteEntryAsync(request.Name);

			if (cdnEntry is null)
			{
				_logger.LogInformation("Attempting to delete entry with name {Name}, but it does not exist in the database", request.Name);

				return DoesNotExist();
			}

			var path = GetPath(cdnEntry.Id);

			var exists = _fileSystem.File.Exists(path);

			if (!exists)
			{
				_logger.LogCritical("Attempting to delete entry with name {Name} but the file does not exist", request.Name);

				return Error();
			}

			_fileSystem.File.Delete(path);

			return Ok();
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

		private static CdnResponse Ok() => new CdnResponse { Result = CdnResult.Ok };
		private static CdnResponse DoesNotExist() => new CdnResponse { Result = CdnResult.DoesNotExist };
		private static CdnResponse Error() => new CdnResponse { Result = CdnResult.Error };
		private static CdnFileResponse OkFile(Stream stream) => new CdnFileResponse { Result = CdnResult.Ok, Content = ByteString.FromStream(stream) };
		private static CdnFileResponse DoesNotExistFile() => new CdnFileResponse { Result = CdnResult.DoesNotExist };
		private static CdnFileResponse ErrorFile() => new CdnFileResponse { Result = CdnResult.Error };
	}
}
