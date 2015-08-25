using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStreamStoppedEventArgs : NetStreamEventArgs
	{
		public NetStoppedReason Reason
		{
			get;
			private set;
		}
		public NetStreamStoppedEventArgs(Guid guid, NetStoppedReason reason) : base(guid)
		{
			this.Reason = reason;
		}
	}
}
