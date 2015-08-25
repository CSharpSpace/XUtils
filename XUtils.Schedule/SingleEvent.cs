using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class SingleEvent : IScheduledItem
	{
		private DateTime _EventTime;
		public SingleEvent(DateTime eventTime)
		{
			this._EventTime = eventTime;
		}
		public void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List)
		{
			if (Begin <= this._EventTime && End > this._EventTime)
			{
				List.Add(this._EventTime);
			}
		}
		public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
		{
			if (IncludeStartTime)
			{
				if (!(this._EventTime >= time))
				{
					return DateTime.MaxValue;
				}
				return this._EventTime;
			}
			else
			{
				if (!(this._EventTime > time))
				{
					return DateTime.MaxValue;
				}
				return this._EventTime;
			}
		}
	}
}
