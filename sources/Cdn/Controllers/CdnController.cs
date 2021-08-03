using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FileSystem = System.IO.File;
using Shared;

namespace Cdn.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CdnController : ControllerBase
	{
		private const int Seconds = 60;
		private const int Minutes = 60;
		private readonly string BaseAssetLocation;
		private readonly ILogger<CdnController> _logger;

		public CdnController(ILogger<CdnController> logger)
		{
			_logger = logger;
			BaseAssetLocation = Environment.GetEnvironmentVariable("BASE_ASSET_LOCATION") 
								?? throw new EnvironmentVariableMissingException("BASE_ASSET_LOCATION");
		}
		
		[HttpGet("{id}")]
		[ResponseCache(Duration = Seconds * Minutes, Location = ResponseCacheLocation.Client, NoStore = false)]
		public async Task<IActionResult> Get(string name)
		{
			var headers = Request.GetTypedHeaders();
			headers.Date = DateTimeOffset.Now;

			using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstAsync(asset => asset.Name == name);

			if (cdnEntry is null)
			{
				return NotFound();
			}

			// RFC 7232 3.3 - If the content was not modified, a 304 "Not Modified" status should be sent.
			if (!WasModified(cdnEntry))
			{
				// RFC 7232 4.1 - The server generating a 304 response MUST generate any of the following header fields that
				// would have been sent in a 200 (OK) response to the same request: Cache-Control, Content-Location, Date,
				// ETag, Expires, and Vary.
				return NotModified();
			}

			headers.LastModified = new DateTimeOffset(cdnEntry.LastModifiedAt);
			headers.Set("ETag", cdnEntry.ETag);
			
			var fileLocation = Path.Join(BaseAssetLocation, cdnEntry.Id.ToString());

			var exists = FileSystem.Exists(fileLocation);

			if (!exists)
			{
				_logger.LogCritical("File with path {Path} was requested and found in the database, but does not exist", fileLocation);
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}

			var fileStream = FileSystem.OpenRead(fileLocation);

			return File(fileStream, cdnEntry.ContentType);
			
			bool WasModified(CdnEntry asset)
			{
				var headers = Request.GetTypedHeaders();

				// RFC 7232 3.2 - If-None-Match
				if (headers.IfNoneMatch.Any(entry => entry.Tag.Value == asset.ETag))
				{
					return false;
				}

				// RFC 7232 3.3 - If-Modified-Since
				var ifModifiedSince = headers.IfModifiedSince;
				if (!ifModifiedSince.HasValue)
				{
					return true;
				}

				// The origin server SHOULD NOT perform the requested method if the selected representation's last
				// // modification date is earlier than or equal to the date provided in the field-value
				return ifModifiedSince.Value.DateTime < asset.LastModifiedAt;
			}
		}

		private static IActionResult NotModified() => new StatusCodeResult(StatusCodes.Status304NotModified);
	}
}