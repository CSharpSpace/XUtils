using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetPayloadClient : NetBaseClient<byte[]>
	{
		protected override NetBaseStream<byte[]> CreateStream(NetworkStream ns, EndPoint ep)
		{
			return new NetPayloadStream(ns, ep);
		}
	}
}
