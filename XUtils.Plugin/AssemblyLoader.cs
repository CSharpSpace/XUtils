using System;
namespace XUtils.Plugin
{
	public static class AssemblyLoader
	{
		public static void LoadAssembly(this IPluginContainer Container, string assemblyFile)
		{
			IPluginDomainConnector pluginDomainConnector = new PluginDomainConnector();
			pluginDomainConnector.LoadAssembly(assemblyFile);
			if (pluginDomainConnector.Plugins.Count == 0)
			{
				pluginDomainConnector.Dispose();
				return;
			}
			Container.AddDomain(pluginDomainConnector.Name, pluginDomainConnector);
		}
	}
}
