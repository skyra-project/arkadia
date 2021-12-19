using System;
using System.Diagnostics.CodeAnalysis;

namespace Notifications.Repositories;

public class DateTimeRepository : IDateTimeRepository
{
	[ExcludeFromCodeCoverage(Justification = "Too simple an implementation to bother testing.")]
	public DateTime GetTime()
	{
		return DateTime.UtcNow;
	}
}
