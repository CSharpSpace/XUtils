using System;
using System.Collections.Generic;
namespace XUtils.Data
{
	public abstract class RowMapperBaseWithCallBacks<TSource, TResult, TRowId> : RowMapperBase<TSource, TResult, TRowId>
	{
		public bool IsCallbackEnabledForAfterRowsMapped
		{
			get;
			set;
		}
		public virtual void OnAfterRowsMapped(IList<TResult> items)
		{
		}
	}
}
