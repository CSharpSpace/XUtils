using System;
namespace XUtils.Caching
{
	public class CacheItemDescriptor
	{
		private string _key;
		private string _type;
		private string _serializedData = string.Empty;
		public string Key
		{
			get
			{
				return this._key;
			}
		}
		public string ItemType
		{
			get
			{
				return this._type;
			}
		}
		public string Data
		{
			get
			{
				return this._serializedData;
			}
		}
		public CacheItemDescriptor(string key, string type) : this(key, type, string.Empty)
		{
		}
		public CacheItemDescriptor(string key, string type, string serializedData)
		{
			this._key = key;
			this._type = type;
			this._serializedData = serializedData;
		}
	}
}
