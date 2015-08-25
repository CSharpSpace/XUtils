using System;
namespace XUtils.Threading.Base.Internal
{
	internal interface ISTPInstancePerformanceCounters : IDisposable
	{
		void Close();
		void SampleThreads(long activeThreads, long inUseThreads);
		void SampleWorkItems(long workItemsQueued, long workItemsProcessed);
		void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime);
		void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime);
	}
}
