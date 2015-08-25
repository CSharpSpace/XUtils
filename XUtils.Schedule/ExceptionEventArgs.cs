using System;
namespace XUtils.Schedule
{
	public class ExceptionEventArgs : EventArgs
	{
		public DateTime EventTime;
		public Exception Error;
		public ExceptionEventArgs(DateTime eventTime, Exception e)
		{
			this.EventTime = eventTime;
			this.Error = e;
		}
	}
}
