using System;
using System.Collections.Generic;
namespace XUtils.Data
{
	public interface IRowMapper<TSource, TResult>
	{
		bool IsCallbackEnabledForAfterRowsMapped
		{
			get;
			set;
		}
		IList<TResult> MapRows(TSource dataSource);
		TResult MapRow(TSource dataSource, int rowId);
		void OnAfterRowsMapped(IList<TResult> items);
	}
}
