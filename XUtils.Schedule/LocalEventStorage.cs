using System;
namespace XUtils.Schedule
{
	public class LocalEventStorage : IEventStorage
	{
		private DateTime _LastTime;
		public LocalEventStorage()
		{
			this._LastTime = DateTime.MaxValue;
		}
		public void RecordLastTime(DateTime Time)
		{
			this._LastTime = Time;
		}
		public DateTime ReadLastTime()
		{
			if (this._LastTime == DateTime.MaxValue)
			{
				this._LastTime = DateTime.Now;
			}
			return this._LastTime;
		}
	}
}
