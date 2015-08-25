using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Udp
{
	public abstract class UdppacketEventArgs<T>
	{
		private Socket m_pSocket;
		private IPEndPoint m_pRemoteEP;
		private T m_pData = default(T);
		public IPEndPoint LocalEndPoint
		{
			get
			{
				return null;
			}
		}
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEP;
			}
		}
		public T Data
		{
			get
			{
				return this.m_pData;
			}
		}
		internal Socket Socket
		{
			get
			{
				return this.m_pSocket;
			}
		}
		public UdppacketEventArgs(Socket socket, IPEndPoint remoteEP, T data)
		{
			this.m_pSocket = socket;
			this.m_pRemoteEP = remoteEP;
			this.m_pData = data;
		}
	}
	public class UdpPacketEventArgs : UdppacketEventArgs<byte[]>
	{
		private UdpServer m_pUdpServer;
		public UdpServer UdpServer
		{
			get
			{
				return this.m_pUdpServer;
			}
		}
		internal UdpPacketEventArgs(UdpServer server, Socket socket, IPEndPoint remoteEP, byte[] data) : base(socket, remoteEP, data)
		{
			this.m_pUdpServer = server;
		}
		public void Send(byte[] data)
		{
			this.m_pUdpServer.Send(data, base.RemoteEndPoint);
		}
	}
}
