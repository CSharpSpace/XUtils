using System;
using System.Collections.Generic;
using System.Configuration;
namespace XUtils.Configuration
{
	public static class Config
	{
		private static ConfigInISettings iniSections = new ConfigInISettings();
		private static ConfigHashSectionSettings sections = new ConfigHashSectionSettings();
		private static ConfigCollectionSectionSettings collections = new ConfigCollectionSectionSettings();
		private static ConfigConnectionSettings connections = new ConfigConnectionSettings();
		private static ConfigAppSettings app = new ConfigAppSettings();
		public static ConfigInISettings InISections
		{
			get
			{
				return Config.iniSections;
			}
		}
		public static ConfigHashSectionSettings Sections
		{
			get
			{
				return Config.sections;
			}
		}
		public static ConfigCollectionSectionSettings Collections
		{
			get
			{
				return Config.collections;
			}
		}
		public static ConfigConnectionSettings Connections
		{
			get
			{
				return Config.connections;
			}
		}
		public static ConfigAppSettings Apps
		{
			get
			{
				return Config.app;
			}
		}
		public static IDictionary<string, string> InISection(string key)
		{
			return Config.iniSections.Get(key);
		}
		public static T InISection<T>(string section, string key)
		{
			return Config.iniSections[section].Get<T>(key);
		}
		[Obsolete("已经过时，该方法仅限系统默认的Config文件中获取")]
		public static T Get<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new Exception("Key is null...");
			}
			string input = ConfigurationManager.AppSettings[key];
			return TypeParsers.ConvertTo<T>(input);
		}
		[Obsolete("已经过时，该方法仅限系统默认的Config文件中获取")]
		public static ConnectionInfo GetConnectionInfo(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new Exception("Key is null...");
			}
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[key];
			return new ConnectionInfo
			{
				ProviderName = connectionStringSettings.ProviderName,
				ConnectionString = connectionStringSettings.ConnectionString
			};
		}
		public static IDictionary<string, string> Section(string key)
		{
			return Config.iniSections.Get(key);
		}
		public static IList<string> Collection(string key)
		{
			return Config.collections.Get(key);
		}
		public static T Section<T>(string section, string key)
		{
            return Config.iniSections[section].Get<T>(key);
		}
		public static ConnectionInfo Connection(string key)
		{
			return Config.connections.Container[key];
		}
		public static T App<T>(string key)
		{
            return Config.app.Container.Get<T>(key);
		}
	}
}
