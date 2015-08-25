using System;
using System.Collections.Generic;
namespace XUtils.Substitutions
{
	public class Substitutor
	{
		private static SubstitutionService _provider;
		static Substitutor()
		{
			Substitutor._provider = new SubstitutionService();
		}
		public static void Substitute(List<string> names)
		{
			Substitutor._provider.Substitute(names);
		}
		public static string Substitute(string substitution)
		{
			return Substitutor._provider[substitution];
		}
		public static void Register(string group, IDictionary<string, Func<string, string>> interpretedVals)
		{
			Substitutor._provider.Register(group, interpretedVals);
		}
		public static void Register(string group, string replacement, Func<string, string> interpretor)
		{
			Substitutor._provider.Register(group, replacement, interpretor);
		}
	}
}
