using System;
namespace XUtils.Plugin
{
	[Serializable]
	public class PluginConnector : MarshalByRefObject, IPluginConnector, IDisposable
	{
		private string name;
		private string assemblyFile;
		private PluginInstanceFactory factory;
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public PluginConnector(string assemblyFile, PluginInstanceFactory factory, string plugName)
		{
			this.name = plugName;
			this.assemblyFile = assemblyFile;
			this.factory = factory;
		}
		public T Create<T>()
		{
			T result = default(T);
			try
			{
				result = this.factory.Create<T>(this.assemblyFile, this.name, null);
			}
			catch (Exception)
			{
			}
			return result;
		}
		public void Dispose()
		{
		}
	}
}
