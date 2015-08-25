using System;
using System.Collections.Generic;
namespace XUtils.Parsers
{
	public class LexArgs : LexBase
	{
		private static LexSettings _defaultSettings = new LexSettings();
		public static IDictionary<string, string> Parse(string line)
		{
			return LexArgs.Parse(line, LexArgs._defaultSettings).ToDictionary<string>();
		}
		public static List<string> ParseList(string line)
		{
			return LexArgs.Parse(line, LexArgs._defaultSettings);
		}
		public static List<string> Parse(string line, LexSettings settings)
		{
			LexArgs lexArgs = new LexArgs(settings);
			return lexArgs.ParseText(line);
		}
		public LexArgs()
		{
			this.Init(LexArgs._defaultSettings);
		}
		public LexArgs(LexSettings settings)
		{
			this.Init(settings);
		}
	}
}
