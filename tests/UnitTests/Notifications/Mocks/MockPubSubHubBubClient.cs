using System.Threading.Tasks;
using Notifications.Clients;
using Remora.Results;

namespace UnitTests.Notifications.Mocks;

public class MockPubSubClient : IPubSubClient
{
	public Task<Result> SubscribeAsync(string channelId)
	{
		return Task.FromResult(Result.FromSuccess());
	}

	public Task<Result> UnsubscribeAsync(string channelId)
	{
		return Task.FromResult(Result.FromSuccess());
	}
}
