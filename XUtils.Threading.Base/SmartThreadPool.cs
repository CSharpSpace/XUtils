using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using XUtils.Threading.Base.Internal;
namespace XUtils.Threading.Base
{
	public class SmartThreadPool : WorkItemsGroupBase, IDisposable
	{
		private class ChoiceIndex
		{
			public int _index = -1;
		}
		internal class ThreadEntry
		{
			private readonly DateTime _creationTime;
			private DateTime _lastAliveTime;
			private readonly SmartThreadPool _associatedSmartThreadPool;
			public WorkItem CurrentWorkItem
			{
				get;
				set;
			}
			public SmartThreadPool AssociatedSmartThreadPool
			{
				get
				{
					return this._associatedSmartThreadPool;
				}
			}
			public ThreadEntry(SmartThreadPool stp)
			{
				this._associatedSmartThreadPool = stp;
				this._creationTime = DateTime.UtcNow;
				this._lastAliveTime = DateTime.MinValue;
			}
			public void IAmAlive()
			{
				this._lastAliveTime = DateTime.UtcNow;
			}
		}
		public const int DefaultMinWorkerThreads = 0;
		public const int DefaultMaxWorkerThreads = 25;
		public const int DefaultIdleTimeout = 60000;
		public const bool DefaultUseCallerCallContext = false;
		public const bool DefaultUseCallerHttpContext = false;
		public const bool DefaultDisposeOfStateObjects = false;
		public const CallToPostExecute DefaultCallToPostExecute = CallToPostExecute.Always;
		public const WorkItemPriority DefaultWorkItemPriority = WorkItemPriority.Normal;
		public const bool DefaultStartSuspended = false;
		public const ThreadPriority DefaultThreadPriority = ThreadPriority.Normal;
		public const bool DefaultFillStateWithArgs = false;
		public static readonly PostExecuteWorkItemCallback DefaultPostExecuteWorkItemCallback;
		public static readonly string DefaultPerformanceCounterInstanceName;
		private readonly SynchronizedDictionary<Thread, SmartThreadPool.ThreadEntry> _workerThreads = new SynchronizedDictionary<Thread, SmartThreadPool.ThreadEntry>();
		private readonly WorkItemsQueue _workItemsQueue = new WorkItemsQueue();
		private int _workItemsProcessed;
		private int _inUseWorkerThreads;
		private STPStartInfo _stpStartInfo;
		private int _currentWorkItemsCount;
		private ManualResetEvent _isIdleWaitHandle = EventWaitHandleFactory.CreateManualResetEvent(true);
		private ManualResetEvent _shuttingDownEvent = EventWaitHandleFactory.CreateManualResetEvent(false);
		private bool _isSuspended;
		private bool _shutdown;
		private int _threadCounter;
		private bool _isDisposed;
		private readonly SynchronizedDictionary<IWorkItemsGroup, IWorkItemsGroup> _workItemsGroups = new SynchronizedDictionary<IWorkItemsGroup, IWorkItemsGroup>();
		private CanceledWorkItemsGroup _canceledSmartThreadPool = new CanceledWorkItemsGroup();
		private ISTPInstancePerformanceCounters _windowsPCs = NullSTPInstancePerformanceCounters.Instance;
		private ISTPInstancePerformanceCounters _localPCs = NullSTPInstancePerformanceCounters.Instance;
		[ThreadStatic]
		private static SmartThreadPool.ThreadEntry _threadEntry;
		private event ThreadInitializationHandler _onThreadInitialization;
		private event ThreadTerminationHandler _onThreadTermination;
		public event ThreadInitializationHandler OnThreadInitialization
		{
			add
			{
				this._onThreadInitialization += value;
			}
			remove
			{
				this._onThreadInitialization -= value;
			}
		}
		public event ThreadTerminationHandler OnThreadTermination
		{
			add
			{
				this._onThreadTermination += value;
			}
			remove
			{
				this._onThreadTermination -= value;
			}
		}
		public override event WorkItemsGroupIdleHandler OnIdle
		{
			add
			{
				throw new NotImplementedException("This event is not implemented in the SmartThreadPool class. Please create a WorkItemsGroup in order to use this feature.");
			}
			remove
			{
				throw new NotImplementedException("This event is not implemented in the SmartThreadPool class. Please create a WorkItemsGroup in order to use this feature.");
			}
		}
		internal static SmartThreadPool.ThreadEntry CurrentThreadEntry
		{
			get
			{
				return SmartThreadPool._threadEntry;
			}
			set
			{
				SmartThreadPool._threadEntry = value;
			}
		}
		public int MinThreads
		{
			get
			{
				this.ValidateNotDisposed();
				return this._stpStartInfo.MinWorkerThreads;
			}
			set
			{
				if (this._stpStartInfo.MaxWorkerThreads < value)
				{
					this._stpStartInfo.MaxWorkerThreads = value;
				}
				this._stpStartInfo.MinWorkerThreads = value;
				this.StartOptimalNumberOfThreads();
			}
		}
		public int MaxThreads
		{
			get
			{
				this.ValidateNotDisposed();
				return this._stpStartInfo.MaxWorkerThreads;
			}
			set
			{
				if (this._stpStartInfo.MinWorkerThreads > value)
				{
					this._stpStartInfo.MinWorkerThreads = value;
				}
				this._stpStartInfo.MaxWorkerThreads = value;
				this.StartOptimalNumberOfThreads();
			}
		}
		public int ActiveThreads
		{
			get
			{
				this.ValidateNotDisposed();
				return this._workerThreads.Count;
			}
		}
		public int InUseThreads
		{
			get
			{
				this.ValidateNotDisposed();
				return this._inUseWorkerThreads;
			}
		}
		public static bool IsWorkItemCanceled
		{
			get
			{
				return SmartThreadPool.CurrentThreadEntry.CurrentWorkItem.IsCanceled;
			}
		}
		public STPStartInfo STPStartInfo
		{
			get
			{
				return this._stpStartInfo.AsReadOnly();
			}
		}
		public bool IsShuttingdown
		{
			get
			{
				return this._shutdown;
			}
		}
		public ISTPPerformanceCountersReader PerformanceCountersReader
		{
			get
			{
				return (ISTPPerformanceCountersReader)this._localPCs;
			}
		}
		public override int Concurrency
		{
			get
			{
				return this.MaxThreads;
			}
			set
			{
				this.MaxThreads = value;
			}
		}
		public override int WaitingCallbacks
		{
			get
			{
				this.ValidateNotDisposed();
				return this._workItemsQueue.Count;
			}
		}
		public override WIGStartInfo WIGStartInfo
		{
			get
			{
				return this._stpStartInfo.AsReadOnly();
			}
		}
		public SmartThreadPool()
		{
			this._stpStartInfo = new STPStartInfo();
			this.Initialize();
		}
		public SmartThreadPool(int idleTimeout)
		{
			this._stpStartInfo = new STPStartInfo
			{
				IdleTimeout = idleTimeout
			};
			this.Initialize();
		}
		public SmartThreadPool(int idleTimeout, int maxWorkerThreads)
		{
			this._stpStartInfo = new STPStartInfo
			{
				IdleTimeout = idleTimeout,
				MaxWorkerThreads = maxWorkerThreads
			};
			this.Initialize();
		}
		public SmartThreadPool(int idleTimeout, int maxWorkerThreads, int minWorkerThreads)
		{
			this._stpStartInfo = new STPStartInfo
			{
				IdleTimeout = idleTimeout,
				MaxWorkerThreads = maxWorkerThreads,
				MinWorkerThreads = minWorkerThreads
			};
			this.Initialize();
		}
		public SmartThreadPool(STPStartInfo stpStartInfo)
		{
			this._stpStartInfo = new STPStartInfo(stpStartInfo);
			this.Initialize();
		}
		private void Initialize()
		{
			base.Name = "ThreadPool";
			this.ValidateSTPStartInfo();
			this._isSuspended = this._stpStartInfo.StartSuspended;
			if (this._stpStartInfo.PerformanceCounterInstanceName != null)
			{
				try
				{
					this._windowsPCs = new STPInstancePerformanceCounters(this._stpStartInfo.PerformanceCounterInstanceName);
				}
				catch (Exception)
				{
					this._windowsPCs = NullSTPInstancePerformanceCounters.Instance;
				}
			}
			if (this._stpStartInfo.EnableLocalPerformanceCounters)
			{
				this._localPCs = new LocalSTPInstancePerformanceCounters();
			}
			if (!this._isSuspended)
			{
				this.StartOptimalNumberOfThreads();
			}
		}
		private void StartOptimalNumberOfThreads()
		{
			int num = Math.Max(this._workItemsQueue.Count, this._stpStartInfo.MinWorkerThreads);
			num = Math.Min(num, this._stpStartInfo.MaxWorkerThreads);
			num -= this._workerThreads.Count;
			if (num > 0)
			{
				this.StartThreads(num);
			}
		}
		private void ValidateSTPStartInfo()
		{
			if (this._stpStartInfo.MinWorkerThreads < 0)
			{
				throw new ArgumentOutOfRangeException("MinWorkerThreads", "MinWorkerThreads cannot be negative");
			}
			if (this._stpStartInfo.MaxWorkerThreads <= 0)
			{
				throw new ArgumentOutOfRangeException("MaxWorkerThreads", "MaxWorkerThreads must be greater than zero");
			}
			if (this._stpStartInfo.MinWorkerThreads > this._stpStartInfo.MaxWorkerThreads)
			{
				throw new ArgumentOutOfRangeException("MinWorkerThreads, maxWorkerThreads", "MaxWorkerThreads must be greater or equal to MinWorkerThreads");
			}
		}
		private static void ValidateCallback(Delegate callback)
		{
			if (callback.GetInvocationList().Length > 1)
			{
				throw new NotSupportedException("SmartThreadPool doesn't support delegates chains");
			}
		}
		private WorkItem Dequeue()
		{
			return this._workItemsQueue.DequeueWorkItem(this._stpStartInfo.IdleTimeout, this._shuttingDownEvent);
		}
		internal override void Enqueue(WorkItem workItem)
		{
			this.IncrementWorkItemsCount();
			workItem.CanceledSmartThreadPool = this._canceledSmartThreadPool;
			this._workItemsQueue.EnqueueWorkItem(workItem);
			workItem.WorkItemIsQueued();
			if (this.InUseThreads + this.WaitingCallbacks > this._workerThreads.Count)
			{
				this.StartThreads(1);
			}
		}
		private void IncrementWorkItemsCount()
		{
			this._windowsPCs.SampleWorkItems((long)this._workItemsQueue.Count, (long)this._workItemsProcessed);
			this._localPCs.SampleWorkItems((long)this._workItemsQueue.Count, (long)this._workItemsProcessed);
			int num = Interlocked.Increment(ref this._currentWorkItemsCount);
			if (num == 1)
			{
				base.IsIdle = false;
				this._isIdleWaitHandle.Reset();
			}
		}
		private void DecrementWorkItemsCount()
		{
			if (Interlocked.Decrement(ref this._currentWorkItemsCount) == 0)
			{
				base.IsIdle = true;
				this._isIdleWaitHandle.Set();
			}
			Interlocked.Increment(ref this._workItemsProcessed);
			if (!this._shutdown)
			{
				this._windowsPCs.SampleWorkItems((long)this._workItemsQueue.Count, (long)this._workItemsProcessed);
				this._localPCs.SampleWorkItems((long)this._workItemsQueue.Count, (long)this._workItemsProcessed);
			}
		}
		internal void RegisterWorkItemsGroup(IWorkItemsGroup workItemsGroup)
		{
			this._workItemsGroups[workItemsGroup] = workItemsGroup;
		}
		internal void UnregisterWorkItemsGroup(IWorkItemsGroup workItemsGroup)
		{
			if (this._workItemsGroups.Contains(workItemsGroup))
			{
				this._workItemsGroups.Remove(workItemsGroup);
			}
		}
		private void InformCompleted()
		{
			if (this._workerThreads.Contains(Thread.CurrentThread))
			{
				this._workerThreads.Remove(Thread.CurrentThread);
				this._windowsPCs.SampleThreads((long)this._workerThreads.Count, (long)this._inUseWorkerThreads);
				this._localPCs.SampleThreads((long)this._workerThreads.Count, (long)this._inUseWorkerThreads);
			}
		}
		private void StartThreads(int threadsCount)
		{
			if (this._isSuspended)
			{
				return;
			}
			object syncRoot;
			Monitor.Enter(syncRoot = this._workerThreads.SyncRoot);
			try
			{
				if (!this._shutdown)
				{
					for (int i = 0; i < threadsCount; i++)
					{
						if (this._workerThreads.Count >= this._stpStartInfo.MaxWorkerThreads)
						{
							break;
						}
						Thread thread = new Thread(new ThreadStart(this.ProcessQueuedItems));
						thread.Name = string.Concat(new object[]
						{
							"STP ",
							base.Name,
							" Thread #",
							this._threadCounter
						});
						thread.IsBackground = true;
						thread.Priority = this._stpStartInfo.ThreadPriority;
						thread.Start();
						this._threadCounter++;
						this._workerThreads[thread] = new SmartThreadPool.ThreadEntry(this);
						this._windowsPCs.SampleThreads((long)this._workerThreads.Count, (long)this._inUseWorkerThreads);
						this._localPCs.SampleThreads((long)this._workerThreads.Count, (long)this._inUseWorkerThreads);
					}
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
		private void ProcessQueuedItems()
		{
			SmartThreadPool.CurrentThreadEntry = this._workerThreads[Thread.CurrentThread];
			this.FireOnThreadInitialization();
			try
			{
				bool flag = false;
				while (!this._shutdown)
				{
					SmartThreadPool.CurrentThreadEntry.IAmAlive();
					if (this._workerThreads.Count > this._stpStartInfo.MaxWorkerThreads)
					{
						object syncRoot;
						Monitor.Enter(syncRoot = this._workerThreads.SyncRoot);
						try
						{
							if (this._workerThreads.Count > this._stpStartInfo.MaxWorkerThreads)
							{
								this.InformCompleted();
								break;
							}
						}
						finally
						{
							Monitor.Exit(syncRoot);
						}
					}
					WorkItem workItem = this.Dequeue();
					SmartThreadPool.CurrentThreadEntry.IAmAlive();
					if (workItem == null && this._workerThreads.Count > this._stpStartInfo.MinWorkerThreads)
					{
						object syncRoot2;
						Monitor.Enter(syncRoot2 = this._workerThreads.SyncRoot);
						try
						{
							if (this._workerThreads.Count > this._stpStartInfo.MinWorkerThreads)
							{
								this.InformCompleted();
								break;
							}
						}
						finally
						{
							Monitor.Exit(syncRoot2);
						}
					}
					if (workItem != null)
					{
						try
						{
							flag = false;
							SmartThreadPool.CurrentThreadEntry.CurrentWorkItem = workItem;
							if (workItem.StartingWorkItem())
							{
								int num = Interlocked.Increment(ref this._inUseWorkerThreads);
								this._windowsPCs.SampleThreads((long)this._workerThreads.Count, (long)num);
								this._localPCs.SampleThreads((long)this._workerThreads.Count, (long)num);
								flag = true;
								workItem.FireWorkItemStarted();
								this.ExecuteWorkItem(workItem);
							}
						}
						catch (Exception ex)
						{
							ex.GetHashCode();
						}
						finally
						{
							workItem.DisposeOfState();
							SmartThreadPool.CurrentThreadEntry.CurrentWorkItem = null;
							if (flag)
							{
								int num2 = Interlocked.Decrement(ref this._inUseWorkerThreads);
								this._windowsPCs.SampleThreads((long)this._workerThreads.Count, (long)num2);
								this._localPCs.SampleThreads((long)this._workerThreads.Count, (long)num2);
							}
							workItem.FireWorkItemCompleted();
							this.DecrementWorkItemsCount();
						}
					}
				}
			}
			catch (ThreadAbortException ex2)
			{
				ex2.GetHashCode();
				Thread.ResetAbort();
			}
			catch (Exception)
			{
			}
			finally
			{
				this.InformCompleted();
				this.FireOnThreadTermination();
			}
		}
		private void ExecuteWorkItem(WorkItem workItem)
		{
			this._windowsPCs.SampleWorkItemsWaitTime(workItem.WaitingTime);
			this._localPCs.SampleWorkItemsWaitTime(workItem.WaitingTime);
			try
			{
				workItem.Execute();
			}
			finally
			{
				this._windowsPCs.SampleWorkItemsProcessTime(workItem.ProcessTime);
				this._localPCs.SampleWorkItemsProcessTime(workItem.ProcessTime);
			}
		}
		private void ValidateWaitForIdle()
		{
			if (SmartThreadPool.CurrentThreadEntry != null && SmartThreadPool.CurrentThreadEntry.AssociatedSmartThreadPool == this)
			{
				throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
			}
		}
		internal static void ValidateWorkItemsGroupWaitForIdle(IWorkItemsGroup workItemsGroup)
		{
			if (SmartThreadPool.CurrentThreadEntry == null)
			{
				return;
			}
			WorkItem currentWorkItem = SmartThreadPool.CurrentThreadEntry.CurrentWorkItem;
			SmartThreadPool.ValidateWorkItemsGroupWaitForIdleImpl(workItemsGroup, currentWorkItem);
			if (workItemsGroup != null && currentWorkItem != null && SmartThreadPool.CurrentThreadEntry.CurrentWorkItem.WasQueuedBy(workItemsGroup))
			{
				throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
			}
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ValidateWorkItemsGroupWaitForIdleImpl(IWorkItemsGroup workItemsGroup, WorkItem workItem)
		{
			if (workItemsGroup != null && workItem != null && workItem.WasQueuedBy(workItemsGroup))
			{
				throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
			}
		}
		public void Shutdown()
		{
			this.Shutdown(true, 0);
		}
		public void Shutdown(bool forceAbort, TimeSpan timeout)
		{
			this.Shutdown(forceAbort, (int)timeout.TotalMilliseconds);
		}
		public void Shutdown(bool forceAbort, int millisecondsTimeout)
		{
			this.ValidateNotDisposed();
			ISTPInstancePerformanceCounters windowsPCs = this._windowsPCs;
			if (NullSTPInstancePerformanceCounters.Instance != this._windowsPCs)
			{
				this._windowsPCs = NullSTPInstancePerformanceCounters.Instance;
				windowsPCs.Dispose();
			}
			object syncRoot;
			Monitor.Enter(syncRoot = this._workerThreads.SyncRoot);
			Thread[] array;
			try
			{
				this._workItemsQueue.Dispose();
				this._shutdown = true;
				this._shuttingDownEvent.Set();
				array = new Thread[this._workerThreads.Count];
				this._workerThreads.Keys.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			int num = millisecondsTimeout;
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
			bool flag = -1 == millisecondsTimeout;
			bool flag2 = false;
			Thread[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Thread thread = array2[i];
				if (!flag && num < 0)
				{
					flag2 = true;
					break;
				}
				if (!thread.Join(num))
				{
					flag2 = true;
					break;
				}
				if (!flag)
				{
					num = millisecondsTimeout - (int)stopwatch.ElapsedMilliseconds;
				}
			}
			if (flag2 && forceAbort)
			{
				Thread[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					Thread thread2 = array3[j];
					if (thread2 != null && thread2.IsAlive)
					{
						try
						{
							thread2.Abort();
						}
						catch (SecurityException ex)
						{
							ex.GetHashCode();
						}
						catch (ThreadStateException ex2)
						{
							ex2.GetHashCode();
						}
					}
				}
			}
		}
		public static bool WaitAll(IWaitableResult[] waitableResults)
		{
			return SmartThreadPool.WaitAll(waitableResults, -1, true);
		}
		public static bool WaitAll(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext)
		{
			return SmartThreadPool.WaitAll(waitableResults, (int)timeout.TotalMilliseconds, exitContext);
		}
		public static bool WaitAll(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return SmartThreadPool.WaitAll(waitableResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
		}
		public static bool WaitAll(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext)
		{
			return WorkItem.WaitAll(waitableResults, millisecondsTimeout, exitContext, null);
		}
		public static bool WaitAll(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return WorkItem.WaitAll(waitableResults, millisecondsTimeout, exitContext, cancelWaitHandle);
		}
		public static int WaitAny(IWaitableResult[] waitableResults)
		{
			return SmartThreadPool.WaitAny(waitableResults, -1, true);
		}
		public static int WaitAny(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext)
		{
			return SmartThreadPool.WaitAny(waitableResults, (int)timeout.TotalMilliseconds, exitContext);
		}
		public static int WaitAny(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return SmartThreadPool.WaitAny(waitableResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
		}
		public static int WaitAny(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext)
		{
			return WorkItem.WaitAny(waitableResults, millisecondsTimeout, exitContext, null);
		}
		public static int WaitAny(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
		{
			return WorkItem.WaitAny(waitableResults, millisecondsTimeout, exitContext, cancelWaitHandle);
		}
		public IWorkItemsGroup CreateWorkItemsGroup(int concurrency)
		{
			return new WorkItemsGroup(this, concurrency, this._stpStartInfo);
		}
		public IWorkItemsGroup CreateWorkItemsGroup(int concurrency, WIGStartInfo wigStartInfo)
		{
			return new WorkItemsGroup(this, concurrency, wigStartInfo);
		}
		private void FireOnThreadInitialization()
		{
			if (this._onThreadInitialization != null)
			{
				Delegate[] invocationList = this._onThreadInitialization.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					ThreadInitializationHandler threadInitializationHandler = (ThreadInitializationHandler)invocationList[i];
					try
					{
						threadInitializationHandler();
					}
					catch (Exception ex)
					{
						ex.GetHashCode();
						throw;
					}
				}
			}
		}
		private void FireOnThreadTermination()
		{
			if (this._onThreadTermination != null)
			{
				Delegate[] invocationList = this._onThreadTermination.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					ThreadTerminationHandler threadTerminationHandler = (ThreadTerminationHandler)invocationList[i];
					try
					{
						threadTerminationHandler();
					}
					catch (Exception ex)
					{
						ex.GetHashCode();
						throw;
					}
				}
			}
		}
		internal void CancelAbortWorkItemsGroup(WorkItemsGroup wig)
		{
			foreach (SmartThreadPool.ThreadEntry current in this._workerThreads.Values)
			{
				WorkItem currentWorkItem = current.CurrentWorkItem;
				if (currentWorkItem != null && currentWorkItem.WasQueuedBy(wig) && !currentWorkItem.IsCanceled)
				{
					current.CurrentWorkItem.GetWorkItemResult().Cancel(true);
				}
			}
		}
		public static void AbortOnWorkItemCancel()
		{
			if (SmartThreadPool.IsWorkItemCanceled)
			{
				Thread.CurrentThread.Abort();
			}
		}
		public void Dispose()
		{
			if (!this._isDisposed)
			{
				if (!this._shutdown)
				{
					this.Shutdown();
				}
				if (this._shuttingDownEvent != null)
				{
					this._shuttingDownEvent.Close();
					this._shuttingDownEvent = null;
				}
				this._workerThreads.Clear();
				if (this._isIdleWaitHandle != null)
				{
					this._isIdleWaitHandle.Close();
					this._isIdleWaitHandle = null;
				}
				this._isDisposed = true;
			}
		}
		private void ValidateNotDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString(), "The SmartThreadPool has been shutdown");
			}
		}
		public override object[] GetStates()
		{
			return this._workItemsQueue.GetStates();
		}
		public override void Start()
		{
			if (!this._isSuspended)
			{
				return;
			}
			this._isSuspended = false;
			ICollection values = this._workItemsGroups.Values;
			foreach (WorkItemsGroup workItemsGroup in values)
			{
				workItemsGroup.OnSTPIsStarting();
			}
			this.StartOptimalNumberOfThreads();
		}
		public override void Cancel(bool abortExecution)
		{
			this._canceledSmartThreadPool.IsCanceled = true;
			this._canceledSmartThreadPool = new CanceledWorkItemsGroup();
			ICollection values = this._workItemsGroups.Values;
			foreach (WorkItemsGroup workItemsGroup in values)
			{
				workItemsGroup.Cancel(abortExecution);
			}
			if (abortExecution)
			{
				foreach (SmartThreadPool.ThreadEntry current in this._workerThreads.Values)
				{
					WorkItem currentWorkItem = current.CurrentWorkItem;
					if (currentWorkItem != null && current.AssociatedSmartThreadPool == this && !currentWorkItem.IsCanceled)
					{
						current.CurrentWorkItem.GetWorkItemResult().Cancel(true);
					}
				}
			}
		}
		public override bool WaitForIdle(int millisecondsTimeout)
		{
			this.ValidateWaitForIdle();
			return STPEventWaitHandle.WaitOne(this._isIdleWaitHandle, millisecondsTimeout, false);
		}
		internal override void PreQueueWorkItem()
		{
			this.ValidateNotDisposed();
		}
		public void Join(IEnumerable<Action> actions)
		{
			WIGStartInfo wigStartInfo = new WIGStartInfo
			{
				StartSuspended = true
			};
			IWorkItemsGroup workItemsGroup = this.CreateWorkItemsGroup(2147483647, wigStartInfo);
			foreach (Action current in actions)
			{
				workItemsGroup.QueueWorkItem(current);
			}
			workItemsGroup.Start();
			workItemsGroup.WaitForIdle();
		}
		public void Join(params Action[] actions)
		{
			this.Join((IEnumerable<Action>)actions);
		}
		public int Choice(IEnumerable<Action> actions)
		{
			WIGStartInfo wigStartInfo = new WIGStartInfo
			{
				StartSuspended = true
			};
			IWorkItemsGroup workItemsGroup = this.CreateWorkItemsGroup(2147483647, wigStartInfo);
			ManualResetEvent anActionCompleted = new ManualResetEvent(false);
			SmartThreadPool.ChoiceIndex choiceIndex = new SmartThreadPool.ChoiceIndex();
			int num = 0;
			foreach (Action current in actions)
			{
				Action act = current;
				int value = num;
				workItemsGroup.QueueWorkItem(delegate
				{
					act();
					Interlocked.CompareExchange(ref choiceIndex._index, value, -1);
					anActionCompleted.Set();
				});
				num++;
			}
			workItemsGroup.Start();
			anActionCompleted.WaitOne();
			return choiceIndex._index;
		}
		public int Choice(params Action[] actions)
		{
			return this.Choice((IEnumerable<Action>)actions);
		}
		public void Pipe<T>(T pipeState, IEnumerable<Action<T>> actions)
		{
			WIGStartInfo wigStartInfo = new WIGStartInfo
			{
				StartSuspended = true
			};
			IWorkItemsGroup workItemsGroup = this.CreateWorkItemsGroup(2147483647, wigStartInfo);
			foreach (Action<T> current in actions)
			{
				Action<T> act = current;
				workItemsGroup.QueueWorkItem(delegate
				{
					act(pipeState);
				});
			}
			workItemsGroup.Start();
			workItemsGroup.WaitForIdle();
		}
		public void Pipe<T>(T pipeState, params Action<T>[] actions)
		{
			this.Pipe<T>(pipeState, (IEnumerable<Action<T>>)actions);
		}
	}
}
