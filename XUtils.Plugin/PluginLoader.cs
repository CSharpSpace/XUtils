using System;
using System.IO;
using System.Threading;
namespace XUtils.Plugin
{
	public class PluginLoader : IPluginLoader
	{
		private static object syncLock = new object();
		private IPluginContainer Container;
		public PluginLoader(IPluginContainer Container)
		{
			this.Container = Container;
		}
		public void LoadFile(string assemblyFile)
		{
			object obj;
			Monitor.Enter(obj = PluginLoader.syncLock);
			try
			{
				this.Container.LoadAssembly(assemblyFile);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void LoadDirectory(string directory, string searchPattern)
		{
			object obj;
			Monitor.Enter(obj = PluginLoader.syncLock);
			try
			{
				string[] files = Directory.GetFiles(Path.GetFullPath(directory), Path.GetFileName(searchPattern));
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string assemblyFile = array[i];
					this.Container.LoadAssembly(assemblyFile);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
	}
}
