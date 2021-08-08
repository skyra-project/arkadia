using System;
using System.Threading.Tasks;
using Database.Models.Entities;

namespace Notifications.Repositories
{
	public interface IYoutubeRepository : IAsyncDisposable
	{
		ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id);
		ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id);
	}
}
