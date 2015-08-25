using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public class DelegateMethodCall : MethodCallBase, IMethodCall
	{
		private Exec _exec;
		private Delegate _f;
		public Delegate f
		{
			get
			{
				return this._f;
			}
			set
			{
				this._f = value;
			}
		}
		public MethodInfo Method
		{
			get
			{
				return this._f.Method;
			}
		}
		public DelegateMethodCall(Delegate f)
		{
			this._f = f;
		}
		public DelegateMethodCall(Delegate f, params object[] Params)
		{
			if (f.Method.GetParameters().Length < Params.Length)
			{
				throw new ArgumentException("Too many parameters specified for delegate", "f");
			}
			this._f = f;
			base.ParamList.Add(new OrderParameterSetter(Params));
		}
		public DelegateMethodCall(Delegate f, IParameterSetter Params)
		{
			this._f = f;
			base.ParamList.Add(Params);
		}
		public object Execute()
		{
			return this.f.DynamicInvoke(base.GetParameterList(this.Method));
		}
		public object Execute(IParameterSetter Params)
		{
			return this.f.DynamicInvoke(base.GetParameterList(this.Method, Params));
		}
		public void EventHandler(object obj, EventArgs e)
		{
			this.Execute();
		}
		public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
		{
			this._exec = new Exec(this.Execute);
			return this._exec.BeginInvoke(callback, obj);
		}
		public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
		{
			Exec2 exec = new Exec2(this.Execute);
			return exec.BeginInvoke(Params, callback, obj);
		}
	}
}
