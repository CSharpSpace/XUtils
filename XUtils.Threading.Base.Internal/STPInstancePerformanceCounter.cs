using System;
using System.Diagnostics;
namespace XUtils.Threading.Base.Internal
{
	internal class STPInstancePerformanceCounter : IDisposable
	{
		private bool _isDisposed;
		private PerformanceCounter _pcs;
		protected STPInstancePerformanceCounter()
		{
			this._isDisposed = false;
		}
		public STPInstancePerformanceCounter(string instance, STPPerformanceCounterType spcType) : this()
		{
			STPPerformanceCounters instance2 = STPPerformanceCounters.Instance;
			this._pcs = new PerformanceCounter("ThreadPool", instance2._stpPerformanceCounters[(int)spcType].Name, instance, false);
			this._pcs.RawValue = this._pcs.RawValue;
		}
		public void Close()
		{
			if (this._pcs != null)
			{
				this._pcs.RemoveInstance();
				this._pcs.Close();
				this._pcs = null;
			}
		}
		public void Dispose()
		{
			this.Dispose(true);
		}
		public virtual void Dispose(bool disposing)
		{
			if (!this._isDisposed && disposing)
			{
				this.Close();
			}
			this._isDisposed = true;
		}
		public virtual void Increment()
		{
			this._pcs.Increment();
		}
		public virtual void IncrementBy(long val)
		{
			this._pcs.IncrementBy(val);
		}
		public virtual void Set(long val)
		{
			this._pcs.RawValue = val;
		}
	}
}
