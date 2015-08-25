using System;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigHashSection : ConfigurationSection
	{
		[ConfigurationProperty("Settings")]
		public ConfigHashCollection HashKeys
		{
			get
			{
				return (ConfigHashCollection)base["Settings"];
			}
		}
	}
}
