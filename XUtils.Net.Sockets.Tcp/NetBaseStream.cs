using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace XUtils.Net.Sockets.Tcp
{
	public abstract class NetBaseStream<T>
	{
		protected Thread thread;
		protected NetworkStream stream;
		public event NetStreamStartedEventHandler OnStarted;
		public event NetStreamStoppedEventHandler OnStopped;
		public event NetStreamReceivedEventHandler<T> OnReceived;
		public Guid Guid
		{
			get;
			private set;
		}
		public long DataSent
		{
			get;
			private set;
		}
		public long DataReceived
		{
			get;
			private set;
		}
		public bool IsActive
		{
			get;
			private set;
		}
		public EndPoint EndPoint
		{
			get;
			private set;
		}
		public int TickRate
		{
			get;
			set;
		}
		public NetBaseStream(NetworkStream stream, EndPoint endpoint)
		{
			this.Guid = Guid.NewGuid();
			this.IsActive = false;
			this.EndPoint = endpoint;
			this.TickRate = 1;
			this.stream = stream;
		}
		public void Start()
		{
			this.IsActive = true;
			this.thread = new Thread(new ThreadStart(this.ThreadedReceive));
			this.thread.Start();
			if (this.OnStarted != null)
			{
				this.OnStarted(this, new NetStreamStartedEventArgs(this.Guid));
			}
		}
		public void Stop()
		{
			this.Stop(NetStoppedReason.Manually);
		}
		protected void Stop(NetStoppedReason reason)
		{
			if (!this.IsActive)
			{
				return;
			}
			this.IsActive = false;
			this.stream.Close();
			if (this.OnStopped != null)
			{
				this.OnStopped(this, new NetStreamStoppedEventArgs(this.Guid, reason));
			}
		}
		public abstract void Send(T data);
		protected void SendRaw(byte[] data)
		{
			if (this.IsActive && this.stream.CanWrite)
			{
				try
				{
					this.stream.Write(data, 0, data.Length);
					this.DataSent += data.LongLength;
				}
				catch (SocketException)
				{
					this.Stop(NetStoppedReason.Remote);
				}
			}
		}
		protected abstract void ReceivedRaw(byte[] bytes);
		protected void ThreadedReceive()
		{
			while (this.IsActive && this.stream.CanRead)
			{
				Thread.Sleep(this.TickRate);
				byte[] array = new byte[512];
				int num = 0;
				try
				{
					num = this.stream.Read(array, 0, array.Length);
					if (num == 0)
					{
						this.Stop(NetStoppedReason.Remote);
						return;
					}
					this.DataReceived += (long)num;
				}
				catch (IOException)
				{
					this.Stop(NetStoppedReason.Remote);
					return;
				}
				catch (Exception ex)
				{
					this.Stop(NetStoppedReason.Exception);
					throw ex;
				}
				if (num < 512)
				{
					byte[] array2 = new byte[num];
					Buffer.BlockCopy(array, 0, array2, 0, num);
					array = array2;
				}
				this.ReceivedRaw(array);
			}
			this.Stop(NetStoppedReason.Manually);
		}
		protected void RaiseOnReceived(T data)
		{
			if (this.OnReceived != null)
			{
				this.OnReceived(this, new NetStreamReceivedEventArgs<T>(this.Guid, data));
			}
		}
	}
}
