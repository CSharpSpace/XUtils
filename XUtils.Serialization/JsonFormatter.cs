using System;
using System.Collections;
using System.Globalization;
using System.Text;
namespace XUtils.Serialization
{
	internal class JsonFormatter
	{
		public static string Serialize(JsonObject doc)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{ ");
			bool flag = true;
			foreach (string current in doc.Keys)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.AppendFormat("\"{0}\": ", current);
				JsonFormatter.SerializeType(doc[current], stringBuilder);
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}
		public static string SerializeForServerSide(object value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				stringBuilder.AppendFormat("new Date({0},{1},{2},{3},{4},{5},{6})", new object[]
				{
					dateTime.Year,
					dateTime.Month - 1,
					dateTime.Day,
					dateTime.Hour,
					dateTime.Minute,
					dateTime.Second,
					dateTime.Millisecond
				});
			}
			else
			{
				JsonFormatter.SerializeType(value, stringBuilder);
			}
			return stringBuilder.ToString();
		}
		private static void SerializeType(object value, StringBuilder json)
		{
			if (value == null)
			{
				json.Append("null");
				return;
			}
			if (value is bool)
			{
				json.Append(((bool)value) ? "true" : "false");
				return;
			}
			if (value is JsonObject)
			{
				json.Append(value);
				return;
			}
			if (value is int || value is long || value is float || value is double)
			{
				json.Append(((IFormattable)value).ToString("G", CultureInfo.InvariantCulture));
				return;
			}
			if (value is string)
			{
				json.AppendFormat("\"{0}\"", JsonFormatter.Escape((string)value));
				return;
			}
			if (value is DateTime)
			{
				json.AppendFormat("\"{0}\"", ((DateTime)value).ToString());
				return;
			}
			if (value is Guid)
			{
				json.Append(string.Format("{{ \"$uid\": \"{0}\" }}", value));
				return;
			}
			if (value is IEnumerable)
			{
				json.Append("[ ");
				bool flag = true;
				foreach (object current in (IEnumerable)value)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						json.Append(", ");
					}
					JsonFormatter.SerializeType(current, json);
				}
				json.Append(" ]");
				return;
			}
			json.AppendFormat("\"{0}\"", JsonFormatter.Escape(value.ToString()));
		}
		public static string Escape(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < text.Length)
			{
				char c = text[i];
				char c2 = c;
				if (c2 <= '"')
				{
					switch (c2)
					{
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					default:
						if (c2 != '"')
						{
							goto IL_E7;
						}
						stringBuilder.Append("\\\"");
						break;
					}
				}
				else
				{
					if (c2 != '\'')
					{
						if (c2 != '\\')
						{
							goto IL_E7;
						}
						stringBuilder.Append("\\\\");
					}
					else
					{
						stringBuilder.Append("\\'");
					}
				}
				IL_118:
				i++;
				continue;
				IL_E7:
				if (c <= '\u001f')
				{
					stringBuilder.Append("\\u");
					StringBuilder arg_108_0 = stringBuilder;
					int num = (int)c;
					arg_108_0.Append(num.ToString("x4"));
					goto IL_118;
				}
				stringBuilder.Append(c);
				goto IL_118;
			}
			return stringBuilder.ToString();
		}
	}
}
