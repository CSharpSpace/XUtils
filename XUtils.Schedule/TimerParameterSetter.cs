using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public class TimerParameterSetter : IParameterSetter
	{
		private DateTime _time;
		private object _sender;
		public TimerParameterSetter(DateTime time, object sender)
		{
			this._time = time;
			this._sender = sender;
		}
		public void reset()
		{
		}
		public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
		{
			string a;
			if ((a = pi.ParameterType.Name.ToLower()) != null)
			{
				if (a == "datetime")
				{
					parameter = this._time;
					return true;
				}
				if (a == "object")
				{
					parameter = this._sender;
					return true;
				}
				if (a == "scheduledeventargs")
				{
					parameter = new ScheduledEventArgs(this._time);
					return true;
				}
				if (a == "eventargs")
				{
					parameter = new ScheduledEventArgs(this._time);
					return true;
				}
			}
			return false;
		}
	}
}
