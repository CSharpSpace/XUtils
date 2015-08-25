using System;
namespace XUtils.Threading.Base
{
	public interface IWaitableResult
	{
		IWorkItemResult GetWorkItemResult();
		IWorkItemResult<TResult> GetWorkItemResultT<TResult>();
	}
}
