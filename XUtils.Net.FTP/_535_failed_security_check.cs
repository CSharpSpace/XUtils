using System;
namespace XUtils.Net.FTP
{
	public class _535_failed_security_check : ServerResponseException
	{
		public _535_failed_security_check(string message) : base(message)
		{
		}
	}
}
