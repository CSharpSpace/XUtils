using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
namespace XUtils.Web
{
	public class UrlSeoUtils
	{
		public const string InvalidSeoUrlChars = "$%#@!*?;:~`_+=()[]{}|\\'<>,/^&\".";
		private static IDictionary<char, bool> _invalidChars;
		static UrlSeoUtils()
		{
			char[] array = "$%#@!*?;:~`_+=()[]{}|\\'<>,/^&\".".ToCharArray();
			UrlSeoUtils._invalidChars = new Dictionary<char, bool>();
			char[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				char key = array2[i];
				UrlSeoUtils._invalidChars.Add(key, true);
			}
		}
		public static string GenerateTitle(string title)
		{
			if (string.IsNullOrEmpty(title))
			{
				return string.Empty;
			}
			title = title.ToLower().Trim();
			StringBuilder stringBuilder = new StringBuilder();
			char c = 'a';
			for (int i = 0; i < title.Length; i++)
			{
				char c2 = title[i];
				if (!UrlSeoUtils._invalidChars.ContainsKey(c2))
				{
					if (c2 == ' ' || c2 == '-')
					{
						if (c != ' ' && c != '-')
						{
							stringBuilder.Append('-');
							c = '-';
						}
					}
					else
					{
						stringBuilder.Append(c2);
						c = c2;
					}
				}
			}
			return stringBuilder.ToString();
		}
		public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			nvc = new NameValueCollection();
			baseUrl = "";
			if (url == "")
			{
				return;
			}
			int num = url.IndexOf('?');
			if (num == -1)
			{
				baseUrl = url;
				return;
			}
			baseUrl = url.Substring(0, num);
			if (num == url.Length - 1)
			{
				return;
			}
			string input = url.Substring(num + 1);
			Regex regex = new Regex("(^|&)?(\\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
			MatchCollection matchCollection = regex.Matches(input);
			foreach (Match match in matchCollection)
			{
				nvc.Add(match.Result("$2").ToLower(), match.Result("$3"));
			}
		}
		public static string BuildValidUrlUsingRegex(string title)
		{
			string text = Regex.Replace(title.Trim(), "\\W", "-");
			text = Regex.Replace(text, "55+", "-").Trim(new char[]
			{
				'-'
			});
			return text.ToLower();
		}
		public static string BuildValidUrl(string title)
		{
			if (string.IsNullOrEmpty(title))
			{
				return string.Empty;
			}
			title = title.ToLower().Trim();
			StringBuilder stringBuilder = new StringBuilder();
			char c = 'a';
			for (int i = 0; i < title.Length; i++)
			{
				char c2 = title[i];
				if (!UrlSeoUtils._invalidChars.ContainsKey(c2))
				{
					if (c2 == ' ' || c2 == '-')
					{
						if (c != ' ' && c != '-')
						{
							stringBuilder.Append('-');
							c = '-';
						}
					}
					else
					{
						stringBuilder.Append(c2);
						c = c2;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
