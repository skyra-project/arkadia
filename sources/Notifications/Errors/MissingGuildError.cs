using Remora.Results;

namespace Notifications.Errors;

public class MissingGuildError : IResultError
{
	public string Message => "Guild does not exist.";
}
