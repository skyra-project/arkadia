using System;

namespace Notifications.Repositories
{
	public class DateTimeRepository : IDateTimeRepository
	{

		public DateTime GetTime() => DateTime.UtcNow;
	}
}
