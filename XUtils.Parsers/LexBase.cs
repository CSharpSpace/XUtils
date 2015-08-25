using System;
using System.Collections.Generic;
using System.Text;
namespace XUtils.Parsers
{
	public class LexBase
	{
		protected ITokenReader _reader;
		protected LexSettings _settings = new LexSettings();
		protected List<string> _tokenList;
		protected IList<string> _errors;
		protected IDictionary<string, string> _whiteSpaceMap;
		public bool AllowNewLine
		{
			get;
			set;
		}
		public virtual void Init(LexSettings settings)
		{
			this._reader = new TokenReader();
			this._errors = new List<string>();
			this._tokenList = new List<string>();
			this._settings = settings;
		}
		public virtual List<string> ParseText(string line)
		{
			this.Reset(line);
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
					if (this._reader.IsToken())
					{
						string currentChar = this._reader.CurrentChar;
						this._tokenList.Add(this.ReadQuotedToken());
						if (this.Expect(currentChar))
						{
							this._reader.ReadChar();
						}
					}
					else
					{
						this._tokenList.Add(this.ReadNonQuotedToken(this._whiteSpaceMap, true));
					}
					this._reader.ReadChar();
				}
			}
			this.CheckAndThrowErrors();
			this.ExcludeNewLinesStored();
			return this._tokenList;
		}
		protected string ReadNonQuotedToken(IDictionary<string, string> separators, bool breakOnEol)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (!separators.ContainsKey(this._reader.CurrentChar) && !this._reader.IsEnd() && (!breakOnEol || !this._reader.IsEol()))
			{
				stringBuilder.Append(this._reader.CurrentChar);
				this._reader.ReadChar();
			}
			return stringBuilder.ToString();
		}
		protected virtual string ReadQuotedToken()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string currentChar = this._reader.CurrentChar;
			this._reader.ReadChar();
			while (this._reader.CurrentChar != currentChar && !this._reader.IsEnd())
			{
				if (!this._reader.IsEscape())
				{
					stringBuilder.Append(this._reader.CurrentChar);
				}
				else
				{
					string value = this._reader.ReadChar();
					stringBuilder.Append(value);
				}
				this._reader.ReadChar();
			}
			return stringBuilder.ToString();
		}
		protected virtual bool Expect(string expectChar)
		{
			bool flag = this._reader.CurrentChar == expectChar;
			if (!flag)
			{
				this.AddError("Expected " + expectChar + ", but found end of line/text.");
			}
			return flag;
		}
		protected virtual void AddError(string error)
		{
			this._errors.Add(error);
		}
		protected void CheckAndThrowErrors()
		{
			if (this._errors.Count > 0)
			{
				throw new ArgumentException("Errors parsing line : " + this._errors.Join(Environment.NewLine));
			}
		}
		protected void ExcludeNewLinesStored()
		{
			if (this._reader.EolChars.ContainsKey(this._tokenList[this._tokenList.Count - 1]))
			{
				this._tokenList.RemoveAt(this._tokenList.Count - 1);
			}
		}
		protected virtual void Reset(string line)
		{
			this._reader.Reset();
			this._errors.Clear();
			this._tokenList.Clear();
			this._whiteSpaceMap = this._settings.WhiteSpaceChars.ToDictionary<string>();
			this._reader.Init(line, "\\", this._settings.QuotesChars, this._settings.WhiteSpaceChars, this._settings.EolChars);
		}
	}
}
