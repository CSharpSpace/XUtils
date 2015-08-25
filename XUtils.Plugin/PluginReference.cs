using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Plugin
{
	public static class PluginReference
	{
		private static object syncObject;
		private static IDictionary<string, IPlugin> instances;
		private static IPluginContainer container;
		public static IPluginContainer Container
		{
			get
			{
				return PluginReference.container;
			}
		}
		public static IDictionary<string, IPluginConnector> Connectors
		{
			get
			{
				return PluginReference.container.Plugins;
			}
		}
		public static IDictionary<string, IPlugin> Instances
		{
			get
			{
				return PluginReference.instances;
			}
		}
		static PluginReference()
		{
			PluginReference.syncObject = new object();
			PluginReference.instances = null;
			PluginReference.container = null;
			PluginReference.container = PluginFactory.Container;
			PluginReference.instances = new Dictionary<string, IPlugin>();
		}
		public static void Register(string directory, string searchPattern = "*.dll")
		{
			PluginFactory.LoadFile(directory, searchPattern);
		}
		public static void RegisterAssemblyFile(string assemblyFile)
		{
			PluginFactory.LoadFile(assemblyFile);
		}
		public static void Unload(string dll)
		{
			object obj;
			Monitor.Enter(obj = PluginReference.syncObject);
			try
			{
				IPluginDomainConnector domain = PluginFactory.Container.GetDomain(dll);
				foreach (KeyValuePair<string, IPluginConnector> current in domain.Plugins)
				{
					if (PluginReference.instances.ContainsKey(current.Key))
					{
						PluginReference.instances.Remove(current.Key);
					}
				}
				PluginFactory.Container.Unload(dll);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void Remove(string key)
		{
			object obj;
			Monitor.Enter(obj = PluginReference.syncObject);
			try
			{
				if (PluginReference.instances.ContainsKey(key))
				{
					PluginReference.instances.Remove(key);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void CreateInstances()
		{
			object obj;
			Monitor.Enter(obj = PluginReference.syncObject);
			try
			{
				foreach (KeyValuePair<string, IPluginConnector> current in PluginReference.container.Plugins)
				{
					IPlugin value = current.Value.Create<IPlugin>();
					PluginReference.instances.Add(current.Key, value);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
	}
}
