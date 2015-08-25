using System;
using System.IO;
using System.Text;
using System.Threading;
namespace XUtils.Logging
{
	public class LogFile : LogBase, ILog, IDisposable
	{
		private string directory;
		private object _lockerFlush = new object();
		private string filerule;
		private string appname;
		public LogFile() : this("file", string.Empty)
		{
		}
		public LogFile(string __directory) : this("file", __directory)
		{
		}
		public LogFile(string name, string __directory) : base(name)
		{
			this.appname = "file";
			this.filerule = "%date%.log";
			this.directory = __directory;
		}
		public override void Log(LogEvent logEvent)
		{
			object lockerFlush;
			Monitor.Enter(lockerFlush = this._lockerFlush);
			try
			{
				string path = AppDomain.CurrentDomain.BaseDirectory + "log\\";
				if (!string.IsNullOrEmpty(this.directory))
				{
					path = this.directory;
				}
				string text = logEvent.Level.ToString() + "\\" + this.filerule;
				text = LogHelper.BuildLogFileName(text, this.appname, DateTime.Now);
				FileInfo fileInfo = new FileInfo(Path.Combine(path, text));
				if (!Directory.Exists(fileInfo.DirectoryName))
				{
					Directory.CreateDirectory(fileInfo.DirectoryName);
				}
				string contents = this.BuilderContent(logEvent);
				File.AppendAllText(fileInfo.FullName, contents);
			}
			finally
			{
				Monitor.Exit(lockerFlush);
			}
		}
		private string BuilderContent(LogEvent logEvent)
		{
			string value = logEvent.FinalMessage;
			if (string.IsNullOrEmpty(value))
			{
				value = this.BuildMessage(logEvent);
			}
			StringBuilder stringBuilder = new StringBuilder();
			LogLevel level = logEvent.Level;
			if (level == LogLevel.Error)
			{
				stringBuilder.AppendLine("Level        : " + logEvent.Level);
				stringBuilder.AppendLine("CreateTime   : " + logEvent.CreateTime);
				stringBuilder.AppendLine("ThreadName   : " + logEvent.ThreadName);
				stringBuilder.AppendLine("Message      : " + logEvent.Message);
				stringBuilder.AppendLine("Error        : " + logEvent.Error);
				if (logEvent.Error != null && logEvent.Error.TargetSite != null)
				{
					stringBuilder.AppendLine("Method       : " + logEvent.Error.TargetSite);
				}
				stringBuilder.AppendLine("-------------------------------------------------------------------");
			}
			else
			{
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}
		public void Dispose()
		{
		}
	}
}
