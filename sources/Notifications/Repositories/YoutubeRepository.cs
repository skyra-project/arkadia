using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Cdn.Repositories;
using Database;
using Database.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Notifications.Repositories
{
	public class YoutubeRepository : IYoutubeRepository
	{
		private readonly ArkadiaDbContext _context = new ArkadiaDbContext();

		[ExcludeFromCodeCoverage]
		public ValueTask DisposeAsync()
		{
			return _context.DisposeAsync();
		}

		public ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id)
		{
			return _context.YoutubeSubscriptions.FindAsync(id);
		}

		public ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id)
		{
			return _context.Guilds.FindAsync(id);
		}
	}
}
