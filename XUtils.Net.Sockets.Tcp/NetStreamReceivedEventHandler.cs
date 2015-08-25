using System;
namespace XUtils.Net.Sockets.Tcp
{
	public delegate void NetStreamReceivedEventHandler<T>(object sender, NetStreamReceivedEventArgs<T> e);
}
