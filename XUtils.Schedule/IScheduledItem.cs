using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public interface IScheduledItem
	{
		void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List);
		DateTime NextRunTime(DateTime time, bool IncludeStartTime);
	}
}
