using System;
using System.Net.Sockets;
using System.Threading;
namespace XUtils.Net.Sockets.Tcp
{
	public static class NetUtility
	{
		public static int ParsePort(string s)
		{
			return int.Parse(s);
		}
		public static bool TryParsePort(string s, out int port)
		{
			bool result;
			try
			{
				port = NetUtility.ParsePort(s);
				result = true;
			}
			catch
			{
				port = 0;
				result = false;
			}
			return result;
		}
		public static bool Ping(string host, int port)
		{
			return NetUtility.Ping(host, port, TimeSpan.MaxValue);
		}
		public static bool Ping(string host, int port, out TimeSpan elapsed)
		{
			return NetUtility.Ping(host, port, TimeSpan.MaxValue, out elapsed);
		}
		public static bool Ping(string host, int port, TimeSpan timeout)
		{
			TimeSpan timeSpan;
			return NetUtility.Ping(host, port, timeout, out timeSpan);
		}
		public static bool Ping(string host, int port, TimeSpan timeout, out TimeSpan elapsed)
		{
			bool result;
			using (TcpClient tcpClient = new TcpClient())
			{
				DateTime now = DateTime.Now;
				IAsyncResult asyncResult = tcpClient.BeginConnect(host, port, null, null);
				WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
				bool flag = true;
				try
				{
					if (!asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
					{
						tcpClient.Close();
						flag = false;
					}
					tcpClient.EndConnect(asyncResult);
				}
				catch
				{
					flag = false;
				}
				finally
				{
					asyncWaitHandle.Close();
				}
				elapsed = DateTime.Now.Subtract(now);
				result = flag;
			}
			return result;
		}
	}
}
