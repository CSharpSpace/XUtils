using System;
namespace XUtils.Logging
{
	public class LogEvent
	{
		public LogLevel Level;
		public string Message;
		public string FinalMessage;
		public Exception Error;
		public string Computer;
		public DateTime CreateTime;
		public string ThreadName;
		public Exception Ex;
		public Type LogType;
		public LogEvent()
		{
		}
		public LogEvent(LogLevel level, string message, Exception ex)
		{
			this.Level = level;
			this.Message = message;
			this.Ex = ex;
		}
	}
}
