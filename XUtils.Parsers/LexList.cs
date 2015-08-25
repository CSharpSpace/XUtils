using System;
using System.Collections.Generic;
namespace XUtils.Parsers
{
	public class LexList : LexBase
	{
		protected List<List<string>> _lines;
		protected IDictionary<string, string> _separatorMap;
		protected static LexListSettings _defaultSettings = new LexListSettings();
		public static IDictionary<string, string> Parse(string line)
		{
			return LexList.Parse(line, LexList._defaultSettings)[0].ToDictionary<string>();
		}
		public static List<string> ParseList(string line)
		{
			return LexList.Parse(line, LexList._defaultSettings)[0];
		}
		public static List<List<string>> ParseTable(string line)
		{
			return LexList.Parse(line, new LexListSettings
			{
				MultipleRecordsUsingNewLine = true
			});
		}
		public static List<List<string>> Parse(string text, LexListSettings settings)
		{
			LexList lexList = new LexList(settings);
			return lexList.ParseLines(text);
		}
		public LexList()
		{
			this.Init(LexList._defaultSettings);
		}
		public LexList(LexListSettings settings)
		{
			this.Init(settings);
		}
		public override void Init(LexSettings settings)
		{
			base.Init(settings);
			this._separatorMap = new Dictionary<string, string>();
			this._separatorMap[","] = ",";
			if (settings is LexListSettings)
			{
				LexListSettings lexListSettings = (LexListSettings)settings;
				this._separatorMap.Clear();
				this._separatorMap[lexListSettings.Delimeter] = lexListSettings.Delimeter;
			}
		}
		public List<List<string>> ParseLines(string text)
		{
			LexListSettings settings = this._settings as LexListSettings;
			this.Reset(text);
			this._reader.ReadChar();
			this._reader.ConsumeWhiteSpace();
			while (!this._reader.IsEnd())
			{
				if (this._reader.IsWhiteSpace())
				{
					this._reader.ConsumeWhiteSpace();
				}
				else
				{
					if (this._reader.CurrentChar == "'" || this._reader.CurrentChar == "\"")
					{
						this.ParseQuotedItem(settings);
					}
					else
					{
						this.ParseNonQuotedItem(settings);
					}
					this.CheckAndConsumeWhiteSpace();
					this.CheckAndHandleComma();
					this.CheckAndConsumeWhiteSpace();
					if (!this.CheckAndHandleNewLine(settings))
					{
						this._reader.ReadChar();
					}
				}
			}
			base.CheckAndThrowErrors();
			if (this._tokenList.Count > 0)
			{
				this._lines.Add(this._tokenList);
			}
			return this._lines;
		}
		protected override void Reset(string line)
		{
			base.Reset(line);
			this._lines = new List<List<string>>();
		}
		private void ParseQuotedItem(LexListSettings settings)
		{
			string currentChar = this._reader.CurrentChar;
			string text = this.ReadQuotedToken();
			if (settings.TrimWhiteSpace)
			{
				text = text.Trim();
			}
			this._tokenList.Add(text);
			if (this.Expect(currentChar))
			{
				this._reader.ReadChar();
			}
		}
		private bool CheckAndHandleComma()
		{
			if (this._separatorMap.ContainsKey(this._reader.CurrentChar))
			{
				return true;
			}
			string key = this._reader.PeekChar();
			if (this._separatorMap.ContainsKey(key))
			{
				this._reader.ReadChar();
				return true;
			}
			return false;
		}
		public bool CheckAndConsumeWhiteSpace()
		{
			if (this._reader.IsWhiteSpace())
			{
				this._reader.ConsumeWhiteSpace();
				return true;
			}
			return false;
		}
		public bool CheckAndHandleNewLine(LexListSettings settings)
		{
			if (this._reader.IsEol())
			{
				this._reader.ConsumeNewLine();
				if (settings.MultipleRecordsUsingNewLine)
				{
					this._lines.Add(this._tokenList);
					this._tokenList = new List<string>();
				}
				return true;
			}
			return false;
		}
		public bool HasMore()
		{
			return !this._reader.IsAtEnd() && !this._reader.IsEnd();
		}
		private void ParseNonQuotedItem(LexListSettings settings)
		{
			string text = base.ReadNonQuotedToken(this._separatorMap, true);
			if (settings.TrimWhiteSpace)
			{
				text = text.Trim();
			}
			this._tokenList.Add(text);
		}
	}
}
