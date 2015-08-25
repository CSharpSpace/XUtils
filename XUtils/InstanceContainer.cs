using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace XUtils
{
	public class InstanceContainer
	{
		private static object syncObject = new object();
		private Dictionary<string, object> Container;
		public int Length
		{
			get
			{
				return this.Container.Count;
			}
		}
		public object this[string key]
		{
			get
			{
				return this.Container[key];
			}
		}
		public string[] Keys
		{
			get
			{
				return this.Container.Keys.ToArray<string>();
			}
		}
		public object[] Values
		{
			get
			{
				return this.Container.Values.ToArray<object>();
			}
		}
		public InstanceContainer()
		{
			this.Container = new Dictionary<string, object>();
		}
		public T Cast<T>(string key) where T : class, new()
		{
			object obj;
			Monitor.Enter(obj = InstanceContainer.syncObject);
			T result;
			try
			{
				object obj2 = null;
				if (!this.Container.TryGetValue(key, out obj2))
				{
					obj2 = Activator.CreateInstance<T>();
					this.Container.Add(key, obj2);
				}
				result = (T)((object)obj2);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public bool Remove(string key)
		{
			object obj;
			Monitor.Enter(obj = InstanceContainer.syncObject);
			bool result;
			try
			{
				result = this.Container.Remove(key);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public void Clear()
		{
			this.Container.Clear();
		}
	}
}
