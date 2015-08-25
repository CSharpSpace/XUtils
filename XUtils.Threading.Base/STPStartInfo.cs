using System;
using System.Threading;
namespace XUtils.Threading.Base
{
	public class STPStartInfo : WIGStartInfo
	{
		private int _idleTimeout = 60000;
		private int _minWorkerThreads;
		private int _maxWorkerThreads = 25;
		private ThreadPriority _threadPriority = ThreadPriority.Normal;
		private string _performanceCounterInstanceName = SmartThreadPool.DefaultPerformanceCounterInstanceName;
		private bool _enableLocalPerformanceCounters;
		public virtual int IdleTimeout
		{
			get
			{
				return this._idleTimeout;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._idleTimeout = value;
			}
		}
		public virtual int MinWorkerThreads
		{
			get
			{
				return this._minWorkerThreads;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._minWorkerThreads = value;
			}
		}
		public virtual int MaxWorkerThreads
		{
			get
			{
				return this._maxWorkerThreads;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._maxWorkerThreads = value;
			}
		}
		public virtual ThreadPriority ThreadPriority
		{
			get
			{
				return this._threadPriority;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._threadPriority = value;
			}
		}
		public virtual string PerformanceCounterInstanceName
		{
			get
			{
				return this._performanceCounterInstanceName;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._performanceCounterInstanceName = value;
			}
		}
		public virtual bool EnableLocalPerformanceCounters
		{
			get
			{
				return this._enableLocalPerformanceCounters;
			}
			set
			{
				base.ThrowIfReadOnly();
				this._enableLocalPerformanceCounters = value;
			}
		}
		public STPStartInfo()
		{
			this._performanceCounterInstanceName = SmartThreadPool.DefaultPerformanceCounterInstanceName;
			this._threadPriority = ThreadPriority.Normal;
			this._maxWorkerThreads = 25;
			this._idleTimeout = 60000;
			this._minWorkerThreads = 0;
		}
		public STPStartInfo(STPStartInfo stpStartInfo) : base(stpStartInfo)
		{
			this._idleTimeout = stpStartInfo.IdleTimeout;
			this._minWorkerThreads = stpStartInfo.MinWorkerThreads;
			this._maxWorkerThreads = stpStartInfo.MaxWorkerThreads;
			this._threadPriority = stpStartInfo.ThreadPriority;
			this._performanceCounterInstanceName = stpStartInfo.PerformanceCounterInstanceName;
			this._enableLocalPerformanceCounters = stpStartInfo._enableLocalPerformanceCounters;
		}
		public new STPStartInfo AsReadOnly()
		{
			return new STPStartInfo(this)
			{
				_readOnly = true
			};
		}
	}
}
