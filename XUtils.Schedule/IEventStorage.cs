using System;
namespace XUtils.Schedule
{
	public interface IEventStorage
	{
		void RecordLastTime(DateTime Time);
		DateTime ReadLastTime();
	}
}
