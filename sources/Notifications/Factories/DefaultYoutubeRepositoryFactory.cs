using System.Diagnostics.CodeAnalysis;
using Notifications.Repositories;

namespace Notifications.Factories
{
	[ExcludeFromCodeCoverage]
	public class DefaultYoutubeRepositoryFactory : IYoutubeRepositoryFactory
	{
		public IYoutubeRepository GetRepository()
		{
			return new YoutubeRepository();
		}
	}
}
