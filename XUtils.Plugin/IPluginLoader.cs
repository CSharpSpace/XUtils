using System;
namespace XUtils.Plugin
{
	public interface IPluginLoader
	{
		void LoadFile(string assemblyFile);
		void LoadDirectory(string directory, string searchPattern);
	}
}
