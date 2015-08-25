using System;
using System.Collections.Generic;
namespace XUtils.Schedule
{
	public class Task
	{
		private delegate void ExecuteHandler(object sender, DateTime EventTime, ExceptionEventHandler Error);
		private Task.ExecuteHandler _ExecuteHandler;
		public IScheduledItem Schedule;
		public bool SyncronizedEvent = true;
		public IResultFilter Filter;
		public IMethodCall Method;
		public bool Enabled = true;
		public Task(IScheduledItem schedule, IMethodCall method)
		{
			this.Schedule = schedule;
			this.Method = method;
			this._ExecuteHandler = new Task.ExecuteHandler(this.ExecuteInternal);
		}
		public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
		{
			if (!this.Enabled)
			{
				return DateTime.MaxValue;
			}
			return this.Schedule.NextRunTime(time, IncludeStartTime);
		}
		public void Execute(object sender, DateTime Begin, DateTime End, ExceptionEventHandler Error)
		{
			if (!this.Enabled)
			{
				return;
			}
			List<DateTime> list = new List<DateTime>();
			this.Schedule.AddEventsInInterval(Begin, End, list);
			if (this.Filter != null)
			{
				this.Filter.FilterResultsInInterval(Begin, End, list);
			}
			foreach (DateTime current in list)
			{
				if (this.SyncronizedEvent)
				{
					this._ExecuteHandler(sender, current, Error);
				}
				else
				{
					this._ExecuteHandler.BeginInvoke(sender, current, Error, null, null);
				}
			}
		}
		private void ExecuteInternal(object sender, DateTime EventTime, ExceptionEventHandler Error)
		{
			try
			{
				TimerParameterSetter @params = new TimerParameterSetter(EventTime, sender);
				this.Method.Execute(@params);
			}
			catch (Exception e)
			{
				if (Error != null)
				{
					try
					{
						Error(this, new ExceptionEventArgs(EventTime, e));
					}
					catch
					{
					}
				}
			}
		}
	}
}
