using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetClientConnectedEventArgs : NetStreamEventArgs
	{
		public bool Reject
		{
			get;
			set;
		}
		public NetClientConnectedEventArgs(Guid guid, bool reject) : base(guid)
		{
			this.Reject = reject;
		}
	}
}
