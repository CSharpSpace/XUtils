using System;
using System.Collections;
using System.Reflection;
namespace XUtils.Schedule
{
	public class NamedParameterSetter : IParameterSetter
	{
		private Hashtable _Params;
		public NamedParameterSetter(Hashtable Params)
		{
			this._Params = Params;
		}
		public void reset()
		{
		}
		public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
		{
			string name = pi.Name;
			if (!this._Params.ContainsKey(name))
			{
				return false;
			}
			parameter = this._Params[name];
			return true;
		}
	}
}
