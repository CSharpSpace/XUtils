using System;
namespace XUtils.ValidationSupport
{
	public class Validator : IValidator, IValidatorStateful, IValidatorNonStateful
	{
		public static readonly IValidator Empty = new Validator();
		protected string _message;
		protected object _target;
		protected IValidationResults _lastValidationResults;
		protected Func<ValidationEvent, bool> _validatorLamda;
		protected int _initialErrorCount;
		protected bool _creatValidationEvent;
		public virtual object Target
		{
			get
			{
				return this._target;
			}
			set
			{
				this._target = value;
			}
		}
		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				this._message = value;
			}
		}
		public bool IsValid
		{
			get
			{
				return this.Validate().IsValid;
			}
		}
		public IValidationResults Results
		{
			get
			{
				return this._lastValidationResults;
			}
		}
		public Validator()
		{
		}
		public Validator(Func<ValidationEvent, bool> validator)
		{
			this._validatorLamda = validator;
		}
		public virtual void Clear()
		{
			this._lastValidationResults = new ValidationResults();
		}
		public virtual IValidationResults Validate()
		{
			this._lastValidationResults = new ValidationResults();
			this.Validate(this.Target, this._lastValidationResults);
			return this._lastValidationResults;
		}
		public virtual IValidationResults ValidateTarget(object target)
		{
			this._lastValidationResults = new ValidationResults();
			this.Validate(new ValidationEvent(target, this._lastValidationResults));
			return this._lastValidationResults;
		}
		public virtual IValidationResults Validate(IValidationResults results)
		{
			this.Validate(new ValidationEvent(this.Target, results));
			return results;
		}
		public bool Validate(object target, IValidationResults results)
		{
			return this.Validate(new ValidationEvent(target, results));
		}
		public virtual bool Validate(ValidationEvent validationEvent)
		{
			return this.ValidateInternal(validationEvent);
		}
		protected virtual bool ValidateInternal(ValidationEvent validationEvent)
		{
			return this._validatorLamda == null || this._validatorLamda(validationEvent);
		}
		protected void AddResult(IValidationResults results, string key, string message)
		{
			results.Add(key, message);
		}
	}
}
