using System.Threading.Tasks;
using Remora.Results;

namespace Notifications.Clients
{
	public interface IPubSubClient
	{
		Task<Result> SubscribeAsync(string channelId);
		Task<Result> UnsubscribeAsync(string channelId);
	}
}
