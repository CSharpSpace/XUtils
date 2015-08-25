using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace XUtils
{
	public static class EnumerableExtensions
	{
		public static void For(long from, long to, Action<long> body)
		{
			while (from <= to)
			{
				body(from);
				from += 1L;
			}
		}
		public static void For(int from, int to, Action<int> body)
		{
			while (from <= to)
			{
				body(from);
				from++;
			}
		}
		public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
		{
			int num = 0;
			foreach (T current in items)
			{
				action(current, num++);
			}
		}
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (T current in items)
			{
				action(current);
			}
		}
		public static string ForEach<T>(this IEnumerable<T> items, Func<T, string> func)
		{
			string val = string.Empty;
			items.ForEach(delegate(T item)
			{
				val += func(item);
			});
			return val;
		}
		public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector, IEqualityComparer<V> comparer = null)
		{
			if (comparer == null)
			{
				return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
			}
			return source.Distinct(new CommonEqualityComparer<T, V>(keySelector, comparer));
		}
		public static string Join<T>(this IList<T> items, string delimeter)
		{
			if (items == null || items.Count == 0)
			{
				return string.Empty;
			}
			if (items.Count == 1)
			{
				T t = items[0];
				return t.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder arg_4D_0 = stringBuilder;
			T t2 = items[0];
			arg_4D_0.Append(t2.ToString());
			for (int i = 1; i < items.Count; i++)
			{
				T t3 = items[i];
				string str = t3.ToString();
				stringBuilder.Append(delimeter + str);
			}
			return stringBuilder.ToString();
		}
		public static string JoinDelimited<T>(this IList<T> items, string delimeter, Func<T, string> appender)
		{
			if (items == null || items.Count == 0)
			{
				return string.Empty;
			}
			if (items.Count == 1)
			{
				return appender(items[0]);
			}
			StringBuilder stringBuilder = new StringBuilder();
			string arg_56_0;
			if (appender != null)
			{
				arg_56_0 = appender(items[0]);
			}
			else
			{
				T t = items[0];
				arg_56_0 = t.ToString();
			}
			string text = arg_56_0;
			stringBuilder.Append(text);
			for (int i = 1; i < items.Count; i++)
			{
				T arg = items[i];
				text = ((appender == null) ? arg.ToString() : appender(arg));
				stringBuilder.Append(delimeter + text);
			}
			return stringBuilder.ToString();
		}
		public static string JoinDelimitedWithNewLine<T>(this IList<T> items, string delimeter, int newLineAfterCount, string newLineText, Func<T, string> appender)
		{
			if (items == null || items.Count == 0)
			{
				return string.Empty;
			}
			if (items.Count == 1)
			{
				return appender(items[0]);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(appender(items[0]));
			for (int i = 1; i < items.Count; i++)
			{
				T arg = items[i];
				string str = appender(arg);
				if (i % newLineAfterCount == 0)
				{
					stringBuilder.Append(newLineText);
				}
				stringBuilder.Append(delimeter + str);
			}
			return stringBuilder.ToString();
		}
		public static string AsDelimited<T>(this IEnumerable<T> items, string delimiter)
		{
			List<string> list = new List<string>();
			foreach (T current in items)
			{
				list.Add(current.ToString());
			}
			return string.Join(delimiter, list.ToArray());
		}
		public static bool IsEmpty<T>(this IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			bool result = !items.GetEnumerator().MoveNext();
			try
			{
				items.GetEnumerator().Reset();
			}
			catch (NotSupportedException)
			{
			}
			return result;
		}
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
		{
			return items == null || items.IsEmpty<T>();
		}
		public static bool HasAnyNulls<T>(this IEnumerable<T> items)
		{
			return items.IsTrueForAny((T t) => t == null);
		}
		public static bool IsTrueForAny<T>(this IEnumerable<T> items, Func<T, bool> executor)
		{
			foreach (T current in items)
			{
				bool flag = executor(current);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
		public static bool IsTrueForAll<T>(this IEnumerable<T> items, Func<T, bool> executor)
		{
			foreach (T current in items)
			{
				if (!executor(current))
				{
					return false;
				}
			}
			return true;
		}
		public static IDictionary<T, T> ToDictionary<T>(this IList<T> items)
		{
			IDictionary<T, T> dictionary = new Dictionary<T, T>();
			foreach (T current in items)
			{
				dictionary[current] = current;
			}
			return dictionary;
		}
	}
}
