using System;
namespace XUtils.Caching
{
	public class CacheSettings
	{
		public string PrefixForCacheKeys = "xutils";
		public bool UsePrefix = true;
		public CacheItemPriority DefaultCachePriority = CacheItemPriority.Normal;
		public bool DefaultSlidingExpirationEnabled;
		public int DefaultTimeToLive = 600;
	}
}
