using System;
namespace XUtils.Data
{
	public abstract class RowMapperBase<TSource, TResult, TRowId>
	{
		public abstract TResult MapRow(TSource source, TRowId rowNumber);
	}
}
