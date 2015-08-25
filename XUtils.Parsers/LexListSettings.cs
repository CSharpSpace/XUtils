using System;
namespace XUtils.Parsers
{
	public class LexListSettings : LexSettings
	{
		public string Delimeter = ",";
		public bool TrimWhiteSpace = true;
		public bool MultipleRecordsUsingNewLine;
		public bool AllowNewLineInFirstLine;
		public bool AllowNewLinesAsTextOnlyAfterFirstLine = true;
	}
}
