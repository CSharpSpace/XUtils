using System;
namespace XUtils.Net.FTP
{
	public class _550_file_unavailable_not_found_no_access_exception : ServerResponseException
	{
		public _550_file_unavailable_not_found_no_access_exception(string message) : base(message)
		{
		}
	}
}
