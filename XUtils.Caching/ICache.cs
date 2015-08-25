using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Caching
{
	public interface ICache
	{
		int Count
		{
			get;
		}
		ICollection Keys
		{
			get;
		}
		bool Contains(string key);
		object Get(object key);
		T Get<T>(object key);
		T GetOrInsert<T>(object key, int timeToLiveInSeconds, bool slidingExpiration, Func<T> fetcher);
		void Remove(object key);
		void RemoveAll(ICollection keys);
		void Clear();
		void Insert(object key, object value);
		void Insert(object key, object value, int timeToLive, bool slidingExpiration);
		void Insert(object key, object value, int timeToLive, bool slidingExpiration, CacheItemPriority priority);
		IList<CacheItemDescriptor> GetDescriptors();
	}
}
