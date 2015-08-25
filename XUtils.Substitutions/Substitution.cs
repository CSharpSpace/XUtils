using System;
namespace XUtils.Substitutions
{
	public class Substitution
	{
		public static readonly Substitution Empty = new Substitution(string.Empty, string.Empty, false, new string[0]);
		public string Groupname;
		public string FuncName;
		public string[] Args;
		public bool IsValid;
		public Substitution()
		{
		}
		public Substitution(string group, string func, bool isValid, string[] args)
		{
			this.Groupname = group;
			this.FuncName = func;
			this.Args = args;
			this.IsValid = isValid;
		}
	}
}
