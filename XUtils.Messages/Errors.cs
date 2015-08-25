using System;
using System.Collections.Generic;
namespace XUtils.Messages
{
	public class Errors : Messages, IErrors, IMessages
	{
		[Obsolete("Use MessageList")]
		public IList<string> ErrorList
		{
			get
			{
				return this._messageList;
			}
			set
			{
				this._messageList = value;
			}
		}
		[Obsolete("Use MessageMap")]
		public IDictionary<string, string> ErrorMap
		{
			get
			{
				return this._messageMap;
			}
			set
			{
				this._messageMap = value;
			}
		}
	}
}
