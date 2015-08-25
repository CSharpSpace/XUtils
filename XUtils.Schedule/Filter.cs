using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class Filter : IResultFilter
	{
		public static IResultFilter Empty = new Filter();
		private Filter()
		{
		}
		public void FilterResultsInInterval(DateTime Start, DateTime End, List<DateTime> List)
		{
			if (List == null)
			{
				return;
			}
			List.Sort();
		}
	}
}
