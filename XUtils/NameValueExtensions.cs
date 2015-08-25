using System;
using System.Collections.Specialized;
namespace XUtils
{
	public class NameValueExtensions
	{
		public static string GetOrDefault(NameValueCollection collection, string key, string defaultValue)
		{
			if (collection == null)
			{
				return defaultValue;
			}
			string text = collection[key];
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			return text;
		}
		public static T GetOrDefault<T>(NameValueCollection collection, string key, T defaultValue)
		{
			if (collection == null)
			{
				return defaultValue;
			}
			string text = collection[key];
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			return TypeParsers.ConvertTo<T>(text);
		}
	}
}
