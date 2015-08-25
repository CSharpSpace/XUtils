using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace XUtils.Net.Sockets.Udp
{
	public abstract class UdpBaseServer
	{
		protected UdpProcessMode m_ProcessMode = UdpProcessMode.Sequential;
		protected int m_MTU = 14000;
		protected int m_MaxQueueSize = 200;
		protected IPEndPoint[] m_pBindings;
		protected DateTime m_StartTime;
		protected List<Socket> m_pSockets;
		protected CircleCollection<Socket> m_pSendSocketsIPv4;
		protected CircleCollection<Socket> m_pSendSocketsIPv6;
		protected Queue<UdpPacket> m_pQueuedPackets;
		protected long m_BytesReceived;
		protected long m_PacketsReceived;
		protected long m_BytesSent;
		protected long m_PacketsSent;
		protected bool m_IsRunning;
		protected bool m_IsDisposed;
		public event ErrorHandler Error;
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}
		public bool IsRunning
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_IsRunning;
			}
		}
		public UdpProcessMode ProcessMode
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_ProcessMode;
			}
			set
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (this.m_IsRunning)
				{
					throw new InvalidOperationException("ProcessMode value can be changed only if UDP server is not running.");
				}
				this.m_ProcessMode = value;
			}
		}
		public int MTU
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_MTU;
			}
			set
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (this.m_IsRunning)
				{
					throw new InvalidOperationException("MTU value can be changed only if UDP server is not running.");
				}
				this.m_MTU = value;
			}
		}
		public int MaxQueueSize
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_MaxQueueSize;
			}
		}
		public IPEndPoint[] Bindings
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_pBindings;
			}
			set
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				bool flag = false;
				if (this.m_pBindings == null)
				{
					flag = true;
				}
				else
				{
					if (this.m_pBindings.Length != value.Length)
					{
						flag = true;
					}
					else
					{
						for (int i = 0; i < this.m_pBindings.Length; i++)
						{
							if (!this.m_pBindings[i].Equals(value[i]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (flag)
				{
					this.m_pBindings = value;
					this.Restart();
				}
			}
		}
		public DateTime StartTime
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (!this.m_IsRunning)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_StartTime;
			}
		}
		public long BytesReceived
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (!this.m_IsRunning)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_BytesReceived;
			}
		}
		public long PacketsReceived
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (!this.m_IsRunning)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_PacketsReceived;
			}
		}
		public long BytesSent
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (!this.m_IsRunning)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_BytesSent;
			}
		}
		public long PacketsSent
		{
			get
			{
				if (this.m_IsDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				if (!this.m_IsRunning)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_PacketsSent;
			}
		}
		public UdpBaseServer()
		{
		}
		public void Dispose()
		{
			if (this.m_IsDisposed)
			{
				return;
			}
			this.m_IsDisposed = false;
			this.Stop();
			this.Error = null;
		}
		public void Start()
		{
			if (this.m_IsRunning)
			{
				return;
			}
			this.m_IsRunning = true;
			this.m_StartTime = DateTime.Now;
			this.m_pQueuedPackets = new Queue<UdpPacket>();
			if (this.m_pBindings != null)
			{
				List<IPEndPoint> list = new List<IPEndPoint>();
				IPEndPoint[] pBindings = this.m_pBindings;
				for (int i = 0; i < pBindings.Length; i++)
				{
					IPEndPoint iPEndPoint = pBindings[i];
					if (iPEndPoint.Address.Equals(IPAddress.Any))
					{
						IPEndPoint item = new IPEndPoint(IPAddress.Loopback, iPEndPoint.Port);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
						IPAddress[] hostAddresses = Dns.GetHostAddresses("");
						for (int j = 0; j < hostAddresses.Length; j++)
						{
							IPAddress address = hostAddresses[j];
							IPEndPoint item2 = new IPEndPoint(address, iPEndPoint.Port);
							if (!list.Contains(item2))
							{
								list.Add(item2);
							}
						}
					}
					else
					{
						if (!list.Contains(iPEndPoint))
						{
							list.Add(iPEndPoint);
						}
					}
				}
				this.m_pSockets = new List<Socket>();
				foreach (IPEndPoint current in list)
				{
					try
					{
						this.m_pSockets.Add(UdpCore.CreateSocket(current, ProtocolType.Udp));
					}
					catch (Exception x)
					{
						this.OnError(x);
					}
				}
				this.m_pSendSocketsIPv4 = new CircleCollection<Socket>();
				this.m_pSendSocketsIPv6 = new CircleCollection<Socket>();
				foreach (Socket current2 in this.m_pSockets)
				{
					if (((IPEndPoint)current2.LocalEndPoint).AddressFamily == AddressFamily.InterNetwork)
					{
						if (!((IPEndPoint)current2.LocalEndPoint).Equals(IPAddress.Loopback))
						{
							this.m_pSendSocketsIPv4.Add(current2);
						}
					}
					else
					{
						if (((IPEndPoint)current2.LocalEndPoint).AddressFamily == AddressFamily.InterNetworkV6)
						{
							this.m_pSendSocketsIPv6.Add(current2);
						}
					}
				}
				Thread thread = new Thread(new ThreadStart(this.ProcessIncomingUdp));
				thread.Start();
				Thread thread2 = new Thread(new ThreadStart(this.ProcessQueuedPackets));
				thread2.Start();
			}
		}
		public void Stop()
		{
			if (!this.m_IsRunning)
			{
				return;
			}
			this.m_IsRunning = false;
			this.m_pQueuedPackets = null;
			foreach (Socket current in this.m_pSockets)
			{
				current.Close();
			}
			this.m_pSockets = null;
			this.m_pSendSocketsIPv4 = null;
			this.m_pSendSocketsIPv6 = null;
		}
		public void Restart()
		{
			if (this.m_IsRunning)
			{
				this.Stop();
				this.Start();
			}
		}
		protected void SendPacket(Socket socket, byte[] packet, int offset, int count, IPEndPoint remoteEP)
		{
			if (socket == null)
			{
				if (remoteEP.AddressFamily == AddressFamily.InterNetwork)
				{
					if (this.m_pSendSocketsIPv4.Count == 0)
					{
						throw new ArgumentException("There is no suitable IPv4 local end point in this.Bindings.");
					}
					socket = this.m_pSendSocketsIPv4.Next();
				}
				else
				{
					if (remoteEP.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new ArgumentException("Invalid remote end point address family.");
					}
					if (this.m_pSendSocketsIPv6.Count == 0)
					{
						throw new ArgumentException("There is no suitable IPv6 local end point in this.Bindings.");
					}
					socket = this.m_pSendSocketsIPv6.Next();
				}
			}
			socket.SendTo(packet, 0, count, SocketFlags.None, remoteEP);
			this.m_BytesSent += (long)count;
			this.m_PacketsSent += 1L;
		}
		private void ProcessIncomingUdp()
		{
			CircleCollection<Socket> circleCollection = new CircleCollection<Socket>();
			foreach (Socket current in this.m_pSockets)
			{
				circleCollection.Add(current);
			}
			byte[] array = new byte[this.m_MTU];
			while (this.m_IsRunning)
			{
				try
				{
					if (this.m_pQueuedPackets.Count >= this.m_MaxQueueSize)
					{
						Thread.Sleep(1);
					}
					else
					{
						bool flag = false;
						for (int i = 0; i < circleCollection.Count; i++)
						{
							Socket socket = circleCollection.Next();
							if (socket.Poll(0, SelectMode.SelectRead))
							{
								EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
								int num = socket.ReceiveFrom(array, ref endPoint);
								this.m_BytesReceived += (long)num;
								this.m_PacketsReceived += 1L;
								byte[] array2 = new byte[num];
								Array.Copy(array, array2, num);
								Queue<UdpPacket> pQueuedPackets;
								Monitor.Enter(pQueuedPackets = this.m_pQueuedPackets);
								try
								{
									this.m_pQueuedPackets.Enqueue(new UdpPacket(socket, (IPEndPoint)endPoint, array2));
								}
								finally
								{
									Monitor.Exit(pQueuedPackets);
								}
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Thread.Sleep(1);
						}
					}
				}
				catch (Exception x)
				{
					this.OnError(x);
				}
			}
		}
		private void ProcessQueuedPackets()
		{
			while (this.m_IsRunning)
			{
				try
				{
					if (this.m_pQueuedPackets.Count == 0)
					{
						Thread.Sleep(1);
					}
					else
					{
						UdpPacket udpPacket = null;
						Queue<UdpPacket> pQueuedPackets;
						Monitor.Enter(pQueuedPackets = this.m_pQueuedPackets);
						try
						{
							udpPacket = this.m_pQueuedPackets.Dequeue();
						}
						finally
						{
							Monitor.Exit(pQueuedPackets);
						}
						if (this.m_ProcessMode == UdpProcessMode.Sequential)
						{
							this.OnUdpPacketReceived(udpPacket);
						}
						else
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessPacketOnTrPool), udpPacket);
						}
					}
				}
				catch (Exception x)
				{
					this.OnError(x);
				}
			}
		}
		private void ProcessPacketOnTrPool(object state)
		{
			try
			{
				this.OnUdpPacketReceived((UdpPacket)state);
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
		}
		protected abstract void OnUdpPacketReceived(UdpPacket packet);
		private void OnError(Exception x)
		{
			if (this.Error != null)
			{
				this.Error(this, new UdpExceptionEventArgs(x, new StackTrace()));
			}
		}
	}
}
