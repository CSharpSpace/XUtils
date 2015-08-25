using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
namespace XUtils.ValidationSupport
{
	public class ValidatorFluent
	{
		private object _target;
		private string _objectName;
		private bool _appendObjectNameToError;
		private string _propertyName;
		private bool _checkCondition;
		private IValidationResults _errors;
		private int _initialErrorCount;
		public bool HasErrors
		{
			get
			{
				return this._errors.Count > this._initialErrorCount;
			}
		}
		public IValidationResults Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				this._errors = value;
			}
		}
		public ValidatorFluent(Type typeToCheck) : this(typeToCheck, false, null)
		{
		}
		public ValidatorFluent(Type typeToCheck, IValidationResults errors) : this(typeToCheck, false, errors)
		{
		}
		public ValidatorFluent(Type typeToCheck, bool appendTypeToError, IValidationResults errors)
		{
			this._objectName = typeToCheck.Name;
			this._appendObjectNameToError = appendTypeToError;
			this._errors = ((errors == null) ? new ValidationResults() : errors);
			this._initialErrorCount = this._errors.Count;
		}
		public ValidatorFluent Check(object target)
		{
			this._checkCondition = true;
			this._propertyName = string.Empty;
			this._target = target;
			return this;
		}
		public ValidatorFluent Check(Expression<Func<object>> exp)
		{
			this._target = ExpressionHelper.GetPropertyNameAndValue(exp, ref this._propertyName);
			this._checkCondition = true;
			return this;
		}
		public ValidatorFluent Check(string propName, object target)
		{
			this._checkCondition = true;
			this._propertyName = propName;
			this._target = target;
			return this;
		}
		public ValidatorFluent If(bool isOkToCheckNext)
		{
			this._checkCondition = isOkToCheckNext;
			return this;
		}
		public ValidatorFluent Is(object val)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (this._target == null && val == null)
			{
				return this;
			}
			if (this._target == null && val != null)
			{
				return this.IsValid(false, "必须等于 : " + val.ToString());
			}
			return this.IsValid(val.Equals(this._target), "必须等于 : " + val.ToString());
		}
		public ValidatorFluent IsNot(object val)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (this._target == null && val == null)
			{
				return this;
			}
			if (this._target == null && val != null)
			{
				return this;
			}
			return this.IsValid(!val.Equals(this._target), "必须不等于 : " + val.ToString());
		}
		public ValidatorFluent IsNull()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(this._target == null, "必须为空");
		}
		public ValidatorFluent IsNotNull()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(this._target != null, "不能为空");
		}
		public ValidatorFluent In<T>(params object[] vals)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (vals == null || vals.Length == 0)
			{
				return this;
			}
			T t = TypeParsers.ConvertTo<T>(this._target);
			bool isValid = false;
			for (int i = 0; i < vals.Length; i++)
			{
				object input = vals[i];
				T t2 = TypeParsers.ConvertTo<T>(input);
				if (t.Equals(t2))
				{
					isValid = true;
					break;
				}
			}
			return this.IsValid(isValid, "不是一个有效值");
		}
		public ValidatorFluent NotIn<T>(params object[] vals)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (vals == null || vals.Length == 0)
			{
				return this;
			}
			T t = TypeParsers.ConvertTo<T>(this._target);
			bool isValid = true;
			for (int i = 0; i < vals.Length; i++)
			{
				object input = vals[i];
				T t2 = TypeParsers.ConvertTo<T>(input);
				if (t.Equals(t2))
				{
					isValid = false;
					break;
				}
			}
			return this.IsValid(isValid, "不是一个有效值");
		}
		public ValidatorFluent Matches(string regex)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Regex.IsMatch((string)this._target, regex), "不匹配 : " + regex);
		}
		public ValidatorFluent IsBetween(int min, int max)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			bool flag = TypeHelper.IsNumeric(this._target ?? string.Empty);
			if (flag)
			{
				double num = Convert.ToDouble(this._target);
				return this.IsValid((double)min <= num && num <= (double)max, string.Concat(new object[]
				{
					"必须介于 : ",
					min,
					", ",
					max
				}));
			}
			string text = this._target as string;
			if (min > 0 && string.IsNullOrEmpty(text))
			{
				return this.IsValid(false, string.Concat(new object[]
				{
					"长度必须介于 : ",
					min,
					", ",
					max
				}));
			}
			return this.IsValid(min <= text.Length && text.Length <= max, string.Concat(new object[]
			{
				"长度必须介于 : ",
				min,
				", ",
				max
			}));
		}
		public ValidatorFluent Contains(string val)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (string.IsNullOrEmpty((string)this._target))
			{
				return this.IsValid(false, "不包含 : " + val);
			}
			string text = (string)this._target;
			return this.IsValid(text.Contains(val), "必须包含 : " + val);
		}
		public ValidatorFluent NotContain(string val)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (string.IsNullOrEmpty((string)this._target))
			{
				return this;
			}
			string text = (string)this._target;
			return this.IsValid(!text.Contains(val), "不应该包含 : " + val);
		}
		public ValidatorFluent Min(int min)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (!TypeHelper.IsNumeric(this._target))
			{
				return this.IsValid(false, "必须为数字值");
			}
			double num = Convert.ToDouble(this._target);
			return this.IsValid(num >= (double)min, "必须大于最小值 : " + min);
		}
		public ValidatorFluent Max(int max)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (!TypeHelper.IsNumeric(this._target))
			{
				return this.IsValid(false, "必须为数字值");
			}
			double num = Convert.ToDouble(this._target);
			return this.IsValid(num <= (double)max, "必须小于最大值 : " + max);
		}
		public ValidatorFluent IsTrue()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (!(this._target is bool))
			{
				return this.IsValid(false, "必须是布尔值");
			}
			return this.IsValid((bool)this._target, "必须为 true");
		}
		public ValidatorFluent IsFalse()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			if (!(this._target is bool))
			{
				return this.IsValid(false, "必须是布尔值");
			}
			return this.IsValid(!(bool)this._target, "必须为 false");
		}
		public ValidatorFluent IsAfterToday()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			this.IsAfter(DateTime.Today);
			this._checkCondition = false;
			return this;
		}
		public ValidatorFluent IsBeforeToday()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			this.IsBefore(DateTime.Today);
			this._checkCondition = false;
			return this;
		}
		public ValidatorFluent IsAfter(DateTime date)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(((DateTime)this._target).Date.CompareTo(date.Date) > 0, "必须大于日期 : " + date.ToString());
		}
		public ValidatorFluent IsBefore(DateTime date)
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(((DateTime)this._target).Date.CompareTo(date.Date) < 0, "必须小于日期 : " + date.ToString());
		}
		public ValidatorFluent IsValidEmail()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsEmail((string)this._target, false), "必须是一个有效的Email");
		}
		public ValidatorFluent IsValidMobilePhone()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsMobilePhone((string)this._target, false), "必须是一个有效的手机号码");
		}
		public ValidatorFluent IsValidTelPhone()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsTelPhone((string)this._target, false), "必须是一个有效的电话号码");
		}
		public ValidatorFluent IsValidIdentityCard()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsIdentityCard((string)this._target, false), "必须是一个有效的身份证号码");
		}
		public ValidatorFluent IsValidUrl()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsUrl((string)this._target, false), "必须是一个有效的URL");
		}
		public ValidatorFluent IsValidZip()
		{
			if (!this._checkCondition)
			{
				return this;
			}
			return this.IsValid(Validation.IsZipCode((string)this._target, false), "必须是一个有效的邮编");
		}
		public ValidatorFluent End()
		{
			return this;
		}
		private ValidatorFluent IsValid(bool isValid, string error)
		{
			if (!isValid)
			{
				string str = string.IsNullOrEmpty(this._propertyName) ? "Property " : (this._propertyName + " ");
				this._errors.Add(str + error);
			}
			return this;
		}
	}
}
