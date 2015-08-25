using System;
namespace XUtils.Plugin
{
	[Serializable]
	public class PluginContext : IPluginContext, ICloneable
	{
		private string key = string.Empty;
		private PluginState state = PluginState.Closed;
		private Action<string, string> logger;
		public string Key
		{
			get
			{
				return this.key;
			}
		}
		public PluginState State
		{
			get
			{
				return this.state;
			}
		}
		public PluginContext(string key, PluginState state, Action<string, string> logger)
		{
			this.key = key;
			this.state = state;
			this.logger = logger;
		}
		public void WriteLine(string log)
		{
			if (this.logger != null)
			{
				this.logger(this.key, log);
			}
		}
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
