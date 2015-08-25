using System;
namespace XUtils
{
	public static class IntegerExtensions
	{
		public static void Times(this int ndx, Action<int> action)
		{
			for (int i = 0; i < ndx; i++)
			{
				action(i);
			}
		}
		public static void Upto(this int start, int end, Action<int> action)
		{
			for (int i = start; i < end; i++)
			{
				action(i);
			}
		}
		public static void UptoIncluding(this int start, int end, Action<int> action)
		{
			for (int i = start; i <= end; i++)
			{
				action(i);
			}
		}
		public static void Downto(this int end, int start, Action<int> action)
		{
			for (int i = end; i > start; i--)
			{
				action(i);
			}
		}
		public static void DowntoIncluding(this int end, int start, Action<int> action)
		{
			for (int i = end; i >= start; i--)
			{
				action(i);
			}
		}
		public static bool IsOdd(this int num)
		{
			return num % 2 != 0;
		}
		public static bool IsEven(this int num)
		{
			return num % 2 == 0;
		}
		public static int MegaBytes(this int numberInBytes)
		{
			return numberInBytes / 1000000;
		}
		public static int KiloBytes(this int numberInBytes)
		{
			return numberInBytes / 1000;
		}
		public static int TeraBytes(this int numberInBytes)
		{
			return numberInBytes / 1000000000;
		}
		public static bool IsLeapYear(this int year)
		{
			return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
		}
		public static DateTime DaysAgo(this int num)
		{
			return DateTime.Now.AddDays((double)(-(double)num));
		}
		public static DateTime MonthsAgo(this int num)
		{
			return DateTime.Now.AddMonths(-num);
		}
		public static DateTime YearsAgo(this int num)
		{
			return DateTime.Now.AddYears(-num);
		}
		public static DateTime HoursAgo(this int num)
		{
			return DateTime.Now.AddHours((double)(-(double)num));
		}
		public static DateTime MinutesAgo(this int num)
		{
			return DateTime.Now.AddMinutes((double)(-(double)num));
		}
		public static DateTime DaysFromNow(this int num)
		{
			return DateTime.Now.AddDays((double)num);
		}
		public static DateTime MonthsFromNow(this int num)
		{
			return DateTime.Now.AddMonths(num);
		}
		public static DateTime YearsFromNow(this int num)
		{
			return DateTime.Now.AddYears(num);
		}
		public static DateTime HoursFromNow(this int num)
		{
			return DateTime.Now.AddHours((double)num);
		}
		public static DateTime MinutesFromNow(this int num)
		{
			return DateTime.Now.AddMinutes((double)num);
		}
		public static TimeSpan Days(this int num)
		{
			return new TimeSpan(num, 0, 0, 0);
		}
		public static TimeSpan Hours(this int num)
		{
			return new TimeSpan(0, num, 0, 0);
		}
		public static TimeSpan Minutes(this int num)
		{
			return new TimeSpan(0, 0, num, 0);
		}
		public static TimeSpan Seconds(this int num)
		{
			return new TimeSpan(0, 0, 0, num);
		}
		public static TimeSpan Time(this int num)
		{
			return num.Time(false);
		}
		public static TimeSpan Time(this int num, bool convertSingleDigitsToHours)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			if (convertSingleDigitsToHours && num <= 24)
			{
				num *= 100;
			}
			int hours = num / 100;
			int minutes = num % 100;
			minValue = new TimeSpan(hours, minutes, 0);
			return minValue;
		}
		public static string TimeWithSuffix(this int num)
		{
			TimeSpan time = num.Time(true);
			return TimeHelper.Format(time);
		}
	}
}
