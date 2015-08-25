using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpNetObjectPacketEventArgs : UdppacketEventArgs<NetObject>
	{
		private UdpObjectServer m_pUdpServer;
		public UdpObjectServer UdpServer
		{
			get
			{
				return this.m_pUdpServer;
			}
		}
		internal UdpNetObjectPacketEventArgs(UdpObjectServer server, Socket socket, IPEndPoint remoteEP, NetObject data) : base(socket, remoteEP, data)
		{
			this.m_pUdpServer = server;
		}
		public void Send(NetObject data)
		{
			this.m_pUdpServer.Send(data, base.RemoteEndPoint);
		}
	}
}
