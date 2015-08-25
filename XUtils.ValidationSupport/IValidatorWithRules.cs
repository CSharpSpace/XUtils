using System;
namespace XUtils.ValidationSupport
{
	public interface IValidatorWithRules : IValidator, IValidatorStateful, IValidatorNonStateful
	{
		int Count
		{
			get;
		}
		Func<ValidationEvent, bool> this[int ndx]
		{
			get;
		}
		void Add(Func<ValidationEvent, bool> rule);
		void Add(string ruleName, Func<ValidationEvent, bool> rule);
		void RemoveAt(int ndx);
		void Remove(string ruleNname);
	}
}
