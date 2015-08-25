using System;
namespace XUtils.Threading.Base.Internal
{
	internal class Stopwatch
	{
		private long _elapsed;
		private bool _isRunning;
		private long _startTimeStamp;
		public TimeSpan Elapsed
		{
			get
			{
				return new TimeSpan(this.GetElapsedDateTimeTicks());
			}
		}
		public long ElapsedMilliseconds
		{
			get
			{
				return this.GetElapsedDateTimeTicks() / 10000L;
			}
		}
		public long ElapsedTicks
		{
			get
			{
				return this.GetRawElapsedTicks();
			}
		}
		public bool IsRunning
		{
			get
			{
				return this._isRunning;
			}
		}
		public Stopwatch()
		{
			this.Reset();
		}
		private long GetElapsedDateTimeTicks()
		{
			return this.GetRawElapsedTicks();
		}
		private long GetRawElapsedTicks()
		{
			long num = this._elapsed;
			if (this._isRunning)
			{
				long num2 = Stopwatch.GetTimestamp() - this._startTimeStamp;
				num += num2;
			}
			return num;
		}
		public static long GetTimestamp()
		{
			return DateTime.UtcNow.Ticks;
		}
		public void Reset()
		{
			this._elapsed = 0L;
			this._isRunning = false;
			this._startTimeStamp = 0L;
		}
		public void Start()
		{
			if (!this._isRunning)
			{
				this._startTimeStamp = Stopwatch.GetTimestamp();
				this._isRunning = true;
			}
		}
		public static Stopwatch StartNew()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}
		public void Stop()
		{
			if (this._isRunning)
			{
				long num = Stopwatch.GetTimestamp() - this._startTimeStamp;
				this._elapsed += num;
				this._isRunning = false;
			}
		}
	}
}
