using System;
namespace XUtils.Threading.Base.Internal
{
	internal enum STPPerformanceCounterType
	{
		ActiveThreads,
		InUseThreads,
		OverheadThreads,
		OverheadThreadsPercent,
		OverheadThreadsPercentBase,
		WorkItems,
		WorkItemsInQueue,
		WorkItemsProcessed,
		WorkItemsQueuedPerSecond,
		WorkItemsProcessedPerSecond,
		AvgWorkItemWaitTime,
		AvgWorkItemWaitTimeBase,
		AvgWorkItemProcessTime,
		AvgWorkItemProcessTimeBase,
		WorkItemsGroups,
		LastCounter = 14
	}
}
