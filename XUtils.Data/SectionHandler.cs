using System;
using System.Collections;
using System.Configuration;
using System.Xml;
namespace XUtils.Data
{
	public class SectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			Hashtable hashtable = new Hashtable();
			XmlNodeList xmlNodeList = section.SelectNodes("daabProvider");
			foreach (XmlNode xmlNode in xmlNodeList)
			{
				if (xmlNode.Attributes["alias"] == null)
				{
					throw new Exception("The 'daabProvider' node must contain an attribute named 'alias' with the alias name for the provider.");
				}
				if (xmlNode.Attributes["assembly"] == null)
				{
					throw new Exception("The 'daabProvider' node must contain an attribute named 'assembly' with the name of the assembly containing the provider.");
				}
				if (xmlNode.Attributes["type"] == null)
				{
					throw new Exception("The 'daabProvider' node must contain an attribute named 'type' with the full name of the type for the provider.");
				}
				hashtable[xmlNode.Attributes["alias"].Value] = new ProviderAlias(xmlNode.Attributes["assembly"].Value, xmlNode.Attributes["type"].Value);
			}
			return hashtable;
		}
	}
}
