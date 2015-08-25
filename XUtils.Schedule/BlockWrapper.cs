using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class BlockWrapper : IScheduledItem
	{
		private IScheduledItem _Item;
		private ScheduledTime _Begin;
		private ScheduledTime _End;
		public BlockWrapper(IScheduledItem item, string StrBase, string BeginOffset, string EndOffset)
		{
			this._Item = item;
			this._Begin = new ScheduledTime(StrBase, BeginOffset);
			this._End = new ScheduledTime(StrBase, EndOffset);
		}
		public void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List)
		{
			DateTime dateTime = this.NextRunTime(Begin, true);
			while (dateTime < End)
			{
				List.Add(dateTime);
				dateTime = this.NextRunTime(dateTime, false);
			}
		}
		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			return this.NextRunTime(time, 100, AllowExact);
		}
		private DateTime NextRunTime(DateTime time, int count, bool AllowExact)
		{
			if (count == 0)
			{
				throw new Exception("Invalid block wrapper combination.");
			}
			DateTime dateTime = this._Item.NextRunTime(time, AllowExact);
			DateTime dateTime2 = this._Begin.NextRunTime(time, true);
			DateTime dateTime3 = this._End.NextRunTime(time, true);
			bool flag = dateTime > dateTime3;
			bool flag2 = dateTime < dateTime2;
			bool flag3 = dateTime3 < dateTime2;
			if (flag3)
			{
				if (flag && flag2)
				{
					return this.NextRunTime(dateTime2, --count, false);
				}
				return dateTime;
			}
			else
			{
				if (!flag && !flag2)
				{
					return dateTime;
				}
				if (!flag)
				{
					return this.NextRunTime(dateTime2, --count, false);
				}
				return this.NextRunTime(dateTime3, --count, false);
			}
		}
	}
}
