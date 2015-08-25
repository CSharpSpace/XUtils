using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStoppedEventArgs : EventArgs
	{
		public NetStoppedReason Reason
		{
			get;
			private set;
		}
		public NetStoppedEventArgs(NetStoppedReason reason)
		{
			this.Reason = reason;
		}
	}
}
