using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpPacket
	{
		private Socket m_pSocket;
		private IPEndPoint m_pRemoteEP;
		private byte[] m_pData;
		public Socket Socket
		{
			get
			{
				return this.m_pSocket;
			}
		}
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEP;
			}
		}
		public byte[] Data
		{
			get
			{
				return this.m_pData;
			}
		}
		public UdpPacket(Socket socket, IPEndPoint remoteEP, byte[] data)
		{
			this.m_pSocket = socket;
			this.m_pRemoteEP = remoteEP;
			this.m_pData = data;
		}
	}
}
