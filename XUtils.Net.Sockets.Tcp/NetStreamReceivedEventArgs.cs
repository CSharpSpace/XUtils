using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetStreamReceivedEventArgs<T> : NetStreamEventArgs
	{
		public T Data
		{
			get;
			private set;
		}
		public NetStreamReceivedEventArgs(Guid guid, T data) : base(guid)
		{
			this.Data = data;
		}
	}
}
