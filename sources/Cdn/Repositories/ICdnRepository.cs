using System;
using System.Threading.Tasks;
using Database.Models.Entities;

namespace Cdn.Repositories;

public interface ICdnRepository : IAsyncDisposable
{
	Task<CdnEntry?> GetEntryByNameOrDefaultAsync(string name);
	ValueTask<CdnEntry> UpsertEntryAsync(string name, string contentType, string eTag, DateTime lastModifiedAt);
	ValueTask<CdnEntry?> DeleteEntryAsync(string name);
}
