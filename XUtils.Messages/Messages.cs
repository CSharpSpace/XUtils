using System;
using System.Collections.Generic;
using System.Text;
namespace XUtils.Messages
{
	public class Messages : IMessages
	{
		protected IDictionary<string, string> _messageMap;
		protected IList<string> _messageList;
		public int Count
		{
			get
			{
				int num = 0;
				if (this._messageList != null)
				{
					num += this._messageList.Count;
				}
				if (this._messageMap != null)
				{
					num += this._messageMap.Count;
				}
				return num;
			}
		}
		public bool HasAny
		{
			get
			{
				return this.Count > 0;
			}
		}
		public IList<string> MessageList
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
		public IDictionary<string, string> MessageMap
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
		public void Add(string key, string error)
		{
			if (this._messageMap == null)
			{
				this._messageMap = new Dictionary<string, string>();
			}
			if (string.IsNullOrEmpty(key))
			{
				this.Add(error);
				return;
			}
			this._messageMap[key] = error;
		}
		public void Add(string key, string format, params object[] args)
		{
			this.Add(key, string.Format(format, args));
		}
		public void Add(string error)
		{
			if (this._messageList == null)
			{
				this._messageList = new List<string>();
			}
			this._messageList.Add(error);
		}
		public void Add(string format, params object[] args)
		{
			this.Add(string.Format(format, args));
		}
		public void Each(Action<string, string> callback)
		{
			if (this._messageMap == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> current in this._messageMap)
			{
				callback(current.Key, current.Value);
			}
		}
		public void EachFull(Action<string> callback)
		{
			if (this._messageMap != null)
			{
				foreach (KeyValuePair<string, string> current in this._messageMap)
				{
					string obj = current.Key + " " + current.Value;
					callback(obj);
				}
			}
			if (this._messageList != null)
			{
				foreach (string current2 in this._messageList)
				{
					callback(current2);
				}
			}
		}
		public string Message()
		{
			return this.Message(Environment.NewLine);
		}
		public string Message(string separator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this._messageList != null)
			{
				foreach (string current in this._messageList)
				{
					stringBuilder.Append(current + separator);
				}
			}
			if (this._messageMap != null)
			{
				foreach (KeyValuePair<string, string> current2 in this._messageMap)
				{
					string str = current2.Key + " " + current2.Value;
					stringBuilder.Append(str + separator);
				}
			}
			return stringBuilder.ToString();
		}
		public void Clear()
		{
			if (this._messageMap != null)
			{
				this._messageMap.Clear();
			}
			if (this._messageList != null)
			{
				this._messageList.Clear();
			}
		}
		public string On(string key)
		{
			if (this._messageMap != null && this._messageMap.ContainsKey(key))
			{
				return this._messageMap[key];
			}
			return string.Empty;
		}
		public IList<string> On()
		{
			return this._messageList ?? null;
		}
		public void CopyTo(IMessages messages)
		{
			if (messages == null)
			{
				return;
			}
			if (this._messageList != null)
			{
				foreach (string current in this._messageList)
				{
					messages.Add(current);
				}
			}
			if (this._messageMap != null)
			{
				foreach (KeyValuePair<string, string> current2 in this._messageMap)
				{
					messages.Add(current2.Key, current2.Value);
				}
			}
		}
	}
}
