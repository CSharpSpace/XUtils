using System;
using System.Reflection;
namespace XUtils.Data
{
	public static class DataBaseFactory
	{
		public static DataBase Provider(string providerAssembly, string providerType)
		{
			Assembly assembly = Assembly.Load(providerAssembly);
			object obj = assembly.CreateInstance(providerType);
			if (obj is DataBase)
			{
				return obj as DataBase;
			}
			throw new Exception("The provider specified does not extends the AdoHelper abstract class.");
		}
		public static DataBase Provider(string providerAlias)
		{
			string a;
			if ((a = providerAlias.ToUpper()) != null)
			{
				if (a == "OLEDB")
				{
					return new DataOleDb();
				}
				if (a == "ORACLE")
				{
					return new DataOracle();
				}
				if (a == "SQLSERVER")
				{
					return new DataSqlServer();
				}
				if (a == "ODBC")
				{
					return new DataOdbc();
				}
			}
			return new DataSqlServer();
		}
		public static DataBase Provider(DbProvideType type)
		{
			switch (type)
			{
			case DbProvideType.ORACLE:
				return new DataOleDb();
			case DbProvideType.SQLSERVER:
				return new DataSqlServer();
			case DbProvideType.OLEDB:
				return new DataOleDb();
			case DbProvideType.ODBC:
				return new DataOdbc();
			default:
				return new DataSqlServer();
			}
		}
		public static DataBase Provider()
		{
			return new DataSqlServer();
		}
	}
}
