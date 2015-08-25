using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Udp
{
	public static class UdpCore
	{
		public static Socket CreateSocket(IPEndPoint localEP, ProtocolType protocolType)
		{
			SocketType socketType = SocketType.Dgram;
			if (protocolType == ProtocolType.Udp)
			{
				socketType = SocketType.Dgram;
			}
			if (localEP.AddressFamily == AddressFamily.InterNetwork)
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
				socket.Bind(localEP);
				return socket;
			}
			if (localEP.AddressFamily == AddressFamily.InterNetworkV6)
			{
				Socket socket2 = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
				socket2.Bind(localEP);
				return socket2;
			}
			throw new ArgumentException("Invalid IPEndPoint address family.");
		}
	}
}
