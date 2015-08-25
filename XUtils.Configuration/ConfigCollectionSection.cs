using System;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigCollectionSection : ConfigurationSection
	{
		[ConfigurationProperty("Settings")]
		public ConfigCollection Collections
		{
			get
			{
				return (ConfigCollection)base["Settings"];
			}
		}
	}
}
