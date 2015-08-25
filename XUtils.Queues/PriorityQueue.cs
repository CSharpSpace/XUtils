using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace XUtils.Queues
{
	public sealed class PriorityQueue : IEnumerable
	{
		private class PriorityQueueEnumerator : IEnumerator
		{
			private readonly PriorityQueue _priorityQueue;
			private int _queueIndex;
			private IEnumerator _enumerator;
			public object Current
			{
				get
				{
					return this._enumerator.Current;
				}
			}
			public PriorityQueueEnumerator(PriorityQueue priorityQueue)
			{
				this._priorityQueue = priorityQueue;
				this._queueIndex = this._priorityQueue.GetNextNonEmptyQueue(-1);
				if (this._queueIndex >= 0)
				{
					this._enumerator = this._priorityQueue._queues[this._queueIndex].GetEnumerator();
					return;
				}
				this._enumerator = null;
			}
			public void Reset()
			{
				this._queueIndex = this._priorityQueue.GetNextNonEmptyQueue(-1);
				if (this._queueIndex >= 0)
				{
					this._enumerator = this._priorityQueue._queues[this._queueIndex].GetEnumerator();
					return;
				}
				this._enumerator = null;
			}
			public bool MoveNext()
			{
				if (this._enumerator == null)
				{
					return false;
				}
				if (this._enumerator.MoveNext())
				{
					return true;
				}
				this._queueIndex = this._priorityQueue.GetNextNonEmptyQueue(this._queueIndex);
				if (-1 == this._queueIndex)
				{
					return false;
				}
				this._enumerator = this._priorityQueue._queues[this._queueIndex].GetEnumerator();
				this._enumerator.MoveNext();
				return true;
			}
		}
		private const int _queuesCount = 10;
		private static object syncLock = new object();
		private readonly LinkedList<IPriority>[] _queues = new LinkedList<IPriority>[10];
		private int _workItemsCount;
		public int Count
		{
			get
			{
				object obj;
				Monitor.Enter(obj = PriorityQueue.syncLock);
				int workItemsCount;
				try
				{
					workItemsCount = this._workItemsCount;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return workItemsCount;
			}
		}
		public PriorityQueue()
		{
			for (int i = 0; i < this._queues.Length; i++)
			{
				this._queues[i] = new LinkedList<IPriority>();
			}
		}
		public void Enqueue(IPriority workItem)
		{
			object obj;
			Monitor.Enter(obj = PriorityQueue.syncLock);
			try
			{
				int num = ((PriorityEnums)10 - workItem.Priority - PriorityEnums.Level_1);
				this._queues[num].AddLast(workItem);
				this._workItemsCount++;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public IPriority Dequeue()
		{
			object obj;
			Monitor.Enter(obj = PriorityQueue.syncLock);
			IPriority result;
			try
			{
				IPriority priority = null;
				if (this._workItemsCount > 0)
				{
					int nextNonEmptyQueue = this.GetNextNonEmptyQueue(-1);
					priority = this._queues[nextNonEmptyQueue].First.Value;
					this._queues[nextNonEmptyQueue].RemoveFirst();
					this._workItemsCount--;
				}
				result = priority;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		private int GetNextNonEmptyQueue(int queueIndex)
		{
			for (int i = queueIndex + 1; i < 10; i++)
			{
				if (this._queues[i].Count > 0)
				{
					return i;
				}
			}
			return -1;
		}
		public void Clear()
		{
			if (this._workItemsCount > 0)
			{
				LinkedList<IPriority>[] queues = this._queues;
				for (int i = 0; i < queues.Length; i++)
				{
					LinkedList<IPriority> linkedList = queues[i];
					linkedList.Clear();
				}
				this._workItemsCount = 0;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return new PriorityQueue.PriorityQueueEnumerator(this);
		}
	}
}
