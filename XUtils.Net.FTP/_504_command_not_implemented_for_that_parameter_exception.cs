using System;
namespace XUtils.Net.FTP
{
	public class _504_command_not_implemented_for_that_parameter_exception : ServerResponseException
	{
		public _504_command_not_implemented_for_that_parameter_exception(string message) : base(message)
		{
		}
	}
}
