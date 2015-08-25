using System;
using System.Collections.Generic;
using System.Data;
namespace XUtils.Data
{
	public abstract class RowMapperReaderBased<T> : RowMapperBaseWithCallBacks<IDataReader, T, int>, IRowMapper<IDataReader, T>
	{
		public IList<T> MapRows(IDataReader reader)
		{
			IList<T> list = new List<T>();
			int num = 0;
			while (reader.Read())
			{
				T item = this.MapRow(reader, num);
				list.Add(item);
				num++;
			}
			if (base.IsCallbackEnabledForAfterRowsMapped)
			{
				this.OnAfterRowsMapped(list);
			}
			return list;
		}
	}
}
