using System;
namespace XUtils
{
	public class RetryTrigger
	{
		internal int MaxRun;
		internal TimeSpan Wait = TimeSpan.FromMilliseconds(500.0);
		public RetryTrigger MaxRuns(int number)
		{
			if (number <= 0)
			{
				number = 1;
			}
			this.MaxRun = number;
			return this;
		}
		public RetryTrigger Waits(TimeSpan timeSpan)
		{
			if (timeSpan == TimeSpan.MinValue || timeSpan == TimeSpan.MaxValue)
			{
				timeSpan = TimeSpan.FromMilliseconds(500.0);
			}
			this.Wait = timeSpan;
			return this;
		}
	}
}
