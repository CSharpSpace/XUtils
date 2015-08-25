using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public class DynamicMethodCall : MethodCallBase, IMethodCall
	{
		private Exec _exec;
		private object _obj;
		private MethodInfo _method;
		public MethodInfo Method
		{
			get
			{
				return this._method;
			}
			set
			{
				this._method = value;
			}
		}
		public DynamicMethodCall(MethodInfo method)
		{
			this._obj = null;
			this._method = method;
		}
		public DynamicMethodCall(object obj, MethodInfo method)
		{
			this._obj = obj;
			this._method = method;
		}
		public DynamicMethodCall(object obj, MethodInfo method, IParameterSetter setter)
		{
			this._obj = obj;
			this._method = method;
			base.ParamList.Add(setter);
		}
		public object Execute()
		{
			return this._method.Invoke(this._obj, base.GetParameterList(this.Method));
		}
		public object Execute(IParameterSetter Params)
		{
			return this._method.Invoke(this._obj, base.GetParameterList(this.Method, Params));
		}
		public void EventHandler(object obj, EventArgs e)
		{
			this.Execute();
		}
		public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
		{
			this._exec = new Exec(this.Execute);
			return this._exec.BeginInvoke(callback, null);
		}
		public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
		{
			Exec2 exec = new Exec2(this.Execute);
			return exec.BeginInvoke(Params, callback, null);
		}
	}
}
