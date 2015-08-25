using System;
using System.Text;
namespace XUtils.Logging
{
	public class LogFormatter
	{
		public static string Format(string formatter, LogEvent logEvent)
		{
			if (string.IsNullOrEmpty(formatter))
			{
				return LogFormatter.Format(logEvent);
			}
			return LogFormatter.Format(logEvent);
		}
		public static string Format(LogEvent logEvent)
		{
			string text = (logEvent.Message == null) ? string.Empty : logEvent.Message.ToString();
			string str = text.ToString();
			string str2 = logEvent.CreateTime.ToString();
			if (!string.IsNullOrEmpty(logEvent.ThreadName))
			{
				str2 = str2 + ":" + logEvent.ThreadName;
			}
			str2 = str2 + " : " + logEvent.Level.ToString();
			return str2 + " : " + str;
		}
		public static string ConvertToString(object[] args)
		{
			if (args == null || args.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj != null)
				{
					stringBuilder.AppendFormat("{0},", obj.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
