using System;
using System.Collections.Generic;
namespace XUtils.Web
{
	public class UrlMapper
	{
		private IDictionary<string, string> _urlMappings;
		public UrlMapper(IDictionary<string, string> urlMappings)
		{
			this.Init(urlMappings);
		}
		public string GetUrl(string url)
		{
			string key = url.ToLower();
			if (this._urlMappings.ContainsKey(key))
			{
				return this._urlMappings[key];
			}
			return url;
		}
		private void Init(IDictionary<string, string> urlMappings)
		{
			Guard.IsNotNull(urlMappings, "Url rewrite mappings were not provided.");
			this._urlMappings = new Dictionary<string, string>();
			IEnumerator<KeyValuePair<string, string>> enumerator = urlMappings.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				string text = current.Key.ToLower();
				IDictionary<string, string> arg_48_0 = this._urlMappings;
				string arg_48_1 = text;
				KeyValuePair<string, string> current2 = enumerator.Current;
				arg_48_0.Add(arg_48_1, current2.Value);
			}
		}
	}
}
