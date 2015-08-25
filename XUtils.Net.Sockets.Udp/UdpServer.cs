using System;
using System.Net;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpServer : UdpBaseServer
	{
		public event ReceivedHandler PacketReceived;
		public void Send(byte[] netObj, IPEndPoint remoteEP)
		{
			base.SendPacket(null, netObj, 0, netObj.Length, remoteEP);
		}
		protected override void OnUdpPacketReceived(UdpPacket packet)
		{
			if (this.PacketReceived != null)
			{
				this.PacketReceived(new UdpPacketEventArgs(this, packet.Socket, packet.RemoteEndPoint, packet.Data));
			}
		}
	}
}
