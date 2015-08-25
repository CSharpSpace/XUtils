using System;
namespace XUtils.Caching
{
	public enum CacheItemPriority
	{
		Low,
		Normal,
		High,
		NotRemovable,
		Default = 1
	}
}
