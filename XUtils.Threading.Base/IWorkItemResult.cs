using System;
using System.Threading;
namespace XUtils.Threading.Base
{
	public interface IWorkItemResult<TResult> : IWaitableResult
	{
		bool IsCompleted
		{
			get;
		}
		bool IsCanceled
		{
			get;
		}
		object State
		{
			get;
		}
		WorkItemPriority WorkItemPriority
		{
			get;
		}
		TResult Result
		{
			get;
		}
		object Exception
		{
			get;
		}
		TResult GetResult();
		TResult GetResult(int millisecondsTimeout, bool exitContext);
		TResult GetResult(TimeSpan timeout, bool exitContext);
		TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle);
		TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle);
		TResult GetResult(out Exception e);
		TResult GetResult(int millisecondsTimeout, bool exitContext, out Exception e);
		TResult GetResult(TimeSpan timeout, bool exitContext, out Exception e);
		TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e);
		TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e);
		bool Cancel();
		bool Cancel(bool abortExecution);
	}
	public interface IWorkItemResult : IWorkItemResult<object>, IWaitableResult
	{
	}
}
