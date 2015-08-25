using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetPayloadStream : NetBasePayloadStream<byte[]>
	{
		public NetPayloadStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
		}
		public override void Send(byte[] data)
		{
			base.SendPayload(data);
		}
		protected override void ReceivedPayload(byte[] data)
		{
			base.RaiseOnReceived(data);
		}
	}
}
