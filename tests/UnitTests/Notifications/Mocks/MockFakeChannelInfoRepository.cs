using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.Repositories;

namespace UnitTests.Notifications.Mocks
{
	public class MockFakeChannelInfoRepository : IChannelInfoRepository
	{
		private readonly Queue<string> _channelIds;

		private readonly Queue<string> _channelNames;
		private readonly bool _consume;
		private bool _failNext;

		public MockFakeChannelInfoRepository(string channelId, string channelName, bool consume = false)
		{
			_channelIds = new Queue<string>();
			_channelIds.Enqueue(channelId);
			_channelNames = new Queue<string>();
			_channelNames.Enqueue(channelName);
			_consume = consume;
		}

		public MockFakeChannelInfoRepository(IEnumerable<string> channelIds, IEnumerable<string> channelNames, bool consume = false)
		{
			_channelIds = new Queue<string>(channelIds);
			_channelNames = new Queue<string>(channelNames);
			_consume = consume;
		}

		public Task<(string?, string?)> GetChannelInfoAsync(string _)
		{
			if (_failNext) return Task.FromResult<(string?, string?)>((null, null));

			_failNext = false;

			var channelId = _consume ? _channelIds.Dequeue() : _channelIds.Peek();
			var channelName = _consume ? _channelNames.Dequeue() : _channelNames.Peek();

			return Task.FromResult<(string?, string?)>((channelId, channelName));
		}

		public void SetChannelIds(IEnumerable<string> ids)
		{
			foreach (var id in ids) _channelIds.Enqueue(id);
		}

		public void SetChannelNames(IEnumerable<string> names)
		{
			foreach (var name in names) _channelNames.Enqueue(name);
		}

		public void FailNext()
		{
			_failNext = true;
		}
	}
}
