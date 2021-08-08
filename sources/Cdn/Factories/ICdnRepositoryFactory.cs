using Cdn.Repositories;

namespace Cdn.Factories
{
	public interface ICdnRepositoryFactory
	{
		ICdnRepository GetRepository();
	}
}
