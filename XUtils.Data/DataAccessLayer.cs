using System;
using XUtils.Configuration;
using XUtils.Ioc;
namespace XUtils.Data
{
	public abstract class DataAccessLayer : Service
	{
		public DataBase db;
		public string DefultConnectionStrings = "DefultConnectionStrings";
		public DataAccessLayer(string settingName)
		{
			try
			{
				string key = Config.App<string>(string.IsNullOrEmpty(settingName) ? this.DefultConnectionStrings : settingName);
				ConnectionInfo connectionInfo = Config.Connection(key);
				this.db = DataBaseFactory.Provider(connectionInfo.ProviderName);
				this.db.ConnectionString = connectionInfo.ConnectionString;
			}
			catch
			{
				throw new Exception(string.Format("Config 中没找到 \"{1}\" 连接字符串配置信息", settingName));
			}
		}
	}
	public abstract class DataAccessLayer<TEntity> : DataAccessLayer where TEntity : class, new()
	{
		public DBUtils<TEntity> dbUtils = DBUtils<TEntity>.New();
		public DataAccessLayer() : this("")
		{
		}
		public DataAccessLayer(string settingName) : base(settingName)
		{
			this.dbUtils.DB = this.db;
		}
	}
}
