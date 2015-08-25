using System;
using System.Data;
namespace XUtils.Data
{
	public class ByteDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetByte(fldnum);
		}
	}
}
