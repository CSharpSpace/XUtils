using System;
namespace XUtils.Plugin
{
	public interface IPluginActivator
	{
		void Start(IPluginContext context);
		void Execute(IPluginContext context);
		void Stop(IPluginContext context);
	}
}
