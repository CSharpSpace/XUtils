using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Configuration
{
	public abstract class ConfigContainer<TKey, TValue>
	{
		private static object syncObject = new object();
		public readonly IDictionary<TKey, TValue> Container = new Dictionary<TKey, TValue>();
		public TValue this[TKey key]
		{
			get
			{
				return this.Get(key);
			}
		}
		public void Opera(ConfigMode mode, TKey Key, TValue value)
		{
			object obj;
			Monitor.Enter(obj = ConfigContainer<TKey, TValue>.syncObject);
			try
			{
				switch (mode)
				{
				case ConfigMode.Insert:
					if (!this.Container.ContainsKey(Key))
					{
						this.Container.Add(Key, value);
					}
					break;
				case ConfigMode.Update:
					if (this.Container.ContainsKey(Key))
					{
						this.Container[Key] = value;
					}
					break;
				case ConfigMode.UpSet:
					if (this.Container.ContainsKey(Key))
					{
						this.Container[Key] = value;
					}
					else
					{
						this.Container.Add(Key, value);
					}
					break;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public abstract void Load();
		public abstract bool Load(TKey Key);
		public void Remove(TKey Key)
		{
			if (this.Container.ContainsKey(Key))
			{
				this.Container.Remove(Key);
			}
		}
		public void Clear()
		{
			object obj;
			Monitor.Enter(obj = ConfigContainer<TKey, TValue>.syncObject);
			try
			{
				this.Container.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public TValue Get(TKey Key)
		{
			if (this.Container.ContainsKey(Key))
			{
				return this.Container[Key];
			}
			if (!this.Load(Key))
			{
				return default(TValue);
			}
			return this.Container[Key];
		}
	}
}
