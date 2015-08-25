using System;
namespace XUtils.Schedule
{
	public class ReportTimer : ScheduleTimerBase
	{
		private delegate void ConvertHandler(ReportEventHandler Handler, int ReportNo, object sender, DateTime time);
		private static ReportTimer.ConvertHandler Handler = new ReportTimer.ConvertHandler(ReportTimer.Converter);
		public event ReportEventHandler Elapsed;
		public void AddReportEvent(string key, IScheduledItem Schedule, int reportNo)
		{
			if (this.Elapsed == null)
			{
				throw new Exception("You must set elapsed before adding Events");
			}
			base.AddTask(key, new Task(Schedule, new DelegateMethodCall(ReportTimer.Handler, new object[]
			{
				this.Elapsed,
				reportNo
			})));
		}
		public void AddAsyncReportEvent(string key, IScheduledItem Schedule, int reportNo)
		{
			if (this.Elapsed == null)
			{
				throw new Exception("You must set elapsed before adding Events");
			}
			base.AddTask(key, new Task(Schedule, new DelegateMethodCall(ReportTimer.Handler, new object[]
			{
				this.Elapsed,
				reportNo
			}))
			{
				SyncronizedEvent = false
			});
		}
		private static void Converter(ReportEventHandler Handler, int ReportNo, object sender, DateTime time)
		{
			if (Handler == null)
			{
				throw new ArgumentNullException("Handler");
			}
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			Handler(sender, new ReportEventArgs(time, ReportNo));
		}
	}
}
