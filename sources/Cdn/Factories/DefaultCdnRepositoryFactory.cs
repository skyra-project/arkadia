using System.Diagnostics.CodeAnalysis;
using Cdn.Repositories;
using Microsoft.Extensions.Logging;

namespace Cdn.Factories
{
	[ExcludeFromCodeCoverage]
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
