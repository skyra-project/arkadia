using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Database;
using Database.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdn.Repositories
{
	public class CdnRepository : ICdnRepository
	{
		private readonly ILogger<CdnRepository> _logger;
		private readonly ArkadiaDbContext _context = new ArkadiaDbContext();

		public CdnRepository(ILogger<CdnRepository> logger)
		{
			_logger = logger;
		}

		[ExcludeFromCodeCoverage]
		public ValueTask DisposeAsync() => _context.DisposeAsync();

		public Task<CdnEntry?> GetEntryByNameOrDefaultAsync(string name)
		{
			return _context.CdnEntries.FirstOrDefaultAsync(entry => entry.Name == name);
		}

		public async ValueTask<CdnEntry> UpsertEntryAsync(string name, string contentType, string eTag, DateTime lastModifiedAt)
		{
			var entry = await GetEntryByNameOrDefaultAsync(name);
			
			if (entry is null) // insert
			{
				var entryEntity = await _context.CdnEntries.AddAsync(new CdnEntry
				{
					Name = name,
					ContentType = contentType,
					ETag = eTag,
					LastModifiedAt = lastModifiedAt
				});

				entry = entryEntity.Entity;

				_logger.LogTrace("Creating new entry with name {Name}", name);
			}
			else // update
			{
				entry.ContentType = contentType;
				entry.LastModifiedAt = lastModifiedAt;
				entry.ETag = eTag;

				_logger.LogTrace("Updating entry with name {Name}", name);
			}

			await _context.SaveChangesAsync();
			return entry;
		}

		public async ValueTask<CdnEntry?> DeleteEntryAsync(string name)
		{
			var entry = await _context.CdnEntries.FirstOrDefaultAsync(entry => entry.Name == name);

			if (entry is not null)
			{
				_context.CdnEntries.Remove(entry);
				await _context.SaveChangesAsync();
			}
			
			return entry;
		}
	}
}
