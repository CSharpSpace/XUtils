using System;
using System.Text.RegularExpressions;
using XUtils.Messages;
namespace XUtils.ValidationSupport
{
	public static class Validation
	{
		public static string errorKey
		{
			get;
			set;
		}
		public static bool IsStringLengthMatch(string text, bool allowNull, bool checkMinLength, bool checkMaxLength, int minLength, int maxLength, IErrors errors, string errorKey)
		{
			if (string.IsNullOrEmpty(text) && allowNull)
			{
				return true;
			}
			int num = (text == null) ? 0 : text.Length;
			if (checkMinLength && minLength > 0 && num < minLength)
			{
				return Validation.CheckError(false, errors, errorKey, "文本提供的最小长度不小于(" + minLength + ")");
			}
			return !checkMaxLength || maxLength <= 0 || num <= maxLength || Validation.CheckError(false, errors, errorKey, "文本提供的最大长度不大于(" + maxLength + ")");
		}
		public static bool IsStringRegExMatch(string text, bool allowNull, string regEx, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, regEx, errors, errorKey, "文本不匹配");
		}
		public static bool IsStringIn(string text, bool allowNull, bool compareCase, string[] allowedValues, IErrors errors, string errorKey)
		{
			bool flag = string.IsNullOrEmpty(text);
			if (flag && allowNull)
			{
				return true;
			}
			if (flag && !allowNull)
			{
				string str = allowedValues.JoinDelimited(",", (string val) => val);
				errors.Add(errorKey, "文本必须包含 : " + str);
				return false;
			}
			bool flag2 = false;
			for (int i = 0; i < allowedValues.Length; i++)
			{
				string strB = allowedValues[i];
				if (string.Compare(text, strB, compareCase) == 0)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				string str2 = allowedValues.JoinDelimited(",", (string val) => val);
				errors.Add(errorKey, "文本必须包含 : " + str2);
				return false;
			}
			return true;
		}
		public static bool IsNumeric(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^\\-?[0-9]*\\.?[0-9]*$", errors, errorKey, "不是有效的数值类型");
		}
		public static bool IsNumericWithinRange(string text, bool checkMinBound, bool checkMaxBound, double min, double max, IErrors errors, string tag)
		{
			if (!Regex.IsMatch(text, "^\\-?[0-9]*\\.?[0-9]*$"))
			{
				errors.Add(tag, "不是有效的数值类型");
				return false;
			}
			double num = double.Parse(text);
			return Validation.IsNumericWithinRange(num, checkMinBound, checkMaxBound, min, max, errors, tag);
		}
		public static bool IsNumericWithinRange(double num, bool checkMinBound, bool checkMaxBound, double min, double max, IErrors errors, string errorKey)
		{
			if (checkMinBound && num < min)
			{
				errors.Add(errorKey, "数值小于 " + min + ".");
				return false;
			}
			if (checkMaxBound && num > max)
			{
				errors.Add(errorKey, "数值大于 " + max + ".");
				return false;
			}
			return true;
		}
		public static bool IsNumber(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^[0-9]+$", errors, errorKey, "不是有效的正整数");
		}
		public static bool IsNumberWithinRange(string text, bool checkMinBound, bool checkMaxBound, int min, int max, IErrors errors, string tag)
		{
			if (!Regex.IsMatch(text, "^[0-9]+$"))
			{
				errors.Add(tag, "不是有效的整数");
				return false;
			}
			int num = int.Parse(text);
			return Validation.IsNumberWithinRange(num, checkMinBound, checkMaxBound, min, max, errors, tag);
		}
		public static bool IsNumberSign(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^[+-]?[0-9]+$", errors, errorKey, "不是有效的整数");
		}
		public static bool IsNumberSignWithinRange(string text, bool checkMinBound, bool checkMaxBound, int min, int max, IErrors errors, string tag)
		{
			if (!Regex.IsMatch(text, "^[+-]?[0-9]+$"))
			{
				errors.Add(tag, "不是有效的整数");
				return false;
			}
			int num = int.Parse(text);
			return Validation.IsNumberWithinRange(num, checkMinBound, checkMaxBound, min, max, errors, tag);
		}
		public static bool IsNumberWithinRange(int num, bool checkMinBound, bool checkMaxBound, int min, int max, IErrors errors, string errorKey)
		{
			if (checkMinBound && num < min)
			{
				errors.Add(errorKey, "整数小于 " + min + ".");
				return false;
			}
			if (checkMaxBound && num > max)
			{
				errors.Add(errorKey, "整数大于 " + max + ".");
				return false;
			}
			return true;
		}
		public static bool IsZHCN(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "[\\u4e00-\\u9fa5]", errors, errorKey, "必须只包含中文 [\\u4e00-\\u9fa5]");
		}
		public static bool IsAlpha(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^[a-zA-Z]*$", errors, errorKey, "必须只包含大小写字母 ^[a-zA-Z]*$");
		}
		public static bool IsAlphaNumeric(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^[a-zA-Z0-9]*$", errors, errorKey, "必须只包含字母和数字字符 ^[a-zA-Z0-9]*$");
		}
		public static bool IsDate(string text, IErrors errors, string errorKey)
		{
			DateTime minValue = DateTime.MinValue;
			return Validation.CheckError(DateTime.TryParse(text, out minValue), errors, errorKey, "不是一个有效的日期");
		}
		public static bool IsDateWithinRange(string text, bool checkMinBound, bool checkMaxBound, DateTime minDate, DateTime maxDate, IErrors errors, string errorKey)
		{
			DateTime minValue = DateTime.MinValue;
			if (!DateTime.TryParse(text, out minValue))
			{
				errors.Add(errorKey, "不是一个有效的日期");
				return false;
			}
			return Validation.IsDateWithinRange(minValue, checkMinBound, checkMaxBound, minDate, maxDate, errors, errorKey);
		}
		public static bool IsDateWithinRange(DateTime date, bool checkMinBound, bool checkMaxBound, DateTime minDate, DateTime maxDate, IErrors errors, string errorKey)
		{
			if (checkMinBound && date.Date < minDate.Date)
			{
				errors.Add(errorKey, "日期不能小于： " + minDate.ToString());
				return false;
			}
			if (checkMaxBound && date.Date > maxDate.Date)
			{
				errors.Add(errorKey, "日期不能大于： " + maxDate.ToString());
				return false;
			}
			return true;
		}
		public static bool IsTimeOfDay(string time, IErrors errors, string errorKey)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			bool isValid = TimeSpan.TryParse(time, out minValue);
			return Validation.CheckError(isValid, errors, errorKey, "不是一个有效的时间.");
		}
		public static bool IsTimeOfDayWithinRange(string time, bool checkMinBound, bool checkMaxBound, TimeSpan min, TimeSpan max, IErrors errors, string errorKey)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			if (!TimeSpan.TryParse(time, out minValue))
			{
				return Validation.CheckError(false, errors, errorKey, "不是一个有效的时间.");
			}
			return Validation.IsTimeOfDayWithinRange(minValue, checkMinBound, checkMaxBound, min, max, errors, errorKey);
		}
		public static bool IsTimeOfDayWithinRange(TimeSpan time, bool checkMinBound, bool checkMaxBound, TimeSpan min, TimeSpan max, IErrors errors, string errorKey)
		{
			if (checkMinBound && time < min)
			{
				errors.Add(errorKey, "时间不能小于： " + min.ToString());
				return false;
			}
			if (checkMaxBound && time > max)
			{
				errors.Add(errorKey, "时间不能大于： " + max.ToString());
				return false;
			}
			return true;
		}
		public static bool IsMobilePhone(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^((\\(\\d{3}\\))|(\\d{3}\\-))?13[0-9]\\d{8}|15[89]\\d{8}", errors, errorKey, "不是一个有效的手机号码.");
		}
		public static bool IsMobilePhone(int phone, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(phone.ToString(), false, "^(0[0-9]{2,3}\\-)?([2-9][0-9]{6,7})+(\\-[0-9]{1,4})?$", errors, errorKey, "不是一个有效的手机号码.");
		}
		public static bool IsTelPhone(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^(0[0-9]{2,3}\\-)?([2-9][0-9]{6,7})+(\\-[0-9]{1,4})?$", errors, errorKey, "不是一个有效的电话号码.");
		}
		public static bool IsTelPhone(int phone, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(phone.ToString(), false, "^((\\(\\d{3}\\))|(\\d{3}\\-))?13[0-9]\\d{8}|15[89]\\d{8}", errors, errorKey, "不是一个有效的电话号码.");
		}
		public static bool IsIdentityCard(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "(^\\d{15}$)|(^\\d{18}$)|(\\d{17}(?:\\d|x|X)$)", errors, errorKey, "不是一个有效的身份证号码.");
		}
		public static bool IsIdentityCard(int identityCard, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(identityCard.ToString(), false, "(^\\d{15}$)|(^\\d{18}$)|(\\d{17}(?:\\d|x|X)$)", errors, errorKey, "不是一个有效的身份证号码.");
		}
		public static bool IsEmail(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", errors, errorKey, "不是一个有效的Email.");
		}
		public static bool IsUrl(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_=]*)?$", errors, errorKey, "不是一个有效的URL.");
		}
		public static bool IsZipCode(string text, bool allowNull, IErrors errors, string errorKey)
		{
			return Validation.CheckErrorRegEx(text, allowNull, "^[1-9]\\d{5}$ ", errors, errorKey, "不是一个有效的邮政编码.");
		}
		public static bool CheckError(bool isValid, IErrors errors, string errorKey, string error)
		{
			if (!isValid)
			{
				errors.Add(errorKey, error);
			}
			return isValid;
		}
		public static bool CheckErrorRegEx(string inputText, bool allowNull, string regExPattern, IErrors errors, string errorKey, string error)
		{
			bool flag = string.IsNullOrEmpty(inputText);
			if (allowNull && flag)
			{
				return true;
			}
			if (!allowNull && flag)
			{
				errors.Add(errorKey, error);
				return false;
			}
			bool flag2 = Regex.IsMatch(inputText, regExPattern);
			if (!flag2)
			{
				errors.Add(errorKey, error);
			}
			return flag2;
		}
		public static bool IsStringLengthMatch(string text, bool allowNull, bool checkMinLength, bool checkMaxLength, int minLength, int maxLength)
		{
			if (string.IsNullOrEmpty(text))
			{
				return allowNull;
			}
			return (!checkMinLength || minLength <= 0 || text.Length >= minLength) && (!checkMaxLength || maxLength <= 0 || text.Length <= maxLength);
		}
		public static bool IsBetween(int val, bool checkMinLength, bool checkMaxLength, int minLength, int maxLength)
		{
			return (!checkMinLength || val >= minLength) && (!checkMaxLength || val <= maxLength);
		}
		public static bool IsSizeBetween(int val, bool checkMinLength, bool checkMaxLength, int minKilobytes, int maxKilobytes)
		{
			val /= 1000;
			return (!checkMinLength || val >= minKilobytes) && (!checkMaxLength || val <= maxKilobytes);
		}
		public static bool IsStringRegExMatch(string text, bool allowNull, string regEx)
		{
			return (allowNull && string.IsNullOrEmpty(text)) || Regex.IsMatch(text, regEx);
		}
		public static bool IsNumeric(string text)
		{
			return Regex.IsMatch(text, "^\\-?[0-9]*\\.?[0-9]*$");
		}
		public static bool IsNumericWithinRange(string text, bool checkMinBound, bool checkMaxBound, double min, double max)
		{
			if (!Regex.IsMatch(text, "^\\-?[0-9]*\\.?[0-9]*$"))
			{
				return false;
			}
			double num = double.Parse(text);
			return Validation.IsNumericWithinRange(num, checkMinBound, checkMaxBound, min, max);
		}
		public static bool IsNumericWithinRange(double num, bool checkMinBound, bool checkMaxBound, double min, double max)
		{
			return (!checkMinBound || num >= min) && (!checkMaxBound || num <= max);
		}
		public static bool IsNumber(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^[0-9]+$");
		}
		public static bool IsNumberWithinRange(string text, bool checkMinBound, bool checkMaxBound, int min, int max)
		{
			if (!Regex.IsMatch(text, "^[0-9]+$"))
			{
				return false;
			}
			int num = int.Parse(text);
			return Validation.IsNumberWithinRange(num, checkMinBound, checkMaxBound, min, max);
		}
		public static bool IsNumberWithinRange(int num, bool checkMinBound, bool checkMaxBound, int min, int max)
		{
			return (!checkMinBound || num >= min) && (!checkMaxBound || num <= max);
		}
		public static bool IsNumberSign(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^[+-]?[0-9]+$");
		}
		public static bool IsNumberSignWithinRange(string text, bool checkMinBound, bool checkMaxBound, int min, int max)
		{
			if (!Regex.IsMatch(text, "^[+-]?[0-9]+$"))
			{
				return false;
			}
			int num = int.Parse(text);
			return Validation.IsNumberWithinRange(num, checkMinBound, checkMaxBound, min, max);
		}
		public static bool IsZHCN(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "[\\u4e00-\\u9fa5]");
		}
		public static bool IsAlpha(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^[a-zA-Z]*$");
		}
		public static bool IsAlphaNumeric(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^[a-zA-Z0-9]*$");
		}
		public static bool IsDate(string text)
		{
			DateTime minValue = DateTime.MinValue;
			return DateTime.TryParse(text, out minValue);
		}
		public static bool IsDateWithinRange(string text, bool checkMinBound, bool checkMaxBound, DateTime minDate, DateTime maxDate)
		{
			DateTime minValue = DateTime.MinValue;
			return DateTime.TryParse(text, out minValue) && Validation.IsDateWithinRange(minValue, checkMinBound, checkMaxBound, minDate, maxDate);
		}
		public static bool IsDateWithinRange(DateTime date, bool checkMinBound, bool checkMaxBound, DateTime minDate, DateTime maxDate)
		{
			return (!checkMinBound || !(date.Date < minDate.Date)) && (!checkMaxBound || !(date.Date > maxDate.Date));
		}
		public static bool IsTimeOfDay(string time)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			return TimeSpan.TryParse(time, out minValue);
		}
		public static bool IsTimeOfDayWithinRange(string time, bool checkMinBound, bool checkMaxBound, TimeSpan min, TimeSpan max)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			return TimeSpan.TryParse(time, out minValue) && Validation.IsTimeOfDayWithinRange(minValue, checkMinBound, checkMaxBound, min, max);
		}
		public static bool IsTimeOfDayWithinRange(TimeSpan time, bool checkMinBound, bool checkMaxBound, TimeSpan min, TimeSpan max)
		{
			return (!checkMinBound || !(time < min)) && (!checkMaxBound || !(time > max));
		}
		public static bool IsMobilePhone(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^((\\(\\d{3}\\))|(\\d{3}\\-))?13[0-9]\\d{8}|15[89]\\d{8}");
		}
		public static bool IsMobilePhone(int phone)
		{
			return Regex.IsMatch(phone.ToString(), "^((\\(\\d{3}\\))|(\\d{3}\\-))?13[0-9]\\d{8}|15[89]\\d{8}");
		}
		public static bool IsTelPhone(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^(0[0-9]{2,3}\\-)?([2-9][0-9]{6,7})+(\\-[0-9]{1,4})?$");
		}
		public static bool IsTelPhone(int phone)
		{
			return Regex.IsMatch(phone.ToString(), "^(0[0-9]{2,3}\\-)?([2-9][0-9]{6,7})+(\\-[0-9]{1,4})?$");
		}
		public static bool IsIdentityCard(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "(^\\d{15}$)|(^\\d{18}$)|(\\d{17}(?:\\d|x|X)$)");
		}
		public static bool IsIdentityCard(int identityCard)
		{
			return Regex.IsMatch(identityCard.ToString(), "(^\\d{15}$)|(^\\d{18}$)|(\\d{17}(?:\\d|x|X)$)");
		}
		public static bool IsEmail(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$");
		}
		public static bool IsUrl(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_=]*)?$");
		}
		public static bool IsZipCode(string text, bool allowNull)
		{
			return Validation.IsMatchRegEx(text, allowNull, "^[1-9]\\d{5}$ ");
		}
		public static bool AreEqual<T>(T obj1, T obj2) where T : IComparable<T>
		{
			return obj1.CompareTo(obj2) == 0;
		}
		public static bool AreNotEqual<T>(T obj1, T obj2) where T : IComparable<T>
		{
			return obj1.CompareTo(obj2) != 0;
		}
		public static bool IsMatchRegEx(string text, bool allowNull, string regExPattern)
		{
			bool flag = string.IsNullOrEmpty(text);
			return (flag && allowNull) || ((!flag || allowNull) && Regex.IsMatch(text, regExPattern));
		}
	}
}
