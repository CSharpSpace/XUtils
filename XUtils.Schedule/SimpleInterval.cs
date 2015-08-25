using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	[Serializable]
	public class SimpleInterval : IScheduledItem
	{
		private TimeSpan _Interval;
		private DateTime _StartTime;
		private DateTime _EndTime;
		public SimpleInterval(DateTime StartTime, TimeSpan Interval)
		{
			this._Interval = Interval;
			this._StartTime = StartTime;
			this._EndTime = DateTime.MaxValue;
		}
		public SimpleInterval(DateTime StartTime, TimeSpan Interval, int count)
		{
			this._Interval = Interval;
			this._StartTime = StartTime;
			this._EndTime = StartTime + TimeSpan.FromTicks(Interval.Ticks * (long)count);
		}
		public SimpleInterval(DateTime StartTime, TimeSpan Interval, DateTime EndTime)
		{
			this._Interval = Interval;
			this._StartTime = StartTime;
			this._EndTime = EndTime;
		}
		public void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List)
		{
			if (End <= this._StartTime)
			{
				return;
			}
			DateTime dateTime = this.NextRunTime(Begin, true);
			while (dateTime < End)
			{
				List.Add(dateTime);
				dateTime = this.NextRunTime(dateTime, false);
			}
		}
		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			DateTime dateTime = this.NextRunTimeInt(time, AllowExact);
			if (!(dateTime >= this._EndTime))
			{
				return dateTime;
			}
			return DateTime.MaxValue;
		}
		private DateTime NextRunTimeInt(DateTime time, bool AllowExact)
		{
			TimeSpan t = time - this._StartTime;
			if (t < TimeSpan.Zero)
			{
				return this._StartTime;
			}
			if (!this.ExactMatch(time))
			{
				uint num = (uint)(this._Interval.TotalMilliseconds - (uint)t.TotalMilliseconds % (uint)this._Interval.TotalMilliseconds);
				return time.AddMilliseconds(num);
			}
			if (!AllowExact)
			{
				return time + this._Interval;
			}
			return time;
		}
		private bool ExactMatch(DateTime time)
		{
			TimeSpan t = time - this._StartTime;
			return !(t < TimeSpan.Zero) && t.TotalMilliseconds % this._Interval.TotalMilliseconds == 0.0;
		}
	}
}
