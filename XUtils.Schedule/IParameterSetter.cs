using System;
using System.Reflection;
namespace XUtils.Schedule
{
	public interface IParameterSetter
	{
		void reset();
		bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter);
	}
}
