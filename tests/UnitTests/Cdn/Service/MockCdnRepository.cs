﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cdn.Repositories;
using Database.Models.Entities;

namespace UnitTests.Cdn.Service
{
	public class MockCdnRepository : ICdnRepository
	{

		private List<CdnEntry> _entries = new List<CdnEntry>();

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}

		public Task<CdnEntry?> GetEntryByNameOrDefaultAsync(string name)
		{
			var entry = _entries.FirstOrDefault(entry => entry.Name == name);
			return Task.FromResult(entry);
		}

		public async ValueTask<CdnEntry> UpsertEntryAsync(string name, string contentType, string eTag, DateTime lastModifiedAt)
		{
			var entry = await GetEntryByNameOrDefaultAsync(name);

			if (entry is null)
			{

				entry = new CdnEntry
				{
					Id = _entries.Count + 1,
					Name = name,
					ContentType = contentType,
					ETag = eTag,
					LastModifiedAt = lastModifiedAt
				};
				
				_entries.Add(entry);
			}
			else
			{
				entry.ContentType = contentType;
				entry.ETag = eTag;
				entry.LastModifiedAt = lastModifiedAt;
			}

			return entry;
		}

		public async ValueTask<CdnEntry?> DeleteEntryAsync(string name)
		{
			var entry = await GetEntryByNameOrDefaultAsync(name);

			if (entry is not null)
			{
				_entries.Remove(entry);
			}

			return entry;
		}
	}
}