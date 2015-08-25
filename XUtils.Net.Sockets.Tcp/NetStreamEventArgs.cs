using System;
namespace XUtils.Net.Sockets.Tcp
{
	public abstract class NetStreamEventArgs : EventArgs
	{
		public Guid Guid
		{
			get;
			private set;
		}
		public NetStreamEventArgs(Guid guid)
		{
			this.Guid = guid;
		}
	}
}
