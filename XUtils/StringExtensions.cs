using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace XUtils
{
	public static class StringExtensions
	{
		public static string Times(this string str, int times)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (times <= 1)
			{
				return str;
			}
			string text = string.Empty;
			for (int i = 0; i < times; i++)
			{
				text += str;
			}
			return text;
		}
		public static string IncreaseTo(this string str, int maxLength, bool truncate)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			if (str.Length == maxLength)
			{
				return str;
			}
			if (str.Length > maxLength && truncate)
			{
				return str.Truncate(maxLength);
			}
			string text = str;
			while (str.Length < maxLength)
			{
				if (str.Length + text.Length < maxLength)
				{
					str += text;
				}
				else
				{
					str += str.Substring(0, maxLength - str.Length);
				}
			}
			return str;
		}
		public static string IncreaseRandomly(this string str, int minLength, int maxLength, bool truncate)
		{
			Random random = new Random(minLength);
			int maxLength2 = random.Next(minLength, maxLength);
			return str.IncreaseTo(maxLength2, truncate);
		}
		public static byte[] ToBytesAscii(this string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return new byte[0];
			}
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			return aSCIIEncoding.GetBytes(txt);
		}
		public static object ToBoolObject(this string txt)
		{
			return txt.ToBool();
		}
		public static bool ToBool(this string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return false;
			}
			string a = txt.Trim().ToLower();
			return a == "yes" || a == "true" || a == "1";
		}
		public static object ToIntObject(this string txt)
		{
			return txt.ToInt();
		}
		public static int ToInt(this string txt)
		{
			return StringExtensions.ToNumber<int>(txt, (string s) => Convert.ToInt32(Convert.ToDouble(s)), 0);
		}
		public static object ToLongObject(this string txt)
		{
			return txt.ToLong();
		}
		public static double ToLong(this string txt)
		{
			return (double)StringExtensions.ToNumber<long>(txt, (string s) => Convert.ToInt64(s), 0L);
		}
		public static object ToDoubleObject(this string txt)
		{
			return txt.ToDouble();
		}
		public static double ToDouble(this string txt)
		{
			return StringExtensions.ToNumber<double>(txt, (string s) => Convert.ToDouble(s), 0.0);
		}
		public static object ToFloatObject(this string txt)
		{
			return txt.ToFloat();
		}
		public static double ToFloat(this string txt)
		{
			return (double)StringExtensions.ToNumber<float>(txt, (string s) => Convert.ToSingle(s), 0f);
		}
		public static T ToNumber<T>(string txt, Func<string, T> callback, T defaultValue)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return defaultValue;
			}
			string text = txt.Trim().ToLower();
			if (text.StartsWith("$") || text.StartsWith("￥"))
			{
				text = text.Substring(1);
			}
			return callback(text);
		}
		public static object ToTimeObject(this string txt)
		{
			return txt.ToTime();
		}
		public static TimeSpan ToTime(this string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return TimeSpan.MinValue;
			}
			string strTime = txt.Trim().ToLower();
			return TimeHelper.Parse(strTime).Item;
		}
		public static object ToDateTimeObject(this string txt)
		{
			return txt.ToDateTime();
		}
		public static DateTime ToDateTime(this string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return DateTime.MinValue;
			}
			string text = txt.Trim().ToLower();
			if (!text.StartsWith("$"))
			{
				return DateTime.Parse(text);
			}
			if (text == "${today}")
			{
				return DateTime.Today;
			}
			if (text == "${yesterday}")
			{
				return DateTime.Today.AddDays(-1.0);
			}
			if (text == "${tommorrow}")
			{
				return DateTime.Today.AddDays(1.0);
			}
			if (text == "${t}")
			{
				return DateTime.Today;
			}
			if (text == "${t-1}")
			{
				return DateTime.Today.AddDays(-1.0);
			}
			if (text == "${t+1}")
			{
				return DateTime.Today.AddDays(1.0);
			}
			if (text == "${today+1}")
			{
				return DateTime.Today.AddDays(1.0);
			}
			if (text == "${today-1}")
			{
				return DateTime.Today.AddDays(-1.0);
			}
			string dateStr = text.Substring(2, text.Length - 1 - 2);
			return DateParser.ParseTPlusMinusX(dateStr);
		}
		public static List<string> PreFixWith(this List<string> items, string prefix)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i] = prefix + items[i];
			}
			return items;
		}
		public static bool IsNotApplicableValue(this string val, bool useNullOrEmptyStringAsNotApplicable = false)
		{
			bool flag = string.IsNullOrEmpty(val);
			if (flag && useNullOrEmptyStringAsNotApplicable)
			{
				return true;
			}
			if (flag && !useNullOrEmptyStringAsNotApplicable)
			{
				return false;
			}
			val = val.Trim().ToLower();
			return val == "na" || val == "n.a." || val == "n/a" || val == "n\\a" || val == "n.a" || val == "not applicable";
		}
		public static int Levenshtein(this string source, string comparison)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source", "Can't parse null string");
			}
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison", "Can't parse null string");
			}
			char[] array = source.ToCharArray();
			char[] array2 = comparison.ToCharArray();
			int length = source.Length;
			int length2 = comparison.Length;
			int[,] array3 = new int[length + 1, length2 + 1];
			if (length == 0)
			{
				return length2;
			}
			if (length2 == 0)
			{
				return length;
			}
			int i = 0;
			while (i <= length)
			{
				array3[i, 0] = i++;
			}
			int j = 0;
			while (j <= length2)
			{
				array3[0, j] = j++;
			}
			for (int k = 1; k <= length; k++)
			{
				for (int l = 1; l <= length2; l++)
				{
					int num = array2[l - 1].Equals(array[k - 1]) ? 0 : 1;
					array3[k, l] = Math.Min(Math.Min(array3[k - 1, l] + 1, array3[k, l - 1] + 1), array3[k - 1, l - 1] + num);
				}
			}
			return array3[length, length2];
		}
		public static string SimplifiedSoundex(this string source)
		{
			return source.SimplifiedSoundex(4);
		}
		public static string SimplifiedSoundex(this string source, int length)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length < 3)
			{
				throw new ArgumentException("Source string must be at least two characters", "source");
			}
			char[] array = source.ToUpper().ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			short num = -1;
			char[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				short num2;
				switch (array2[i])
				{
				case 'A':
				case 'E':
				case 'H':
				case 'I':
				case 'O':
				case 'U':
				case 'W':
				case 'Y':
					num2 = 0;
					break;
				case 'B':
				case 'F':
				case 'P':
				case 'V':
					num2 = 1;
					break;
				case 'C':
				case 'G':
				case 'J':
				case 'K':
				case 'Q':
				case 'S':
				case 'X':
				case 'Z':
					num2 = 2;
					break;
				case 'D':
				case 'T':
					num2 = 3;
					break;
				case 'L':
					num2 = 4;
					break;
				case 'M':
				case 'N':
					num2 = 5;
					break;
				case 'R':
					num2 = 6;
					break;
				default:
					throw new ApplicationException("Invalid state in switch statement");
				}
				if (num2 != num)
				{
					stringBuilder.Append(num2);
				}
				num = num2;
			}
			stringBuilder.Remove(0, 1).Insert(0, array.First<char>());
			stringBuilder.Replace("0", "");
			while (stringBuilder.Length < length)
			{
				stringBuilder.Append('0');
			}
			return stringBuilder.ToString().Substring(0, length);
		}
	}
}
