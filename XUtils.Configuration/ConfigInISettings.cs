using System;
using System.Collections.Generic;
using XUtils.IO;
namespace XUtils.Configuration
{
	public class ConfigInISettings : ConfigBase<string, IDictionary<string, string>>
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
		public ConfigInISettings()
		{
			base.DirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
			this.Load();
		}
		public override void Load()
		{
			base.Clear();
			this.DirectorySearch(base.DirectoryPath, "*.ini");
			foreach (string current in base.ConfigFiles)
			{
				this.Load(current);
			}
		}
		public override bool Load(string file)
		{
			IniDocument iniDocument = new IniDocument(file);
			foreach (object current in iniDocument.Sections)
			{
				IniSection iniSection = (IniSection)current;
				if (iniSection != null)
				{
					IDictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (IniKey iniKey in iniSection.Keys)
					{
						dictionary.Add(iniKey.Name, iniKey.Value);
					}
					base.Opera(ConfigMode.UpSet, iniSection.Name, dictionary);
				}
			}
			return false;
		}
	}
}
