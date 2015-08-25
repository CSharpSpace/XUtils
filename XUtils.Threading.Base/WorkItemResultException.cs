using System;
using System.Runtime.Serialization;
namespace XUtils.Threading.Base
{
	[Serializable]
	public sealed class WorkItemResultException : Exception
	{
		public WorkItemResultException()
		{
		}
		public WorkItemResultException(string message) : base(message)
		{
		}
		public WorkItemResultException(string message, Exception e) : base(message, e)
		{
		}
		public WorkItemResultException(SerializationInfo si, StreamingContext sc) : base(si, sc)
		{
		}
	}
}
