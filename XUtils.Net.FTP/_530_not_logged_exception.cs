using System;
namespace XUtils.Net.FTP
{
	public class _530_not_logged_exception : ServerResponseException
	{
		public _530_not_logged_exception(string message) : base(message)
		{
		}
	}
}
