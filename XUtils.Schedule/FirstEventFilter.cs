using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class FirstEventFilter : IResultFilter
	{
		public static IResultFilter Filter = new FirstEventFilter();
		private FirstEventFilter()
		{
		}
		public void FilterResultsInInterval(DateTime Start, DateTime End, List<DateTime> List)
		{
			if (List == null || List.Count < 2)
			{
				return;
			}
			List.Sort();
			List.RemoveRange(1, List.Count - 1);
		}
	}
}
