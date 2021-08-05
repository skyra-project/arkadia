﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using FileSystem = System.IO.File;

namespace Cdn.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CdnController : ControllerBase
	{
		private const int Seconds = 60;
		private const int Minutes = 60;
		private readonly ILogger<CdnController> _logger;
		private readonly string _baseAssetLocation;

		public CdnController(ILogger<CdnController> logger)
		{
			_logger = logger;
			_baseAssetLocation = Environment.GetEnvironmentVariable("BASE_ASSET_LOCATION")
								?? throw new EnvironmentVariableMissingException("BASE_ASSET_LOCATION");
		}

		[HttpGet("{name}")]
		[ResponseCache(Duration = Seconds * Minutes, Location = ResponseCacheLocation.Client, NoStore = false)]
		public async Task<IActionResult> Get(string name)
		{
			var requestHeaders = Request.GetTypedHeaders();
			requestHeaders.Date = DateTimeOffset.Now;

			await using var context = new ArkadiaDbContext();

			var cdnEntry = await context.CdnEntries.FirstOrDefaultAsync(asset => asset.Name == name);

			if (cdnEntry is null)
			{
				return NotFound();
			}

			// RFC 7232 3.3 - If the content was not modified, a 304 "Not Modified" status should be sent.
			if (!WasModified(cdnEntry, requestHeaders))
			{
				// RFC 7232 4.1 - The server generating a 304 response MUST generate any of the following header fields that
				// would have been sent in a 200 (OK) response to the same request: Cache-Control, Content-Location, Date,
				// ETag, Expires, and Vary.
				return NotModified();
			}

			var fileLocation = Path.Join(_baseAssetLocation, cdnEntry.Id.ToString());

			var exists = FileSystem.Exists(fileLocation);

			if (!exists)
			{
				_logger.LogCritical("File with path {Path} was requested and found in the database, but does not exist", fileLocation);
				return InternalError();
			}

			var fileStream = FileSystem.OpenRead(fileLocation);

			Response.Headers.Add("Last-Modified", cdnEntry.LastModifiedAt.ToUniversalTime().ToString("R"));
			Response.Headers.Add("ETag", cdnEntry.ETag);

			return File(fileStream, cdnEntry.ContentType);

			bool WasModified(CdnEntry asset, RequestHeaders headers)
			{
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

		private static IActionResult NotModified()
		{
			return new StatusCodeResult(StatusCodes.Status304NotModified);
		}

		private static IActionResult InternalError()
		{
			return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		}
	}
}