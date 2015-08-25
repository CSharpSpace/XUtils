using System;
using System.Collections.Generic;
using System.Timers;
namespace XUtils.Schedule
{
	public class ScheduleTimerBase : IDisposable
	{
		public IEventStorage EventStorage = new LocalEventStorage();
		private static TimeSpan MAX_INTERVAL = new TimeSpan(0, 0, 1);
		private DateTime _LastTime;
		private Timer _Timer;
		private TaskContainer container;
		private volatile bool _StopFlag;
		public event ExceptionEventHandler Error;
		public int TasksCount
		{
			get
			{
				return this.container.Tasks.Length;
			}
		}
		public ScheduleTimerBase()
		{
			this._Timer = new Timer();
			this._Timer.AutoReset = false;
			this._Timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
			this.container = new TaskContainer();
			this._LastTime = DateTime.MaxValue;
		}
		public void AddTask(string key, IScheduledItem Schedule, Delegate f, params object[] Params)
		{
			this.container.Add(key, new Task(Schedule, new DelegateMethodCall(f, Params)));
		}
		public void AddAsyncTask(string key, IScheduledItem Schedule, Delegate f, params object[] Params)
		{
			Task task = new Task(Schedule, new DelegateMethodCall(f, Params));
			task.SyncronizedEvent = false;
			this.container.Add(key, task);
		}
		public void AddTask(string key, Task task)
		{
			this.container.Add(key, task);
		}
		public void Remove(string key)
		{
			this.container.Remove(key);
		}
		public void Clear()
		{
			this.container.Clear();
		}
		public void Start()
		{
			this._StopFlag = false;
			this.QueueNextTime(this.EventStorage.ReadLastTime());
		}
		public void Stop()
		{
			this._StopFlag = true;
			this._Timer.Stop();
		}
		private double NextInterval(DateTime thisTime)
		{
			TimeSpan t = this.container.NextRunTime(thisTime) - thisTime;
			if (t > ScheduleTimerBase.MAX_INTERVAL)
			{
				t = ScheduleTimerBase.MAX_INTERVAL;
			}
			if (t.TotalMilliseconds != 0.0)
			{
				return t.TotalMilliseconds;
			}
			return 1.0;
		}
		private void QueueNextTime(DateTime thisTime)
		{
			this._Timer.Interval = this.NextInterval(thisTime);
			this._LastTime = thisTime;
			this.EventStorage.RecordLastTime(thisTime);
			this._Timer.Start();
		}
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				if (this.container != null)
				{
					this._Timer.Stop();
					KeyValuePair<string, Task>[] tasks = this.container.Tasks;
					for (int i = 0; i < tasks.Length; i++)
					{
						KeyValuePair<string, Task> keyValuePair = tasks[i];
						try
						{
							keyValuePair.Value.Execute(this, this._LastTime, e.SignalTime, this.Error);
						}
						catch (Exception e2)
						{
							this.OnError(DateTime.Now, keyValuePair.Value, e2);
						}
					}
				}
			}
			catch (Exception e3)
			{
				this.OnError(DateTime.Now, null, e3);
			}
			finally
			{
				if (!this._StopFlag)
				{
					this.QueueNextTime(e.SignalTime);
				}
			}
		}
		private void OnError(DateTime eventTime, Task task, Exception e)
		{
			if (this.Error == null)
			{
				return;
			}
			try
			{
				this.Error(this, new ExceptionEventArgs(eventTime, e));
			}
			catch (Exception)
			{
			}
		}
		public void Dispose()
		{
			if (this._Timer != null)
			{
				this._Timer.Dispose();
			}
		}
	}
}
