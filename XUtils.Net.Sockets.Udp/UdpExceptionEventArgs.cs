using System;
using System.Diagnostics;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpExceptionEventArgs
	{
		private Exception m_pException;
		private StackTrace m_pStackTrace;
		private string m_Text = "";
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}
		public StackTrace StackTrace
		{
			get
			{
				return this.m_pStackTrace;
			}
		}
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}
		public UdpExceptionEventArgs(Exception x, StackTrace stackTrace)
		{
			this.m_pException = x;
			this.m_pStackTrace = stackTrace;
		}
	}
}
