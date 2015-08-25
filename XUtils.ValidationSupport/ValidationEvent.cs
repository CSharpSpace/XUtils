using System;
namespace XUtils.ValidationSupport
{
	public class ValidationEvent
	{
		public object Target;
		public IValidationResults Results;
		public object Context;
		public T TargetT<T>()
		{
			if (this.Target == null)
			{
				return default(T);
			}
			return (T)((object)this.Target);
		}
		public ValidationEvent(object target, IValidationResults results, object context)
		{
			this.Target = target;
			this.Results = results;
			this.Context = context;
		}
		public ValidationEvent(object target, IValidationResults results)
		{
			this.Target = target;
			this.Results = results;
		}
	}
}
