using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetDisconnectedEventArgs : EventArgs
	{
		public NetStoppedReason Reason
		{
			get;
			private set;
		}
		public NetDisconnectedEventArgs(NetStoppedReason reason)
		{
			this.Reason = reason;
		}
	}
}
