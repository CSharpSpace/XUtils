using System;
namespace XUtils
{
	public class DateParseResult
	{
		public readonly bool IsValid;
		public readonly string Error;
		public readonly DateTime Start;
		public readonly DateTime End;
		public DateParseResult(bool valid, string error, DateTime start, DateTime end)
		{
			this.IsValid = valid;
			this.Error = error;
			this.Start = start;
			this.End = end;
		}
	}
}
