using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStreamStartedEventArgs : NetStreamEventArgs
	{
		public NetStreamStartedEventArgs(Guid guid) : base(guid)
		{
		}
	}
}
