using System;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetExceptionEventArgs : EventArgs
	{
		public Exception Exception
		{
			get;
			private set;
		}
		public NetExceptionEventArgs(Exception ex)
		{
			this.Exception = ex;
		}
	}
}
