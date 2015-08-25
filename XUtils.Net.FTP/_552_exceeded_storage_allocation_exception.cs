using System;
namespace XUtils.Net.FTP
{
	public class _552_exceeded_storage_allocation_exception : ServerResponseException
	{
		public _552_exceeded_storage_allocation_exception(string message) : base(message)
		{
		}
	}
}
