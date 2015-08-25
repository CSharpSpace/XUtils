using System;
using System.Data;
namespace XUtils.Data
{
	public static class DataFieldExtensions
	{
		public static T Get<T>(this DataRow dr, string fldname)
		{
			if (dr.IsNull(fldname))
			{
				return default(T);
			}
			object obj = dr[fldname];
			if (obj == DBNull.Value || object.Equals(obj, null))
			{
				return default(T);
			}
			return (T)((object)obj);
		}
		public static T Get<T>(this IDataRecord rec, string fldname)
		{
			int ordinal = rec.GetOrdinal(fldname);
			object obj = DataFieldExtensions.Get(rec, typeof(T), ordinal);
			if (obj == DBNull.Value || object.Equals(obj, null))
			{
				return default(T);
			}
			return (T)((object)obj);
		}
		private static object Get(IDataRecord rec, Type type, int fldnum)
		{
			if (rec.IsDBNull(fldnum))
			{
				return DBNull.Value;
			}
			IDataTypeReader reader = DataReaderFactory.GetReader(type);
			if (reader == null)
			{
				return DBNull.Value;
			}
			return reader.Read(rec, fldnum);
		}
		public static T GetOutPutParam<T>(this IDataParameter param, T defaultValue)
		{
			if (param.Value is DBNull || param.Value == null || param.Value == DBNull.Value)
			{
				return defaultValue;
			}
			return (T)((object)param.Value);
		}
		public static T GetReturnPram<T>(this IDataParameter param)
		{
			if (param.Value is DBNull || param.Value == null || param.Value == DBNull.Value)
			{
				return default(T);
			}
			return (T)((object)param.Value);
		}
	}
}
