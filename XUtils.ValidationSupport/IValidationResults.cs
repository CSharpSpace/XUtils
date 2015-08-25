using System;
using XUtils.Messages;
namespace XUtils.ValidationSupport
{
	public interface IValidationResults : IErrors, IMessages
	{
		bool IsValid
		{
			get;
		}
	}
}
