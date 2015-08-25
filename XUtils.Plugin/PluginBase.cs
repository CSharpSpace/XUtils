using System;
namespace XUtils.Plugin
{
	public abstract class PluginBase : MarshalByRefObject, IPlugin, IPluginActivator
	{
		public abstract void Start(IPluginContext context);
		public abstract void Execute(IPluginContext context);
		public abstract void Stop(IPluginContext context);
	}
}
