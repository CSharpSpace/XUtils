using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetClientRejectedEventArgs : NetStreamEventArgs
	{
		public NetRejectedReason Reason
		{
			get;
			private set;
		}
		public NetClientRejectedEventArgs(Guid guid, NetRejectedReason reason) : base(guid)
		{
			this.Reason = reason;
		}
	}
}
