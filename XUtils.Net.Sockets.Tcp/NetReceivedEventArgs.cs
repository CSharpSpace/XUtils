using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetReceivedEventArgs<T> : EventArgs
	{
		public T Data
		{
			get;
			private set;
		}
		public NetReceivedEventArgs(T data)
		{
			this.Data = data;
		}
	}
}
