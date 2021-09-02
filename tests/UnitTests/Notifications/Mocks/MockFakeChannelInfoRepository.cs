using System.Threading.Tasks;
using Notifications.Repositories;

namespace UnitTests.Notifications.Mocks
{
	public class MockFakeChannelInfoRepository : IChannelInfoRepository
	{

		private readonly string _channelName;
		private readonly string _channelId;
		private int _failOnCall;

		public MockFakeChannelInfoRepository(string channelId, string channelName, int failOnCall = -1)
		{
			_channelId = channelId;
			_channelName = channelName;
			_failOnCall = failOnCall;
		}

		public Task<(string?, string?)> GetChannelInfoAsync(string channelUrl)
		{
			if (_failOnCall == 0)
			{
				return Task.FromResult<(string?, string?)>((null, null));
			}

			_failOnCall -= 1;

			return Task.FromResult<(string?, string?)>((_channelId, _channelName));
		}
	}
}
