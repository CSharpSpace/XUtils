using System;
using System.Collections.Generic;
namespace XUtils
{
	public static class ListExtensions
	{
		public static IList<T> AddRange<T>(this IList<T> items, IList<T> itemsToAdd)
		{
			if (items == null || itemsToAdd == null)
			{
				return items;
			}
			foreach (T current in itemsToAdd)
			{
				items.Add(current);
			}
			return items;
		}
		[Obsolete("Method moved to Utilities.EnumerableExtensions.IsNullOrEmpty()")]
		public static bool IsNullOrEmpty<T>(IList<T> items)
		{
			return items.IsNullOrEmpty<T>();
		}
	}
}
