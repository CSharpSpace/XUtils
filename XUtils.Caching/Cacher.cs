using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Caching
{
	public class Cacher
	{
		private static ICache _provider;
		public static ICache Provider
		{
			get
			{
				return Cacher._provider;
			}
		}
		public static int Count
		{
			get
			{
				return Cacher._provider.Count;
			}
		}
		public static ICollection Keys
		{
			get
			{
				return Cacher._provider.Keys;
			}
		}
		static Cacher()
		{
			Cacher._provider = new CacheAspNet();
		}
		public static void Init(ICache cacheProvider)
		{
			Cacher._provider = cacheProvider;
		}
		public static bool Contains(string key)
		{
			return Cacher._provider.Contains(key);
		}
		public static object Get(object key)
		{
			return Cacher._provider.Get(key);
		}
		public static T Get<T>(object key)
		{
			return Cacher._provider.Get<T>(key);
		}
		public static T Get<T>(string key, int timeInSeconds, Func<T> fetcher)
		{
			return Cacher.Get<T>(key, true, timeInSeconds, false, fetcher);
		}
		public static T Get<T>(string key, TimeSpan timeInSeconds, Func<T> fetcher)
		{
			return Cacher.Get<T>(key, true, timeInSeconds.Seconds, false, fetcher);
		}
		public static T Get<T>(string key, bool useCache, TimeSpan timeInSeconds, Func<T> fetcher)
		{
			return Cacher.Get<T>(key, useCache, timeInSeconds.Seconds, false, fetcher);
		}
		public static T Get<T>(string key, bool useCache, int timeInSeconds, bool slidingExpiration, Func<T> fetcher)
		{
			if (!useCache)
			{
				return fetcher();
			}
			return Cacher._provider.GetOrInsert<T>(key, timeInSeconds, slidingExpiration, fetcher);
		}
		public static void Remove(object key)
		{
			Cacher._provider.Remove(key);
		}
		public static void RemoveAll(ICollection keys)
		{
			Cacher._provider.RemoveAll(keys);
		}
		public static void Clear()
		{
			Cacher._provider.Clear();
		}
		public static void Insert(object key, object value)
		{
			Cacher._provider.Insert(key, value);
		}
		public static void Insert(object key, object value, int timeToLive, bool slidingExpiration)
		{
			Cacher._provider.Insert(key, value, timeToLive, slidingExpiration);
		}
		public static void Insert(object key, object value, int timeToLive, bool slidingExpiration, CacheItemPriority priority)
		{
			Cacher._provider.Insert(key, value, timeToLive, slidingExpiration, priority);
		}
		public static IList<CacheItemDescriptor> GetDescriptors()
		{
			return Cacher._provider.GetDescriptors();
		}
	}
}
