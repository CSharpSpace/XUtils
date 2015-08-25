using System;
using System.Collections.Generic;
namespace XUtils.Substitutions
{
	public class SubstitutionService : ISubstitutionService
	{
		internal Dictionary<string, IDictionary<string, Func<string, string>>> _groups;
		public string this[string funcCall]
		{
			get
			{
				Substitution substitution = SubstitutionUtils.Parse(funcCall, this);
				if (!substitution.IsValid)
				{
					return funcCall;
				}
				return SubstitutionUtils.Eval(substitution, this);
			}
		}
		public SubstitutionService()
		{
			this._groups = new Dictionary<string, IDictionary<string, Func<string, string>>>();
			this.Init();
		}
		public void Substitute(List<string> names)
		{
			for (int i = 0; i < names.Count; i++)
			{
				string funcCall = names[i];
				string value = this[funcCall];
				names[i] = value;
			}
		}
		public void Register(string group, IDictionary<string, Func<string, string>> interpretedVals)
		{
			this._groups[group] = interpretedVals;
		}
		public void Register(string group, string replacement, Func<string, string> interpretor)
		{
			this._groups[group][replacement] = interpretor;
		}
		private void Init()
		{
			this._groups = new Dictionary<string, IDictionary<string, Func<string, string>>>();
			this._groups[""] = new Dictionary<string, Func<string, string>>();
			this._groups[""]["today"] = ((string s) => DateTime.Today.ToString());
			this._groups[""]["yesterday"] = ((string s) => DateTime.Today.AddDays(-1.0).ToString());
			this._groups[""]["tommorrow"] = ((string s) => DateTime.Today.AddDays(1.0).ToString());
			this._groups[""]["t"] = ((string s) => DateTime.Today.ToString());
			this._groups[""]["t-1"] = ((string s) => DateTime.Today.AddDays(-1.0).ToString());
			this._groups[""]["t+1"] = ((string s) => DateTime.Today.AddDays(1.0).ToString());
			this._groups[""]["today+1"] = ((string s) => DateTime.Today.AddDays(1.0).ToString());
			this._groups[""]["today-1"] = ((string s) => DateTime.Today.AddDays(-1.0).ToString());
			this._groups[""]["username"] = ((string s) => Environment.UserName);
			this._groups["env"] = new Dictionary<string, Func<string, string>>();
			this._groups["env"]["var"] = ((string name) => Environment.GetEnvironmentVariable(name));
			this._groups["enc"] = new Dictionary<string, Func<string, string>>();
			this._groups["enc"]["decode"] = ((string name) => name);
		}
	}
}
