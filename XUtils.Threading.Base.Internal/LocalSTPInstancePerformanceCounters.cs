using System;
namespace XUtils.Threading.Base.Internal
{
	internal class LocalSTPInstancePerformanceCounters : ISTPInstancePerformanceCounters, IDisposable, ISTPPerformanceCountersReader
	{
		private long _activeThreads;
		private long _inUseThreads;
		private long _workItemsQueued;
		private long _workItemsProcessed;
		public long InUseThreads
		{
			get
			{
				return this._inUseThreads;
			}
		}
		public long ActiveThreads
		{
			get
			{
				return this._activeThreads;
			}
		}
		public long WorkItemsQueued
		{
			get
			{
				return this._workItemsQueued;
			}
		}
		public long WorkItemsProcessed
		{
			get
			{
				return this._workItemsProcessed;
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
			this._activeThreads = activeThreads;
			this._inUseThreads = inUseThreads;
		}
		public void SampleWorkItems(long workItemsQueued, long workItemsProcessed)
		{
			this._workItemsQueued = workItemsQueued;
			this._workItemsProcessed = workItemsProcessed;
		}
		public void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime)
		{
		}
		public void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime)
		{
		}
	}
}
