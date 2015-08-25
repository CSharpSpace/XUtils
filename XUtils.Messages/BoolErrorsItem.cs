using System;
using XUtils.ValidationSupport;
namespace XUtils.Messages
{
	public class BoolErrorsItem : BoolMessageItem
	{
		protected IErrors _errors;
		public IErrors Errors
		{
			get
			{
				return this._errors;
			}
		}
		public BoolErrorsItem(object item, bool success, string message, IErrors errors) : base(item, success, message)
		{
			this._errors = errors;
		}
	}
	public class BoolErrorsItem<T> : BoolErrorsItem
	{
		public new T Item
		{
			get
			{
				return (T)((object)base.Item);
			}
		}
		public BoolErrorsItem(T item, bool success, string message, IValidationResults errors) : base(item, success, message, errors)
		{
			this._errors = errors;
		}
	}
}
