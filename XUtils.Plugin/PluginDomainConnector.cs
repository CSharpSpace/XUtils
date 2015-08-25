using System;
using System.Collections.Generic;
using System.IO;
namespace XUtils.Plugin
{
	public class PluginDomainConnector : IPluginDomainConnector, IDisposable
	{
		private static int domainID = 2;
		private AppDomain domain;
		private string name;
		private PluginInstanceFactory factory;
		private IDictionary<string, IPluginConnector> plugins;
		private bool _isDisposed;
		public IDictionary<string, IPluginConnector> Plugins
		{
			get
			{
				return this.plugins;
			}
		}
		public AppDomain Domain
		{
			get
			{
				return this.domain;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public PluginDomainConnector()
		{
			PluginDomainConnector.domainID++;
			this.plugins = new Dictionary<string, IPluginConnector>();
		}
		~PluginDomainConnector()
		{
			this.Dispose(false);
		}
		public void LoadAssembly(string assemblyFile)
		{
			try
			{
				this.name = Path.GetFileName(assemblyFile);
				this.domain = AppDomain.CreateDomain(this.name);
				Type typeFromHandle = typeof(PluginInstanceFactory);
				this.factory = (PluginInstanceFactory)this.domain.CreateInstance(typeFromHandle.Assembly.FullName, typeFromHandle.FullName).Unwrap();
				IDictionary<string, string> dictionary = this.factory.LoadTypeForAll(assemblyFile);
				foreach (KeyValuePair<string, string> current in dictionary)
				{
					PluginConnector value = new PluginConnector(assemblyFile, this.factory, current.Value);
					this.plugins.Add(current.Key, value);
				}
			}
			catch (Exception)
			{
			}
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (this._isDisposed)
			{
				return;
			}
			if (disposing)
			{
				try
				{
					AppDomain.Unload(this.domain);
					this.domain = null;
				}
				catch
				{
				}
			}
			this._isDisposed = true;
		}
	}
}
