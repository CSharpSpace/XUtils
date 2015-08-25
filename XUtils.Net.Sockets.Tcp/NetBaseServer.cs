using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace XUtils.Net.Sockets.Tcp
{
	public abstract class NetBaseServer<T>
	{
		protected List<Guid> clients;
		protected Dictionary<Guid, NetBaseStream<T>> streams;
		protected TcpListener tcp;
		protected Thread thread;
		protected bool active;
		public event NetStartedEventHandler OnStarted;
		public event NetStoppedEventHandler OnStopped;
		public event NetExceptionEventHandler OnError;
		public event NetClientConnectedEventHandler OnClientConnected;
		public event NetClientAcceptedEventHandler OnClientAccepted;
		public event NetClientRejectedEventHandler OnClientRejected;
		public event NetClientDisconnectedEventHandler OnClientDisconnected;
		public event NetClientReceivedEventHandler<T> OnReceived;
		public int MaxClients
		{
			get;
			private set;
		}
		public int Port
		{
			get;
			private set;
		}
		public bool IsOnline
		{
			get
			{
				return this.active;
			}
		}
		public int TickRate
		{
			get;
			set;
		}
		public IPAddress[] LocalAddresses
		{
			get
			{
				return Dns.GetHostAddresses(Dns.GetHostName());
			}
		}
		public IPAddress DefaultAddress
		{
			get
			{
				if (this.LocalAddresses.Length > 0)
				{
					return this.LocalAddresses[0];
				}
				return IPAddress.None;
			}
		}
		public IPAddress Address
		{
			get;
			private set;
		}
		public Guid[] Clients
		{
			get
			{
				return this.clients.ToArray();
			}
		}
		public int ClientCount
		{
			get
			{
				return this.clients.Count;
			}
		}
		public Dictionary<Guid, NetBaseStream<T>> ClientStreams
		{
			get
			{
				return this.streams;
			}
		}
		public NetEchoMode EchoMode
		{
			get;
			set;
		}
		public NetBaseServer()
		{
			this.clients = new List<Guid>();
			this.streams = new Dictionary<Guid, NetBaseStream<T>>();
			this.TickRate = 1;
			this.EchoMode = NetEchoMode.None;
		}
		public void Start(int port)
		{
			this.Start(port, 0);
		}
		public void Start(int port, int maxClients)
		{
			this.Start(this.DefaultAddress, port);
		}
		public void Start(IPAddress address, int port)
		{
			this.Start(address, port, 0);
		}
		public void Start(IPAddress address, int port, int maxClients)
		{
			if (this.active)
			{
				throw new Exception("Server already started.");
			}
			this.Address = address;
			this.Port = port;
			this.MaxClients = maxClients;
			this.active = true;
			this.tcp = new TcpListener(address, port);
			this.tcp.Start();
			if (this.OnStarted != null)
			{
				this.OnStarted(this, new NetStartedEventArgs());
			}
			this.thread = new Thread(new ThreadStart(this.ThreadedAccept));
			this.thread.Start();
		}
		public void Stop()
		{
			this.Stop(NetStoppedReason.Manually);
		}
		protected void Stop(NetStoppedReason reason)
		{
			if (!this.active)
			{
				return;
			}
			this.DisconnectAll();
			this.tcp.Stop();
			this.active = false;
			if (this.OnStopped != null)
			{
				this.OnStopped(this, new NetStoppedEventArgs(reason));
			}
		}
		public void DisconnectClient(Guid guid)
		{
			if (!this.streams.ContainsKey(guid))
			{
				throw new Exception("Client ID not found.");
			}
			this.streams[guid].Stop();
		}
		public void DisconnectAll()
		{
			while (this.clients.Count > 0)
			{
				this.streams[this.clients[0]].Stop();
			}
		}
		public void DispatchTo(Guid guid, T data)
		{
			if (!this.streams.ContainsKey(guid))
			{
				throw new Exception("Client ID not found.");
			}
			this.streams[guid].Send(data);
		}
		public void DispatchTo(Guid[] guid, T data)
		{
			for (int i = 0; i < guid.Length; i++)
			{
				Guid guid2 = guid[i];
				this.DispatchTo(guid2, data);
			}
		}
		public void DispatchAll(T data)
		{
			foreach (Guid current in this.clients)
			{
				this.DispatchTo(current, data);
			}
		}
		public void DispatchAllExcept(Guid guid, T data)
		{
			foreach (Guid current in this.clients)
			{
				if (current != guid)
				{
					this.DispatchTo(current, data);
				}
			}
		}
		protected void ThreadedAccept()
		{
			while (this.active)
			{
				Thread.Sleep(this.TickRate);
				TcpClient tcpClient = null;
				NetworkStream networkStream = null;
				try
				{
					tcpClient = this.tcp.AcceptTcpClient();
					networkStream = tcpClient.GetStream();
				}
				catch (SocketException ex)
				{
					if (this.OnError != null)
					{
						this.OnError(this, new NetExceptionEventArgs(ex));
					}
					if (networkStream != null)
					{
						networkStream.Close();
					}
					if (tcpClient != null)
					{
						tcpClient.Close();
					}
					continue;
				}
				NetBaseStream<T> netBaseStream = this.CreateStream(networkStream, tcpClient.Client.RemoteEndPoint);
				netBaseStream.OnStopped += new NetStreamStoppedEventHandler(this.OnClientStopped);
				netBaseStream.OnReceived += new NetStreamReceivedEventHandler<T>(this.OnClientReceived);
				netBaseStream.Start();
				this.clients.Add(netBaseStream.Guid);
				this.streams.Add(netBaseStream.Guid, netBaseStream);
				NetClientConnectedEventArgs netClientConnectedEventArgs = new NetClientConnectedEventArgs(netBaseStream.Guid, false);
				if (this.OnClientConnected != null)
				{
					this.OnClientConnected(this, netClientConnectedEventArgs);
				}
				if ((this.ClientCount < this.MaxClients || this.MaxClients == 0) && !netClientConnectedEventArgs.Reject)
				{
					if (this.OnClientAccepted != null)
					{
						this.OnClientAccepted(this, new NetClientAcceptedEventArgs(netBaseStream.Guid));
					}
				}
				else
				{
					if (this.OnClientRejected != null)
					{
						this.OnClientRejected(this, new NetClientRejectedEventArgs(netBaseStream.Guid, NetRejectedReason.Other));
					}
					networkStream.Close();
					tcpClient.Close();
				}
			}
			this.Stop(NetStoppedReason.Manually);
		}
		protected void OnClientReceived(object sender, NetStreamReceivedEventArgs<T> e)
		{
			NetClientReceivedEventArgs<T> netClientReceivedEventArgs = new NetClientReceivedEventArgs<T>(e.Data, this.EchoMode, e.Guid);
			if (this.OnReceived != null)
			{
				this.OnReceived(this, netClientReceivedEventArgs);
			}
			switch (netClientReceivedEventArgs.EchoMode)
			{
			case NetEchoMode.None:
				break;
			case NetEchoMode.EchoAll:
				this.DispatchAll(e.Data);
				return;
			case NetEchoMode.EchoAllExceptSender:
				this.DispatchAllExcept(e.Guid, e.Data);
				return;
			case NetEchoMode.EchoSender:
				this.DispatchTo(e.Guid, e.Data);
				break;
			default:
				return;
			}
		}
		protected void OnClientStopped(object sender, NetStreamStoppedEventArgs e)
		{
			if (this.OnClientDisconnected != null)
			{
				this.OnClientDisconnected(this, new NetClientDisconnectedEventArgs(e.Guid, e.Reason));
			}
			this.clients.Remove(e.Guid);
			this.streams.Remove(e.Guid);
		}
		protected abstract NetBaseStream<T> CreateStream(NetworkStream ns, EndPoint ep);
	}
}
