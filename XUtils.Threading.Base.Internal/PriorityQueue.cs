using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Threading.Base.Internal
{
	public sealed class PriorityQueue : IEnumerable
	{
		private class PriorityQueueEnumerator : IEnumerator
		{
			private readonly PriorityQueue _priorityQueue;
			private int _version;
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
				this._version = this._priorityQueue._version;
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
				this._version = this._priorityQueue._version;
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
				if (this._version != this._priorityQueue._version)
				{
					throw new InvalidOperationException("The collection has been modified");
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
		private const int _queuesCount = 5;
		private readonly LinkedList<IHasWorkItemPriority>[] _queues = new LinkedList<IHasWorkItemPriority>[5];
		private int _workItemsCount;
		private int _version;
		public int Count
		{
			get
			{
				return this._workItemsCount;
			}
		}
		public PriorityQueue()
		{
			for (int i = 0; i < this._queues.Length; i++)
			{
				this._queues[i] = new LinkedList<IHasWorkItemPriority>();
			}
		}
		public void Enqueue(IHasWorkItemPriority workItem)
		{
			int num = 5 - (int)workItem.WorkItemPriority - (int)WorkItemPriority.BelowNormal;
			this._queues[num].AddLast(workItem);
			this._workItemsCount++;
			this._version++;
		}
		public IHasWorkItemPriority Dequeue()
		{
			IHasWorkItemPriority result = null;
			if (this._workItemsCount > 0)
			{
				int nextNonEmptyQueue = this.GetNextNonEmptyQueue(-1);
				result = this._queues[nextNonEmptyQueue].First.Value;
				this._queues[nextNonEmptyQueue].RemoveFirst();
				this._workItemsCount--;
				this._version++;
			}
			return result;
		}
		private int GetNextNonEmptyQueue(int queueIndex)
		{
			for (int i = queueIndex + 1; i < 5; i++)
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
				LinkedList<IHasWorkItemPriority>[] queues = this._queues;
				for (int i = 0; i < queues.Length; i++)
				{
					LinkedList<IHasWorkItemPriority> linkedList = queues[i];
					linkedList.Clear();
				}
				this._workItemsCount = 0;
				this._version++;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return new PriorityQueue.PriorityQueueEnumerator(this);
		}
	}
}
