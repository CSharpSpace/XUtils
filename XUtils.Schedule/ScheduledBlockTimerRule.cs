using System;
namespace XUtils.Schedule
{
	public class ScheduledBlockTimerRule : IScheduledRule
	{
		public string Begin
		{
			get;
			set;
		}
		public TaskRule Rule
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
