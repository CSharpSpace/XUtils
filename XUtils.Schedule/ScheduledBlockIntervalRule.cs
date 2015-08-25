using System;
namespace XUtils.Schedule
{
	public class ScheduledBlockIntervalRule : IScheduledRule
	{
		public string Begin
		{
			get;
			set;
		}
		public string Interval
		{
			get;
			set;
		}
		public TaskRegion Region
		{
			get;
			set;
		}
	}
}
