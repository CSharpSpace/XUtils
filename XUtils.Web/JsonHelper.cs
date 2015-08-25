using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace XUtils.Web
{
	public class JsonHelper
	{
		public static string EscapeString(string text, bool encloseInQuotes = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				if (!encloseInQuotes)
				{
					return string.Empty;
				}
				return "\"\"";
			}
			else
			{
				text = text.Replace("\\", "\\\\");
				text = text.Replace("\"", "\\\"");
				if (!encloseInQuotes)
				{
					return text;
				}
				return "\"" + text + "\"";
			}
		}
		public static string ConvertToJsonString<T>(PagedList<T> result, IList<PropertyInfo> _columnProps, bool convertBoolToYesNo = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < result.Count; i++)
			{
				T t = result[i];
				string arg_1A_0 = string.Empty;
				string text = string.Empty;
				stringBuilder.Append("{ ");
				for (int j = 0; j < _columnProps.Count; j++)
				{
					PropertyInfo propertyInfo = _columnProps[j];
					object value = _columnProps[j].GetValue(t, null);
					Type propertyType = propertyInfo.PropertyType;
					bool flag = false;
					if (propertyType == typeof(string))
					{
						text = ((value == null) ? string.Empty : value.ToString());
						text = text.Replace("\\", "\\\\");
						text = text.Replace("\"", "\\\"");
						flag = true;
					}
					else
					{
						if (propertyType == typeof(bool))
						{
							text = ((value == null) ? "false" : value.ToString().ToLower());
							if (convertBoolToYesNo)
							{
								text = ((text == "true") ? "\"yes\"" : "\"no\"");
							}
						}
						else
						{
							if (propertyType == typeof(DateTime))
							{
								if (value == null)
								{
									text = "0";
								}
								else
								{
									text = ((DateTime)value).ToShortDateString();
									flag = true;
								}
							}
							else
							{
								if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(float) || propertyType == typeof(double))
								{
									text = TypeParsers.ConvertTo<double>(value).ToString();
								}
							}
						}
					}
					if (flag)
					{
						stringBuilder.Append(string.Concat(new string[]
						{
							"\"",
							_columnProps[j].Name,
							"\": \"",
							text,
							"\""
						}));
					}
					else
					{
						stringBuilder.Append("\"" + _columnProps[j].Name + "\": " + text);
					}
					if (j != _columnProps.Count - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				stringBuilder.Append(" }");
				if (i != result.Count - 1)
				{
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
