using System;
namespace XUtils.Schedule
{
	public static class SchedulerFactory
	{
		private static IScheduledItem BlockIntervalRule(ScheduledBlockIntervalRule rule)
		{
			if (rule == null)
			{
				return null;
			}
			IScheduledItem result;
			try
			{
				IScheduledItem scheduledItem = new BlockWrapper(new SimpleInterval(DateTime.Parse(rule.Begin), TimeSpan.Parse(rule.Interval)), rule.Region.Type, rule.Region.StartOffest, rule.Region.StopOffest);
				result = scheduledItem;
			}
			catch
			{
				result = null;
			}
			return result;
		}
		private static IScheduledItem BlockTimerRule(ScheduledBlockTimerRule rule)
		{
			if (rule == null)
			{
				return null;
			}
			IScheduledItem result;
			try
			{
				IScheduledItem scheduledItem = new BlockWrapper(new ScheduledTime(rule.Rule.Type, rule.Rule.Offest), rule.Region.Type, rule.Region.StartOffest, rule.Region.StopOffest);
				result = scheduledItem;
			}
			catch
			{
				result = null;
			}
			return result;
		}
		private static IScheduledItem BaseRule(ScheduledRule rule)
		{
			if (rule == null)
			{
				return null;
			}
			IScheduledItem result = null;
			try
			{
				string type;
				switch (type = rule.Rule.Type)
				{
				case "BySecond":
				case "ByMinute":
				case "Hourly":
				case "Daily":
				case "Weekly":
				case "Monthly":
					result = new ScheduledTime(rule.Rule.Type, rule.Rule.Offest);
					break;
				case "BySleep":
					if (rule.Count == 0 && (rule.End == DateTime.MaxValue || rule.End == DateTime.MinValue))
					{
						result = new SimpleInterval(DateTime.Now, TimeSpan.Parse(rule.Rule.Offest));
					}
					else
					{
						if (rule.Count > 0)
						{
							result = new SimpleInterval(DateTime.Now, TimeSpan.Parse(rule.Rule.Offest), rule.Count);
						}
						else
						{
							if (rule.End != DateTime.MaxValue)
							{
								result = new SimpleInterval(DateTime.Now, TimeSpan.Parse(rule.Rule.Offest), rule.End);
							}
						}
					}
					break;
				}
			}
			catch
			{
			}
			return result;
		}
		public static IScheduledItem Provider(IScheduledRule rule)
		{
			IScheduledItem result;
			if (rule is ScheduledBlockIntervalRule)
			{
				ScheduledBlockIntervalRule rule2 = rule as ScheduledBlockIntervalRule;
				result = SchedulerFactory.BlockIntervalRule(rule2);
			}
			else
			{
				if (rule is ScheduledBlockTimerRule)
				{
					ScheduledBlockTimerRule rule3 = rule as ScheduledBlockTimerRule;
					result = SchedulerFactory.BlockTimerRule(rule3);
				}
				else
				{
					ScheduledRule rule4 = rule as ScheduledRule;
					result = SchedulerFactory.BaseRule(rule4);
				}
			}
			return result;
		}
	}
}
