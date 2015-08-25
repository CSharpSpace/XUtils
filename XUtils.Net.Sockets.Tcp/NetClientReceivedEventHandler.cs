using System;
namespace XUtils.Net.Sockets.Tcp
{
	public delegate void NetClientReceivedEventHandler<T>(object sender, NetClientReceivedEventArgs<T> e);
}
