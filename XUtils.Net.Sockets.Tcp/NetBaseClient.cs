using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public abstract class NetBaseClient<T>
	{
		protected TcpClient tcp;
		protected NetBaseStream<T> stream;
		public event NetReceivedEventHandler<T> OnReceived;
		public event NetConnectedEventHandler OnConnected;
		public event NetDisconnectedEventHandler OnDisconnected;
		public string RemoteHost
		{
			get;
			private set;
		}
		public int RemotePort
		{
			get;
			private set;
		}
		public bool IsConnected
		{
			get;
			private set;
		}
		public NetBaseClient()
		{
			this.IsConnected = false;
		}
		public void Connect(string host, int port)
		{
			if (this.IsConnected)
			{
				this.Disconnect(NetStoppedReason.Manually);
			}
			this.RemoteHost = host;
			this.RemotePort = port;
			this.tcp = new TcpClient();
			this.tcp.Connect(host, port);
			this.IsConnected = true;
			if (this.OnConnected != null)
			{
				this.OnConnected(this, new NetConnectedEventArgs());
			}
			NetworkStream ns = this.tcp.GetStream();
			EndPoint remoteEndPoint = this.tcp.Client.RemoteEndPoint;
			this.stream = this.CreateStream(ns, remoteEndPoint);
			this.stream.OnReceived += new NetStreamReceivedEventHandler<T>(this.stream_OnReceived);
			this.stream.OnStopped += new NetStreamStoppedEventHandler(this.stream_OnStopped);
			this.stream.Start();
		}
		public bool TryConnect(string host, int port)
		{
			bool result;
			try
			{
				this.Connect(host, port);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public void Disconnect()
		{
			this.Disconnect(NetStoppedReason.Manually);
		}
		protected void Disconnect(NetStoppedReason reason)
		{
			if (!this.IsConnected)
			{
				return;
			}
			this.stream.Stop();
			this.tcp.Close();
			this.IsConnected = false;
			if (this.OnDisconnected != null)
			{
				this.OnDisconnected(this, new NetDisconnectedEventArgs(reason));
			}
		}
		public void Send(T data)
		{
			this.stream.Send(data);
		}
		protected abstract NetBaseStream<T> CreateStream(NetworkStream ns, EndPoint ep);
		protected void stream_OnReceived(object sender, NetStreamReceivedEventArgs<T> e)
		{
			if (this.OnReceived != null)
			{
				this.OnReceived(this, new NetReceivedEventArgs<T>(e.Data));
			}
		}
		protected void stream_OnStopped(object sender, NetStreamStoppedEventArgs e)
		{
			this.Disconnect(e.Reason);
		}
	}
}
