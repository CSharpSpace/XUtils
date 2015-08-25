using System;
namespace XUtils.Threading.Base
{
	public interface IWorkItemsGroup
	{
		event WorkItemsGroupIdleHandler OnIdle;
		string Name
		{
			get;
			set;
		}
		int Concurrency
		{
			get;
			set;
		}
		int WaitingCallbacks
		{
			get;
		}
		WIGStartInfo WIGStartInfo
		{
			get;
		}
		bool IsIdle
		{
			get;
		}
		object[] GetStates();
		void Start();
		void Cancel();
		void Cancel(bool abortExecution);
		void WaitForIdle();
		bool WaitForIdle(TimeSpan timeout);
		bool WaitForIdle(int millisecondsTimeout);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute);
		IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority);
		IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback);
		IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state);
		IWorkItemResult QueueWorkItem(Action action);
		IWorkItemResult QueueWorkItem<T>(Action<T> action, T arg);
		IWorkItemResult QueueWorkItem<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);
		IWorkItemResult QueueWorkItem<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);
		IWorkItemResult QueueWorkItem<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
		IWorkItemResult<TResult> QueueWorkItem<TResult>(Func<TResult> func);
		IWorkItemResult<TResult> QueueWorkItem<T, TResult>(Func<T, TResult> func, T arg);
		IWorkItemResult<TResult> QueueWorkItem<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2);
		IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3);
		IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	}
}
