using System;
namespace XUtils.Threading.Base.Internal
{
	internal interface IInternalWaitableResult
	{
		IWorkItemResult GetWorkItemResult();
	}
}
