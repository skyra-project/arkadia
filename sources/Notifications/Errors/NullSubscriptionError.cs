using Remora.Results;

namespace Notifications.Errors
{
	public class NullSubscriptionError : IResultError
	{
		public string Message => "The subscription does not exist, or was null.";
	}
}