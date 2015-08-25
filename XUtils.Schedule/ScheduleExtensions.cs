using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public static class ScheduleExtensions
	{
		public static void Register(this ScheduleTimer timer, string key, IScheduledRule rule, Delegate @delegate, params object[] args)
		{
			IScheduledItem scheduledItem = SchedulerFactory.Provider(rule);
			if (scheduledItem == null)
			{
				return;
			}
			timer.AddAsyncTask(key, scheduledItem, @delegate, args);
		}
		public static void Register(this ScheduleTimer timer, string key, string rules, Delegate @delegate, params object[] args)
		{
			IList<IScheduledRule> rules2 = ScheduledUtils.LoadRules(rules);
			timer.Register(key, rules2, @delegate, args);
		}
		public static void Register(this ScheduleTimer timer, string key, IList<IScheduledRule> rules, Delegate @delegate, params object[] args)
		{
			EventQueue eventQueue = new EventQueue();
			foreach (IScheduledRule current in rules)
			{
				IScheduledItem scheduledItem = SchedulerFactory.Provider(current);
				if (scheduledItem != null)
				{
					eventQueue.Add(scheduledItem);
				}
			}
			timer.AddAsyncTask(key, eventQueue, @delegate, args);
		}
	}
}
