using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpStringPacketEventArgs : UdppacketEventArgs<string>
	{
		private UdpStringServer m_pUdpServer;
		public UdpStringServer UdpServer
		{
			get
			{
				return this.m_pUdpServer;
			}
		}
		internal UdpStringPacketEventArgs(UdpStringServer server, Socket socket, IPEndPoint remoteEP, string data) : base(socket, remoteEP, data)
		{
			this.m_pUdpServer = server;
		}
		public void Send(string data)
		{
			this.m_pUdpServer.Send(data, base.RemoteEndPoint);
		}
	}
}
