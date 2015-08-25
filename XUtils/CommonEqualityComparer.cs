using System;
using System.Collections.Generic;
namespace XUtils
{
	public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
	{
		private Func<T, V> keySelector;
		private IEqualityComparer<V> comparer;
		public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
		{
			this.keySelector = keySelector;
			this.comparer = comparer;
		}
		public CommonEqualityComparer(Func<T, V> keySelector) : this(keySelector, EqualityComparer<V>.Default)
		{
		}
		public bool Equals(T x, T y)
		{
			return this.comparer.Equals(this.keySelector(x), this.keySelector(y));
		}
		public int GetHashCode(T obj)
		{
			return this.comparer.GetHashCode(this.keySelector(obj));
		}
	}
}
