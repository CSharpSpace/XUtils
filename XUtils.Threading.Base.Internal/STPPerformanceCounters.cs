using System;
using System.Diagnostics;
namespace XUtils.Threading.Base.Internal
{
	internal class STPPerformanceCounters
	{
		internal const string _stpCategoryHelp = "ThreadPool performance counters";
		internal const string _stpCategoryName = "ThreadPool";
		internal STPPerformanceCounter[] _stpPerformanceCounters;
		private static readonly STPPerformanceCounters _instance;
		public static STPPerformanceCounters Instance
		{
			get
			{
				return STPPerformanceCounters._instance;
			}
		}
		static STPPerformanceCounters()
		{
			STPPerformanceCounters._instance = new STPPerformanceCounters();
		}
		private STPPerformanceCounters()
		{
			STPPerformanceCounter[] stpPerformanceCounters = new STPPerformanceCounter[]
			{
				new STPPerformanceCounter("Active threads", "The current number of available in the thread pool.", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("In use threads", "The current number of threads that execute a work item.", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("Overhead threads", "The current number of threads that are active, but are not in use.", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("% overhead threads", "The current number of threads that are active, but are not in use in percents.", PerformanceCounterType.RawFraction),
				new STPPerformanceCounter("% overhead threads base", "The current number of threads that are active, but are not in use in percents.", PerformanceCounterType.RawBase),
				new STPPerformanceCounter("Work Items", "The number of work items in the Smart Thread Pool. Both queued and processed.", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("Work Items in queue", "The current number of work items in the queue", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("Work Items processed", "The number of work items already processed", PerformanceCounterType.NumberOfItems32),
				new STPPerformanceCounter("Work Items queued/sec", "The number of work items queued per second", PerformanceCounterType.RateOfCountsPerSecond32),
				new STPPerformanceCounter("Work Items processed/sec", "The number of work items processed per second", PerformanceCounterType.RateOfCountsPerSecond32),
				new STPPerformanceCounter("Avg. Work Item wait time/sec", "The average time a work item supends in the queue waiting for its turn to execute.", PerformanceCounterType.AverageCount64),
				new STPPerformanceCounter("Avg. Work Item wait time base", "The average time a work item supends in the queue waiting for its turn to execute.", PerformanceCounterType.AverageBase),
				new STPPerformanceCounter("Avg. Work Item process time/sec", "The average time it takes to process a work item.", PerformanceCounterType.AverageCount64),
				new STPPerformanceCounter("Avg. Work Item process time base", "The average time it takes to process a work item.", PerformanceCounterType.AverageBase),
				new STPPerformanceCounter("Work Items Groups", "The current number of work item groups associated with the Smart Thread Pool.", PerformanceCounterType.NumberOfItems32)
			};
			this._stpPerformanceCounters = stpPerformanceCounters;
			this.SetupCategory();
		}
		private void SetupCategory()
		{
			if (!PerformanceCounterCategory.Exists("ThreadPool"))
			{
				CounterCreationDataCollection counterData = new CounterCreationDataCollection();
				for (int i = 0; i < this._stpPerformanceCounters.Length; i++)
				{
					this._stpPerformanceCounters[i].AddCounterToCollection(counterData);
				}
				PerformanceCounterCategory.Create("ThreadPool", "ThreadPool performance counters", PerformanceCounterCategoryType.MultiInstance, counterData);
			}
		}
	}
}
