using System;
namespace XUtils
{
	public class TimeParserConstants
	{
		public const string Am = "am";
		public const string AmWithPeriods = "a.m.";
		public const string Pm = "pm";
		public const string PmWithPeriods = "p.m.";
		public const string ErrorEndTimeLessThanStart = "End time must be greater than or equal to start time.";
		public const string ErrorStartEndTimeSepartorNotPresent = "Start and end time separator not present, use '-' or 'to'";
		public const string ErrorStartTimeNotProvided = "Start time not provided";
		public const string ErrorEndTimeNotProvided = "End time not provided";
		public static readonly DateTime MinDate = new DateTime(2000, 1, 1);
		public static readonly DateTime MaxDate = new DateTime(2050, 1, 1);
	}
}
