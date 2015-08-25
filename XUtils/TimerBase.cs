using System;
using System.Timers;
namespace XUtils
{
	public abstract class TimerBase : IDisposable
	{
		public Timer timer;
		private bool _alreadyDisposed;
		public TimerBase(int Interval)
		{
			this.timer = new Timer((double)Interval);
			this.timer.AutoReset = false;
			this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
		}
		public abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing && this.timer != null)
			{
				this.timer.Elapsed -= new ElapsedEventHandler(this.timer_Elapsed);
				this.timer.Stop();
				this.timer.Close();
				this.timer.Dispose();
			}
			this._alreadyDisposed = true;
		}
	}
}
