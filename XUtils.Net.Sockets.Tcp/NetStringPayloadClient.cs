using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStringPayloadClient : NetBaseClient<string>
	{
		protected override NetBaseStream<string> CreateStream(NetworkStream ns, EndPoint ep)
		{
			return new NetStringPayloadStream(ns, ep);
		}
	}
}
