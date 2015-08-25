using System;
using System.Collections.Generic;
using System.Data;
namespace XUtils.Data
{
	public abstract class RowMapperTableBased<T> : RowMapperBaseWithCallBacks<DataTable, T, int>, IRowMapper<DataTable, T>
	{
		public IList<T> MapRows(DataTable table)
		{
			IList<T> list = new List<T>();
			for (int i = 0; i < table.Rows.Count; i++)
			{
				T item = this.MapRow(table, i);
				list.Add(item);
			}
			if (base.IsCallbackEnabledForAfterRowsMapped)
			{
				this.OnAfterRowsMapped(list);
			}
			return list;
		}
	}
}
