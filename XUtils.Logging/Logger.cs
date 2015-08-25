using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Logging
{
	public class Logger
	{
		private static Dictionary<string, ILogMulti> _loggers = new Dictionary<string, ILogMulti>
		{

			{
				"default",
				new LogMulti("default", new LogConsole("console", true))
			}
		};
		private static ReaderWriterLock _readwriteLock = new ReaderWriterLock();
		private static int _lockMilliSecondsForRead = 1000;
		private static int _lockMilliSecondsForWrite = 1000;
		public static ILogMulti Default
		{
			get
			{
				return Logger.Get("default");
			}
		}
		public static int Count
		{
			get
			{
				int count = 0;
				Logger.ExecuteRead(delegate
				{
					count = Logger._loggers.Count;
				});
				return count;
			}
		}
		private Logger()
		{
		}
		public static void Log(LogLevel level, string message)
		{
			Logger.Log(level, message, null, null);
		}
		public static void Log(LogLevel level, string message, params object[] args)
		{
			Logger.Log(level, message, null, args);
		}
		public static void Log(LogLevel level, string message, Exception exception)
		{
			Logger.Log(level, message, exception, null);
		}
		public static void Log(LogLevel level, string message, Exception exception, params object[] args)
		{
			LogEvent logEvent = LogHelper.BuildLogEvent(typeof(Logger), level, message, exception, args);
			Logger.Default.Log(logEvent);
		}
		public static void Warn(string message)
		{
			Logger.Log(LogLevel.Warn, message, null, null);
		}
		public static void Warn(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Warn, message, null, arguments);
		}
		public static void Warn(string message, Exception exception)
		{
			Logger.Log(LogLevel.Warn, message, exception, null);
		}
		public static void Warn(string message, Exception exception, params object[] arguments)
		{
			Logger.Log(LogLevel.Warn, message, exception, arguments);
		}
		public static void Error(string message)
		{
			Logger.Log(LogLevel.Error, message, null, null);
		}
		public static void Error(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Error, message, null, arguments);
		}
		public static void Error(string message, Exception exception)
		{
			Logger.Log(LogLevel.Error, message, exception, null);
		}
		public static void Error(string message, Exception exception, params object[] arguments)
		{
			Logger.Log(LogLevel.Error, message, exception, arguments);
		}
		public static void Debug(string message)
		{
			Logger.Log(LogLevel.Debug, message, null, null);
		}
		public static void Debug(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Debug, message, null, arguments);
		}
		public static void Debug(string message, Exception exception)
		{
			Logger.Log(LogLevel.Debug, message, exception, null);
		}
		public static void Debug(string message, Exception exception, params object[] arguments)
		{
			Logger.Log(LogLevel.Debug, message, exception, arguments);
		}
		public static void Fatal(string message)
		{
			Logger.Log(LogLevel.Fatal, message, null, null);
		}
		public static void Fatal(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Fatal, message, null, arguments);
		}
		public static void Fatal(string message, Exception exception)
		{
			Logger.Log(LogLevel.Fatal, message, exception, null);
		}
		public static void Fatal(string message, Exception exception, object[] arguments)
		{
			Logger.Log(LogLevel.Fatal, message, exception, arguments);
		}
		public static void Info(string message)
		{
			Logger.Log(LogLevel.Info, message, null, null);
		}
		public static void Info(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Info, message, null, arguments);
		}
		public static void Info(string message, Exception exception)
		{
			Logger.Log(LogLevel.Info, message, exception, null);
		}
		public static void Info(string message, Exception exception, params object[] arguments)
		{
			Logger.Log(LogLevel.Info, message, exception, arguments);
		}
		public static void Message(string message)
		{
			Logger.Log(LogLevel.Message, message, null, null);
		}
		public static void Message(string message, params object[] arguments)
		{
			Logger.Log(LogLevel.Message, message, null, arguments);
		}
		public static void Message(string message, Exception exception)
		{
			Logger.Log(LogLevel.Message, message, exception, null);
		}
		public static void Message(string message, Exception exception, params object[] arguments)
		{
			Logger.Log(LogLevel.Message, message, exception, arguments);
		}
		public static void Add(ILogMulti logger)
		{
			Logger.ExecuteWrite(delegate
			{
				Logger._loggers[logger.Name] = logger;
			});
		}
		public static void Clear()
		{
			Logger.ExecuteWrite(delegate
			{
				Logger._loggers.Clear();
				Logger._loggers["default"] = new LogMulti("default", new LogConsole());
			});
		}
		public static ILogMulti Get(string name)
		{
			ILogMulti logger = null;
			Logger.ExecuteRead(delegate
			{
				if (!Logger._loggers.ContainsKey(name))
				{
					return;
				}
				logger = Logger._loggers[name];
			});
			return logger;
		}
		public static ILogMulti Get(int index)
		{
			ILogMulti logger = null;
			if (index < 0)
			{
				return logger;
			}
			Logger.ExecuteRead(delegate
			{
				if (index >= Logger._loggers.Count)
				{
					return;
				}
				logger = Logger._loggers[index.ToString()];
			});
			return logger;
		}
		public static void Init(ILogMulti logger)
		{
			Logger.ExecuteWrite(delegate
			{
				if (Logger._loggers == null)
				{
					Logger._loggers = new Dictionary<string, ILogMulti>();
				}
				Logger._loggers["default"] = new LogMulti("default", new List<ILog>
				{
					logger
				});
			});
		}
		public static void Flush()
		{
			Logger.ExecuteRead(delegate
			{
				foreach (KeyValuePair<string, ILogMulti> current in Logger._loggers)
				{
					current.Value.Flush();
				}
			});
		}
		public static void ShutDown()
		{
			Logger.Flush();
			Logger.ExecuteRead(delegate
			{
				foreach (KeyValuePair<string, ILogMulti> current in Logger._loggers)
				{
					current.Value.ShutDown();
				}
			});
		}
		protected static void ExecuteRead(Action executor)
		{
			Logger.AcquireReaderLock();
			try
			{
				executor();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to execute write action in Logger." + ex.Message);
			}
			finally
			{
				Logger.ReleaseReaderLock();
			}
		}
		protected static void ExecuteWrite(Action executor)
		{
			Logger.AcquireWriterLock();
			try
			{
				executor();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to execute write action in Logger." + ex.Message);
			}
			finally
			{
				Logger.ReleaseWriterLock();
			}
		}
		protected static void AcquireReaderLock()
		{
			Logger._readwriteLock.AcquireReaderLock(Logger._lockMilliSecondsForRead);
		}
		protected static void ReleaseReaderLock()
		{
			Logger._readwriteLock.ReleaseReaderLock();
		}
		protected static void AcquireWriterLock()
		{
			Logger._readwriteLock.AcquireWriterLock(Logger._lockMilliSecondsForWrite);
		}
		protected static void ReleaseWriterLock()
		{
			Logger._readwriteLock.ReleaseWriterLock();
		}
	}
}
