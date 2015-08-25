using System;
using System.Configuration;
namespace XUtils.Configuration
{
	[ConfigurationCollection(typeof(ConfigHashElement))]
	public class ConfigHashCollection : ConfigurationElementCollection
	{
		public ConfigHashElement this[int idx]
		{
			get
			{
				return (ConfigHashElement)base.BaseGet(idx);
			}
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigHashElement();
		}
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ConfigHashElement)element).Key;
		}
	}
}
