using System;
using System.Collections.Generic;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigHashSectionSettings : ConfigBase<string, IDictionary<string, string>>
	{
		public string this[string section, string key]
		{
			get
			{
				IDictionary<string, string> dictionary = base.Get(section);
				if (dictionary != null)
				{
					return dictionary[key];
				}
				return "";
			}
		}
		public ConfigHashSectionSettings()
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
			foreach (ConfigurationSection configurationSection in configuration.Sections)
			{
				ConfigHashSection configHashSection = configurationSection as ConfigHashSection;
				if (configHashSection != null)
				{
					IDictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (ConfigHashElement configHashElement in configHashSection.HashKeys)
					{
						dictionary.Add(configHashElement.Key, configHashElement.Value);
					}
					base.Opera(ConfigMode.UpSet, configurationSection.SectionInformation.Name, dictionary);
				}
			}
			return false;
		}
	}
}
