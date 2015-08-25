using System;
namespace XUtils
{
	public class RegexPatterns
	{
		public const string Number = "^[0-9]+$";
		public const string NumberSign = "^[+-]?[0-9]+$";
		public const string ZHCN = "[\\u4e00-\\u9fa5]";
		public const string Alpha = "^[a-zA-Z]*$";
		public const string AlphaUpperCase = "^[A-Z]*$";
		public const string AlphaLowerCase = "^[a-z]*$";
		public const string AlphaNumeric = "^[a-zA-Z0-9]*$";
		public const string AlphaNumericSpace = "^[a-zA-Z0-9 ]*$";
		public const string AlphaNumericSpaceDash = "^[a-zA-Z0-9 \\-]*$";
		public const string AlphaNumericSpaceDashUnderscore = "^[a-zA-Z0-9 \\-_]*$";
		public const string AlphaNumericSpaceDashUnderscorePeriod = "^[a-zA-Z0-9\\. \\-_]*$";
		public const string Numeric = "^\\-?[0-9]*\\.?[0-9]*$";
		public const string IdentityCard = "(^\\d{15}$)|(^\\d{18}$)|(\\d{17}(?:\\d|x|X)$)";
		public const string Email = "^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";
		public const string Url = "^^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_=]*)?$";
		public const string ZipCode = "^[1-9]\\d{5}$ ";
		public const string MobilePhone = "^((\\(\\d{3}\\))|(\\d{3}\\-))?13[0-9]\\d{8}|15[89]\\d{8}";
		public const string TelPhone = "^(0[0-9]{2,3}\\-)?([2-9][0-9]{6,7})+(\\-[0-9]{1,4})?$";
	}
}
