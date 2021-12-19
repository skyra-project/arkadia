using Cdn.Factories;
using Cdn.Repositories;

namespace UnitTests.Cdn;

public class MockCdnRepositoryFactory : ICdnRepositoryFactory
{
	private readonly ICdnRepository? _instance;

	public MockCdnRepositoryFactory(ICdnRepository instance)
	{
		_instance = instance;
	}

	public MockCdnRepositoryFactory()
	{
	}

	public ICdnRepository GetRepository()
	{
		return _instance ?? new MockCdnRepository();
	}
}
