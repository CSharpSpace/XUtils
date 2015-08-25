using System;
using System.Runtime.CompilerServices;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	public class WorkItemsGroup : WorkItemsGroupBase
	{
		private readonly object _lock = new object();
		private readonly SmartThreadPool _stp;
		private bool _isSuspended;
		private int _concurrency;
		private readonly PriorityQueue _workItemsQueue;
		private int _workItemsInStpQueue;
		private int _workItemsExecutingInStp;
		private readonly WIGStartInfo _workItemsGroupStartInfo;
		private readonly ManualResetEvent _isIdleWaitHandle = EventWaitHandleFactory.CreateManualResetEvent(true);
		private CanceledWorkItemsGroup _canceledWorkItemsGroup = new CanceledWorkItemsGroup();
		private event WorkItemsGroupIdleHandler _onIdle;
		public override event WorkItemsGroupIdleHandler OnIdle
		{
			add
			{
				this._onIdle += value;
			}
			remove
			{
				this._onIdle -= value;
			}
		}
		public override int Concurrency
		{
			get
			{
				return this._concurrency;
			}
			set
			{
				int num = value - this._concurrency;
				this._concurrency = value;
				if (num > 0)
				{
					this.EnqueueToSTPNextNWorkItem(num);
				}
			}
		}
		public override int WaitingCallbacks
		{
			get
			{
				return this._workItemsQueue.Count;
			}
		}
		public override WIGStartInfo WIGStartInfo
		{
			get
			{
				return this._workItemsGroupStartInfo;
			}
		}
		public WorkItemsGroup(SmartThreadPool stp, int concurrency, WIGStartInfo wigStartInfo)
		{
			if (concurrency <= 0)
			{
				throw new ArgumentOutOfRangeException("concurrency", concurrency, "concurrency must be greater than zero");
			}
			this._stp = stp;
			this._concurrency = concurrency;
			this._workItemsGroupStartInfo = new WIGStartInfo(wigStartInfo).AsReadOnly();
			this._workItemsQueue = new PriorityQueue();
			base.Name = "WorkItemsGroup";
			this._workItemsInStpQueue = this._workItemsExecutingInStp;
			this._isSuspended = this._workItemsGroupStartInfo.StartSuspended;
		}
		public override object[] GetStates()
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			object[] result;
			try
			{
				object[] array = new object[this._workItemsQueue.Count];
				int num = 0;
				foreach (WorkItem workItem in this._workItemsQueue)
				{
					array[num] = workItem.GetWorkItemResult().State;
					num++;
				}
				result = array;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public override void Start()
		{
			if (!this._isSuspended)
			{
				return;
			}
			this._isSuspended = false;
			this.EnqueueToSTPNextNWorkItem(Math.Min(this._workItemsQueue.Count, this._concurrency));
		}
		public override void Cancel(bool abortExecution)
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			try
			{
				this._canceledWorkItemsGroup.IsCanceled = true;
				this._workItemsQueue.Clear();
				this._workItemsInStpQueue = 0;
				this._canceledWorkItemsGroup = new CanceledWorkItemsGroup();
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			if (abortExecution)
			{
				this._stp.CancelAbortWorkItemsGroup(this);
			}
		}
		public override bool WaitForIdle(int millisecondsTimeout)
		{
			SmartThreadPool.ValidateWorkItemsGroupWaitForIdle(this);
			return STPEventWaitHandle.WaitOne(this._isIdleWaitHandle, millisecondsTimeout, false);
		}
		private void RegisterToWorkItemCompletion(IWorkItemResult wir)
		{
			IInternalWorkItemResult internalWorkItemResult = (IInternalWorkItemResult)wir;
			internalWorkItemResult.OnWorkItemStarted += new WorkItemStateCallback(this.OnWorkItemStartedCallback);
			internalWorkItemResult.OnWorkItemCompleted += new WorkItemStateCallback(this.OnWorkItemCompletedCallback);
		}
		public void OnSTPIsStarting()
		{
			if (this._isSuspended)
			{
				return;
			}
			this.EnqueueToSTPNextNWorkItem(this._concurrency);
		}
		public void EnqueueToSTPNextNWorkItem(int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.EnqueueToSTPNextWorkItem(null, false);
			}
		}
		private object FireOnIdle(object state)
		{
			this.FireOnIdleImpl(this._onIdle);
			return null;
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void FireOnIdleImpl(WorkItemsGroupIdleHandler onIdle)
		{
			if (onIdle == null)
			{
				return;
			}
			Delegate[] invocationList = onIdle.GetInvocationList();
			Delegate[] array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				WorkItemsGroupIdleHandler workItemsGroupIdleHandler = (WorkItemsGroupIdleHandler)array[i];
				try
				{
					workItemsGroupIdleHandler(this);
				}
				catch
				{
				}
			}
		}
		private void OnWorkItemStartedCallback(WorkItem workItem)
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			try
			{
				this._workItemsExecutingInStp++;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		private void OnWorkItemCompletedCallback(WorkItem workItem)
		{
			this.EnqueueToSTPNextWorkItem(null, true);
		}
		internal override void Enqueue(WorkItem workItem)
		{
			this.EnqueueToSTPNextWorkItem(workItem);
		}
		private void EnqueueToSTPNextWorkItem(WorkItem workItem)
		{
			this.EnqueueToSTPNextWorkItem(workItem, false);
		}
		private void EnqueueToSTPNextWorkItem(WorkItem workItem, bool decrementWorkItemsInStpQueue)
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			try
			{
				if (decrementWorkItemsInStpQueue)
				{
					this._workItemsInStpQueue--;
					if (this._workItemsInStpQueue < 0)
					{
						this._workItemsInStpQueue = 0;
					}
					this._workItemsExecutingInStp--;
					if (this._workItemsExecutingInStp < 0)
					{
						this._workItemsExecutingInStp = 0;
					}
				}
				if (workItem != null)
				{
					workItem.CanceledWorkItemsGroup = this._canceledWorkItemsGroup;
					this.RegisterToWorkItemCompletion(workItem.GetWorkItemResult());
					this._workItemsQueue.Enqueue(workItem);
					if (1 == this._workItemsQueue.Count && this._workItemsInStpQueue == 0)
					{
						this._stp.RegisterWorkItemsGroup(this);
						base.IsIdle = false;
						this._isIdleWaitHandle.Reset();
					}
				}
				if (this._workItemsQueue.Count == 0)
				{
					if (this._workItemsInStpQueue == 0)
					{
						this._stp.UnregisterWorkItemsGroup(this);
						base.IsIdle = true;
						this._isIdleWaitHandle.Set();
						if (decrementWorkItemsInStpQueue && this._onIdle != null && this._onIdle.GetInvocationList().Length > 0)
						{
							this._stp.QueueWorkItem(new WorkItemCallback(this.FireOnIdle));
						}
					}
				}
				else
				{
					if (!this._isSuspended && this._workItemsInStpQueue < this._concurrency)
					{
						WorkItem workItem2 = this._workItemsQueue.Dequeue() as WorkItem;
						try
						{
							this._stp.Enqueue(workItem2);
						}
						catch (ObjectDisposedException ex)
						{
							ex.GetHashCode();
						}
						this._workItemsInStpQueue++;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
	}
}
