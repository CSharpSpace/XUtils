using System;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Schedule
{
	public class TaskContainer
	{
		private static object syncObject = new object();
		private static IDictionary<string, Task> container = new Dictionary<string, Task>();
		public KeyValuePair<string, Task>[] Tasks
		{
			get
			{
				object obj;
				Monitor.Enter(obj = TaskContainer.syncObject);
				KeyValuePair<string, Task>[] result;
				try
				{
					KeyValuePair<string, Task>[] array = new KeyValuePair<string, Task>[TaskContainer.container.Count];
					TaskContainer.container.CopyTo(array, 0);
					result = array;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return result;
			}
		}
		public void Add(string key, Task task)
		{
			object obj;
			Monitor.Enter(obj = TaskContainer.syncObject);
			try
			{
				if (!TaskContainer.container.ContainsKey(key))
				{
					TaskContainer.container.Add(key, task);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Remove(string key)
		{
			object obj;
			Monitor.Enter(obj = TaskContainer.syncObject);
			try
			{
				if (TaskContainer.container.ContainsKey(key))
				{
					TaskContainer.container.Remove(key);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Clear()
		{
			object obj;
			Monitor.Enter(obj = TaskContainer.syncObject);
			try
			{
				TaskContainer.container.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public DateTime NextRunTime(DateTime time)
		{
			DateTime dateTime = DateTime.MaxValue;
			foreach (Task current in TaskContainer.container.Values)
			{
				DateTime dateTime2 = current.NextRunTime(time, true);
				dateTime = ((dateTime2 < dateTime) ? dateTime2 : dateTime);
			}
			return dateTime;
		}
	}
}
