using System;
using System.Data;
namespace XUtils.Data
{
	public class LongDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetInt64(fldnum);
		}
	}
}
