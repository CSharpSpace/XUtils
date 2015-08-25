using System;
namespace XUtils.Threading.Base
{
	public delegate void PostExecuteWorkItemCallback(IWorkItemResult wir);
	public delegate void PostExecuteWorkItemCallback<TResult>(IWorkItemResult<TResult> wir);
}
