using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
namespace XUtils.Caching
{
	public class CacheAspNet : ICache
	{
		private Cache _cache;
		private CacheSettings _settings = new CacheSettings();
		public CacheSettings Settings
		{
			get
			{
				return this._settings;
			}
		}
		public int Count
		{
			get
			{
				if (!this._settings.UsePrefix)
				{
					return this._cache.Count;
				}
				int num = 0;
				foreach (DictionaryEntry dictionaryEntry in this._cache)
				{
					string text = (string)dictionaryEntry.Key;
					if (text.StartsWith(this._settings.PrefixForCacheKeys))
					{
						num++;
					}
				}
				return num;
			}
		}
		public ICollection Keys
		{
			get
			{
				IList<string> list = new List<string>();
				foreach (DictionaryEntry dictionaryEntry in this._cache)
				{
					string text = (string)dictionaryEntry.Key;
					if (!this._settings.UsePrefix)
					{
						list.Add(text);
					}
					else
					{
						if (text.StartsWith(this._settings.PrefixForCacheKeys))
						{
							list.Add(text.Substring(this._settings.PrefixForCacheKeys.Length + 1));
						}
					}
				}
				return list as ICollection;
			}
		}
		public CacheAspNet()
		{
			this.Init(new CacheSettings());
		}
		public CacheAspNet(CacheSettings settings)
		{
			this.Init(settings);
		}
		public bool Contains(string key)
		{
			string key2 = this.BuildKey(key);
			return this._cache.Get(key2) != null;
		}
		public object Get(object key)
		{
			string key2 = this.BuildKey(key);
			return this._cache.Get(key2);
		}
		public T Get<T>(object key)
		{
			string key2 = this.BuildKey(key);
			object obj = this._cache.Get(key2);
			if (obj == null)
			{
				return default(T);
			}
			return (T)((object)obj);
		}
		public T GetOrInsert<T>(object key, int timeToLiveInSeconds, bool slidingExpiration, Func<T> fetcher)
		{
			object obj = this.Get(key);
			if (obj == null)
			{
				T t = fetcher();
				this.Insert(key, t, timeToLiveInSeconds, slidingExpiration);
				return t;
			}
			return (T)((object)obj);
		}
		public void Remove(object key)
		{
			string key2 = this.BuildKey(key);
			this._cache.Remove(key2);
		}
		public void RemoveAll(ICollection keys)
		{
			foreach (object current in keys)
			{
				string key = this.BuildKey((string)current);
				this._cache.Remove(key);
			}
		}
		public void Clear()
		{
			ICollection keys = this.Keys;
			foreach (string key in keys)
			{
				string key2 = this.BuildKey(key);
				this._cache.Remove(key2);
			}
		}
		public void Insert(object key, object value)
		{
			this.Insert(key, value, this._settings.DefaultTimeToLive, this._settings.DefaultSlidingExpirationEnabled, this._settings.DefaultCachePriority);
		}
		public void Insert(object key, object value, int timeToLive, bool slidingExpiration)
		{
			this.Insert(key, value, timeToLive, slidingExpiration, this._settings.DefaultCachePriority);
		}
		public void Insert(object keyName, object value, int timeToLive, bool slidingExpiration, CacheItemPriority priority)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)timeToLive);
			Guard.IsNotNull(keyName, "key");
			Guard.IsTrue(TimeSpan.Zero <= timeSpan, "timeToLive");
			System.Web.Caching.CacheItemPriority priority2 = this.ConvertToAspNetPriority(priority);
			string key = this.BuildKey(keyName);
			if (!(TimeSpan.Zero < timeSpan))
			{
				this._cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority2, null);
				return;
			}
			if (slidingExpiration)
			{
				this._cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, timeSpan, priority2, null);
				return;
			}
			DateTime absoluteExpiration = DateTime.Now.AddSeconds((double)timeToLive);
			this._cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration, priority2, null);
		}
		public IList<CacheItemDescriptor> GetDescriptors()
		{
			IList<CacheItemDescriptor> list = new List<CacheItemDescriptor>();
			ICollection keys = this.Keys;
			IEnumerator enumerator = keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string key = enumerator.Current as string;
				object obj = this.Get(key);
				list.Add(new CacheItemDescriptor(key, obj.GetType().FullName));
			}
			((List<CacheItemDescriptor>)list).Sort((CacheItemDescriptor c1, CacheItemDescriptor c2) => c1.Key.CompareTo(c2.Key));
			return list;
		}
		private void Init(CacheSettings settings)
		{
			this._cache = HttpRuntime.Cache;
			this._settings = settings;
		}
		private string BuildKey(object key)
		{
			if (this._settings.UsePrefix)
			{
				return this._settings.PrefixForCacheKeys + "." + key.ToString();
			}
			return key.ToString();
		}
		private System.Web.Caching.CacheItemPriority ConvertToAspNetPriority(CacheItemPriority priority)
		{
			if (priority == CacheItemPriority.Normal)
			{
				return System.Web.Caching.CacheItemPriority.Normal;
			}
			if (priority == CacheItemPriority.High)
			{
				return System.Web.Caching.CacheItemPriority.High;
			}
			if (priority == CacheItemPriority.Low)
			{
				return System.Web.Caching.CacheItemPriority.Low;
			}
			if (priority == CacheItemPriority.Normal)
			{
				return System.Web.Caching.CacheItemPriority.Normal;
			}
			return System.Web.Caching.CacheItemPriority.NotRemovable;
		}
	}
}
