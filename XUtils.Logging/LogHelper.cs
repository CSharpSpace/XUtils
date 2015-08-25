using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
namespace XUtils.Logging
{
	public class LogHelper
	{
		public void LogToConsole<T>(LogLevel level, string message, Exception ex, params object[] args)
		{
			LogEvent logEvent = LogHelper.BuildLogEvent(typeof(T), level, message, ex, null);
			Console.WriteLine(logEvent.FinalMessage);
		}
		public static LogEvent BuildLogEvent(Type logType, LogLevel level, string message, Exception ex, params object[] args)
		{
			LogEvent logEvent = new LogEvent();
			logEvent.Level = level;
			logEvent.Message = ((args == null) ? message : string.Format(message, args));
			logEvent.Error = ex;
			logEvent.Computer = Environment.MachineName;
			logEvent.CreateTime = DateTime.Now;
			logEvent.ThreadName = Thread.CurrentThread.Name;
			logEvent.LogType = logType;
			logEvent.FinalMessage = LogFormatter.Format(null, logEvent);
			return logEvent;
		}
		public static LogLevel GetLogLevel(string loglevel)
		{
			return (LogLevel)Enum.Parse(typeof(LogLevel), loglevel, true);
		}
		public static string BuildLogFileName(string logFileName, string appName, DateTime date)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["%datetime%"] = date.ToString("yyyy-MM-dd-HH-mm-ss");
			dictionary["%date%"] = date.ToString("yyyy-MM-dd");
			dictionary["%yyyy%"] = date.ToString("yyyy");
			dictionary["%MM%"] = date.ToString("MM");
			dictionary["%dd%"] = date.ToString("dd");
			dictionary["%MMM%"] = date.ToString("MMM");
			dictionary["%hh%"] = date.ToString("hh");
			dictionary["%HH%"] = date.ToString("HH");
			dictionary["%mm%"] = date.ToString("mm");
			dictionary["%ss%"] = date.ToString("ss");
			dictionary["%name%"] = appName;
			dictionary["%user%"] = Dns.GetHostName();
			foreach (KeyValuePair<string, string> current in dictionary)
			{
				logFileName = logFileName.Replace(current.Key, current.Value);
			}
			if (!logFileName.Contains(".log") && !logFileName.Contains(".txt"))
			{
				logFileName += ".log";
			}
			logFileName = logFileName.Replace("%", "_");
			logFileName = logFileName.Replace("--", "-");
			logFileName = logFileName.Replace("__", "_");
			if (logFileName.StartsWith("-"))
			{
				logFileName = "Log" + logFileName;
			}
			if (logFileName.StartsWith("_"))
			{
				logFileName = "Log" + logFileName;
			}
			return logFileName;
		}
	}
}
