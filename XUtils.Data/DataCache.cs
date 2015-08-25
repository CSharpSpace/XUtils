using System;
using System.Web;
using System.Web.Caching;
namespace XUtils.Data
{
	public static class DataCache
	{
		public static void Add<T>(T o, string key, SqlCacheDependency denpendency)
		{
			HttpRuntime.Cache.Insert(key, o, denpendency, Cache.NoAbsoluteExpiration, TimeSpan.Zero);
		}
		public static void Clear(string key)
		{
			HttpRuntime.Cache.Remove(key);
		}
		public static bool Exists(string key)
		{
			return HttpRuntime.Cache[key] != null;
		}
		public static bool Get<T>(string key, out T value)
		{
			try
			{
				if (!DataCache.Exists(key))
				{
					value = default(T);
					bool result = false;
					return result;
				}
				value = (T)((object)HttpRuntime.Cache[key]);
			}
			catch
			{
				value = default(T);
				bool result = false;
				return result;
			}
			return true;
		}
	}
}
