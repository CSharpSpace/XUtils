using System;
namespace XUtils.Threading.Base.Internal
{
	internal interface IInternalWorkItemResult
	{
		event WorkItemStateCallback OnWorkItemStarted;
		event WorkItemStateCallback OnWorkItemCompleted;
	}
}
