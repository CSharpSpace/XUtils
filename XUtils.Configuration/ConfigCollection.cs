using System;
using System.Configuration;
namespace XUtils.Configuration
{
	[ConfigurationCollection(typeof(ConfigCollectionElement))]
	public class ConfigCollection : ConfigurationElementCollection
	{
		public ConfigCollectionElement this[int idx]
		{
			get
			{
				return (ConfigCollectionElement)base.BaseGet(idx);
			}
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigCollectionElement();
		}
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ConfigCollectionElement)element).Item;
		}
	}
}
