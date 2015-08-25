using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	internal class SynchronizedDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _dictionary;
		private readonly object _lock;
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}
		public object SyncRoot
		{
			get
			{
				return this._lock;
			}
		}
		public TValue this[TKey key]
		{
			get
			{
				object @lock;
				Monitor.Enter(@lock = this._lock);
				TValue result;
				try
				{
					result = this._dictionary[key];
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				return result;
			}
			set
			{
				object @lock;
				Monitor.Enter(@lock = this._lock);
				try
				{
					this._dictionary[key] = value;
				}
				finally
				{
					Monitor.Exit(@lock);
				}
			}
		}
		public Dictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				object @lock;
				Monitor.Enter(@lock = this._lock);
				Dictionary<TKey, TValue>.KeyCollection keys;
				try
				{
					keys = this._dictionary.Keys;
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				return keys;
			}
		}
		public Dictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				object @lock;
				Monitor.Enter(@lock = this._lock);
				Dictionary<TKey, TValue>.ValueCollection values;
				try
				{
					values = this._dictionary.Values;
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				return values;
			}
		}
		public SynchronizedDictionary()
		{
			this._lock = new object();
			this._dictionary = new Dictionary<TKey, TValue>();
		}
		public bool Contains(TKey key)
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			bool result;
			try
			{
				result = this._dictionary.ContainsKey(key);
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public void Remove(TKey key)
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			try
			{
				this._dictionary.Remove(key);
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public void Clear()
		{
			object @lock;
			Monitor.Enter(@lock = this._lock);
			try
			{
				this._dictionary.Clear();
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
	}
}
