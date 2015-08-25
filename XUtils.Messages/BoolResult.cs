using System;
using XUtils.ValidationSupport;
namespace XUtils.Messages
{
	public class BoolResult<T> : BoolMessageItem<T>
	{
		private IValidationResults _errors;
		public new static readonly BoolResult<T> False = new BoolResult<T>(default(T), false, string.Empty, ValidationResults.Empty);
		public new static readonly BoolResult<T> True = new BoolResult<T>(default(T), true, string.Empty, ValidationResults.Empty);
		public IValidationResults Errors
		{
			get
			{
				return this._errors;
			}
		}
		public BoolResult(T item, bool success, string message, IValidationResults errors) : base(item, success, message)
		{
			this._errors = errors;
		}
	}
}
