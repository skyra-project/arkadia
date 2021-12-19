using System.Threading.Tasks;
using Notifications.Repositories;

namespace UnitTests.Notifications.Mocks;

public class MockNullReturningChannelInfoRepository : IChannelInfoRepository
{
	public Task<(string?, string?)> GetChannelInfoAsync(string channelUrl)
	{
		return Task.FromResult<(string?, string?)>((null, null));
	}
}
