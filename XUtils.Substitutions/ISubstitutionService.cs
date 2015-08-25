using System;
using System.Collections.Generic;
namespace XUtils.Substitutions
{
	public interface ISubstitutionService
	{
		string this[string funcCall]
		{
			get;
		}
		void Register(string group, IDictionary<string, Func<string, string>> interpretedVals);
		void Substitute(List<string> names);
	}
}
