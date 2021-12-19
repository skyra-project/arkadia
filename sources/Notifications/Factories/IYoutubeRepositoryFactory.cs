using Notifications.Repositories;

namespace Notifications.Factories;

public interface IYoutubeRepositoryFactory
{
	IYoutubeRepository GetRepository();
}
