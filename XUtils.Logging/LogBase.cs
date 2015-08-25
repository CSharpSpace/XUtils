using System;
using System.Threading;
namespace XUtils.Logging
{
	public abstract class LogBase : ILog
	{
		protected ILog _parent;
		protected ReaderWriterLock _readwriteLock = new ReaderWriterLock();
		protected int _lockMilliSecondsForRead = 1000;
		protected int _lockMilliSecondsForWrite = 1000;
		public virtual string Name
		{
			get;
			set;
		}
		public ILog Parent
		{
			get;
			set;
		}
		public LogSettings Settings
		{
			get;
			set;
		}
		public virtual LogLevel Level
		{
			get
			{
				return this.Settings.Level;
			}
			set
			{
				this.Settings.Level = value;
			}
		}
		public virtual bool IsDebugEnabled
		{
			get
			{
				return this.IsEnabled(LogLevel.Debug);
			}
		}
		public virtual bool IsInfoEnabled
		{
			get
			{
				return this.IsEnabled(LogLevel.Info);
			}
		}
		public virtual bool IsWarnEnabled
		{
			get
			{
				return this.IsEnabled(LogLevel.Warn);
			}
		}
		public virtual bool IsErrorEnabled
		{
			get
			{
				return this.IsEnabled(LogLevel.Error);
			}
		}
		public virtual bool IsFatalEnabled
		{
			get
			{
				return this.IsEnabled(LogLevel.Fatal);
			}
		}
		public virtual ILog this[string loggerName]
		{
			get
			{
				return this;
			}
		}
		public virtual ILog this[int logIndex]
		{
			get
			{
				return this;
			}
		}
		public LogBase()
		{
		}
		public LogBase(Type type)
		{
			this.Name = type.FullName;
			this.Settings = new LogSettings();
			this.Settings.Level = LogLevel.Info;
		}
		public LogBase(string name)
		{
			this.Name = name;
			this.Settings = new LogSettings();
			this.Settings.Level = LogLevel.Info;
		}
		public virtual bool IsEnabled(LogLevel level)
		{
			return level >= this.Settings.Level;
		}
		public abstract void Log(LogEvent logEvent);
		public virtual void Flush()
		{
		}
		public virtual void Warn(string format)
		{
			if (this.IsEnabled(LogLevel.Warn))
			{
				this.InternalLog(LogLevel.Warn, format, null, null);
			}
		}
		public virtual void Warn(string format, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Warn))
			{
				this.InternalLog(LogLevel.Warn, format, null, args);
			}
		}
		public virtual void Warn(string format, Exception exception)
		{
			if (this.IsEnabled(LogLevel.Warn))
			{
				this.InternalLog(LogLevel.Warn, format, exception, null);
			}
		}
		public virtual void Warn(string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Warn))
			{
				this.InternalLog(LogLevel.Warn, format, ex, args);
			}
		}
		public virtual void Error(string format)
		{
			if (this.IsEnabled(LogLevel.Error))
			{
				this.InternalLog(LogLevel.Error, format, null, null);
			}
		}
		public virtual void Error(string format, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Error))
			{
				this.InternalLog(LogLevel.Error, format, null, args);
			}
		}
		public virtual void Error(string format, Exception exception)
		{
			if (this.IsEnabled(LogLevel.Error))
			{
				this.InternalLog(LogLevel.Error, format, exception, null);
			}
		}
		public virtual void Error(string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Error))
			{
				this.InternalLog(LogLevel.Error, format, ex, args);
			}
		}
		public virtual void Debug(string format)
		{
			if (this.IsEnabled(LogLevel.Debug))
			{
				this.InternalLog(LogLevel.Debug, format, null, null);
			}
		}
		public virtual void Debug(string format, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Debug))
			{
				this.InternalLog(LogLevel.Debug, format, null, args);
			}
		}
		public virtual void Debug(string format, Exception exception)
		{
			if (this.IsEnabled(LogLevel.Debug))
			{
				this.InternalLog(LogLevel.Debug, format, exception, null);
			}
		}
		public virtual void Debug(string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Debug))
			{
				this.InternalLog(LogLevel.Debug, format, ex, args);
			}
		}
		public virtual void Fatal(string format)
		{
			if (this.IsEnabled(LogLevel.Fatal))
			{
				this.InternalLog(LogLevel.Fatal, format, null, null);
			}
		}
		public virtual void Fatal(string format, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Fatal))
			{
				this.InternalLog(LogLevel.Fatal, format, null, args);
			}
		}
		public virtual void Fatal(string format, Exception exception)
		{
			if (this.IsEnabled(LogLevel.Fatal))
			{
				this.InternalLog(LogLevel.Fatal, format, exception, null);
			}
		}
		public virtual void Fatal(string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Fatal))
			{
				this.InternalLog(LogLevel.Fatal, format, ex, args);
			}
		}
		public virtual void Info(string format)
		{
			if (this.IsEnabled(LogLevel.Info))
			{
				this.InternalLog(LogLevel.Info, format, null, null);
			}
		}
		public virtual void Info(string format, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Info))
			{
				this.InternalLog(LogLevel.Info, format, null, args);
			}
		}
		public virtual void Info(string format, Exception exception)
		{
			if (this.IsEnabled(LogLevel.Info))
			{
				this.InternalLog(LogLevel.Info, format, exception, null);
			}
		}
		public virtual void Info(string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(LogLevel.Info))
			{
				this.InternalLog(LogLevel.Info, format, ex, args);
			}
		}
		public virtual void Message(string format)
		{
			this.InternalLog(LogLevel.Message, format, null, null);
		}
		public virtual void Message(string format, params object[] args)
		{
			this.InternalLog(LogLevel.Message, format, null, args);
		}
		public virtual void Message(string format, Exception exception)
		{
			this.InternalLog(LogLevel.Message, format, exception, null);
		}
		public virtual void Message(string format, Exception ex, params object[] args)
		{
			this.InternalLog(LogLevel.Message, format, ex, args);
		}
		public virtual void Log(LogLevel level, string format)
		{
			if (this.IsEnabled(level))
			{
				this.InternalLog(level, format, null, null);
			}
		}
		public virtual void Log(LogLevel level, string format, params object[] args)
		{
			if (this.IsEnabled(level))
			{
				this.InternalLog(level, format, null, args);
			}
		}
		public virtual void Log(LogLevel level, string format, Exception exception)
		{
			if (this.IsEnabled(level))
			{
				this.InternalLog(level, format, exception, null);
			}
		}
		public virtual void Log(LogLevel level, string format, Exception ex, params object[] args)
		{
			if (this.IsEnabled(level))
			{
				this.InternalLog(level, format, ex, args);
			}
		}
		public virtual void InternalLog(LogLevel level, string format, Exception ex, params object[] args)
		{
			LogEvent logEvent = this.BuildLogEvent(level, format, ex, args);
			this.Log(logEvent);
		}
		public virtual void ShutDown()
		{
			Console.WriteLine("Shutting down logger " + this.Name);
		}
		public virtual LogEvent BuildLogEvent(LogLevel level, string format, Exception ex, params object[] args)
		{
			return LogHelper.BuildLogEvent(base.GetType(), level, format, ex, args);
		}
		protected virtual string BuildMessage(LogEvent logEvent)
		{
			return LogFormatter.Format("", logEvent);
		}
		protected void ExecuteRead(Action executor)
		{
			this.AcquireReaderLock();
			try
			{
				executor();
			}
			catch (Exception)
			{
			}
			finally
			{
				this.ReleaseReaderLock();
			}
		}
		protected void ExecuteWrite(Action executor)
		{
			this.AcquireWriterLock();
			try
			{
				executor();
			}
			catch (Exception)
			{
			}
			finally
			{
				this.ReleaseWriterLock();
			}
		}
		protected void AcquireReaderLock()
		{
			this._readwriteLock.AcquireReaderLock(this._lockMilliSecondsForRead);
		}
		protected void ReleaseReaderLock()
		{
			this._readwriteLock.ReleaseReaderLock();
		}
		protected void AcquireWriterLock()
		{
			this._readwriteLock.AcquireWriterLock(this._lockMilliSecondsForWrite);
		}
		protected void ReleaseWriterLock()
		{
			this._readwriteLock.ReleaseWriterLock();
		}
	}
}
