using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStringPayloadStream : NetBasePayloadStream<string>
	{
		public NetStringPayloadStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
		}
		public override void Send(string data)
		{
			base.SendPayload(Encoding.Default.GetBytes(data));
		}
		protected override void ReceivedPayload(byte[] data)
		{
			base.RaiseOnReceived(Encoding.Default.GetString(data));
		}
	}
}
