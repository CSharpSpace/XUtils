using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace XUtils.Data
{
	public static class DataBulkCopy
	{
		public static void CopyTo(this DataBase db, string tableName, DataRow[] sources, KeyValuePair<string, string>[] args = null, int batchSize = 10000, int copyTimeout = 60000)
		{
			db.CopyTo(tableName, sources.CopyToDataTable<DataRow>(), args, batchSize, copyTimeout);
		}
		public static void CopyTo(this DataBase db, string tableName, DataTable sources, KeyValuePair<string, string>[] args = null, int batchSize = 10000, int copyTimeout = 60000)
		{
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(db.ConnectionString, SqlBulkCopyOptions.Default);
			sqlBulkCopy.DestinationTableName = tableName;
			sqlBulkCopy.BatchSize = batchSize;
			sqlBulkCopy.BulkCopyTimeout = copyTimeout;
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					KeyValuePair<string, string> keyValuePair = args[i];
					sqlBulkCopy.ColumnMappings.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			try
			{
				sqlBulkCopy.WriteToServer(sources);
			}
			catch
			{
				throw;
			}
			finally
			{
				sqlBulkCopy.Close();
			}
		}
		public static void CopyTo(this DataBase db, string tableName, IDataReader sources, KeyValuePair<string, string>[] args = null, int batchSize = 10000, int copyTimeout = 60000)
		{
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(db.ConnectionString, SqlBulkCopyOptions.Default);
			sqlBulkCopy.DestinationTableName = tableName;
			sqlBulkCopy.BatchSize = batchSize;
			sqlBulkCopy.BulkCopyTimeout = copyTimeout;
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					KeyValuePair<string, string> keyValuePair = args[i];
					sqlBulkCopy.ColumnMappings.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			try
			{
				sqlBulkCopy.WriteToServer(sources);
			}
			catch
			{
				throw;
			}
			finally
			{
				sqlBulkCopy.Close();
			}
		}
	}
}
