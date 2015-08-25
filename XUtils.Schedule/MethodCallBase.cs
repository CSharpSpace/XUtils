using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public class MethodCallBase
	{
		private ParameterSetterList _ParamList = new ParameterSetterList();
		public ParameterSetterList ParamList
		{
			get
			{
				return this._ParamList;
			}
		}
		protected object[] GetParameterList(MethodInfo Method)
		{
			this.ParamList.reset();
			return this.ParamList.GetParameters(Method);
		}
		protected object[] GetParameterList(MethodInfo Method, IParameterSetter Params)
		{
			this.ParamList.reset();
			return this.ParamList.GetParameters(Method, Params);
		}
	}
}
