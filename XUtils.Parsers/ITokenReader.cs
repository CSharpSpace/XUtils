using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Parsers
{
	public interface ITokenReader
	{
		string CurrentChar
		{
			get;
		}
		string PreviousChar
		{
			get;
		}
		string PreviousCharAny
		{
			get;
		}
		IDictionary<string, string> EolChars
		{
			get;
		}
		void RegisterEol(IDictionary<string, string> eolchars);
		void RegisterWhiteSpace(IDictionary<string, string> whitespaceChars);
		void Init(string text, string escapeChar, string[] tokens, string[] whiteSpaceTokens, string[] eolTokens);
		void Init(string text, TokenReaderSettings settings);
		void Reset();
		string PeekChar();
		string PeekChars(int count);
		string PeekLine();
		void ConsumeChar();
		void ConsumeChars(int count);
		void ConsumeWhiteSpace();
		void ConsumeWhiteSpace(bool readFirst);
		void ConsumeWhiteSpace(ref int tabCount, ref int whiteSpace);
		void ConsumeNewLine();
		void ConsumeNewLines();
		void ReadBackChar();
		void ReadBackChar(int count);
		string ReadChar();
		string ReadChars(int count);
		string ReadToEol();
		string ReadToken(string endChar, string escapeChar, bool includeEndChar, bool advanceFirst, bool expectEndChar, bool readPastEndChar);
		int CurrentCharIndex();
		int CurrentCharInt();
		bool IsEscape();
		bool IsToken();
		bool IsEnd();
		bool IsAtEnd();
		bool IsEol();
		bool IsEol(IDictionary eolChars);
		bool IsWhiteSpace(IDictionary whitespaceChars);
		bool IsWhiteSpace();
	}
}
