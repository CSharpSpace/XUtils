using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class LastEventFilter : IResultFilter
	{
		public static IResultFilter Filter = new LastEventFilter();
		private LastEventFilter()
		{
		}
		public void FilterResultsInInterval(DateTime Start, DateTime End, List<DateTime> List)
		{
			if (List == null || List.Count < 2)
			{
				return;
			}
			List.Sort();
			List.RemoveRange(0, List.Count - 1);
		}
	}
}
