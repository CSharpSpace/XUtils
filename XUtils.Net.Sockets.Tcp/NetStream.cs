using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStream : NetBaseStream<byte[]>
	{
		public NetStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
		}
		public override void Send(byte[] data)
		{
			base.SendRaw(data);
		}
		protected override void ReceivedRaw(byte[] bytes)
		{
			base.RaiseOnReceived(bytes);
		}
	}
}
