using System;
using System.Linq;
using System.Text;
namespace XUtils
{
	public static class LatinWordExtensions
	{
		public static readonly char[] WhiteSpaceCharacters = new char[]
		{
			' ',
			'\t',
			'\n',
			'\v',
			'\f',
			'\r',
			'\u00a0',
			'\u0085',
			'\u1680',
			'\u2000',
			'\u2001',
			'\u2002',
			'\u2003',
			'\u2004',
			'\u2005',
			'\u2006',
			'\u2007',
			'\u2008',
			'\u2009',
			'\u200a',
			'​',
			'\u2028',
			'\u2029',
			'\u202f',
			'\u205f',
			'\u3000'
		};
		public static readonly string[] EnglishInitialsExclusionWords = new string[]
		{
			"a",
			"an",
			"as",
			"at",
			"by",
			"for",
			"from",
			"in",
			"into",
			"of",
			"on",
			"to",
			"the",
			"with"
		};
		private static bool IsDelimiter(char c, char[] delimiters)
		{
			if (delimiters == null)
			{
				delimiters = LatinWordExtensions.WhiteSpaceCharacters;
			}
			int num = delimiters.Length;
			for (int i = 0; i < num; i++)
			{
				if (c.Equals(delimiters[i]))
				{
					return true;
				}
			}
			return false;
		}
		public static string Initials(this string phrase)
		{
			return phrase.Initials(LatinWordExtensions.WhiteSpaceCharacters, null);
		}
		public static string Initials(this string phrase, char[] delimiters)
		{
			return phrase.Initials(delimiters, null);
		}
		public static string Initials(this string phrase, char[] delimiters, string[] exclusions)
		{
			if (phrase == null)
			{
				string message = "Can't process null string";
				throw new ArgumentNullException("phrase", message);
			}
			if (delimiters == null)
			{
				delimiters = LatinWordExtensions.WhiteSpaceCharacters;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = phrase.Split(delimiters);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if ((exclusions == null || !exclusions.Contains(text)) && text.Length > 0)
				{
					stringBuilder.Append(text[0]);
				}
			}
			return stringBuilder.ToString();
		}
		public static string Wrap(this string text, int lineLength)
		{
			return text.Wrap(lineLength, null);
		}
		public static string Wrap(this string text, int lineLength, string newLineMarker)
		{
			if (text == null)
			{
				string message = "Can't process null string";
				throw new ArgumentNullException("text", message);
			}
			if (lineLength <= 0)
			{
				return text;
			}
			if (newLineMarker == null)
			{
				newLineMarker = Environment.NewLine;
			}
			int num = 0;
			int capacity = (int)Math.Round(text.Length * 1.2m);
			StringBuilder stringBuilder = new StringBuilder(capacity);
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				int length2 = stringBuilder.Length;
				if (LatinWordExtensions.IsDelimiter(c, LatinWordExtensions.WhiteSpaceCharacters))
				{
					num = length2;
				}
				stringBuilder.Append(c);
				if (length2 > 0 && length2 % lineLength == 0)
				{
					stringBuilder.Remove(num, 1);
					stringBuilder.Insert(num, newLineMarker);
				}
			}
			if (text.Length % lineLength > 0)
			{
				stringBuilder.Remove(num, 1);
				stringBuilder.Insert(num, newLineMarker);
			}
			return stringBuilder.ToString();
		}
		public static string NormalizeWhitespace(this string text)
		{
			return text.NormalizeWhitespace(' ', LatinWordExtensions.WhiteSpaceCharacters);
		}
		public static string NormalizeWhitespace(this string text, char whitespaceChar)
		{
			return text.NormalizeWhitespace(whitespaceChar, LatinWordExtensions.WhiteSpaceCharacters);
		}
		public static string NormalizeWhitespace(this string text, char whitespaceChar, char[] whitespace)
		{
			if (text == null)
			{
				string message = "Can't process null string";
				throw new ArgumentNullException("text", message);
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			bool flag = false;
			char[] array = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if (LatinWordExtensions.IsDelimiter(c, whitespace))
				{
					if (!flag)
					{
						stringBuilder.Append(whitespaceChar);
					}
					flag = true;
				}
				else
				{
					stringBuilder.Append(c);
					flag = false;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
