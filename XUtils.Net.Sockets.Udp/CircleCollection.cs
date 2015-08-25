using System;
using System.Collections.Generic;
namespace XUtils.Net.Sockets.Udp
{
	public class CircleCollection<T>
	{
		private Queue<T> m_pItems;
		public int Count
		{
			get
			{
				return this.m_pItems.Count;
			}
		}
		public CircleCollection()
		{
			this.m_pItems = new Queue<T>();
		}
		public void Add(T[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				T item = items[i];
				this.Add(item);
			}
		}
		public void Add(T item)
		{
			this.m_pItems.Enqueue(item);
		}
		public void Clear()
		{
			this.m_pItems.Clear();
		}
		public T Next()
		{
			T t = this.m_pItems.Dequeue();
			this.m_pItems.Enqueue(t);
			return t;
		}
	}
}
