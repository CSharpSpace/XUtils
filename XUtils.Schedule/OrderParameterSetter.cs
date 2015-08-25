using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public class OrderParameterSetter : IParameterSetter
	{
		private object[] _ParamList;
		private int _counter;
		public OrderParameterSetter(params object[] _Params)
		{
			this._ParamList = _Params;
		}
		public void reset()
		{
			this._counter = 0;
		}
		public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
		{
			if (this._counter >= this._ParamList.Length)
			{
				return false;
			}
			parameter = this._ParamList[this._counter++];
			return true;
		}
	}
}
