using Notifications.Factories;
using Notifications.Repositories;

namespace UnitTests.Notifications
{
	public class MockNotificationRepositoryFactory : IYoutubeRepositoryFactory
	{

		private readonly IYoutubeRepository? _instance;

		public MockNotificationRepositoryFactory(IYoutubeRepository instance)
		{
			_instance = instance;
		}
		
		public MockNotificationRepositoryFactory() {}

		public IYoutubeRepository GetRepository()
		{
			return _instance ?? new MockNotificationRepository();
		}
	}
}
