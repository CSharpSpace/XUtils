using System;
using System.Text.RegularExpressions;
namespace XUtils.Substitutions
{
	public class SubstitutionUtils
	{
		public static Substitution Parse(string funcCall, SubstitutionService subContainer)
		{
			Match match = Regex.Match(funcCall, "\\$\\{(?<name>[\\S]+)\\}");
			if (match == null || !match.Success)
			{
				return new Substitution(string.Empty, funcCall, false, null);
			}
			string value = match.Groups["name"].Value;
			int num = value.IndexOf(".");
			if (num < 0)
			{
				if (!subContainer._groups[""].ContainsKey(value))
				{
					return new Substitution(string.Empty, value, false, null);
				}
				return new Substitution(string.Empty, value, true, null);
			}
			else
			{
				string text = value.Substring(0, num);
				string text2 = value.Substring(num + 1);
				if (!subContainer._groups.ContainsKey(text) || !subContainer._groups[text].ContainsKey(text2))
				{
					return new Substitution(string.Empty, funcCall, false, null);
				}
				return new Substitution(text, text2, true, null);
			}
		}
		public static string Eval(Substitution sub, SubstitutionService subContainer)
		{
			if (!sub.IsValid)
			{
				return sub.FuncName;
			}
			return subContainer._groups[sub.Groupname][sub.FuncName](sub.FuncName);
		}
	}
}
