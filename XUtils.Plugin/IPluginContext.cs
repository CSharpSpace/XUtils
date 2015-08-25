using System;
namespace XUtils.Plugin
{
	public interface IPluginContext : ICloneable
	{
		string Key
		{
			get;
		}
		PluginState State
		{
			get;
		}
		void WriteLine(string log);
	}
}
