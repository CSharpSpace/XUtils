using System;
namespace XUtils.Net.FTP
{
	public class _421_service_not_available_exception : ServerResponseException
	{
		public _421_service_not_available_exception(string message) : base(message)
		{
		}
	}
}
