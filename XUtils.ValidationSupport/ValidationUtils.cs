using System;
using System.Collections.Generic;
using XUtils.Messages;
namespace XUtils.ValidationSupport
{
	public class ValidationUtils
	{
		public static bool Validate(bool isError, IList<string> errors, string message)
		{
			if (isError)
			{
				errors.Add(message);
			}
			return !isError;
		}
		public static bool Validate(bool isError, IErrors errors, string key, string message)
		{
			if (isError)
			{
				errors.Add(key, message);
			}
			return !isError;
		}
		public static bool Validate(bool isError, IErrors errors, string message)
		{
			if (isError)
			{
				errors.Add(string.Empty, message);
			}
			return !isError;
		}
		public static void TransferMessages(IList<string> messages, IErrors errors)
		{
			foreach (string current in messages)
			{
				errors.Add(string.Empty, current);
			}
		}
		public static bool Validate(IList<IValidator> validators, IValidationResults destinationResults)
		{
			if (validators == null || validators.Count == 0)
			{
				return true;
			}
			int count = destinationResults.Count;
			foreach (IValidator current in validators)
			{
				current.Validate(destinationResults);
			}
			return count == destinationResults.Count;
		}
		public static BoolMessage Validate(IValidator validator)
		{
			IValidationResults validationResults = validator.Validate();
			if (validationResults.IsValid)
			{
				return new BoolMessage(true, string.Empty);
			}
			string message = validationResults.Message();
			return new BoolMessage(false, message);
		}
		public static bool ValidateAndCollect(IValidator validator, IValidationResults results)
		{
			IValidationResults validationResults = validator.Validate(results);
			return validationResults.IsValid;
		}
	}
}
