using System;
using XUtils.Configuration;
namespace XUtils.Data
{
	public class ConnectionSettings
	{
		public const string DefaultName = "ConnectionString";
		private string userID = string.Empty;
		private string passWord = string.Empty;
		private string host = string.Empty;
		private string dataBaseName = string.Empty;
		private int port;
		public static string Default
		{
			get
			{
				return Config.Connection("ConnectionString").ConnectionString;
			}
		}
		public static string GetAppSettings(string strName)
		{
			return Config.App<string>(strName);
		}
		public ConnectionSettings(string host, string dataBaseName, string userID, string passWord)
		{
			this.host = host;
			this.dataBaseName = dataBaseName;
			this.userID = userID;
			this.passWord = passWord;
		}
		public ConnectionSettings(string host, int port, string dataBaseName, string userID, string passWord)
		{
			this.host = host;
			this.dataBaseName = dataBaseName;
			this.userID = userID;
			this.passWord = passWord;
			this.port = port;
		}
		public override string ToString()
		{
			return string.Format("Data Source={0},{1};Initial Catalog={2};Persist Security Info=True;User ID={3};Password={4}", new object[]
			{
				this.host,
				(this.port == 0) ? 1433 : this.port,
				this.dataBaseName,
				this.userID,
				this.passWord
			});
		}
	}
}
