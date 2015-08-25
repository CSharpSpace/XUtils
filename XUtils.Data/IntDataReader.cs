using System;
using System.Data;
namespace XUtils.Data
{
	public class IntDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetInt32(fldnum);
		}
	}
}
