using System;
namespace XUtils.ValidationSupport
{
	public class ValidationRuleDef
	{
		public string Name;
		public Func<ValidationEvent, bool> Rule;
	}
}
