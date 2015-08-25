using System;
namespace XUtils.Schedule
{
	public class NullEventStorage : IEventStorage
	{
		public void RecordLastTime(DateTime Time)
		{
		}
		public DateTime ReadLastTime()
		{
			return DateTime.Now;
		}
	}
}
