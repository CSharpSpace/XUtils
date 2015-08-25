using System;
using System.Net;
using System.Text;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpStringServer : UdpBaseServer
	{
		public event ReceivedStringHandler PacketReceived;
		public void Send(string data, IPEndPoint remoteEP)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			base.SendPacket(null, bytes, 0, bytes.Length, remoteEP);
		}
		protected override void OnUdpPacketReceived(UdpPacket packet)
		{
			if (this.PacketReceived != null)
			{
				string @string = Encoding.UTF8.GetString(packet.Data);
				this.PacketReceived(new UdpStringPacketEventArgs(this, packet.Socket, packet.RemoteEndPoint, @string));
			}
		}
	}
}
