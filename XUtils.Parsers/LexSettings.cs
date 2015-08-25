using System;
namespace XUtils.Parsers
{
	public class LexSettings
	{
		public string[] QuotesChars = new string[]
		{
			"\"",
			"'"
		};
		public string EscapeChar = "\\";
		public string[] WhiteSpaceChars = new string[]
		{
			" ",
			"\t"
		};
		public string[] EolChars = new string[]
		{
			"\n",
			"\r\n"
		};
	}
}
