using System;
using Notifications.Repositories;

namespace UnitTests.Notifications.Mocks
{
	public class MockDateTimeRepository : IDateTimeRepository
	{

		private readonly DateTime _instance;

		public MockDateTimeRepository(DateTime instance)
		{
			_instance = instance;
		}

		public DateTime GetTime()
		{
			return _instance;
		}
	}
}
