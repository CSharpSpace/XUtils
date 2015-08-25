using System;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigConnectionSettings : ConfigBase<string, ConnectionInfo>
	{
		public ConfigConnectionSettings()
		{
			base.DirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
			this.Load();
		}
		public override void Load()
		{
			base.Clear();
			this.DirectorySearch(base.DirectoryPath, "*.config");
			foreach (string current in base.ConfigFiles)
			{
				this.Load(current);
			}
		}
		public override bool Load(string file)
		{
            System.Configuration.Configuration configuration = base.GetConfiguration(file);
			foreach (ConnectionStringSettings connectionStringSettings in configuration.ConnectionStrings.ConnectionStrings)
			{
				ConnectionInfo connectionInfo = new ConnectionInfo();
				connectionInfo.ConnectionString = connectionStringSettings.ConnectionString;
				connectionInfo.ProviderName = connectionStringSettings.ProviderName;
				base.Opera(ConfigMode.UpSet, connectionStringSettings.Name, connectionInfo);
			}
			return false;
		}
	}
}
