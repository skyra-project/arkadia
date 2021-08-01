using Remora.Results;

namespace Notifications.Errors
{
	public class AllParametersNullError : IResultError
	{
		public string Message => "All parameters given were null.";
	}
}