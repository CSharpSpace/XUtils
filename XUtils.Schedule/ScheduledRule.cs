using System;
namespace XUtils.Schedule
{
	public class ScheduledRule : IScheduledRule
	{
		public TaskRule Rule
		{
			get;
			set;
		}
		public DateTime End
		{
			get;
			set;
		}
		public int Count
		{
			get;
			set;
		}
	}
}
