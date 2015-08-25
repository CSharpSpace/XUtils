using System;
namespace XUtils
{
	public class LamdaLogger
	{
		private static Action<object, Exception, object[]> _criticalLogger;
		private static Action<object, Exception, object[]> _errorLogger;
		private static Action<object, Exception, object[]> _infoLogger;
		private static Action<object, Exception, object[]> _debugLogger;
		public LamdaLogger()
		{
			LamdaLogger._criticalLogger = delegate(object message, Exception ex, object[] args)
			{
				Console.WriteLine(LamdaLogger.BuildMessage("critical", message, ex, args));
			};
			LamdaLogger._errorLogger = delegate(object message, Exception ex, object[] args)
			{
				Console.WriteLine(LamdaLogger.BuildMessage("error", message, ex, args));
			};
			LamdaLogger._infoLogger = delegate(object message, Exception ex, object[] args)
			{
				Console.WriteLine(LamdaLogger.BuildMessage("info", message, ex, args));
			};
			LamdaLogger._debugLogger = delegate(object message, Exception ex, object[] args)
			{
				Console.WriteLine(LamdaLogger.BuildMessage("debug", message, ex, args));
			};
		}
		public void Init(Action<object, Exception, object[]> criticalLogger, Action<object, Exception, object[]> errorLogger, Action<object, Exception, object[]> infoLogger, Action<object, Exception, object[]> debugLogger)
		{
			if (criticalLogger != null)
			{
				LamdaLogger._criticalLogger = criticalLogger;
			}
			if (errorLogger != null)
			{
				LamdaLogger._errorLogger = errorLogger;
			}
			if (infoLogger != null)
			{
				LamdaLogger._infoLogger = infoLogger;
			}
			if (debugLogger != null)
			{
				LamdaLogger._debugLogger = debugLogger;
			}
		}
		public void Critical(object message, Exception ex = null, object[] args = null)
		{
			if (LamdaLogger._criticalLogger != null)
			{
				LamdaLogger._criticalLogger(message, ex, args);
			}
		}
		public void Error(object message, Exception ex = null, object[] args = null)
		{
			if (LamdaLogger._errorLogger != null)
			{
				LamdaLogger._errorLogger(message, ex, args);
			}
		}
		public void Info(object message, Exception ex = null, object[] args = null)
		{
			if (LamdaLogger._infoLogger != null)
			{
				LamdaLogger._infoLogger(message, ex, args);
			}
		}
		public void Debug(object message, Exception ex = null, object[] args = null)
		{
			if (LamdaLogger._debugLogger != null)
			{
				LamdaLogger._debugLogger(message, ex, args);
			}
		}
		private static string BuildMessage(string level, object message, Exception ex, object[] args)
		{
			return string.Concat(new object[]
			{
				level.ToUpper(),
				" : ",
				message,
				Environment.NewLine,
				ex.Message,
				Environment.NewLine,
				ex.StackTrace,
				Environment.NewLine
			});
		}
	}
}
