using Remora.Results;

namespace Notifications.Errors
{
	public class ChannelInfoRetrievalError : IResultError
	{
		public string Message => "Error fetching channel information.";
	}
}