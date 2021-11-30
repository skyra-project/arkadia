using System.Threading.Tasks;
using Notifications.Clients;
using Notifications.Errors;
using Remora.Results;

namespace UnitTests.Notifications.Mocks
{
	public class MockFailingPubSubHubClient : IPubSubClient
	{

		public Task<Result> SubscribeAsync(string channelId)
		{
			return Task.FromResult(Result.FromError(new PubSubHubBubError()));
		}

		public Task<Result> UnsubscribeAsync(string channelId)
		{
			return Task.FromResult(Result.FromError(new PubSubHubBubError()));
		}
	}
}
