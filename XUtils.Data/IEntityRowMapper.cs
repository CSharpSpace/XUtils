using System;
using System.Data;
namespace XUtils.Data
{
	public interface IEntityRowMapper<T> : IRowMapper<IDataReader, T>
	{
	}
}
