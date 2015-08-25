using System;
using System.Runtime.Serialization;
namespace XUtils.Threading.Base
{
	[Serializable]
	public sealed class WorkItemCancelException : Exception
	{
		public WorkItemCancelException()
		{
		}
		public WorkItemCancelException(string message) : base(message)
		{
		}
		public WorkItemCancelException(string message, Exception e) : base(message, e)
		{
		}
		public WorkItemCancelException(SerializationInfo si, StreamingContext sc) : base(si, sc)
		{
		}
	}
}
