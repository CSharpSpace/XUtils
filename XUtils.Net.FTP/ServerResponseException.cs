using System;
namespace XUtils.Net.FTP
{
	public class ServerResponseException : Exception
	{
		private string message;
		public int Code
		{
			get
			{
				return int.Parse(this.message.Substring(0, 4));
			}
		}
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public ServerResponseException(string message)
		{
			this.message = message;
		}
		public override string ToString()
		{
			return this.message;
		}
	}
}
