using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetClientDisconnectedEventArgs : NetStreamStoppedEventArgs
	{
		public NetClientDisconnectedEventArgs(Guid guid, NetStoppedReason reason) : base(guid, reason)
		{
		}
	}
}
