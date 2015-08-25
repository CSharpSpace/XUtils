using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetObjectServer : NetBaseServer<NetObject>
	{
		protected override NetBaseStream<NetObject> CreateStream(NetworkStream ns, EndPoint ep)
		{
			return new NetObjectStream(ns, ep);
		}
	}
}
