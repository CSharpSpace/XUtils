using System;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	public class WorkItem : IHasWorkItemPriority
	{
		private enum WorkItemState
		{
			InQueue,
			InProgress,
			Completed,
			Canceled
		}
		private class WorkItemResult : IWorkItemResult, IWorkItemResult<object>, IWaitableResult, IInternalWorkItemResult, IInternalWaitableResult
		{
			private readonly WorkItem _workItem;
			public event WorkItemStateCallback OnWorkItemStarted
			{
				add
				{
					this._workItem.OnWorkItemStarted += value;
				}
				remove
				{
					this._workItem.OnWorkItemStarted -= value;
				}
			}
			public event WorkItemStateCallback OnWorkItemCompleted
			{
				add
				{
					this._workItem.OnWorkItemCompleted += value;
				}
				remove
				{
					this._workItem.OnWorkItemCompleted -= value;
				}
			}
			public bool IsCompleted
			{
				get
				{
					return this._workItem.IsCompleted;
				}
			}
			public bool IsCanceled
			{
				get
				{
					return this._workItem.IsCanceled;
				}
			}
			public object State
			{
				get
				{
					return this._workItem._state;
				}
			}
			public WorkItemPriority WorkItemPriority
			{
				get
				{
					return this._workItem._workItemInfo.WorkItemPriority;
				}
			}
			public object Result
			{
				get
				{
					return this.GetResult();
				}
			}
			public object Exception
			{
				get
				{
					return this._workItem._exception;
				}
			}
			public WorkItemResult(WorkItem workItem)
			{
				this._workItem = workItem;
			}
			internal WorkItem GetWorkItem()
			{
				return this._workItem;
			}
			public object GetResult()
			{
				return this._workItem.GetResult(-1, true, null);
			}
			public object GetResult(int millisecondsTimeout, bool exitContext)
			{
				return this._workItem.GetResult(millisecondsTimeout, exitContext, null);
			}
			public object GetResult(TimeSpan timeout, bool exitContext)
			{
				return this._workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, null);
			}
			public object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
			{
				return this._workItem.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle);
			}
			public object GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
			{
				return this._workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
			}
			public object GetResult(out Exception e)
			{
				return this._workItem.GetResult(-1, true, null, out e);
			}
			public object GetResult(int millisecondsTimeout, bool exitContext, out Exception e)
			{
				return this._workItem.GetResult(millisecondsTimeout, exitContext, null, out e);
			}
			public object GetResult(TimeSpan timeout, bool exitContext, out Exception e)
			{
				return this._workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, null, out e);
			}
			public object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
			{
				return this._workItem.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out e);
			}
			public object GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
			{
				return this._workItem.GetResult((int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle, out e);
			}
			public bool Cancel()
			{
				return this.Cancel(false);
			}
			public bool Cancel(bool abortExecution)
			{
				return this._workItem.Cancel(abortExecution);
			}
			public IWorkItemResult GetWorkItemResult()
			{
				return this;
			}
			public IWorkItemResult<TResult> GetWorkItemResultT<TResult>()
			{
				return new WorkItemResultTWrapper<TResult>(this);
			}
		}
		private readonly WorkItemCallback _callback;
		private object _state;
		private readonly CallerThreadContext _callerContext;
		private object _result;
		private Exception _exception;
		private WorkItem.WorkItemState _workItemState;
		private ManualResetEvent _workItemCompleted;
		private int _workItemCompletedRefCount;
		private readonly WorkItem.WorkItemResult _workItemResult;
		private readonly WorkItemInfo _workItemInfo;
		private CanceledWorkItemsGroup _canceledWorkItemsGroup = CanceledWorkItemsGroup.NotCanceledWorkItemsGroup;
		private CanceledWorkItemsGroup _canceledSmartThreadPool = CanceledWorkItemsGroup.NotCanceledWorkItemsGroup;
		private readonly IWorkItemsGroup _workItemsGroup;
		private Thread _executingThread;
		private long _expirationTime;
		private Stopwatch _waitingOnQueueStopwatch;
		private Stopwatch _processingStopwatch;
		private event WorkItemStateCallback _workItemStartedEvent;
		private event WorkItemStateCallback _workItemCompletedEvent;
		internal event WorkItemStateCallback OnWorkItemStarted
		{
			add
			{
				this._workItemStartedEvent += value;
			}
			remove
			{
				this._workItemStartedEvent -= value;
			}
		}
		internal event WorkItemStateCallback OnWorkItemCompleted
		{
			add
			{
				this._workItemCompletedEvent += value;
			}
			remove
			{
				this._workItemCompletedEvent -= value;
			}
		}
		public TimeSpan WaitingTime
		{
			get
			{
				return this._waitingOnQueueStopwatch.Elapsed;
			}
		}
		public TimeSpan ProcessTime
		{
			get
			{
				return this._processingStopwatch.Elapsed;
			}
		}
		internal WorkItemInfo WorkItemInfo
		{
			get
			{
				return this._workItemInfo;
			}
		}
		internal CanceledWorkItemsGroup CanceledWorkItemsGroup
		{
			get
			{
				return this._canceledWorkItemsGroup;
			}
			set
			{
				this._canceledWorkItemsGroup = value;
			}
		}
		internal CanceledWorkItemsGroup CanceledSmartThreadPool
		{
			get
			{
				return this._canceledSmartThreadPool;
			}
			set
			{
				this._canceledSmartThreadPool = value;
			}
		}
		private bool IsCompleted
		{
			get
			{
				Monitor.Enter(this);
				bool result;
				try
				{
					WorkItem.WorkItemState workItemState = this.GetWorkItemState();
					result = (workItemState == WorkItem.WorkItemState.Completed || workItemState == WorkItem.WorkItemState.Canceled);
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public bool IsCanceled
		{
			get
			{
				Monitor.Enter(this);
				bool result;
				try
				{
					result = (this.GetWorkItemState() == WorkItem.WorkItemState.Canceled);
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public WorkItemPriority WorkItemPriority
		{
			get
			{
				return this._workItemInfo.WorkItemPriority;
			}
		}
		private static bool IsValidStatesTransition(WorkItem.WorkItemState currentState, WorkItem.WorkItemState nextState)
		{
			bool result = false;
			switch (currentState)
			{
			case WorkItem.WorkItemState.InQueue:
				result = (WorkItem.WorkItemState.InProgress == nextState || WorkItem.WorkItemState.Canceled == nextState);
				break;
			case WorkItem.WorkItemState.InProgress:
				result = (WorkItem.WorkItemState.Completed == nextState || WorkItem.WorkItemState.Canceled == nextState);
				break;
			}
			return result;
		}
		public WorkItem(IWorkItemsGroup workItemsGroup, WorkItemInfo workItemInfo, WorkItemCallback callback, object state)
		{
			this._workItemsGroup = workItemsGroup;
			this._workItemInfo = workItemInfo;
			if (this._workItemInfo.UseCallerCallContext || this._workItemInfo.UseCallerHttpContext)
			{
				this._callerContext = CallerThreadContext.Capture(this._workItemInfo.UseCallerCallContext, this._workItemInfo.UseCallerHttpContext);
			}
			this._callback = callback;
			this._state = state;
			this._workItemResult = new WorkItem.WorkItemResult(this);
			this.Initialize();
		}
		internal void Initialize()
		{
			this._workItemState = WorkItem.WorkItemState.InQueue;
			this._workItemCompleted = null;
			this._workItemCompletedRefCount = 0;
			this._waitingOnQueueStopwatch = new Stopwatch();
			this._processingStopwatch = new Stopwatch();
			this._expirationTime = ((this._workItemInfo.Timeout > 0L) ? (DateTime.UtcNow.Ticks + this._workItemInfo.Timeout * 10000L) : 9223372036854775807L);
		}
		internal bool WasQueuedBy(IWorkItemsGroup workItemsGroup)
		{
			return workItemsGroup == this._workItemsGroup;
		}
		public bool StartingWorkItem()
		{
			this._waitingOnQueueStopwatch.Stop();
			this._processingStopwatch.Start();
			Monitor.Enter(this);
			try
			{
				if (this.IsCanceled)
				{
					bool result = false;
					if (this._workItemInfo.PostExecuteWorkItemCallback != null && (this._workItemInfo.CallToPostExecute & CallToPostExecute.WhenWorkItemCanceled) == CallToPostExecute.WhenWorkItemCanceled)
					{
						result = true;
					}
					return result;
				}
				this._executingThread = Thread.CurrentThread;
				this.SetWorkItemState(WorkItem.WorkItemState.InProgress);
			}
			finally
			{
				Monitor.Exit(this);
			}
			return true;
		}
		public void Execute()
		{
			CallToPostExecute callToPostExecute = CallToPostExecute.Never;
			switch (this.GetWorkItemState())
			{
			case WorkItem.WorkItemState.InProgress:
				callToPostExecute |= CallToPostExecute.WhenWorkItemNotCanceled;
				this.ExecuteWorkItem();
				goto IL_37;
			case WorkItem.WorkItemState.Canceled:
				callToPostExecute |= CallToPostExecute.WhenWorkItemCanceled;
				goto IL_37;
			}
			throw new NotSupportedException();
			IL_37:
			if ((callToPostExecute & this._workItemInfo.CallToPostExecute) != CallToPostExecute.Never)
			{
				this.PostExecute();
			}
			this._processingStopwatch.Stop();
		}
		internal void FireWorkItemCompleted()
		{
			try
			{
				if (this._workItemCompletedEvent != null)
				{
					this._workItemCompletedEvent(this);
				}
			}
			catch
			{
			}
		}
		internal void FireWorkItemStarted()
		{
			try
			{
				if (this._workItemStartedEvent != null)
				{
					this._workItemStartedEvent(this);
				}
			}
			catch
			{
			}
		}
		private void ExecuteWorkItem()
		{
			CallerThreadContext callerThreadContext = null;
			if (this._callerContext != null)
			{
				callerThreadContext = CallerThreadContext.Capture(this._callerContext.CapturedCallContext, this._callerContext.CapturedHttpContext);
				CallerThreadContext.Apply(this._callerContext);
			}
			Exception exception = null;
			object result = null;
			try
			{
				try
				{
					result = this._callback(this._state);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				if (Interlocked.CompareExchange<Thread>(ref this._executingThread, null, this._executingThread) == null)
				{
					Thread.Sleep(60000);
				}
			}
			catch (ThreadAbortException)
			{
				if (!SmartThreadPool.CurrentThreadEntry.AssociatedSmartThreadPool.IsShuttingdown)
				{
					Thread.ResetAbort();
				}
			}
			if (this._callerContext != null)
			{
				CallerThreadContext.Apply(callerThreadContext);
			}
			if (!SmartThreadPool.IsWorkItemCanceled)
			{
				this.SetResult(result, exception);
			}
		}
		private void PostExecute()
		{
			if (this._workItemInfo.PostExecuteWorkItemCallback != null)
			{
				try
				{
					this._workItemInfo.PostExecuteWorkItemCallback(this._workItemResult);
				}
				catch (Exception)
				{
				}
			}
		}
		internal void SetResult(object result, Exception exception)
		{
			this._result = result;
			this._exception = exception;
			this.SignalComplete(false);
		}
		internal IWorkItemResult GetWorkItemResult()
		{
			return this._workItemResult;
		}
		internal static bool WaitAll(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			if (waitableResults.Length == 0)
			{
				return true;
			}
			WaitHandle[] array = new WaitHandle[waitableResults.Length];
			WorkItem.GetWaitHandles(waitableResults, array);
			bool result;
			if (cancelWaitHandle == null && array.Length <= 64)
			{
				result = STPEventWaitHandle.WaitAll(array, millisecondsTimeout, exitContext);
			}
			else
			{
				result = true;
				int num = millisecondsTimeout;
				Stopwatch stopwatch = Stopwatch.StartNew();
				WaitHandle[] array2;
				if (cancelWaitHandle != null)
				{
					array2 = new WaitHandle[]
					{
						null,
						cancelWaitHandle
					};
				}
				else
				{
					WaitHandle[] array3 = new WaitHandle[1];
					array2 = array3;
				}
				bool flag = -1 == millisecondsTimeout;
				for (int i = 0; i < waitableResults.Length; i++)
				{
					if (!flag && num < 0)
					{
						result = false;
						break;
					}
					array2[0] = array[i];
					int num2 = STPEventWaitHandle.WaitAny(array2, num, exitContext);
					if (num2 > 0 || -1 == num2)
					{
						result = false;
						break;
					}
					if (!flag)
					{
						num = millisecondsTimeout - (int)stopwatch.ElapsedMilliseconds;
					}
				}
			}
			WorkItem.ReleaseWaitHandles(waitableResults);
			return result;
		}
		internal static int WaitAny(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			WaitHandle[] array;
			if (cancelWaitHandle != null)
			{
				array = new WaitHandle[waitableResults.Length + 1];
				WorkItem.GetWaitHandles(waitableResults, array);
				array[waitableResults.Length] = cancelWaitHandle;
			}
			else
			{
				array = new WaitHandle[waitableResults.Length];
				WorkItem.GetWaitHandles(waitableResults, array);
			}
			int num = STPEventWaitHandle.WaitAny(array, millisecondsTimeout, exitContext);
			if (cancelWaitHandle != null && num == waitableResults.Length)
			{
				num = -1;
			}
			WorkItem.ReleaseWaitHandles(waitableResults);
			return num;
		}
		private static void GetWaitHandles(IWaitableResult[] waitableResults, WaitHandle[] waitHandles)
		{
			for (int i = 0; i < waitableResults.Length; i++)
			{
				WorkItem.WorkItemResult workItemResult = waitableResults[i].GetWorkItemResult() as WorkItem.WorkItemResult;
				waitHandles[i] = workItemResult.GetWorkItem().GetWaitHandle();
			}
		}
		private static void ReleaseWaitHandles(IWaitableResult[] waitableResults)
		{
			for (int i = 0; i < waitableResults.Length; i++)
			{
				WorkItem.WorkItemResult workItemResult = (WorkItem.WorkItemResult)waitableResults[i].GetWorkItemResult();
				workItemResult.GetWorkItem().ReleaseWaitHandle();
			}
		}
		private WorkItem.WorkItemState GetWorkItemState()
		{
			Monitor.Enter(this);
			WorkItem.WorkItemState result;
			try
			{
				if (WorkItem.WorkItemState.Completed == this._workItemState)
				{
					result = this._workItemState;
				}
				else
				{
					long ticks = DateTime.UtcNow.Ticks;
					if (WorkItem.WorkItemState.Canceled != this._workItemState && ticks > this._expirationTime)
					{
						this._workItemState = WorkItem.WorkItemState.Canceled;
					}
					if (WorkItem.WorkItemState.InProgress == this._workItemState)
					{
						result = this._workItemState;
					}
					else
					{
						if (this.CanceledSmartThreadPool.IsCanceled || this.CanceledWorkItemsGroup.IsCanceled)
						{
							result = WorkItem.WorkItemState.Canceled;
						}
						else
						{
							result = this._workItemState;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		private void SetWorkItemState(WorkItem.WorkItemState workItemState)
		{
			Monitor.Enter(this);
			try
			{
				if (WorkItem.IsValidStatesTransition(this._workItemState, workItemState))
				{
					this._workItemState = workItemState;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		private void SignalComplete(bool canceled)
		{
			this.SetWorkItemState(canceled ? WorkItem.WorkItemState.Canceled : WorkItem.WorkItemState.Completed);
			Monitor.Enter(this);
			try
			{
				if (this._workItemCompleted != null)
				{
					this._workItemCompleted.Set();
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		internal void WorkItemIsQueued()
		{
			this._waitingOnQueueStopwatch.Start();
		}
		private bool Cancel(bool abortExecution)
		{
			bool result = false;
			bool flag = false;
			Monitor.Enter(this);
			try
			{
				switch (this.GetWorkItemState())
				{
				case WorkItem.WorkItemState.InQueue:
					flag = true;
					result = true;
					break;
				case WorkItem.WorkItemState.InProgress:
					if (abortExecution)
					{
						Thread thread = Interlocked.CompareExchange<Thread>(ref this._executingThread, null, this._executingThread);
						if (thread != null)
						{
							thread.Abort();
							result = true;
							flag = true;
						}
					}
					else
					{
						result = true;
						flag = true;
					}
					break;
				case WorkItem.WorkItemState.Canceled:
					result = true;
					break;
				}
				if (flag)
				{
					this.SignalComplete(true);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		private object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			Exception ex;
			object result = this.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out ex);
			if (ex != null)
			{
				throw new WorkItemResultException("The work item caused an excpetion, see the inner exception for details", ex);
			}
			return result;
		}
		private object GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
		{
			e = null;
			if (WorkItem.WorkItemState.Canceled == this.GetWorkItemState())
			{
				throw new WorkItemCancelException("Work item canceled");
			}
			if (this.IsCompleted)
			{
				e = this._exception;
				return this._result;
			}
			if (cancelWaitHandle == null)
			{
				WaitHandle waitHandle = this.GetWaitHandle();
				bool flag = !STPEventWaitHandle.WaitOne(waitHandle, millisecondsTimeout, exitContext);
				this.ReleaseWaitHandle();
				if (flag)
				{
					throw new WorkItemTimeoutException("Work item timeout");
				}
			}
			else
			{
				WaitHandle waitHandle2 = this.GetWaitHandle();
				int num = STPEventWaitHandle.WaitAny(new WaitHandle[]
				{
					waitHandle2,
					cancelWaitHandle
				});
				this.ReleaseWaitHandle();
				switch (num)
				{
				case -1:
				case 1:
					throw new WorkItemTimeoutException("Work item timeout");
				}
			}
			if (WorkItem.WorkItemState.Canceled == this.GetWorkItemState())
			{
				throw new WorkItemCancelException("Work item canceled");
			}
			e = this._exception;
			return this._result;
		}
		private WaitHandle GetWaitHandle()
		{
			Monitor.Enter(this);
			try
			{
				if (this._workItemCompleted == null)
				{
					this._workItemCompleted = EventWaitHandleFactory.CreateManualResetEvent(this.IsCompleted);
				}
				this._workItemCompletedRefCount++;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return this._workItemCompleted;
		}
		private void ReleaseWaitHandle()
		{
			Monitor.Enter(this);
			try
			{
				if (this._workItemCompleted != null)
				{
					this._workItemCompletedRefCount--;
					if (this._workItemCompletedRefCount == 0)
					{
						this._workItemCompleted.Close();
						this._workItemCompleted = null;
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void DisposeOfState()
		{
			if (this._workItemInfo.DisposeOfStateObjects)
			{
				IDisposable disposable = this._state as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
					this._state = null;
				}
			}
		}
	}
}
