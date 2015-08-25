using System;
using System.Configuration;
namespace XUtils.Configuration
{
	public class ConfigCollectionElement : ConfigurationElement
	{
		[ConfigurationProperty("item", DefaultValue = "", IsKey = false, IsRequired = true)]
		public string Item
		{
			get
			{
				return (string)base["item"];
			}
			set
			{
				base["item"] = value;
			}
		}
	}
}
