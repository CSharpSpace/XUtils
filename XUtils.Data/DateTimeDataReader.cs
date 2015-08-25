using System;
using System.Data;
namespace XUtils.Data
{
	public class DateTimeDataReader : IDataTypeReader
	{
		public object Read(IDataRecord rec, int fldnum)
		{
			return rec.GetDateTime(fldnum);
		}
	}
}
