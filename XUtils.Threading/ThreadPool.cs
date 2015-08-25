using System;
using System.Threading;
using XUtils.Threading.Base;
namespace XUtils.Threading
{
	public class ThreadPool : IDisposable
	{
		private SmartThreadPool _smartThreadPool;
		private IWorkItemsGroup _workItemsGroup;
		private static ThreadPool xThreadPool = null;
		private static object syncObject = new object();
		private int workItemsGroup;
		public static ThreadPool GetInstance
		{
			get
			{
				if (ThreadPool.xThreadPool == null)
				{
					object obj;
					Monitor.Enter(obj = ThreadPool.syncObject);
					try
					{
						if (ThreadPool.xThreadPool == null)
						{
							ThreadPool.xThreadPool = new ThreadPool();
							ThreadPool.xThreadPool.Start();
						}
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				return ThreadPool.xThreadPool;
			}
		}
		public int InUseThreads
		{
			get
			{
				return this._smartThreadPool.InUseThreads;
			}
		}
		public int ActiveThreads
		{
			get
			{
				return this._smartThreadPool.ActiveThreads;
			}
		}
		public int WaitingCallbacks
		{
			get
			{
				return this._workItemsGroup.WaitingCallbacks;
			}
		}
		public int WorkItemsGroup
		{
			get
			{
				if (this.workItemsGroup == 0)
				{
					this.workItemsGroup = 1;
				}
				return this.workItemsGroup;
			}
			set
			{
				this.workItemsGroup = value;
			}
		}
		public SmartThreadPool SmartThreadPool
		{
			get
			{
				return this._smartThreadPool;
			}
		}
		~ThreadPool()
		{
			this.Close();
		}
		public void Start()
		{
			this._smartThreadPool = new SmartThreadPool(new STPStartInfo
			{
				IdleTimeout = 10000
			});
			this._workItemsGroup = this._smartThreadPool.CreateWorkItemsGroup(this.workItemsGroup);
		}
		public void Close()
		{
			if (this._smartThreadPool != null)
			{
				this._smartThreadPool.Shutdown();
				this._smartThreadPool.Dispose();
				this._smartThreadPool = null;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
		public void QueueWorkItem(WorkItemPriority _workItemPriority, Action<IWorkItemsGroup> _action)
		{
			this._workItemsGroup.QueueWorkItem(delegate(object state)
			{
				(state as Action<IWorkItemsGroup>)(this._workItemsGroup);
				return null;
			}, _action, _workItemPriority);
		}
		public void QueueWorkItem<T>(WorkItemPriority _workItemPriority, System.Action<IWorkItemsGroup, T> _action, T t)
		{
			this._workItemsGroup.QueueWorkItem(delegate(object state)
			{
				(state as System.Action<IWorkItemsGroup, T>)(this._workItemsGroup, t);
				return null;
			}, _action, _workItemPriority);
		}
		public void QueueWorkItem<T1, T2>(WorkItemPriority _workItemPriority, System.Action<IWorkItemsGroup, T1, T2> _action, T1 t1, T2 t2)
		{
			this._workItemsGroup.QueueWorkItem(delegate(object state)
			{
				(state as System.Action<IWorkItemsGroup, T1, T2>)(this._workItemsGroup, t1, t2);
				return null;
			}, _action, _workItemPriority);
		}
		public void QueueWorkItem<T1, T2, T3>(WorkItemPriority _workItemPriority, System.Action<IWorkItemsGroup, T1, T2, T3> _action, T1 t1, T2 t2, T3 t3)
		{
			this._workItemsGroup.QueueWorkItem(delegate(object state)
			{
				(state as System.Action<IWorkItemsGroup, T1, T2, T3>)(this._workItemsGroup, t1, t2, t3);
				return null;
			}, _action, _workItemPriority);
		}
		public void QueueWorkItem(Action<IWorkItemsGroup> _action)
		{
			this._workItemsGroup.QueueWorkItem<Action<IWorkItemsGroup>>(delegate(Action<IWorkItemsGroup> state)
			{
				state(this._workItemsGroup);
			}, _action);
		}
		public void QueueWorkItem<T>(System.Action<IWorkItemsGroup, T> _action, T t)
		{
			this._workItemsGroup.QueueWorkItem<System.Action<IWorkItemsGroup, T>>(delegate(System.Action<IWorkItemsGroup, T> state)
			{
				state(this._workItemsGroup, t);
			}, _action);
		}
		public void QueueWorkItem<T1, T2>(System.Action<IWorkItemsGroup, T1, T2> _action, T1 t1, T2 t2)
		{
			this._workItemsGroup.QueueWorkItem<System.Action<IWorkItemsGroup, T1, T2>>(delegate(System.Action<IWorkItemsGroup, T1, T2> state)
			{
				state(this._workItemsGroup, t1, t2);
			}, _action);
		}
		public void QueueWorkItem<T1, T2, T3>(System.Action<IWorkItemsGroup, T1, T2, T3> _action, T1 t1, T2 t2, T3 t3)
		{
			this._workItemsGroup.QueueWorkItem<System.Action<IWorkItemsGroup, T1, T2, T3>>(delegate(System.Action<IWorkItemsGroup, T1, T2, T3> state)
			{
				state(this._workItemsGroup, t1, t2, t3);
			}, _action);
		}
		public void Dispose()
		{
			this.Close();
		}
	}
}
