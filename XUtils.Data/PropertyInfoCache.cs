using System;
using System.Collections.Generic;
using System.Reflection;
namespace XUtils.Data
{
	internal static class PropertyInfoCache
	{
		private static Dictionary<string, List<PropertyInfo>> cache = new Dictionary<string, List<PropertyInfo>>();
		internal static List<PropertyInfo> GetCache(string typeName)
		{
			List<PropertyInfo> result = null;
			PropertyInfoCache.cache.TryGetValue(typeName, out result);
			return result;
		}
		internal static void SetCache(string typeName, List<PropertyInfo> mappingInfoList)
		{
			PropertyInfoCache.cache.Add(typeName, mappingInfoList);
		}
		public static void ClearCache()
		{
			PropertyInfoCache.cache.Clear();
		}
	}
}
