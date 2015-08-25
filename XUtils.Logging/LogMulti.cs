using System;
using System.Collections.Generic;
namespace XUtils.Logging
{
	public class LogMulti : LogBase, ILogMulti, ILog
	{
		private Dictionary<string, ILog> _loggers;
		private LogLevel _lowestLevel = LogLevel.Debug;
		public int Count
		{
			get
			{
				int count = 0;
				base.ExecuteRead(delegate
				{
					count = this._loggers.Count;
				});
				return count;
			}
		}
		public override ILog this[string loggerName]
		{
			get
			{
				ILog logger = null;
				base.ExecuteRead(delegate
				{
					if (!this._loggers.ContainsKey(loggerName))
					{
						return;
					}
					logger = this._loggers[loggerName];
				});
				return logger;
			}
		}
		public override ILog this[int logIndex]
		{
			get
			{
				ILog logger = null;
				if (logIndex < 0)
				{
					return null;
				}
				base.ExecuteRead(delegate
				{
					if (logIndex >= this._loggers.Count)
					{
						return;
					}
					logger = this._loggers[logIndex.ToString()];
				});
				return logger;
			}
		}
		public override LogLevel Level
		{
			get
			{
				return this._lowestLevel;
			}
			set
			{
				base.ExecuteWrite(delegate
				{
					foreach (KeyValuePair<string, ILog> current in this._loggers)
					{
						current.Value.Level = value;
					}
					this._lowestLevel = value;
				});
			}
		}
		public LogMulti(string name, ILog logger) : base(typeof(LogMulti).FullName)
		{
			this.Init(name, new List<ILog>
			{
				logger
			});
		}
		public LogMulti(string name, IList<ILog> loggers) : base(typeof(LogMulti).FullName)
		{
			this.Init(name, loggers);
		}
		public void Init(string name, IList<ILog> loggers)
		{
			this.Name = name;
			this._loggers = new Dictionary<string, ILog>();
			foreach (ILog current in loggers)
			{
				this._loggers.Add(current.Name, current);
			}
			this.ActivateOptions();
		}
		public override void Log(LogEvent logEvent)
		{
			base.ExecuteRead(delegate
			{
				foreach (KeyValuePair<string, ILog> current in this._loggers)
				{
					current.Value.Log(logEvent);
				}
			});
		}
		public void Append(ILog logger)
		{
			base.ExecuteWrite(delegate
			{
				this._loggers.Add(logger.Name, logger);
			});
		}
		public bool ContainsKey(string key)
		{
			bool relust = false;
			base.ExecuteRead(delegate
			{
				relust = this._loggers.ContainsKey(key);
			});
			return relust;
		}
		public void Replace(ILog logger)
		{
			this.Clear();
			this.Append(logger);
		}
		public void Clear()
		{
			base.ExecuteWrite(delegate
			{
				this._loggers.Clear();
				this._lowestLevel = LogLevel.Message;
				this._loggers.Add("console", new LogConsole());
			});
		}
		public override bool IsEnabled(LogLevel level)
		{
			return this._lowestLevel <= level;
		}
		public override void Flush()
		{
			base.ExecuteRead(delegate
			{
				foreach (KeyValuePair<string, ILog> current in this._loggers)
				{
					current.Value.Flush();
				}
			});
		}
		public override void ShutDown()
		{
			base.ExecuteRead(delegate
			{
				foreach (KeyValuePair<string, ILog> current in this._loggers)
				{
					current.Value.ShutDown();
				}
			});
		}
		public void ActivateOptions()
		{
			base.ExecuteRead(delegate
			{
				LogLevel logLevel = LogLevel.Fatal;
				for (int i = 0; i < this._loggers.Count; i++)
				{
					ILog log = this._loggers[i.ToString()];
					if (log.Level <= logLevel)
					{
						logLevel = log.Level;
					}
				}
				this._lowestLevel = logLevel;
			});
		}
	}
}
