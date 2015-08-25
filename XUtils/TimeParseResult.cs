using System;
namespace XUtils
{
	public class TimeParseResult
	{
		public readonly bool IsValid;
		public readonly string Error;
		public readonly TimeSpan Start;
		public readonly TimeSpan End;
		public DateTime StartTimeAsDate
		{
			get
			{
				if (this.Start == TimeSpan.MinValue)
				{
					return TimeParserConstants.MinDate;
				}
				return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this.Start.Hours, this.Start.Minutes, this.Start.Seconds);
			}
		}
		public DateTime EndTimeAsDate
		{
			get
			{
				if (this.End == TimeSpan.MaxValue)
				{
					return TimeParserConstants.MaxDate;
				}
				return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this.End.Hours, this.End.Minutes, this.End.Seconds);
			}
		}
		public TimeParseResult(bool valid, string error, TimeSpan start, TimeSpan end)
		{
			this.IsValid = valid;
			this.Error = error;
			this.Start = start;
			this.End = end;
		}
	}
}
