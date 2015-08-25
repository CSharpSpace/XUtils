using System;
namespace XUtils.Schedule
{
	public class ScheduledEventArgs : EventArgs
	{
		public DateTime EventTime;
		public ScheduledEventArgs(DateTime eventTime)
		{
			this.EventTime = eventTime;
		}
	}
}
