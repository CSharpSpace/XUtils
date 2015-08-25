using System;
namespace XUtils.Plugin
{
	public interface IPluginConnector : IDisposable
	{
		string Name
		{
			get;
		}
		T Create<T>();
	}
}
