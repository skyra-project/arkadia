using Cdn.Factories;
using Cdn.Repositories;

namespace UnitTests.Cdn.Service
{
	public class MockCdnRepositoryFactory : ICdnRepositoryFactory
	{

		private readonly ICdnRepository? _instance;

		public MockCdnRepositoryFactory(ICdnRepository instance)
		{
			_instance = instance;
		}

		public ICdnRepository GetRepository()
		{
			return _instance ?? new MockCdnRepository();
		}
	}
}
