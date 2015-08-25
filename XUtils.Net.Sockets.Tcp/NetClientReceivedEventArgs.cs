using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetClientReceivedEventArgs<T> : NetStreamEventArgs
	{
		public T Data
		{
			get;
			private set;
		}
		public NetEchoMode EchoMode
		{
			get;
			set;
		}
		public NetClientReceivedEventArgs(T data, NetEchoMode echoMode, Guid guid) : base(guid)
		{
			this.Data = data;
			this.EchoMode = echoMode;
		}
	}
}
