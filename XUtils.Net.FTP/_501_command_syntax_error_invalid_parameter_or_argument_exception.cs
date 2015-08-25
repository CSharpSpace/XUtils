using System;
namespace XUtils.Net.FTP
{
	public class _501_command_syntax_error_invalid_parameter_or_argument_exception : ServerResponseException
	{
		public _501_command_syntax_error_invalid_parameter_or_argument_exception(string message) : base(message)
		{
		}
	}
}
