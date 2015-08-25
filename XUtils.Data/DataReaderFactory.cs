using System;
using System.Collections.Generic;
namespace XUtils.Data
{
	public static class DataReaderFactory
	{
		private static readonly IDictionary<Type, IDataTypeReader> readers;
		static DataReaderFactory()
		{
			DataReaderFactory.readers = new Dictionary<Type, IDataTypeReader>();
			DataReaderFactory.readers.Add(typeof(int), new IntDataReader());
			DataReaderFactory.readers.Add(typeof(int?), new IntDataReader());
			DataReaderFactory.readers.Add(typeof(long), new LongDataReader());
			DataReaderFactory.readers.Add(typeof(long?), new LongDataReader());
			DataReaderFactory.readers.Add(typeof(string), new StringDataReader());
			DataReaderFactory.readers.Add(typeof(bool), new BooleanDataReader());
			DataReaderFactory.readers.Add(typeof(bool?), new BooleanDataReader());
			DataReaderFactory.readers.Add(typeof(double), new DoubleDataReader());
			DataReaderFactory.readers.Add(typeof(double?), new DoubleDataReader());
			DataReaderFactory.readers.Add(typeof(DateTime), new DateTimeDataReader());
			DataReaderFactory.readers.Add(typeof(DateTime?), new DateTimeDataReader());
			DataReaderFactory.readers.Add(typeof(float), new FloatDataReader());
			DataReaderFactory.readers.Add(typeof(float?), new FloatDataReader());
			DataReaderFactory.readers.Add(typeof(Guid), new GuidDataReader());
			DataReaderFactory.readers.Add(typeof(Guid?), new GuidDataReader());
			DataReaderFactory.readers.Add(typeof(short), new Int16DataReader());
			DataReaderFactory.readers.Add(typeof(short?), new Int16DataReader());
			DataReaderFactory.readers.Add(typeof(decimal), new DecimalDataReader());
			DataReaderFactory.readers.Add(typeof(decimal?), new DecimalDataReader());
			DataReaderFactory.readers.Add(typeof(byte), new ByteDataReader());
			DataReaderFactory.readers.Add(typeof(byte?), new ByteDataReader());
		}
		public static IDataTypeReader GetReader(Type type)
		{
			if (DataReaderFactory.readers.ContainsKey(type))
			{
				return DataReaderFactory.readers[type];
			}
			return null;
		}
	}
}
