using Cdn.Repositories;
using Microsoft.Extensions.Logging;

namespace Cdn.Factories
{
	public class DefaultCdnRepositoryFactory : ICdnRepositoryFactory
	{
		private readonly ILogger<CdnRepository> _logger;

		public DefaultCdnRepositoryFactory(ILogger<CdnRepository> logger)
		{
			_logger = logger;
		}

		public ICdnRepository GetRepository()
		{
			return new CdnRepository(_logger);
		}
	}
}
