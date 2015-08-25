using System;
namespace XUtils.Net.FTP
{
	public class _553_file_name_not_allowed_exception : ServerResponseException
	{
		public _553_file_name_not_allowed_exception(string message) : base(message)
		{
		}
	}
}
