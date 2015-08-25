using System;
namespace XUtils.Logging
{
	public class LogConsole : LogBase, ILog
	{
		private static object _colorSync = new object();
		private bool useColorCoding;
		public LogConsole() : base(typeof(LogConsole).FullName)
		{
		}
		public LogConsole(string name) : this(name, false)
		{
		}
		public LogConsole(string name, bool useColorCoding) : base(name)
		{
			this.useColorCoding = useColorCoding;
		}
		public override void Log(LogEvent logEvent)
		{
			switch (logEvent.Level)
			{
			case LogLevel.Message:
				Console.ForegroundColor = ConsoleColor.White;
				break;
			case LogLevel.Debug:
				Console.ForegroundColor = ConsoleColor.Gray;
				break;
			case LogLevel.Info:
				Console.ForegroundColor = ConsoleColor.White;
				break;
			case LogLevel.Warn:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			case LogLevel.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				break;
			case LogLevel.Fatal:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			}
			if (!string.IsNullOrEmpty(logEvent.FinalMessage))
			{
				Console.WriteLine(logEvent.FinalMessage);
			}
			else
			{
				string value = this.BuildMessage(logEvent);
				Console.WriteLine(value);
			}
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
