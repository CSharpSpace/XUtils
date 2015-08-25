using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace XUtils.Net.Sockets
{
	public class UDPP2PSock
	{
		private const int MAX_CREATE_TRY = 5536;
		private const int MAX_CONNECT_TRY = 10;
		private UdpClient m_udpServer;
		private UdpClient m_udpClient;
		private int m_iMyServerPort;
		private int m_iMyClientPort;
		private bool m_bServerCreated;
		private bool m_bClientCreated;
		private Thread m_serverThread;
		private Thread m_clientThread;
		private IPEndPoint m_remotePoint;
		private string m_strMyPublicEndPoint;
		private string m_strMyPrivateEndPoint;
		private StringBuilder m_sbResponse = new StringBuilder();
		private bool m_bRecvAck;
		private IPEndPoint m_requestPrivateEndPoint;
		private IPEndPoint m_requestPublicEndPoint;
		private ToEndPoint m_toEndPoint;
		public event UdpUserLogInDelegate OnUserLogInU;
		public event UdpMessageDelegate OnSockMessageU;
		public event UdpNewConnectDelegate OnNewConnectU;
		public string MyPublicEndPoint
		{
			get
			{
				return this.m_strMyPublicEndPoint;
			}
		}
		public string MyPrivateEndPoint
		{
			get
			{
				return this.m_strMyPrivateEndPoint;
			}
		}
		public UDPP2PSock()
		{
			this.m_iMyServerPort = 2280;
			this.m_iMyClientPort = 60000;
			this.m_bClientCreated = false;
			this.m_bServerCreated = false;
			this.m_toEndPoint = new ToEndPoint();
			this.m_serverThread = new Thread(new ThreadStart(this.RunUDPServer));
			this.m_clientThread = new Thread(new ThreadStart(this.RunUDPClient));
		}
		public void CreateUDPSever()
		{
			int num = 0;
			while (!this.m_bServerCreated && num < 5536)
			{
				try
				{
					this.m_udpServer = new UdpClient(this.m_iMyServerPort);
					this.m_bServerCreated = true;
				}
				catch
				{
					this.m_iMyServerPort++;
					num++;
				}
			}
			if (!this.m_bServerCreated && num == 5536)
			{
				throw new Exception("创建服务器尝试失败！");
			}
			this.m_serverThread.Start();
		}
		public void CreateUDPClient(string strServerIP, int iServerPort)
		{
			int num = 0;
			while (!this.m_bClientCreated && num < 5536)
			{
				try
				{
					this.m_udpClient = new UdpClient(this.m_iMyClientPort);
					this.m_bClientCreated = true;
					string str = Dns.GetHostAddresses("localhost")[0].ToString();
					this.m_strMyPrivateEndPoint = str + ":" + this.m_iMyClientPort.ToString();
				}
				catch
				{
					this.m_iMyClientPort++;
					num++;
				}
			}
			if (!this.m_bClientCreated && num == 5536)
			{
				throw new Exception("创建客户端尝试失败！");
			}
			IPEndPoint rEP = new IPEndPoint(IPAddress.Parse(strServerIP), iServerPort);
			string strLocalIP = Dns.GetHostAddresses("localhost")[0].ToString();
			this.SendLocalPoint(strLocalIP, this.m_iMyClientPort, rEP);
			this.m_clientThread.Start();
		}
		private void RunUDPServer()
		{
			while (true)
			{
				byte[] bytes = this.m_udpServer.Receive(ref this.m_remotePoint);
				this.m_sbResponse.Append(Encoding.Default.GetString(bytes));
				this.CheckCommand();
				Thread.Sleep(10);
			}
		}
		private void RunUDPClient()
		{
			while (true)
			{
				byte[] bytes = this.m_udpClient.Receive(ref this.m_remotePoint);
				this.m_sbResponse.Append(Encoding.Default.GetString(bytes));
				this.CheckCommand();
				Thread.Sleep(10);
			}
		}
		public void DisposeUDPServer()
		{
			this.m_serverThread.Abort();
			this.m_udpServer.Close();
		}
		public void DisposeUDPClient()
		{
			this.m_clientThread.Abort();
			this.m_udpClient.Close();
		}
		public void SendData(string strMsg, IPEndPoint REP)
		{
			byte[] bytes = Encoding.Default.GetBytes(strMsg.ToCharArray());
			this.m_udpClient.Send(bytes, bytes.Length, REP);
		}
		private void ServerSendData(string strMsg, IPEndPoint REP)
		{
			byte[] bytes = Encoding.Default.GetBytes(strMsg.ToCharArray());
			this.m_udpServer.Send(bytes, bytes.Length, REP);
		}
		public void SendLocalPoint(string strLocalIP, int iLocalPort, IPEndPoint REP)
		{
			string strMsg = string.Concat(new string[]
			{
				"\u0001\u0002",
				strLocalIP,
				":",
				iLocalPort.ToString(),
				"\u0002\u0001"
			});
			this.SendData(strMsg, REP);
		}
		public void StartBurrowTo(IPEndPoint pubEndPoint, IPEndPoint prEndPoint)
		{
			Thread thread = new Thread(new ThreadStart(this.BurrowProc));
			this.m_toEndPoint.m_privateEndPoint = prEndPoint;
			this.m_toEndPoint.m_publicEndPoint = pubEndPoint;
			thread.Start();
		}
		private void BurrowProc()
		{
			IPEndPoint privateEndPoint = this.m_toEndPoint.m_privateEndPoint;
			IPEndPoint publicEndPoint = this.m_toEndPoint.m_publicEndPoint;
			for (int i = 0; i < 10; i++)
			{
				this.SendData("\u0001\a\a\u0001", privateEndPoint);
				this.SendData("\u0001\a\a\u0001", publicEndPoint);
				for (int j = 0; j < 10; j++)
				{
					if (this.m_bRecvAck)
					{
						this.m_bRecvAck = false;
						this.SendData("\u0001\a\a\u0001", privateEndPoint);
						Thread.Sleep(50);
						this.SendData("\u0001\a\a\u0001", publicEndPoint);
						UDPSockEventArgs uDPSockEventArgs = new UDPSockEventArgs("");
						uDPSockEventArgs.RemoteEndPoint = publicEndPoint;
						if (this.OnNewConnectU != null)
						{
							this.OnNewConnectU(this, uDPSockEventArgs);
						}
						return;
					}
					Thread.Sleep(100);
				}
				Thread.Sleep(100);
			}
			throw new Exception("打洞失败！");
		}
		public void SendBurrowRequest(string strSrcPrEndpoint, string strSrcPubEndPoint, IPEndPoint REP)
		{
			string strMsg = string.Concat(new string[]
			{
				"\u0004\a",
				strSrcPrEndpoint,
				" ",
				strSrcPubEndPoint,
				"\a\u0004"
			});
			this.ServerSendData(strMsg, REP);
		}
		private void CheckCommand()
		{
			string text = this.m_sbResponse.ToString();
			int num;
			if ((num = text.IndexOf("\u0001\u0002")) > -1)
			{
				this.ReceiveName(text, num);
				string strMsg = "\u0003\a" + this.m_remotePoint.ToString() + "\a\u0003";
				this.SendData(strMsg, this.m_remotePoint);
				return;
			}
			if ((num = text.IndexOf("\u0003\a")) > -1)
			{
				this.ReceiveMyPublicEndPoint(text, num);
				return;
			}
			if ((num = text.IndexOf("\u0004\a")) > -1)
			{
				this.ReceiveAndSendAck(text, num);
				return;
			}
			if ((num = text.IndexOf("\u0001\a")) > -1)
			{
				this.m_bRecvAck = true;
				int num2 = text.IndexOf("\a\u0001");
				if (num2 > -1)
				{
					this.m_sbResponse.Remove(num, num2 - num + 2);
				}
				return;
			}
			this.m_sbResponse.Remove(0, text.Length);
			this.RaiseMessageEvent(text);
		}
		private void ReceiveName(string strCmd, int nPos)
		{
			int num = strCmd.IndexOf("\u0002\u0001");
			if (num == -1)
			{
				return;
			}
			this.m_sbResponse.Remove(nPos, num - nPos + 2);
			string remoteUserName = strCmd.Substring(nPos + 2, num - nPos - 2);
			UDPSockEventArgs uDPSockEventArgs = new UDPSockEventArgs("");
			uDPSockEventArgs.RemoteUserName = remoteUserName;
			uDPSockEventArgs.RemoteEndPoint = this.m_remotePoint;
			if (this.OnUserLogInU != null)
			{
				this.OnUserLogInU(this, uDPSockEventArgs);
			}
		}
		private void ReceiveAndSendAck(string strCmd, int nPos)
		{
			int num = strCmd.IndexOf("\a\u0004");
			if (num == -1)
			{
				return;
			}
			this.m_sbResponse.Remove(nPos, num - nPos + 2);
			string text = strCmd.Substring(nPos + 2, num - nPos - 2);
			string[] array = text.Split(new char[]
			{
				' '
			});
			string[] array2 = array[0].Split(new char[]
			{
				':'
			});
			string[] array3 = array[1].Split(new char[]
			{
				':'
			});
			this.m_requestPrivateEndPoint = new IPEndPoint(IPAddress.Parse(array2[0]), int.Parse(array2[1]));
			this.m_requestPublicEndPoint = new IPEndPoint(IPAddress.Parse(array3[0]), int.Parse(array3[1]));
			this.StartBurrowTo(this.m_requestPublicEndPoint, this.m_requestPrivateEndPoint);
		}
		private void ReceiveMyPublicEndPoint(string strCmd, int nPos)
		{
			int num = strCmd.IndexOf("\a\u0003");
			if (num == -1)
			{
				return;
			}
			this.m_sbResponse.Remove(nPos, num - nPos + 2);
			this.m_strMyPublicEndPoint = strCmd.Substring(nPos + 2, num - nPos - 2);
		}
		private void RaiseMessageEvent(string strMsg)
		{
			UDPSockEventArgs uDPSockEventArgs = new UDPSockEventArgs("");
			uDPSockEventArgs.SockMessage = strMsg;
			uDPSockEventArgs.RemoteEndPoint = this.m_remotePoint;
			if (this.OnSockMessageU != null)
			{
				this.OnSockMessageU(this, uDPSockEventArgs);
			}
		}
	}
}
