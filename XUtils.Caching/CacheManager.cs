using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Caching
{
	public class CacheManager
	{
		private ICache _cache;
		public ICache Cache
		{
			get
			{
				return this._cache;
			}
		}
		public CacheManager()
		{
			this._cache = new CacheAspNet();
		}
		public CacheManager(ICache cache)
		{
			this._cache = cache;
		}
		public IList<CacheItemDescriptor> GetDescriptors()
		{
			IList<CacheItemDescriptor> list = new List<CacheItemDescriptor>();
			ICollection keys = this._cache.Keys;
			IEnumerator enumerator = keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string key = enumerator.Current as string;
				object obj = this._cache.Get(key);
				list.Add(new CacheItemDescriptor(key, obj.GetType().FullName));
			}
			((List<CacheItemDescriptor>)list).Sort((CacheItemDescriptor c1, CacheItemDescriptor c2) => c1.Key.CompareTo(c2.Key));
			return list;
		}
	}
}
