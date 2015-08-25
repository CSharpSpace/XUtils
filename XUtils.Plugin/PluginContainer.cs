using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Plugin
{
	public class PluginContainer : IPluginContainer
	{
		private static object _syncObject = new object();
		private IDictionary<string, IPluginDomainConnector> domainContainer;
		private IDictionary<string, IPluginConnector> pluginContainer;
		public IDictionary<string, IPluginConnector> Plugins
		{
			get
			{
				return this.pluginContainer;
			}
		}
		public IDictionary<string, IPluginDomainConnector> Domains
		{
			get
			{
				return this.domainContainer;
			}
		}
		public PluginContainer()
		{
			this.pluginContainer = new Dictionary<string, IPluginConnector>();
			this.domainContainer = new Dictionary<string, IPluginDomainConnector>();
		}
		public void AddDomain(string key, IPluginDomainConnector item)
		{
			object syncObject;
			Monitor.Enter(syncObject = PluginContainer._syncObject);
			try
			{
				if (!this.domainContainer.ContainsKey(key))
				{
					this.domainContainer.Add(key, item);
				}
				foreach (KeyValuePair<string, IPluginConnector> current in item.Plugins)
				{
					if (!this.pluginContainer.ContainsKey(current.Key))
					{
						this.pluginContainer.Add(current);
					}
				}
			}
			finally
			{
				Monitor.Exit(syncObject);
			}
		}
		public IPluginDomainConnector GetDomain(string key)
		{
			if (this.domainContainer.ContainsKey(key))
			{
				return this.domainContainer[key];
			}
			return null;
		}
		public void Unload(string key)
		{
			object syncObject;
			Monitor.Enter(syncObject = PluginContainer._syncObject);
			try
			{
				if (this.domainContainer.ContainsKey(key))
				{
					foreach (KeyValuePair<string, IPluginConnector> current in this.domainContainer[key].Plugins)
					{
						if (this.pluginContainer.ContainsKey(current.Key))
						{
							this.pluginContainer.Remove(current.Key);
						}
					}
					this.domainContainer[key].Dispose();
					this.domainContainer.Remove(key);
				}
			}
			finally
			{
				Monitor.Exit(syncObject);
			}
		}
		public void UnloadAll()
		{
			object syncObject;
			Monitor.Enter(syncObject = PluginContainer._syncObject);
			try
			{
				foreach (string current in this.domainContainer.Keys)
				{
					this.domainContainer[current].Dispose();
				}
				this.domainContainer.Clear();
				this.pluginContainer.Clear();
			}
			finally
			{
				Monitor.Exit(syncObject);
			}
		}
	}
}
