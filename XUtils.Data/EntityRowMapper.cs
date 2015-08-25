using System;
using System.Data;
namespace XUtils.Data
{
	public abstract class EntityRowMapper<T> : RowMapperReaderBased<T>, IEntityRowMapper<T>, IRowMapper<IDataReader, T>
	{
		protected Func<object, T> _entityFactoryMethod;
	}
}
