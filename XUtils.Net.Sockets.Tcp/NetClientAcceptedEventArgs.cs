using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetClientAcceptedEventArgs : NetStreamEventArgs
	{
		public NetClientAcceptedEventArgs(Guid guid) : base(guid)
		{
		}
	}
}
