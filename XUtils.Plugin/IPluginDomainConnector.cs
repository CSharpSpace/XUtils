using System;
using System.Collections.Generic;
namespace XUtils.Plugin
{
	public interface IPluginDomainConnector : IDisposable
	{
		IDictionary<string, IPluginConnector> Plugins
		{
			get;
		}
		AppDomain Domain
		{
			get;
		}
		string Name
		{
			get;
		}
		void LoadAssembly(string assemblyFile);
	}
}
