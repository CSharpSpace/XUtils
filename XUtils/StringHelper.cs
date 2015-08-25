using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
namespace XUtils
{
	public static class StringHelper
	{
		public static readonly char[] LineBreakingCharacters = new char[]
		{
			'\n',
			'\r',
			'\f',
			'\u2028',
			'\u2029',
			'\u0085'
		};
		public static readonly char[] DosLineSeparator = new char[]
		{
			'\r',
			'\n'
		};
		public static readonly char[] UnixLineSeparator = new char[]
		{
			'\n'
		};
		public static readonly char[] MacOs9Separator = new char[]
		{
			'\r'
		};
		public static readonly char[] UnicodeSeparator = new char[]
		{
			'\u2028'
		};
		public static void ConvertLineSeparators(TextReader reader, TextWriter writer, char[] separator)
		{
			for (int num = reader.Read(); num != -1; num = reader.Read())
			{
				if (StringHelper.LineBreakingCharacters.Contains((char)num))
				{
					if (num == 13 && reader.Peek() == 10)
					{
						reader.Read();
					}
					writer.Write(separator);
				}
				else
				{
					writer.Write((char)num);
				}
			}
		}
		public static string ConvertLineSeparators(string text, char[] separator)
		{
			StringReader reader = new StringReader(text);
			StringWriter stringWriter = new StringWriter();
			StringHelper.ConvertLineSeparators(reader, stringWriter, separator);
			return stringWriter.ToString();
		}
		public static IList<string> ReadLines(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return new List<string>();
			}
			StringReader stringReader = new StringReader(text);
			string item = stringReader.ReadLine();
			IList<string> list = new List<string>();
			while (item != null)
			{
				list.Add(item);
				item = stringReader.ReadLine();
			}
			stringReader.Close();
			stringReader.Dispose();
			return list;
		}
		public static string Truncate(this string txt, int maxChars)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return txt;
			}
			if (txt.Length <= maxChars)
			{
				return txt;
			}
			return txt.Substring(0, maxChars);
		}
		public static string TruncateWithText(this string txt, int maxChars, string suffix)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return txt;
			}
			if (txt.Length <= maxChars)
			{
				return txt;
			}
			string str = txt.Substring(0, maxChars);
			return str + suffix;
		}
		public static string TruncateWithText(this string txt, int maxChars)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return txt;
			}
			txt = Regex.Replace(txt, "<[^>]+>", "");
			txt = txt.Trim().Replace("\r\n", "").Replace("\t", "").Replace(" ", "");
			return txt.TruncateWithText(maxChars, "...");
		}
		public static T Get<T>(this IDictionary<string, string> items, string Key)
		{
			if (items == null)
			{
				return default(T);
			}
			if (!items.ContainsKey(Key))
			{
				return default(T);
			}
			return TypeParsers.ConvertTo<T>(items[Key]);
		}
		public static string Join(this IDictionary<string, string> items, char delimeter, Func<KeyValuePair<string, string>, string> appender)
		{
			if (items == null || items.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> current in items)
			{
				string arg = (appender == null) ? current.ToString() : appender(current);
				stringBuilder.Append(delimeter + arg);
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder = stringBuilder.Remove(0, 1);
			}
			return stringBuilder.ToString();
		}
		public static string Join(this IList<string> items, string delimeter)
		{
			string str = "";
			int i;
			for (i = 0; i < items.Count - 2; i++)
			{
				str = str + items[i] + delimeter;
			}
			return str + items[i];
		}
		public static string ConvertToSentanceCase(this string s, char delimiter)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			s = s.Trim();
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (s.IndexOf(delimiter) < 0)
			{
				s = s.ToLower();
				s = s[0].ToString().ToUpper() + s.Substring(1);
				return s;
			}
			string[] array = s.Split(new char[]
			{
				delimiter
			});
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string text2 = text.ToLower();
				text2 = text2[0].ToString().ToUpper() + text2.Substring(1);
				stringBuilder.Append(text2 + delimiter);
			}
			s = stringBuilder.ToString();
			return s.TrimEnd(new char[]
			{
				delimiter
			});
		}
		public static int GetIndexOfSpacer(this string txt, int currentPosition, ref bool isNewLine)
		{
			int num = txt.IndexOf(" ", currentPosition);
			int num2 = txt.IndexOf(Environment.NewLine, currentPosition);
			bool flag = num > -1;
			bool flag2 = num2 > -1;
			isNewLine = false;
			if (flag && flag2)
			{
				if (num < num2)
				{
					return num;
				}
				isNewLine = true;
				return num2;
			}
			else
			{
				if (flag && !flag2)
				{
					return num;
				}
				if (!flag && flag2)
				{
					isNewLine = true;
					return num2;
				}
				return -1;
			}
		}
		public static string ConvertToString(this object[] args)
		{
			if (args == null || args.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj != null)
				{
					stringBuilder.Append(obj.ToString());
				}
			}
			return stringBuilder.ToString();
		}
		public static string[] ToStringArray(this string delimitedText, params string[] delimeter)
		{
			if (string.IsNullOrEmpty(delimitedText))
			{
				return new string[0];
			}
			return delimitedText.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
		}
		public static string[] ToStringArray(this string delimitedText, char delimeter)
		{
			if (string.IsNullOrEmpty(delimitedText))
			{
				return null;
			}
			return delimitedText.Split(new char[]
			{
				delimeter
			});
		}
		public static string[] Split(this string rawText, string excludeText, char delimiter)
		{
			int num = rawText.IndexOf(excludeText);
			string text = rawText.Substring(num + excludeText.Length);
			return text.Split(new string[]
			{
				delimiter.ToString()
			}, StringSplitOptions.RemoveEmptyEntries);
		}
		public static string Substitute(this IDictionary<string, string> subsitutions, string contentPlaceholders)
		{
			if (string.IsNullOrEmpty(contentPlaceholders))
			{
				return contentPlaceholders;
			}
			if (subsitutions == null || subsitutions.Count == 0)
			{
				return contentPlaceholders;
			}
			string replacedValues = contentPlaceholders;
			subsitutions.ForEach((KeyValuePair<string, string> kv) => replacedValues = replacedValues.Replace("${" + kv.Key + "}", kv.Value));
			return replacedValues;
		}
		public static string[] GetDelimitedChars(this string rawText, string excludeText, char delimiter)
		{
			int num = rawText.IndexOf(excludeText);
			return rawText.Substring(num + excludeText.Length).Split(new string[]
			{
				delimiter.ToString()
			}, StringSplitOptions.RemoveEmptyEntries);
		}
		public static IDictionary<string, string> ToMap(this string delimitedText, char keyValuePairDelimiter, char keyValueDelimeter, bool makeKeysCaseSensitive, bool trimValues)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = delimitedText.Split(new char[]
			{
				keyValuePairDelimiter
			});
			if (array == null)
			{
				return dictionary;
			}
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					string[] array3 = text.Split(new char[]
					{
						keyValueDelimeter
					});
					if (array3.Length >= 2)
					{
						string text2 = array3[0];
						string text3 = array3[1];
						if (makeKeysCaseSensitive)
						{
							text2 = text2.ToLower();
							text3 = text3.ToLower();
						}
						if (trimValues)
						{
							text2 = text2.Trim();
							text3 = text3.Trim();
						}
						dictionary[text2] = text3;
					}
				}
			}
			return dictionary;
		}
		public static string UnicodetoAscII(this string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return content;
			}
			char[] array = content.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			char[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				char c = array2[i];
				stringBuilder.Append("Ox" + (int)c + ";");
			}
			return stringBuilder.ToString();
		}
		public static string HtmlFilter(this string strHtml)
		{
			if (string.IsNullOrEmpty(strHtml))
			{
				return strHtml;
			}
			string text = strHtml.Trim();
			text = text.Replace("&ldquo;", "“");
			text = text.Replace("&rdquo;", "”");
			text = Regex.Replace(text, "<!\\[CDATA\\[(.*)\\]\\]>", "$1");
			text = Regex.Replace(text, "<.+?>", "");
			text = Regex.Replace(text, "<!--/*[^<>]*-->", "");
			text = Regex.Replace(text, "(?:&nbsp;?)+", " ");
			text = Regex.Replace(text, "&\\w+?;", "");
			return Regex.Replace(text, "\\s+", " ");
		}
		public static string TextHTMLEncode(this string strSourceText)
		{
			if (string.IsNullOrEmpty(strSourceText))
			{
				return strSourceText;
			}
			string text = strSourceText.Replace("&", "&#38");
			text = text.Replace("'", "&#39");
			text = text.Replace("\"", "&#34");
			text = text.Replace("<", "&#60");
			text = text.Replace(">", "&#62");
			text = text.Replace(" ", "&nbsp;");
			text = text.Replace("\n", "<br/>");
			return text.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
		}
		public static string TextHTMLDecode(this string strSourceText)
		{
			if (string.IsNullOrEmpty(strSourceText))
			{
				return strSourceText;
			}
			string text = strSourceText.Replace("&#38", "&");
			text = text.Replace("&#39", "'");
			text = text.Replace("&#34", "\"");
			text = text.Replace("&#60", "<");
			text = text.Replace("&#62", ">");
			text = text.Replace("&nbsp;", " ");
			text = text.Replace("<br/>", "\n");
			return text.Replace("&nbsp;&nbsp;&nbsp;&nbsp;", "\t");
		}
		public static string Filtrate(this string Htmlstring)
		{
			if (string.IsNullOrEmpty(Htmlstring))
			{
				return Htmlstring;
			}
			Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "<(.[^>]*)>", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "-->", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "<!--.*", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "¡", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "¢", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "£", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "©", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&#(\\d+);", "", RegexOptions.IgnoreCase);
			Htmlstring.Replace("<", "");
			Htmlstring.Replace(">", "");
			Htmlstring.Replace("\r\n", "");
			Htmlstring = Htmlstring.HtmlEncode();
			Htmlstring = Htmlstring.Replace("\"", "\\\"");
			Htmlstring = Htmlstring.Replace("\\", "\\\\");
			Htmlstring = Htmlstring.Replace("/", "\\/");
			Htmlstring = Htmlstring.Replace("\b", "\\b");
			Htmlstring = Htmlstring.Replace("\f", "\\f");
			Htmlstring = Htmlstring.Replace("\n", "\\n");
			Htmlstring = Htmlstring.Replace("\r", "\\r");
			Htmlstring = Htmlstring.Replace("\t", "\\t");
			return Htmlstring;
		}
		public static string HtmlEncode(this string str)
		{
			return HttpUtility.HtmlEncode(str);
		}
		public static string HtmlDecode(this string str)
		{
			return HttpUtility.HtmlDecode(str);
		}
		public static string SqlFilter(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			str = str.Replace("'", "''");
			str = str.Replace(";", "；");
			str = str.Replace("(", "（");
			str = str.Replace(")", "）");
			str = str.Replace("Exec", "");
			str = str.Replace("Execute", "");
			str = str.Replace("xp_", "x p_");
			str = str.Replace("sp_", "s p_");
			str = str.Replace("0x", "0 x");
			return str;
		}
		public static string GetNetCardIP()
		{
			string result;
			try
			{
				string text = "";
				ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection instances = managementClass.GetInstances();
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						if ((bool)managementObject["IPEnabled"])
						{
							string[] array = (string[])managementObject["IPAddress"];
							if (array.Length > 0)
							{
								text = array[0].ToString();
							}
						}
					}
				}
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}
		public static string GetCpuID()
		{
			string result;
			try
			{
				ManagementClass managementClass = new ManagementClass("Win32_Processor");
				ManagementObjectCollection instances = managementClass.GetInstances();
				string text = null;
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						text = managementObject.Properties["ProcessorId"].Value.ToString();
					}
				}
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}
		public static string GetNetCardMAC()
		{
			string result;
			try
			{
				string text = "";
				ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection instances = managementClass.GetInstances();
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						if ((bool)managementObject["IPEnabled"])
						{
							text += managementObject["MACAddress"].ToString();
						}
					}
				}
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}
		public static string GetIP()
		{
			string result;
			if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
			{
				result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
			}
			else
			{
				result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
			}
			return result;
		}
		public static long GetLongIP()
		{
			string[] array = StringHelper.GetIP().Split(".".ToCharArray());
			string text = "";
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string value = array2[i];
				text += Convert.ToInt16(value).ToString("x");
			}
			return long.Parse(text, NumberStyles.HexNumber);
		}
		public static string GetSpells(this string input)
		{
			int length = input.Length;
			string text = "";
			for (int i = 0; i < length; i++)
			{
				text += input.Substring(i, 1).GetSpell();
			}
			return text;
		}
		public static string GetSpell(this string cn)
		{
			byte[] bytes = Encoding.Default.GetBytes(cn);
			if (bytes.Length > 1)
			{
				int num = (int)bytes[0];
				int num2 = (int)bytes[1];
				int num3 = (num << 8) + num2;
				int[] array = new int[]
				{
					45217,
					45253,
					45761,
					46318,
					46826,
					47010,
					47297,
					47614,
					48119,
					48119,
					49062,
					49324,
					49896,
					50371,
					50614,
					50622,
					50906,
					51387,
					51446,
					52218,
					52698,
					52698,
					52698,
					52980,
					53689,
					54481
				};
				for (int i = 0; i < 26; i++)
				{
					int num4 = 55290;
					if (i != 25)
					{
						num4 = array[i + 1];
					}
					if (array[i] <= num3 && num3 < num4)
					{
						return Encoding.Default.GetString(new byte[]
						{
							(byte)(65 + i)
						});
					}
				}
				return "?";
			}
			return cn;
		}
	}
}
