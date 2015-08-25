using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Schedule
{
	public class EventQueue : IScheduledItem
	{
		private static object syncObject = new object();
		private List<IScheduledItem> container;
		public EventQueue()
		{
			this.container = new List<IScheduledItem>();
		}
		public void Add(IScheduledItem time)
		{
			object obj;
			Monitor.Enter(obj = EventQueue.syncObject);
			try
			{
				if (time != null)
				{
					this.container.Add(time);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Clear()
		{
			object obj;
			Monitor.Enter(obj = EventQueue.syncObject);
			try
			{
				this.container.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void AddEventsInInterval(DateTime Begin, DateTime End, List<DateTime> List)
		{
			foreach (IScheduledItem current in this.container)
			{
				current.AddEventsInInterval(Begin, End, List);
			}
			List.Sort();
		}
		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			DateTime dateTime = DateTime.MaxValue;
			foreach (IScheduledItem current in this.container)
			{
				DateTime dateTime2 = current.NextRunTime(time, AllowExact);
				dateTime = ((dateTime2 < dateTime) ? dateTime2 : dateTime);
			}
			return dateTime;
		}
	}
}
