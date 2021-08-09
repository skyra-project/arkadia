using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models.Entities;

namespace Notifications.Repositories
{
	public interface IYoutubeRepository : IAsyncDisposable
	{
		ValueTask<YoutubeSubscription?> GetSubscriptionByIdOrDefaultAsync(string id);
		ValueTask<Guild?> GetGuildByIdOrDefaultAsync(string id);
		Task ModifyExpiryAsync(string id, DateTime newTime);
		IEnumerable<YoutubeSubscription> GetSubscriptions();
		Task AddSubscriptionAsync(string id, DateTime expiresAt, string[] guildIds, string channelTitle);
	}
}
