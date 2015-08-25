using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace XUtils
{
	public class DateParser
	{
		public const string ErrorStartDateGreaterThanEnd = "End date must be greater or equal to start date.";
		public static DateParseResult ParseDateRange(string val, IList<string> errors, string delimiter)
		{
			int num = val.IndexOf(delimiter);
			string text = val.Substring(0, num);
			string text2 = val.Substring(num + delimiter.Length);
			DateTime today = DateTime.Today;
			DateTime today2 = DateTime.Today;
			int count = errors.Count;
			if (string.IsNullOrEmpty(text))
			{
				errors.Add("Start date not supplied.");
			}
			if (string.IsNullOrEmpty(text2))
			{
				errors.Add("End date not supplied.");
			}
			if (errors.Count > count)
			{
				return new DateParseResult(false, errors[0], TimeParserConstants.MinDate, TimeParserConstants.MaxDate);
			}
			if (!DateTime.TryParse(text, out today2))
			{
				errors.Add("Start date '" + text + "' is not valid.");
			}
			if (!DateTime.TryParse(text2, out today))
			{
				errors.Add("End date '" + text2 + "' is not valid.");
			}
			if (errors.Count > count)
			{
				return new DateParseResult(false, errors[0], TimeParserConstants.MinDate, TimeParserConstants.MaxDate);
			}
			if (today2.Date > today.Date)
			{
				errors.Add("End date must be greater or equal to start date.");
				return new DateParseResult(false, errors[0], TimeParserConstants.MinDate, TimeParserConstants.MaxDate);
			}
			return new DateParseResult(true, string.Empty, today2, today);
		}
		public static DateTime ParseTPlusMinusX(string dateStr)
		{
			return DateParser.ParseTPlusMinusX(dateStr, DateTime.MinValue);
		}
		public static DateTime ParseTPlusMinusX(string dateStr, DateTime defaultVal)
		{
			string pattern = "(?<datepart>[0-9a-zA-Z\\\\/]+)\\s*((?<addop>[\\+\\-]{1})\\s*(?<addval>[0-9]+))?";
			Match match = Regex.Match(dateStr, pattern);
			DateTime result = defaultVal;
			if (match.Success)
			{
				string value = match.Groups["datepart"].Value;
				if (value.ToLower().Trim() == "t")
				{
					result = DateTime.Now;
				}
				else
				{
					result = DateTime.Parse(value);
				}
				if (match.Groups["addop"].Success && match.Groups["addval"].Success)
				{
					string value2 = match.Groups["addop"].Value;
					int num = Convert.ToInt32(match.Groups["addval"].Value);
					if (value2 == "-")
					{
						num *= -1;
					}
					result = result.AddDays((double)num);
				}
			}
			return result;
		}
	}
}
