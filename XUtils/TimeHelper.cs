using System;
using System.Text.RegularExpressions;
using XUtils.Messages;
namespace XUtils
{
	public class TimeHelper
	{
		public static TimeParseResult ParseStartEndTimes(string startAndEndTimeRange)
		{
			string text = "-";
			int num = startAndEndTimeRange.IndexOf(text);
			if (num < 0)
			{
				text = "to";
				num = startAndEndTimeRange.IndexOf(text);
				if (num < 0)
				{
					BoolMessageItem<TimeSpan> boolMessageItem = TimeHelper.Parse(startAndEndTimeRange);
					TimeSpan start = TimeSpan.MinValue;
					if (!boolMessageItem.Success)
					{
						return new TimeParseResult(false, boolMessageItem.Message, start, TimeSpan.MaxValue);
					}
					start = boolMessageItem.Item;
					return new TimeParseResult(true, string.Empty, start, TimeSpan.MaxValue);
				}
			}
			string starts = startAndEndTimeRange.Substring(0, num);
			string ends = startAndEndTimeRange.Substring(num + text.Length);
			return TimeHelper.ParseStartEndTimes(starts, ends, true);
		}
		public static TimeParseResult ParseStartEndTimes(string starts, string ends, bool checkEndTime)
		{
			if (string.IsNullOrEmpty(starts))
			{
				return new TimeParseResult(false, "Start time not provided", TimeSpan.MinValue, TimeSpan.MaxValue);
			}
			if (checkEndTime && string.IsNullOrEmpty(ends))
			{
				return new TimeParseResult(false, "End time not provided", TimeSpan.MinValue, TimeSpan.MaxValue);
			}
			BoolMessageItem<TimeSpan> boolMessageItem = TimeHelper.Parse(starts);
			TimeSpan timeSpan = TimeSpan.MinValue;
			TimeSpan timeSpan2 = TimeSpan.MaxValue;
			if (!boolMessageItem.Success)
			{
				return new TimeParseResult(false, boolMessageItem.Message, TimeSpan.MinValue, TimeSpan.MaxValue);
			}
			timeSpan = boolMessageItem.Item;
			if (checkEndTime)
			{
				BoolMessageItem<TimeSpan> boolMessageItem2 = TimeHelper.Parse(ends);
				if (!boolMessageItem2.Success)
				{
					return new TimeParseResult(false, boolMessageItem2.Message, TimeSpan.MinValue, TimeSpan.MaxValue);
				}
				timeSpan2 = boolMessageItem2.Item;
				if (timeSpan2 < timeSpan)
				{
					return new TimeParseResult(false, "End time must be greater than or equal to start time.", timeSpan, timeSpan2);
				}
			}
			return new TimeParseResult(true, string.Empty, timeSpan, timeSpan2);
		}
		public static BoolMessageItem<TimeSpan> Parse(string strTime)
		{
			strTime = strTime.Trim().ToLower();
			string pattern = "(?<hours>[0-9]+)((\\:)(?<minutes>[0-9]+))?\\s*(?<ampm>(am|a\\.m\\.|a\\.m|pm|p\\.m\\.|p\\.m))?\\s*";
			Match match = Regex.Match(strTime, pattern);
			if (!match.Success)
			{
				return new BoolMessageItem<TimeSpan>(TimeSpan.MinValue, false, "Time : " + strTime + " is not a valid time.");
			}
			string text = (match.Groups["hours"] != null) ? match.Groups["hours"].Value : string.Empty;
			string text2 = (match.Groups["minutes"] != null) ? match.Groups["minutes"].Value : string.Empty;
			string text3 = (match.Groups["ampm"] != null) ? match.Groups["ampm"].Value : string.Empty;
			int num = 0;
			int minutes = 0;
			if (!string.IsNullOrEmpty(text) && !int.TryParse(text, out num))
			{
				return new BoolMessageItem<TimeSpan>(TimeSpan.MinValue, false, "Hours are invalid.");
			}
			if (!string.IsNullOrEmpty(text2) && !int.TryParse(text2, out minutes))
			{
				return new BoolMessageItem<TimeSpan>(TimeSpan.MinValue, false, "Minutes are invalid.");
			}
			bool flag;
			if (string.IsNullOrEmpty(text3) || text3 == "am" || text3 == "a.m" || text3 == "a.m.")
			{
				flag = true;
			}
			else
			{
				if (!(text3 == "pm") && !(text3 == "p.m") && !(text3 == "p.m."))
				{
					return new BoolMessageItem<TimeSpan>(TimeSpan.MinValue, false, "unknown am/pm statement");
				}
				flag = false;
			}
			if (num != 12 && !flag)
			{
				num += 12;
			}
			if (num == 12 && flag)
			{
				return new BoolMessageItem<TimeSpan>(new TimeSpan(0, minutes, 0), true, string.Empty);
			}
			return new BoolMessageItem<TimeSpan>(new TimeSpan(num, minutes, 0), true, string.Empty);
		}
		public static TimeSpan ConvertFromMilitaryTime(int military)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			int hours = military / 100;
			int minutes = military % 100;
			minValue = new TimeSpan(hours, minutes, 0);
			return minValue;
		}
		public static int ConvertToMilitary(TimeSpan timeSpan)
		{
			return timeSpan.Hours * 100 + timeSpan.Minutes;
		}
		public static string Format(int militaryTime, bool convertSingleDigit)
		{
			if (convertSingleDigit && militaryTime < 10)
			{
				militaryTime *= 100;
			}
			TimeSpan time = TimeHelper.ConvertFromMilitaryTime(militaryTime);
			return TimeHelper.Format(time);
		}
		public static string Format(TimeSpan time)
		{
			int num = time.Hours;
			string text = (num < 12) ? "am" : "pm";
			if (num > 12)
			{
				num -= 12;
			}
			if (time.Minutes == 0)
			{
				return num + text;
			}
			if (time.Minutes > 10)
			{
				return string.Concat(new object[]
				{
					num,
					":",
					time.Minutes,
					text
				});
			}
			return string.Concat(new object[]
			{
				num,
				":0",
				time.Minutes,
				text
			});
		}
		public static StartTimeOfDay GetTimeOfDay(TimeSpan time)
		{
			if (time.Hours < 12)
			{
				return StartTimeOfDay.Morning;
			}
			if (time.Hours >= 12 && time.Hours <= 16)
			{
				return StartTimeOfDay.Afternoon;
			}
			return StartTimeOfDay.Evening;
		}
		public static StartTimeOfDay GetTimeOfDay(int militaryTime)
		{
			return TimeHelper.GetTimeOfDay(TimeHelper.ConvertFromMilitaryTime(militaryTime));
		}
	}
}
