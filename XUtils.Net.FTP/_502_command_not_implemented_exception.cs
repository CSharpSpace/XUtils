using System;
namespace XUtils.Net.FTP
{
	public class _502_command_not_implemented_exception : ServerResponseException
	{
		public _502_command_not_implemented_exception(string message) : base(message)
		{
		}
	}
}
