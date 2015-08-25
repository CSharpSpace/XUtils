using System;
using System.Data;
namespace XUtils.Data
{
	public class GuidDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetGuid(fldnum);
		}
	}
}
