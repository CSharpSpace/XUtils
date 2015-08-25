using System;
namespace XUtils.Net.FTP
{
	public class _503_bad_sequence_of_commands_exception : ServerResponseException
	{
		public _503_bad_sequence_of_commands_exception(string message) : base(message)
		{
		}
	}
}
