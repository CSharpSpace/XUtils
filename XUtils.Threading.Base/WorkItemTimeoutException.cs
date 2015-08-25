using System;
using System.Runtime.Serialization;
namespace XUtils.Threading.Base
{
	[Serializable]
	public sealed class WorkItemTimeoutException : Exception
	{
		public WorkItemTimeoutException()
		{
		}
		public WorkItemTimeoutException(string message) : base(message)
		{
		}
		public WorkItemTimeoutException(string message, Exception e) : base(message, e)
		{
		}
		public WorkItemTimeoutException(SerializationInfo si, StreamingContext sc) : base(si, sc)
		{
		}
	}
}
