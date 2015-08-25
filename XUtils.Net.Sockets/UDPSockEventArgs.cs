using System;
using System.Net;
namespace XUtils.Net.Sockets
{
	public class UDPSockEventArgs : EventArgs
	{
		private string m_strMsg;
		private string m_strUserName;
		private IPEndPoint m_EndPoint;
		public string RemoteUserName
		{
			get
			{
				return this.m_strUserName;
			}
			set
			{
				this.m_strUserName = value;
			}
		}
		public string SockMessage
		{
			get
			{
				return this.m_strMsg;
			}
			set
			{
				this.m_strMsg = value;
			}
		}
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_EndPoint;
			}
			set
			{
				this.m_EndPoint = value;
			}
		}
		public UDPSockEventArgs(string sMsg)
		{
			this.m_strMsg = sMsg;
		}
	}
}
