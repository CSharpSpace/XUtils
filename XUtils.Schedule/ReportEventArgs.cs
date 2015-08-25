using System;
namespace XUtils.Schedule
{
	public class ReportEventArgs : EventArgs
	{
		public int ReportNo;
		public DateTime EventTime;
		public ReportEventArgs(DateTime Time, int reportNo)
		{
			this.EventTime = Time;
			this.ReportNo = reportNo;
		}
	}
}
