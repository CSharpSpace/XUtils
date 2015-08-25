using System;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	public class WorkItemsQueue : IDisposable
	{
		public sealed class WaiterEntry : IDisposable
		{
			private AutoResetEvent _waitHandle = EventWaitHandleFactory.CreateAutoResetEvent();
			private bool _isTimedout;
			private bool _isSignaled;
			private WorkItem _workItem;
			private bool _isDisposed;
			internal WorkItemsQueue.WaiterEntry _nextWaiterEntry;
			internal WorkItemsQueue.WaiterEntry _prevWaiterEntry;
			public WaitHandle WaitHandle
			{
				get
				{
					return this._waitHandle;
				}
			}
			public WorkItem WorkItem
			{
				get
				{
					return this._workItem;
				}
			}
			public WaiterEntry()
			{
				this.Reset();
			}
			public bool Signal(WorkItem workItem)
			{
				Monitor.Enter(this);
				try
				{
					if (!this._isTimedout)
					{
						this._workItem = workItem;
						this._isSignaled = true;
						this._waitHandle.Set();
						return true;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
				return false;
			}
			public bool Timeout()
			{
				Monitor.Enter(this);
				try
				{
					if (!this._isSignaled)
					{
						this._isTimedout = true;
						return true;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
				return false;
			}
			public void Reset()
			{
				this._workItem = null;
				this._isTimedout = false;
				this._isSignaled = false;
				this._waitHandle.Reset();
			}
			public void Close()
			{
				if (this._waitHandle != null)
				{
					this._waitHandle.Close();
					this._waitHandle = null;
				}
			}
			public void Dispose()
			{
				Monitor.Enter(this);
				try
				{
					if (!this._isDisposed)
					{
						this.Close();
					}
					this._isDisposed = true;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		private readonly WorkItemsQueue.WaiterEntry _headWaiterEntry = new WorkItemsQueue.WaiterEntry();
		private int _waitersCount;
		private readonly PriorityQueue _workItems = new PriorityQueue();
		private bool _isWorkItemsQueueActive = true;
		[ThreadStatic]
		private static WorkItemsQueue.WaiterEntry _waiterEntry;
		private bool _isDisposed;
		private static WorkItemsQueue.WaiterEntry CurrentWaiterEntry
		{
			get
			{
				return WorkItemsQueue._waiterEntry;
			}
			set
			{
				WorkItemsQueue._waiterEntry = value;
			}
		}
		public int Count
		{
			get
			{
				return this._workItems.Count;
			}
		}
		public int WaitersCount
		{
			get
			{
				return this._waitersCount;
			}
		}
		public bool EnqueueWorkItem(WorkItem workItem)
		{
			if (workItem == null)
			{
				throw new ArgumentNullException("workItem", "workItem cannot be null");
			}
			bool flag = true;
			Monitor.Enter(this);
			try
			{
				this.ValidateNotDisposed();
				if (!this._isWorkItemsQueueActive)
				{
					return false;
				}
				while (this._waitersCount > 0)
				{
					WorkItemsQueue.WaiterEntry waiterEntry = this.PopWaiter();
					if (waiterEntry.Signal(workItem))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this._workItems.Enqueue(workItem);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return true;
		}
		public WorkItem DequeueWorkItem(int millisecondsTimeout, WaitHandle cancelEvent)
		{
			WorkItem workItem = null;
			WorkItemsQueue.WaiterEntry threadWaiterEntry;
			try
			{
				while (!Monitor.TryEnter(this))
				{
				}
				this.ValidateNotDisposed();
				if (this._workItems.Count > 0)
				{
					workItem = (this._workItems.Dequeue() as WorkItem);
					return workItem;
				}
				threadWaiterEntry = WorkItemsQueue.GetThreadWaiterEntry();
				this.PushWaiter(threadWaiterEntry);
			}
			finally
			{
				Monitor.Exit(this);
			}
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				threadWaiterEntry.WaitHandle,
				cancelEvent
			};
			int num = STPEventWaitHandle.WaitAny(waitHandles, millisecondsTimeout, true);
			Monitor.Enter(this);
			try
			{
				bool flag = 0 == num;
				bool flag2 = !flag;
				if (flag2)
				{
					flag2 = threadWaiterEntry.Timeout();
					if (flag2)
					{
						this.RemoveWaiter(threadWaiterEntry, false);
					}
					flag = !flag2;
				}
				if (flag)
				{
					workItem = threadWaiterEntry.WorkItem;
					if (workItem == null)
					{
						workItem = (this._workItems.Dequeue() as WorkItem);
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return workItem;
		}
		private void Cleanup()
		{
			Monitor.Enter(this);
			try
			{
				if (this._isWorkItemsQueueActive)
				{
					this._isWorkItemsQueueActive = false;
					foreach (WorkItem workItem in this._workItems)
					{
						workItem.DisposeOfState();
					}
					this._workItems.Clear();
					while (this._waitersCount > 0)
					{
						WorkItemsQueue.WaiterEntry waiterEntry = this.PopWaiter();
						waiterEntry.Timeout();
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public object[] GetStates()
		{
			Monitor.Enter(this);
			object[] result;
			try
			{
				object[] array = new object[this._workItems.Count];
				int num = 0;
				foreach (WorkItem workItem in this._workItems)
				{
					array[num] = workItem.GetWorkItemResult().State;
					num++;
				}
				result = array;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		private static WorkItemsQueue.WaiterEntry GetThreadWaiterEntry()
		{
			if (WorkItemsQueue.CurrentWaiterEntry == null)
			{
				WorkItemsQueue.CurrentWaiterEntry = new WorkItemsQueue.WaiterEntry();
			}
			WorkItemsQueue.CurrentWaiterEntry.Reset();
			return WorkItemsQueue.CurrentWaiterEntry;
		}
		public void PushWaiter(WorkItemsQueue.WaiterEntry newWaiterEntry)
		{
			this.RemoveWaiter(newWaiterEntry, false);
			if (this._headWaiterEntry._nextWaiterEntry == null)
			{
				this._headWaiterEntry._nextWaiterEntry = newWaiterEntry;
				newWaiterEntry._prevWaiterEntry = this._headWaiterEntry;
			}
			else
			{
				WorkItemsQueue.WaiterEntry nextWaiterEntry = this._headWaiterEntry._nextWaiterEntry;
				this._headWaiterEntry._nextWaiterEntry = newWaiterEntry;
				newWaiterEntry._nextWaiterEntry = nextWaiterEntry;
				newWaiterEntry._prevWaiterEntry = this._headWaiterEntry;
				nextWaiterEntry._prevWaiterEntry = newWaiterEntry;
			}
			this._waitersCount++;
		}
		private WorkItemsQueue.WaiterEntry PopWaiter()
		{
			WorkItemsQueue.WaiterEntry nextWaiterEntry = this._headWaiterEntry._nextWaiterEntry;
			WorkItemsQueue.WaiterEntry nextWaiterEntry2 = nextWaiterEntry._nextWaiterEntry;
			this.RemoveWaiter(nextWaiterEntry, true);
			this._headWaiterEntry._nextWaiterEntry = nextWaiterEntry2;
			if (nextWaiterEntry2 != null)
			{
				nextWaiterEntry2._prevWaiterEntry = this._headWaiterEntry;
			}
			return nextWaiterEntry;
		}
		private void RemoveWaiter(WorkItemsQueue.WaiterEntry waiterEntry, bool popDecrement)
		{
			WorkItemsQueue.WaiterEntry prevWaiterEntry = waiterEntry._prevWaiterEntry;
			WorkItemsQueue.WaiterEntry nextWaiterEntry = waiterEntry._nextWaiterEntry;
			bool flag = popDecrement;
			waiterEntry._prevWaiterEntry = null;
			waiterEntry._nextWaiterEntry = null;
			if (prevWaiterEntry != null)
			{
				prevWaiterEntry._nextWaiterEntry = nextWaiterEntry;
				flag = true;
			}
			if (nextWaiterEntry != null)
			{
				nextWaiterEntry._prevWaiterEntry = prevWaiterEntry;
				flag = true;
			}
			if (flag)
			{
				this._waitersCount--;
			}
		}
		public void Dispose()
		{
			if (!this._isDisposed)
			{
				this.Cleanup();
			}
			this._isDisposed = true;
		}
		private void ValidateNotDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString(), "The SmartThreadPool has been shutdown");
			}
		}
	}
}
