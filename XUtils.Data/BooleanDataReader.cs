using System;
using System.Data;
namespace XUtils.Data
{
	public class BooleanDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetBoolean(fldnum);
		}
	}
}
