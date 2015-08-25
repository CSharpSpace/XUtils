using System;
using System.Collections.Generic;
namespace XUtils.Plugin
{
	public interface IPluginContainer
	{
		IDictionary<string, IPluginConnector> Plugins
		{
			get;
		}
		IDictionary<string, IPluginDomainConnector> Domains
		{
			get;
		}
		void AddDomain(string key, IPluginDomainConnector item);
		IPluginDomainConnector GetDomain(string key);
		void Unload(string key);
		void UnloadAll();
	}
}
