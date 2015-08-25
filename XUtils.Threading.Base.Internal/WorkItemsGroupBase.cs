using System;
namespace XUtils.Threading.Base.Internal
{
	public abstract class WorkItemsGroupBase : IWorkItemsGroup
	{
		private string _name = "WorkItemsGroupBase";
		public abstract event WorkItemsGroupIdleHandler OnIdle;
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		public abstract int Concurrency
		{
			get;
			set;
		}
		public abstract int WaitingCallbacks
		{
			get;
		}
		public abstract WIGStartInfo WIGStartInfo
		{
			get;
		}
		public bool IsIdle
		{
			get;
			protected set;
		}
		public WorkItemsGroupBase()
		{
			this.IsIdle = true;
		}
		public abstract object[] GetStates();
		public abstract void Start();
		public abstract void Cancel(bool abortExecution);
		public abstract bool WaitForIdle(int millisecondsTimeout);
		internal abstract void Enqueue(WorkItem workItem);
		internal virtual void PreQueueWorkItem()
		{
		}
		public virtual void Cancel()
		{
			this.Cancel(false);
		}
		public void WaitForIdle()
		{
			this.WaitForIdle(-1);
		}
		public bool WaitForIdle(TimeSpan timeout)
		{
			return this.WaitForIdle((int)timeout.TotalMilliseconds);
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback)
		{
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, workItemPriority);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, workItemInfo, callback);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state)
		{
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state, workItemPriority);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, workItemInfo, callback, state);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state, postExecuteWorkItemCallback);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state, postExecuteWorkItemCallback, workItemPriority);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute, workItemPriority);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem(XUtils.Threading.Base.Action action)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, delegate
			{
				action();
				return null;
			});
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem<T>(Action<T> action, T arg)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, delegate(object state)
			{
				action(arg);
				return null;
			}, this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg
			} : null);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem<T1, T2>(XUtils.Threading.Base.Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, delegate(object state)
			{
				action(arg1, arg2);
				return null;
			}, this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2
			} : null);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem<T1, T2, T3>(XUtils.Threading.Base.Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, delegate(object state)
			{
				action(arg1, arg2, arg3);
				return null;
			}, this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2,
				arg3
			} : null);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult QueueWorkItem<T1, T2, T3, T4>(XUtils.Threading.Base.Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, delegate(object state)
			{
				action(arg1, arg2, arg3, arg4);
				return null;
			}, this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2,
				arg3,
				arg4
			} : null);
			this.Enqueue(workItem);
			return workItem.GetWorkItemResult();
		}
		public IWorkItemResult<TResult> QueueWorkItem<TResult>(XUtils.Threading.Base.Func<TResult> func)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, (object state) => func());
			this.Enqueue(workItem);
			return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
		}
		public IWorkItemResult<TResult> QueueWorkItem<T, TResult>(XUtils.Threading.Base.Func<T, TResult> func, T arg)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, (object state) => func(arg), this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg
			} : null);
			this.Enqueue(workItem);
			return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
		}
		public IWorkItemResult<TResult> QueueWorkItem<T1, T2, TResult>(XUtils.Threading.Base.Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, (object state) => func(arg1, arg2), this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2
			} : null);
			this.Enqueue(workItem);
			return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
		}
		public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, TResult>(XUtils.Threading.Base.Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, (object state) => func(arg1, arg2, arg3), this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2,
				arg3
			} : null);
			this.Enqueue(workItem);
			return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
		}
		public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, T4, TResult>(XUtils.Threading.Base.Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			this.PreQueueWorkItem();
			WorkItem workItem = WorkItemFactory.CreateWorkItem(this, this.WIGStartInfo, (object state) => func(arg1, arg2, arg3, arg4), this.WIGStartInfo.FillStateWithArgs ? new object[]
			{
				arg1,
				arg2,
				arg3,
				arg4
			} : null);
			this.Enqueue(workItem);
			return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
		}
	}
}
