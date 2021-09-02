using Remora.Results;

namespace Notifications.Errors
{
	public class MissingSubscriptionError : IResultError
	{
		public string Message => "Subscription does not exist.";
	}
}
