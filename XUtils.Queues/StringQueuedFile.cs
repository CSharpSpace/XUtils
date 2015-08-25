using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace XUtils.Queues
{
	public class StringQueuedFile : IDisposable
	{
		private StreamRW dataFile;
		private string FileName;
		private int count;
		private int current;
		private int realCurrent;
		private int cursor;
		private int realCursor;
		private object SyObject;
		public int Count
		{
			get
			{
				return this.count;
			}
		}
		public int Current
		{
			get
			{
				return this.current;
			}
		}
		public int RealCurrent
		{
			get
			{
				return this.realCurrent;
			}
		}
		public long Cursor
		{
			get
			{
				return (long)this.cursor;
			}
		}
		public long RealCursor
		{
			get
			{
				return (long)this.realCursor;
			}
		}
		public StringQueuedFile(string fileName)
		{
			this.FileName = fileName;
			this.dataFile = new StreamRW(fileName);
			this.SyObject = new object();
			if (this.dataFile.Length == 0L)
			{
				this.dataFile.WriteFileHeader(0, 0, 0);
				this.count = (this.current = 0);
				this.dataFile.Seek(0L, SeekOrigin.End);
				this.realCursor = (this.cursor = (int)this.dataFile.Position);
				return;
			}
			FileHeader fileHeader = this.dataFile.ReadFileHeader();
			this.count = fileHeader.Count;
			this.realCurrent = (this.current = fileHeader.Current);
			this.realCursor = (this.cursor = fileHeader.Position);
			this.dataFile.Seek((long)this.cursor, SeekOrigin.Begin);
		}
		public static StringQueuedFile CreateFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			return new StringQueuedFile(fileName);
		}
		public static StringQueuedFile CreateFile(string fileName, string[] items)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			StringQueuedFile stringQueuedFile = new StringQueuedFile(fileName);
			for (int i = 0; i < items.Length; i++)
			{
				stringQueuedFile.Enqueue(items[i]);
			}
			return stringQueuedFile;
		}
		public void Enqueue(string str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				this.dataFile.AppendStringItem(str);
				this.count++;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public void Enqueue(string[] str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				for (int i = 0; i < str.Length; i++)
				{
					string item = str[i];
					this.dataFile.AppendStringItem(item);
				}
				this.count += str.Length;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public void Enqueue(List<string> str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				for (int i = 0; i < str.Count; i++)
				{
					string item = str[i];
					this.dataFile.AppendStringItem(item);
				}
				this.count += str.Count;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public List<string> Dequeue(int n)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			List<string> result;
			try
			{
				this.UpdateState();
				List<string> list = new List<string>(n);
				int num = 0;
				while (true)
				{
					StringEntry stringEntry = this.dataFile.ReadStringEntry();
					if (stringEntry == null)
					{
						break;
					}
					list.Add(stringEntry.Value);
					num++;
					this.realCurrent++;
					this.realCursor = (int)this.dataFile.Position;
					if (num == n)
					{
						goto Block_4;
					}
				}
				result = list;
				return result;
				Block_4:
				result = list;
			}
			finally
			{
				Monitor.Exit(syObject);
			}
			return result;
		}
		public List<string> ShadowDequeueAll()
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			List<string> result;
			try
			{
				List<string> list = new List<string>();
				using (StreamRW streamRW = new StreamRW(this.FileName))
				{
					streamRW.Seek(12L, SeekOrigin.Begin);
					while (true)
					{
						StringEntry stringEntry = streamRW.ReadStringEntry();
						if (stringEntry == null)
						{
							break;
						}
						list.Add(stringEntry.Value);
					}
					result = list;
				}
			}
			finally
			{
				Monitor.Exit(syObject);
			}
			return result;
		}
		public void UpdateState()
		{
			if (this.realCursor != this.cursor)
			{
				this.cursor = this.realCursor;
				this.current = this.realCurrent;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
		}
		public void Dispose()
		{
			this.dataFile.Dispose();
		}
	}
}
