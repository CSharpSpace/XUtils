using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Ioc
{
	internal class TypeCollection : ITypeCollection, IEnumerable<Type>, IEnumerable
	{
		internal IEnumerable<Type> Actual
		{
			get;
			set;
		}
		public IEnumerator<Type> GetEnumerator()
		{
			return this.Actual.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Actual.GetEnumerator();
		}
	}
}
