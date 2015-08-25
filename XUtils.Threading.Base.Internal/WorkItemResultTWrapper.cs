using System;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	internal class WorkItemResultTWrapper<TResult> : IWorkItemResult<TResult>, IWaitableResult, IInternalWaitableResult
	{
		private readonly IWorkItemResult _workItemResult;
		public bool IsCompleted
		{
			get
			{
				return this._workItemResult.IsCompleted;
			}
		}
		public bool IsCanceled
		{
			get
			{
				return this._workItemResult.IsCanceled;
			}
		}
		public object State
		{
			get
			{
				return this._workItemResult.State;
			}
		}
		public WorkItemPriority WorkItemPriority
		{
			get
			{
				return this._workItemResult.WorkItemPriority;
			}
		}
		public TResult Result
		{
			get
			{
				return (TResult)((object)this._workItemResult.Result);
			}
		}
		public object Exception
		{
			get
			{
				return (TResult)((object)this._workItemResult.Exception);
			}
		}
		public WorkItemResultTWrapper(IWorkItemResult workItemResult)
		{
			this._workItemResult = workItemResult;
		}
		public TResult GetResult()
		{
			return (TResult)((object)this._workItemResult.GetResult());
		}
		public TResult GetResult(int millisecondsTimeout, bool exitContext)
		{
			return (TResult)((object)this._workItemResult.GetResult(millisecondsTimeout, exitContext));
		}
		public TResult GetResult(TimeSpan timeout, bool exitContext)
		{
			return (TResult)((object)this._workItemResult.GetResult(timeout, exitContext));
		}
		public TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return (TResult)((object)this._workItemResult.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle));
		}
		public TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return (TResult)((object)this._workItemResult.GetResult(timeout, exitContext, cancelWaitHandle));
		}
		public TResult GetResult(out Exception e)
		{
			return (TResult)((object)this._workItemResult.GetResult(out e));
		}
		public TResult GetResult(int millisecondsTimeout, bool exitContext, out Exception e)
		{
			return (TResult)((object)this._workItemResult.GetResult(millisecondsTimeout, exitContext, out e));
		}
		public TResult GetResult(TimeSpan timeout, bool exitContext, out Exception e)
		{
			return (TResult)((object)this._workItemResult.GetResult(timeout, exitContext, out e));
		}
		public TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
		{
			return (TResult)((object)this._workItemResult.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out e));
		}
		public TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
		{
			return (TResult)((object)this._workItemResult.GetResult(timeout, exitContext, cancelWaitHandle, out e));
		}
		public bool Cancel()
		{
			return this._workItemResult.Cancel();
		}
		public bool Cancel(bool abortExecution)
		{
			return this._workItemResult.Cancel(abortExecution);
		}
		public IWorkItemResult GetWorkItemResult()
		{
			return this._workItemResult.GetWorkItemResult();
		}
		public IWorkItemResult<TRes> GetWorkItemResultT<TRes>()
		{
			return (IWorkItemResult<TRes>)this;
		}
	}
}
