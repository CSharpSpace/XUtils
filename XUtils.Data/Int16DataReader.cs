using System;
using System.Data;
namespace XUtils.Data
{
	public class Int16DataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetInt16(fldnum);
		}
	}
}
