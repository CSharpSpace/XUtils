using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	[Serializable]
	public class ScheduledTime : IScheduledItem
	{
		private EventTimeEnums _Base;
		private TimeSpan _Offset;
		public ScheduledTime(EventTimeEnums Base, TimeSpan Offset)
		{
			this._Base = Base;
			this._Offset = Offset;
		}
		public ScheduledTime(string StrBase, string StrOffset)
		{
			this._Base = (EventTimeEnums)Enum.Parse(typeof(EventTimeEnums), StrBase, true);
			this.Init(StrOffset);
		}
		public void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List)
		{
			DateTime dateTime = this.NextRunTime(Begin, true);
			while (dateTime < End)
			{
				List.Add(dateTime);
				dateTime = this.IncInterval(dateTime);
			}
		}
		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			DateTime dateTime = this.LastSyncForTime(time) + this._Offset;
			if (dateTime == time && AllowExact)
			{
				return time;
			}
			if (dateTime > time)
			{
				return dateTime;
			}
			return this.IncInterval(dateTime);
		}
		private DateTime LastSyncForTime(DateTime time)
		{
			switch (this._Base)
			{
			case EventTimeEnums.BySecond:
				return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
			case EventTimeEnums.ByMinute:
				return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
			case EventTimeEnums.Hourly:
				return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
			case EventTimeEnums.Daily:
				return new DateTime(time.Year, time.Month, time.Day);
			case EventTimeEnums.Weekly:
				return new DateTime(time.Year, time.Month, time.Day).AddDays((double)(-(double)time.DayOfWeek));
			case EventTimeEnums.Monthly:
				return new DateTime(time.Year, time.Month, 1);
			default:
				throw new Exception("Invalid base specified for timer.");
			}
		}
		private DateTime IncInterval(DateTime Last)
		{
			switch (this._Base)
			{
			case EventTimeEnums.BySecond:
				return Last.AddSeconds(1.0);
			case EventTimeEnums.ByMinute:
				return Last.AddMinutes(1.0);
			case EventTimeEnums.Hourly:
				return Last.AddHours(1.0);
			case EventTimeEnums.Daily:
				return Last.AddDays(1.0);
			case EventTimeEnums.Weekly:
				return Last.AddDays(7.0);
			case EventTimeEnums.Monthly:
				return Last.AddMonths(1);
			default:
				throw new Exception("Invalid base specified for timer.");
			}
		}
		private void Init(string StrOffset)
		{
			switch (this._Base)
			{
			case EventTimeEnums.BySecond:
				this._Offset = new TimeSpan(0, 0, 0, 0, DateTime.Parse(StrOffset).Millisecond);
				return;
			case EventTimeEnums.ByMinute:
			{
				DateTime dateTime = DateTime.Parse(StrOffset);
				this._Offset = new TimeSpan(0, 0, 0, dateTime.Second, dateTime.Millisecond);
				return;
			}
			case EventTimeEnums.Hourly:
			{
				DateTime dateTime2 = DateTime.Parse(StrOffset);
				this._Offset = new TimeSpan(0, 0, dateTime2.Minute, dateTime2.Second, dateTime2.Millisecond);
				return;
			}
			case EventTimeEnums.Daily:
			{
				DateTime dateTime3 = DateTime.Parse(StrOffset);
				this._Offset = new TimeSpan(0, dateTime3.Hour, dateTime3.Minute, dateTime3.Second, dateTime3.Millisecond);
				return;
			}
			case EventTimeEnums.Weekly:
			{
				string[] array = StrOffset.Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					throw new Exception("Weekly offset must be in the format n, time where n is the day of the week starting with 0 for sunday");
				}
				DateTime dateTime4 = DateTime.Parse(array[1]);
				this._Offset = new TimeSpan(int.Parse(array[0]), dateTime4.Hour, dateTime4.Minute, dateTime4.Second, dateTime4.Millisecond);
				return;
			}
			case EventTimeEnums.Monthly:
			{
				string[] array2 = StrOffset.Split(new char[]
				{
					','
				});
				if (array2.Length != 2)
				{
					throw new Exception("Monthly offset must be in the format n, time where n is the day of the month starting with 1 for the first day of the month.");
				}
				DateTime dateTime5 = DateTime.Parse(array2[1]);
				this._Offset = new TimeSpan(int.Parse(array2[0]), dateTime5.Hour, dateTime5.Minute, dateTime5.Second, dateTime5.Millisecond);
				return;
			}
			default:
				throw new Exception("Invalid base specified for timer.");
			}
		}
	}
}
