using System;
namespace XUtils.Schedule
{
	public class EventInstance : IComparable
	{
		public DateTime Time;
		public IScheduledItem ScheduleItem;
		public object Data;
		public EventInstance(DateTime time, IScheduledItem scheduleItem, object data)
		{
			this.Time = time;
			this.ScheduleItem = scheduleItem;
			this.Data = data;
		}
		public int CompareTo(object obj)
		{
			if (obj is EventInstance)
			{
				return this.Time.CompareTo(((EventInstance)obj).Time);
			}
			if (obj is DateTime)
			{
				return this.Time.CompareTo((DateTime)obj);
			}
			return 0;
		}
	}
}
