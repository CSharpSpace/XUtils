using System;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigAppSettings : ConfigBase<string, string>
	{
		public ConfigAppSettings()
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
			foreach (KeyValueConfigurationElement keyValueConfigurationElement in configuration.AppSettings.Settings)
			{
				base.Opera(ConfigMode.UpSet, keyValueConfigurationElement.Key, keyValueConfigurationElement.Value);
			}
			return false;
		}
	}
}
