using System;
using System.Collections.Generic;
using System.Text;
namespace XUtils
{
	public class UrlArguments
	{
		private IDictionary<string, object> Args = new Dictionary<string, object>();
		public string Url;
		public UrlArguments()
		{
		}
		public UrlArguments(string url)
		{
			this.Url = url;
		}
		public UrlArguments(string url, params KeyValuePair<string, object>[] keyValues)
		{
			this.Url = url;
			for (int i = 0; i < keyValues.Length; i++)
			{
				KeyValuePair<string, object> keyValuePair = keyValues[i];
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		public UrlArguments SetUrl(string url)
		{
			this.Url = url;
			return this;
		}
		public UrlArguments Add(string key, object value)
		{
			if (!this.Args.ContainsKey(key))
			{
				this.Args.Add(key, value);
			}
			return this;
		}
		public UrlArguments Remove(string key)
		{
			if (this.Args != null && this.Args.ContainsKey(key))
			{
				this.Args.Remove(key);
			}
			return this;
		}
		public UrlArguments Clear()
		{
			this.Args.Clear();
			this.Url = string.Empty;
			return this;
		}
		public UrlArguments Complete()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}?", this.Url);
			foreach (KeyValuePair<string, object> current in this.Args)
			{
				stringBuilder.AppendFormat("{0}={1}&", current.Key, current.Value);
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			this.Url = stringBuilder.ToString();
			return this;
		}
		public override string ToString()
		{
			if (this.Url.IndexOf("?") == -1)
			{
				return this.Complete().Url;
			}
			return this.Url;
		}
	}
}
