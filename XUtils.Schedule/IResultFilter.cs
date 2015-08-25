using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public interface IResultFilter
	{
		void FilterResultsInInterval(DateTime Start, DateTime End, List<DateTime> List);
	}
}
