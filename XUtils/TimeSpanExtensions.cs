using System;
namespace XUtils
{
	public static class TimeSpanExtensions
	{
		public static bool IsMidnightExactly(this TimeSpan t)
		{
			return t.Hours == 0 && t.Minutes == 0 && t.Seconds == 0;
		}
		public static string ToMilitaryString(this TimeSpan t)
		{
			string text = string.Concat(new object[]
			{
				t.Hours,
				":",
				t.Minutes,
				":",
				t.Seconds
			});
			if (t.Days > 0)
			{
				text = t.Days + "dys " + text;
			}
			return text;
		}
		public static int ToMilitaryInt(this TimeSpan t)
		{
			int num = t.Hours * 100;
			return num + t.Minutes;
		}
	}
}
