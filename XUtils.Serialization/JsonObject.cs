using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
namespace XUtils.Serialization
{
	[Serializable]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IDictionary, ICollection, IEnumerable, IXmlSerializable
	{
		private readonly List<string> _orderedKeys;
		private readonly Dictionary<string, object> _dictionary;
		private readonly IComparer<string> _keyComparer;
		public object this[string key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				this.Set(key, value);
			}
		}
		ICollection IDictionary.Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}
		ICollection IDictionary.Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}
		public ICollection<object> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}
		public ICollection<string> Keys
		{
			get
			{
				return this._orderedKeys.AsReadOnly();
			}
		}
		object IDictionary.this[object key]
		{
			get
			{
				return this.Get(Convert.ToString(key));
			}
			set
			{
				this.Set(Convert.ToString(key), value);
			}
		}
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}
		object ICollection.SyncRoot
		{
			get
			{
				return this._orderedKeys;
			}
		}
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}
		public JsonObject()
		{
			this._dictionary = new Dictionary<string, object>();
			this._orderedKeys = new List<string>();
		}
		public JsonObject(IComparer<string> keyComparer) : this()
		{
			if (keyComparer == null)
			{
				throw new ArgumentNullException("keyComparer");
			}
			this._keyComparer = keyComparer;
		}
		public JsonObject(string key, object value) : this()
		{
			this.Add(key, value);
		}
		public JsonObject(IEnumerable<KeyValuePair<string, object>> dictionary) : this()
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			foreach (KeyValuePair<string, object> current in dictionary)
			{
				this.Add(current.Key, current.Value);
			}
		}
		public object Get(string key)
		{
			object result;
			if (!this._dictionary.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}
		public T Get<T>(string key)
		{
			object obj = this.Get(key);
			if (obj == null)
			{
				return default(T);
			}
			return (T)((object)Convert.ChangeType(obj, typeof(T)));
		}
		public bool TryGetValue(string key, out object value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}
		public bool ContainsKey(string key)
		{
			return this._dictionary.ContainsKey(key);
		}
		public JsonObject Add(string key, object value)
		{
			this._dictionary.Add(key, value);
			this._orderedKeys.Add(key);
			this.EnsureKeyOrdering();
			return this;
		}
		void IDictionary<string, object>.Add(string key, object value)
		{
			this.Add(key, value);
		}
		public JsonObject Set(string key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!this._orderedKeys.Contains(key))
			{
				this._orderedKeys.Add(key);
			}
			this._dictionary[key] = value;
			this.EnsureKeyOrdering();
			return this;
		}
		public void Insert(string key, object value, int position)
		{
			this._dictionary.Add(key, value);
			this._orderedKeys.Insert(position, key);
			this.EnsureKeyOrdering();
		}
		public JsonObject Prepend(string key, object value)
		{
			this.Insert(key, value, 0);
			return this;
		}
		public JsonObject Merge(JsonObject source)
		{
			if (source == null)
			{
				return this;
			}
			foreach (string current in source.Keys)
			{
				this[current] = source[current];
			}
			return this;
		}
		public bool Remove(string key)
		{
			this._orderedKeys.Remove(key);
			return this._dictionary.Remove(key);
		}
		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.Add(item.Key, item.Value);
		}
		bool IDictionary.Contains(object key)
		{
			return this._orderedKeys.Contains(Convert.ToString(key));
		}
		void IDictionary.Add(object key, object value)
		{
			this.Add(Convert.ToString(key), value);
		}
		public void Clear()
		{
			this._dictionary.Clear();
			this._orderedKeys.Clear();
		}
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)this._dictionary).GetEnumerator();
		}
		void IDictionary.Remove(object key)
		{
			this.Remove(Convert.ToString(key));
		}
		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)this._dictionary).Contains(item);
		}
		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)this._dictionary).CopyTo(array, arrayIndex);
		}
		public void CopyTo(JsonObject destinationDocument)
		{
			if (destinationDocument == null)
			{
				throw new ArgumentNullException("destinationDocument");
			}
			foreach (string current in this._orderedKeys)
			{
				if (destinationDocument.ContainsKey(current))
				{
					destinationDocument.Remove(current);
				}
				destinationDocument[current] = this[current];
			}
		}
		public bool Remove(KeyValuePair<string, object> item)
		{
			bool flag = ((ICollection<KeyValuePair<string, object>>)this._dictionary).Remove(item);
			if (flag)
			{
				this._orderedKeys.Remove(item.Key);
			}
			return flag;
		}
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this._dictionary).CopyTo(array, index);
		}
		public override bool Equals(object obj)
		{
			if (obj is JsonObject)
			{
				return this.Equals(obj as JsonObject);
			}
			return base.Equals(obj);
		}
		public bool Equals(JsonObject document)
		{
			return document != null && this._orderedKeys.Count == document._orderedKeys.Count && this.GetHashCode() == document.GetHashCode();
		}
		public override int GetHashCode()
		{
			int num = 27;
			foreach (string current in this._orderedKeys)
			{
				int valueHashCode = this.GetValueHashCode(this[current]);
				num = 13 * num + current.GetHashCode();
				num = 13 * num + valueHashCode;
			}
			return num;
		}
		private int GetValueHashCode(object value)
		{
			if (value == null)
			{
				return 0;
			}
			if (!(value is Array))
			{
				return value.GetHashCode();
			}
			return this.GetArrayHashcode((Array)value);
		}
		private int GetArrayHashcode(Array array)
		{
			int num = 0;
			foreach (object current in array)
			{
				int valueHashCode = this.GetValueHashCode(current);
				num = 13 * num + valueHashCode;
			}
			return num;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return (
				from orderedKey in this._orderedKeys
				select new KeyValuePair<string, object>(orderedKey, this._dictionary[orderedKey])).GetEnumerator();
		}
		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>(this);
		}
		public static JsonObject Parse(string json)
		{
			IDictionary dictionary = JsonConvert.Decode(json) as IDictionary;
			if (dictionary != null)
			{
				IEnumerator enumerator = dictionary.GetEnumerator();
				return JsonParser.ParseRawObject(enumerator);
			}
			return new JsonObject();
		}
		public override string ToString()
		{
			return JsonFormatter.Serialize(this);
		}
		private void EnsureKeyOrdering()
		{
			if (this._keyComparer == null)
			{
				return;
			}
			this._orderedKeys.Sort(this._keyComparer);
		}
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.IsStartElement())
			{
				string name = reader.Name;
				object value = null;
				if (reader.MoveToAttribute("type"))
				{
					Type type = Type.GetType(reader.Value);
					reader.ReadStartElement();
					XmlSerializer xmlSerializer = new XmlSerializer(type);
					value = xmlSerializer.Deserialize(reader);
				}
				else
				{
					reader.Read();
				}
				this.Add(name, value);
			}
		}
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (KeyValuePair<string, object> current in this)
			{
				writer.WriteStartElement(current.Key);
				if (current.Value != null)
				{
					Type type = current.Value.GetType();
					writer.WriteAttributeString("type", type.AssemblyQualifiedName);
					XmlSerializer xmlSerializer = new XmlSerializer(type);
					xmlSerializer.Serialize(writer, current.Value);
				}
			}
		}
	}
}
