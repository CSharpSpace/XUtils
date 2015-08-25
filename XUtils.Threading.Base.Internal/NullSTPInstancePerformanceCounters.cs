using System;
namespace XUtils.Threading.Base.Internal
{
	internal class NullSTPInstancePerformanceCounters : ISTPInstancePerformanceCounters, IDisposable, ISTPPerformanceCountersReader
	{
		private static readonly NullSTPInstancePerformanceCounters _instance = new NullSTPInstancePerformanceCounters();
		public static NullSTPInstancePerformanceCounters Instance
		{
			get
			{
				return NullSTPInstancePerformanceCounters._instance;
			}
		}
		public long InUseThreads
		{
			get
			{
				return 0L;
			}
		}
		public long ActiveThreads
		{
			get
			{
				return 0L;
			}
		}
		public long WorkItemsQueued
		{
			get
			{
				return 0L;
			}
		}
		public long WorkItemsProcessed
		{
			get
			{
				return 0L;
			}
		}
		public void Close()
		{
		}
		public void Dispose()
		{
		}
		public void SampleThreads(long activeThreads, long inUseThreads)
		{
		}
		public void SampleWorkItems(long workItemsQueued, long workItemsProcessed)
		{
		}
		public void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime)
		{
		}
		public void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime)
		{
		}
	}
}
