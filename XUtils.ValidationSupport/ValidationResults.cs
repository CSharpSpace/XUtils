using System;
using XUtils.Messages;
namespace XUtils.ValidationSupport
{
	public class ValidationResults : Errors, IValidationResults, IErrors, IMessages
	{
		public static readonly ValidationResults Empty = new ValidationResults();
		public bool IsValid
		{
			get
			{
				return base.Count == 0;
			}
		}
	}
}
