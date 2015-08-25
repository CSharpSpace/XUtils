using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetObjectClient : NetBaseClient<NetObject>
	{
		public void Send(string name, object obj)
		{
			base.Send(new NetObject
			{
				Name = name,
				Object = obj
			});
		}
		protected override NetBaseStream<NetObject> CreateStream(NetworkStream ns, EndPoint ep)
		{
			return new NetObjectStream(ns, ep);
		}
	}
}
