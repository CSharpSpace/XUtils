using System;
namespace XUtils.Net.FTP
{
	public class _522_protocol_not_supported : ServerResponseException
	{
		public _522_protocol_not_supported(string message) : base(message)
		{
		}
	}
}
