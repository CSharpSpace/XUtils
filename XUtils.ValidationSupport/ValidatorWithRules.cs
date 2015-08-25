using System;
using System.Collections.Generic;
namespace XUtils.ValidationSupport
{
	public class ValidatorWithRules : Validator, IValidatorWithRules, IValidator, IValidatorStateful, IValidatorNonStateful
	{
		protected List<ValidationRuleDef> _rules;
		public Func<ValidationEvent, bool> this[int ndx]
		{
			get
			{
				if (ndx < 0 || ndx >= this._rules.Count)
				{
					return null;
				}
				return this._rules[ndx].Rule;
			}
		}
		public int Count
		{
			get
			{
				return this._rules.Count;
			}
		}
		public ValidatorWithRules() : this(null)
		{
		}
		public ValidatorWithRules(Func<ValidationEvent, bool> validator) : base(validator)
		{
			this._rules = new List<ValidationRuleDef>();
		}
		public void Add(Func<ValidationEvent, bool> rule)
		{
			this.Add(string.Empty, rule);
		}
		public void Add(string ruleName, Func<ValidationEvent, bool> rule)
		{
			ValidationRuleDef item = new ValidationRuleDef
			{
				Name = ruleName,
				Rule = rule
			};
			this._rules.Add(item);
		}
		public void RemoveAt(int ndx)
		{
			if (ndx < 0 || ndx >= this._rules.Count)
			{
				return;
			}
			this._rules.RemoveAt(ndx);
		}
		public void Remove(string name)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this._rules.Count; i++)
			{
				ValidationRuleDef validationRuleDef = this._rules[i];
				if (string.Compare(validationRuleDef.Name, name, true) == 0)
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				list.Reverse();
				foreach (int current in list)
				{
					this._rules.RemoveAt(current);
				}
			}
		}
		public override void Clear()
		{
			this._lastValidationResults = new ValidationResults();
			this._rules.Clear();
		}
		protected override bool ValidateInternal(ValidationEvent validationEvent)
		{
			if (this._validatorLamda != null)
			{
				return this._validatorLamda(validationEvent);
			}
			int count = validationEvent.Results.Count;
			foreach (ValidationRuleDef current in this._rules)
			{
				current.Rule(validationEvent);
			}
			return validationEvent.Results.Count == count;
		}
	}
}
