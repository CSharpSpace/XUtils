using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils
{
	public static class DictionaryExtensions
	{
		public static T Get<T>(this IDictionary d, string key)
		{
			object obj = d[key];
			if (obj == null)
			{
				return default(T);
			}
			return TypeParsers.ConvertTo<T>(obj);
		}
		public static T GetOrDefault<T>(this IDictionary d, string key, T defaultValue)
		{
			if (!d.Contains(key))
			{
				return defaultValue;
			}
            return d.Get<T>(key); ;
		}
		public static object Get(this IDictionary d, string sectionName, string key)
		{
			if (!d.Contains(sectionName))
			{
				return null;
			}
			IDictionary dictionary = d[sectionName] as IDictionary;
			if (!dictionary.Contains(key))
			{
				return null;
			}
			return dictionary[key];
		}
		public static T Get<T>(this IDictionary d, string section, string key)
		{
			object obj = d.Get(section, key);
			if (obj == null)
			{
				return default(T);
			}
			return TypeParsers.ConvertTo<T>(obj);
		}
		public static T GetOrDefault<T>(this IDictionary d, string section, string key, T defaultValue)
		{
			if (string.IsNullOrEmpty(section))
			{
				return defaultValue;
			}
			if (!d.Contains(section, key))
			{
				return defaultValue;
			}
			return d.Get<T>(section, key);
		}
		public static IDictionary Section(this IDictionary d, string section)
		{
			if (d == null || d.Count == 0)
			{
				return null;
			}
			if (d.Contains(section))
			{
				return d[section] as IDictionary;
			}
			return null;
		}
		public static bool Contains(this IDictionary d, string sectionName, string key)
		{
			IDictionary dictionary = d.Section(sectionName);
			return dictionary != null && dictionary.Contains(key);
		}
		public static T Get<T>(this IDictionary<string, object> d, string key)
		{
			object obj = d[key];
			if (obj == null)
			{
				return default(T);
			}
			return TypeParsers.ConvertTo<T>(obj);
		}
		public static T GetOrDefault<T>(this IDictionary<string, object> d, string key, T defaultValue)
		{
			if (!d.ContainsKey(key))
			{
				return defaultValue;
			}
            return d.Get<T>(key);
		}
		public static object Get(this IDictionary<string, object> d, string sectionName, string key)
		{
			if (!d.ContainsKey(sectionName))
			{
				return null;
			}
			IDictionary dictionary = d[sectionName] as IDictionary;
			if (!dictionary.Contains(key))
			{
				return null;
			}
			return dictionary[key];
		}
		public static T Get<T>(this IDictionary<string, object> d, string section, string key)
		{
			object obj = d.Get(section, key);
			if (obj == null)
			{
				return default(T);
			}
			return TypeParsers.ConvertTo<T>(obj);
		}
		public static T Get<T>(this IDictionary<string, object> d, string section, string key, T defaultValue)
		{
			if (string.IsNullOrEmpty(section))
			{
				return defaultValue;
			}
			if (!d.Contains(section, key))
			{
				return defaultValue;
			}
			return d.Get<T>(section, key);
		}
		public static IDictionary<string, object> GetSection(this IDictionary<string, object> d, string section)
		{
			if (d.ContainsKey(section))
			{
				return d[section] as IDictionary<string, object>;
			}
			return null;
		}
		public static bool Contains(this IDictionary<string, object> d, string sectionName, string key)
		{
			IDictionary<string, object> section = d.GetSection(sectionName);
			return section != null && section.ContainsKey(key);
		}
	}
}
