using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStringStream : NetBaseStream<string>
	{
		public string Security
		{
			get;
			set;
		}
		public NetStringStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
		}
		public override void Send(string data)
		{
			base.SendRaw(Encoding.UTF8.GetBytes(data));
		}
		protected override void ReceivedRaw(byte[] bytes)
		{
			string @string = Encoding.UTF8.GetString(bytes);
			if (@string.Equals("<policy-file-request/>\0"))
			{
				this.Send(this.Security);
				base.Stop(NetStoppedReason.Remote);
				return;
			}
			base.RaiseOnReceived(@string);
		}
	}
}
