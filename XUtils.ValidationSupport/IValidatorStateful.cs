using System;
namespace XUtils.ValidationSupport
{
	public interface IValidatorStateful
	{
		object Target
		{
			get;
			set;
		}
		string Message
		{
			get;
		}
		bool IsValid
		{
			get;
		}
		IValidationResults Results
		{
			get;
		}
		IValidationResults Validate();
		IValidationResults Validate(IValidationResults results);
		void Clear();
	}
}
