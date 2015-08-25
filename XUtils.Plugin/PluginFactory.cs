using System;
using System.Threading;
namespace XUtils.Plugin
{
	public static class PluginFactory
	{
		private static object _syncObject;
		private static IPluginContainer container;
		private static IPluginLoader loader;
		public static IPluginContainer Container
		{
			get
			{
				return PluginFactory.container;
			}
		}
		static PluginFactory()
		{
			PluginFactory._syncObject = new object();
			PluginFactory.container = new PluginContainer();
			PluginFactory.loader = new PluginLoader(PluginFactory.container);
		}
		public static void LoadFile(string assemblyFile)
		{
			object syncObject;
			Monitor.Enter(syncObject = PluginFactory._syncObject);
			try
			{
				PluginFactory.loader.LoadFile(assemblyFile);
			}
			finally
			{
				Monitor.Exit(syncObject);
			}
		}
		public static void LoadFile(string directory, string searchPattern = "*.dll")
		{
			object syncObject;
			Monitor.Enter(syncObject = PluginFactory._syncObject);
			try
			{
				if (string.IsNullOrEmpty(directory))
				{
					Guard.IsNotNull(null, "DLL目录不能为空");
				}
				PluginFactory.loader.LoadDirectory(directory, searchPattern);
			}
			finally
			{
				Monitor.Exit(syncObject);
			}
		}
	}
}
