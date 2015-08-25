using System;
namespace XUtils.Threading.Base
{
	public interface ISTPPerformanceCountersReader
	{
		long InUseThreads
		{
			get;
		}
		long ActiveThreads
		{
			get;
		}
		long WorkItemsQueued
		{
			get;
		}
		long WorkItemsProcessed
		{
			get;
		}
	}
}
