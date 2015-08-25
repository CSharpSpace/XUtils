using System;
using System.Data;
namespace XUtils.Data
{
	public class FloatDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetFloat(fldnum);
		}
	}
}
