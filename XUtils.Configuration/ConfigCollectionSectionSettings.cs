using System;
using System.Collections.Generic;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigCollectionSectionSettings : ConfigBase<string, IList<string>>
	{
		public ConfigCollectionSectionSettings()
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
				ConfigCollectionSection configCollectionSection = configurationSection as ConfigCollectionSection;
				if (configCollectionSection != null)
				{
					IList<string> list = new List<string>();
					foreach (ConfigCollectionElement configCollectionElement in configCollectionSection.Collections)
					{
						list.Add(configCollectionElement.Item);
					}
					base.Opera(ConfigMode.UpSet, configurationSection.SectionInformation.Name, list);
				}
			}
			return false;
		}
	}
}
