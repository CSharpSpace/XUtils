using System;
using System.Data;
namespace XUtils.Data
{
	public class DecimalDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetDecimal(fldnum);
		}
	}
}
