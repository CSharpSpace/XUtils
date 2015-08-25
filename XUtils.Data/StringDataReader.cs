using System;
using System.Data;
namespace XUtils.Data
{
	public class StringDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetString(fldnum);
		}
	}
}
