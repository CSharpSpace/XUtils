using System;
namespace XUtils.Net.FTP
{
	public class _450_file_unavailable_busy_exception : ServerResponseException
	{
		public _450_file_unavailable_busy_exception(string message) : base(message)
		{
		}
	}
}
