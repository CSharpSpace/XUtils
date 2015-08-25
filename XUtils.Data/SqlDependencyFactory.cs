using System;
using System.Data.SqlClient;
using System.Web.Caching;
namespace XUtils.Data
{
	public static class SqlDependencyFactory
	{
		public static bool isDependency
		{
			get;
			set;
		}
		public static string Database
		{
			get;
			set;
		}
		public static string TableName
		{
			get;
			set;
		}
		public static void Start(string connectionString)
		{
			SqlDependency.Start(connectionString);
		}
		public static void Stop(string connectionString)
		{
			SqlDependency.Stop(connectionString);
		}
		public static void Add<T>(string key, T t)
		{
			if (SqlDependencyFactory.isDependency)
			{
				Guard.IsNotNull(SqlDependencyFactory.Database, "数据库名不能为空");
				Guard.IsNotNull(SqlDependencyFactory.TableName, "表名不能为空");
				SqlCacheDependency denpendency = new SqlCacheDependency(SqlDependencyFactory.Database, SqlDependencyFactory.TableName);
				if (!DataCache.Exists(key))
				{
					DataCache.Add<T>(t, key, denpendency);
				}
			}
		}
		public static void Clear(string key)
		{
			DataCache.Clear(key);
		}
		public static bool Exists(string key)
		{
			return DataCache.Exists(key);
		}
		public static T Get<T>(string key)
		{
			T result = default(T);
			if (DataCache.Exists(key))
			{
				DataCache.Get<T>(key, out result);
			}
			return result;
		}
	}
}
