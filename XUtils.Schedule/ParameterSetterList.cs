using System;
using System.Collections.Generic;
using System.Reflection;
namespace XUtils.Schedule
{
	public class ParameterSetterList
	{
		private List<IParameterSetter> setters = new List<IParameterSetter>();
		public void Add(IParameterSetter setter)
		{
			this.setters.Add(setter);
		}
		public IParameterSetter[] ToArray()
		{
			return this.setters.ToArray();
		}
		public void reset()
		{
			foreach (IParameterSetter current in this.setters)
			{
				current.reset();
			}
		}
		public object[] GetParameters(MethodInfo Method)
		{
			ParameterInfo[] parameters = Method.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				this.SetValue(parameters[i], i, ref array[i]);
			}
			return array;
		}
		public object[] GetParameters(MethodInfo Method, IParameterSetter LastSetter)
		{
			ParameterInfo[] parameters = Method.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!this.SetValue(parameters[i], i, ref array[i]))
				{
					LastSetter.GetParameterValue(parameters[i], i, ref array[i]);
				}
			}
			return array;
		}
		private bool SetValue(ParameterInfo Info, int i, ref object Value)
		{
			foreach (IParameterSetter current in this.setters)
			{
				if (current.GetParameterValue(Info, i, ref Value))
				{
					return true;
				}
			}
			return false;
		}
	}
}
