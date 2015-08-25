using System;
using System.Data;
namespace XUtils.Data
{
	public class DoubleDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetDouble(fldnum);
		}
	}
}
