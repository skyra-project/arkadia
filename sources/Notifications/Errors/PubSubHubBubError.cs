using System.Diagnostics.CodeAnalysis;
using Remora.Results;

namespace Notifications.Errors
{
	[ExcludeFromCodeCoverage]
	public class PubSubHubBubError : IResultError
	{
		public string Message => "";
	}
}
