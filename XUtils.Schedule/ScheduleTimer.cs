using System;
namespace XUtils.Schedule
{
	public class ScheduleTimer : ScheduleTimerBase
	{
		public event ScheduledEventHandler Elapsed;
		public void AddEvent(string key, IScheduledItem Schedule)
		{
			if (this.Elapsed == null)
			{
				throw new ArgumentNullException("Elapsed", "member variable is null.");
			}
			base.AddTask(key, new Task(Schedule, new DelegateMethodCall(this.Elapsed)));
		}
	}
}
