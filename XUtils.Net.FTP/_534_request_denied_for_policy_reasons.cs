using System;
namespace XUtils.Net.FTP
{
	public class _534_request_denied_for_policy_reasons : ServerResponseException
	{
		public _534_request_denied_for_policy_reasons(string message) : base(message)
		{
		}
	}
}
