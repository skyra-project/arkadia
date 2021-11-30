using System.Threading.Tasks;

namespace Notifications.Repositories
{
	public interface IChannelInfoRepository
	{
		Task<(string?, string?)> GetChannelInfoAsync(string channelUrl);
	}
}
