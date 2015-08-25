using System;
namespace XUtils.Net.Sockets.Tcp
{
	public delegate void NetReceivedEventHandler<T>(object sender, NetReceivedEventArgs<T> e);
}
