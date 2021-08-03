using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
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

		public override Task<CdnResponse> Create(CreateRequest request, ServerCallContext _)
		{
			using var context = new ArkadiaDbContext();
			
		}
	}
}