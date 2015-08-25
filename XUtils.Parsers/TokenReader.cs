using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace XUtils.Parsers
{
	public class TokenReader : ITokenReader
	{
		private int _pos;
		private string _text;
		private IDictionary<string, string> _whiteSpaceChars;
		private IDictionary<string, string> _eolChars;
		private IDictionary<string, string> _tokens;
		private Dictionary<string, string> _readonlyWhiteSpaceMap;
		private Dictionary<string, string> _readonlyEolMap;
		private string _currentChar;
		private StringBuilder _backBuffer;
		private string _escapeChar;
		private int _currentCharInt;
		private int LAST_POSITION;
		private List<string> _errors = new List<string>();
		private string END_CHAR = "";
		public string CurrentChar
		{
			get
			{
				return this._currentChar;
			}
		}
		public string PreviousChar
		{
			get
			{
				if (this._pos <= 0 || this._backBuffer.Length <= 0)
				{
					return string.Empty;
				}
				return this._backBuffer[this._backBuffer.Length - 2].ToString();
			}
		}
		public IDictionary<string, string> EolChars
		{
			get
			{
				return this._readonlyEolMap;
			}
		}
		public string PreviousCharAny
		{
			get
			{
				if (this._pos <= 0)
				{
					return string.Empty;
				}
				return this._text[this._pos - 1].ToString();
			}
		}
		public TokenReader()
		{
			this.Init(string.Empty, "\\", new string[]
			{
				"\"",
				"'"
			}, new string[]
			{
				" ",
				"\t"
			}, new string[]
			{
				"\n",
				"\r\n"
			});
		}
		public TokenReader(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolChars)
		{
			this.Init(text, escapeChar, tokens, whiteSpaceTokens, eolChars);
		}
		public void Init(string text, TokenReaderSettings settings)
		{
			this.Init(text, settings.EscapeChar, settings.Tokens, settings.WhiteSpaceTokens, settings.EolTokens);
		}
		public void Init(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolTokens)
		{
			this.Reset();
			this._text = text;
			this.LAST_POSITION = this._text.Length - 1;
			this._tokens = tokens.ToDictionary<string>();
			this._whiteSpaceChars = whiteSpaceTokens.ToDictionary<string>();
			this._eolChars = eolTokens.ToDictionary<string>();
			this._readonlyEolMap = new Dictionary<string, string>(this._eolChars);
			this._readonlyWhiteSpaceMap = new Dictionary<string, string>(this._whiteSpaceChars);
		}
		public void RegisterEol(IDictionary<string, string> eolchars)
		{
			this._eolChars = eolchars;
		}
		public void RegisterWhiteSpace(IDictionary<string, string> whitespaceChars)
		{
			this._whiteSpaceChars = whitespaceChars;
		}
		public void Reset()
		{
			this._pos = -1;
			this._text = string.Empty;
			this._backBuffer = new StringBuilder();
			this._whiteSpaceChars = new Dictionary<string, string>();
			this._tokens = new Dictionary<string, string>();
			this._eolChars = new Dictionary<string, string>();
			this._escapeChar = "\\";
		}
		public string PeekChar()
		{
			if (this._pos >= this._text.Length - 1)
			{
				return string.Empty;
			}
			return this._text[this._pos + 1].ToString();
		}
		public string PeekChars(int count)
		{
			if (this._pos + count > this._text.Length)
			{
				return string.Empty;
			}
			return this._text.Substring(this._pos + 1, count);
		}
		public string PeekLine()
		{
			int num = this._text.IndexOf(Environment.NewLine, this._pos + 1);
			if (num == -1)
			{
				return this._text.Substring(this._pos + 1);
			}
			return this._text.Substring(this._pos + 1, num - this._pos);
		}
		public void ConsumeChar()
		{
			this._pos++;
		}
		public void ConsumeChars(int count)
		{
			this._pos += count;
		}
		public void ConsumeWhiteSpace()
		{
			this.ConsumeWhiteSpace(false);
		}
		public void ConsumeWhiteSpace(bool readFirst)
		{
			string key = this._currentChar;
			if (readFirst)
			{
				key = this.ReadChar();
			}
			while (!this.IsEnd() && this._whiteSpaceChars.ContainsKey(key))
			{
				this.ReadChar();
				key = this._currentChar;
			}
		}
		public void ConsumeWhiteSpace(ref int tabCount, ref int whiteSpace)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public void ConsumeNewLine()
		{
			string key = this._currentChar + this.PeekChar();
			if (this._eolChars.ContainsKey(key))
			{
				this.ReadChar();
				this.ReadChar();
				return;
			}
			if (this._eolChars.ContainsKey(this._currentChar))
			{
				this.ReadChar();
			}
		}
		public void ConsumeNewLines()
		{
			string key = this._currentChar + this.PeekChar();
			while (this._eolChars.ContainsKey(this._currentChar) || this._eolChars.ContainsKey(key))
			{
				this.ConsumeNewLine();
				key = this._currentChar + this.PeekChar();
			}
		}
		public void ReadBackChar()
		{
			this._pos--;
			this._backBuffer.Remove(this._backBuffer.Length - 1, 1);
			this._currentChar = this._text[this._pos].ToString();
		}
		public void ReadBackChar(int count)
		{
			this._pos -= count;
			this._backBuffer.Remove(this._backBuffer.Length - count, count);
			this._currentChar = this._text[this._pos].ToString();
		}
		public string ReadChar()
		{
			this._pos++;
			if (this._pos >= this._text.Length)
			{
				this._currentChar = this.END_CHAR;
				return this._currentChar;
			}
			this._currentChar = this._text[this._pos].ToString();
			this._backBuffer.Append(this._currentChar);
			return this._currentChar;
		}
		public string ReadChars(int count)
		{
			string text = this._text.Substring(this._pos + 1, count);
			this._pos += count;
			this._currentChar = this._text[this._pos].ToString();
			this._backBuffer.Append(text);
			return text;
		}
		public string ReadToEol()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (!this.IsEol(this._eolChars as IDictionary) && this._pos <= this.LAST_POSITION)
			{
				stringBuilder.Append(this._currentChar);
				this.ReadChar();
			}
			return stringBuilder.ToString();
		}
		public string ReadToken(string endChar, string escapeChar, bool includeEndChar, bool advanceFirst, bool expectEndChar, bool readPastEndChar)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = advanceFirst ? this.ReadChar() : this._currentChar;
			while (text != endChar && this._pos <= this.LAST_POSITION)
			{
				if (text == escapeChar)
				{
					text = this.ReadChar();
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.Append(text);
				}
				text = this.ReadChar();
			}
			bool flag = true;
			if (expectEndChar && this._currentChar != endChar)
			{
				this._errors.Add(string.Concat(new object[]
				{
					"Expected ",
					endChar,
					" at : ",
					this._pos
				}));
				flag = false;
			}
			if (flag && readPastEndChar)
			{
				this.ReadChar();
			}
			return stringBuilder.ToString();
		}
		public int CurrentCharIndex()
		{
			return this._pos;
		}
		public int CurrentCharInt()
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public bool IsToken()
		{
			return this._tokens.ContainsKey(this._currentChar);
		}
		public bool IsEscape()
		{
			return string.Compare(this._currentChar, this._escapeChar, false) == 0;
		}
		public bool IsEnd()
		{
			return this._pos >= this._text.Length;
		}
		public bool IsAtEnd()
		{
			return this._pos == this._text.Length - 1;
		}
		public bool IsEol()
		{
			return this.IsEol(this._eolChars as IDictionary);
		}
		public bool IsEol(IDictionary eolChars)
		{
			string key = this._currentChar + this.PeekChar();
			return eolChars.Contains(key) || eolChars.Contains(this._currentChar);
		}
		public bool IsWhiteSpace(IDictionary whitespaceChars)
		{
			return whitespaceChars.Contains(this._currentChar);
		}
		public bool IsWhiteSpace()
		{
			return this.IsWhiteSpace(this._whiteSpaceChars as IDictionary);
		}
		private void Init(IDictionary<string, bool> tokens, string[] tokenList)
		{
			for (int i = 0; i < tokenList.Length; i++)
			{
				string key = tokenList[i];
				tokens[key] = true;
			}
		}
	}
}
