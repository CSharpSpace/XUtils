using System;
using System.Data;
namespace XUtils.Data
{
	public interface IDataTypeReader
	{
		object Read(IDataRecord rec, int fldnum);
	}
}
