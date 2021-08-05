using Remora.Results;

namespace Notifications.Errors
{
	public class UnconfiguredError : IResultError
	{
		public string Message => "The request cannot be performed due to incorrect configuration.";
	}
}
