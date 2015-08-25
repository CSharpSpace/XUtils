using System;
namespace XUtils.Schedule
{
	public interface IMethodCall
	{
		ParameterSetterList ParamList
		{
			get;
		}
		object Execute();
		object Execute(IParameterSetter Params);
		void EventHandler(object obj, EventArgs e);
		IAsyncResult BeginExecute(AsyncCallback callback, object obj);
		IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj);
	}
}
