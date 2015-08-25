using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
namespace XUtils.Queues
{
	public class BinaryQueuedFile : IDisposable
	{
		private StreamRW dataFile;
		private string FileName;
		private int count;
		private int current;
		private int realCurrent;
		private int cursor;
		private int realCursor;
		private object SyObject;
		private BinaryFormatter mSerialize;
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
		public BinaryQueuedFile(string fileName)
		{
			this.mSerialize = new BinaryFormatter();
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
		public static BinaryQueuedFile CreateFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			return new BinaryQueuedFile(fileName);
		}
		public static BinaryQueuedFile CreateFile(string fileName, string[] items)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			BinaryQueuedFile binaryQueuedFile = new BinaryQueuedFile(fileName);
			for (int i = 0; i < items.Length; i++)
			{
				binaryQueuedFile.Enqueue(items[i]);
			}
			return binaryQueuedFile;
		}
		public void Enqueue(object str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				this.dataFile.AppendBinaryItem(this.Serialize(str));
				this.count++;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public void Enqueue(object[] str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				for (int i = 0; i < str.Length; i++)
				{
					byte[] item = this.Serialize(str[i]);
					this.dataFile.AppendBinaryItem(item);
				}
				this.count += str.Length;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public void Enqueue(List<object> str)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			try
			{
				for (int i = 0; i < str.Count; i++)
				{
					byte[] item = this.Serialize(str[i]);
					this.dataFile.AppendBinaryItem(item);
				}
				this.count += str.Count;
				this.dataFile.WriteFileHeader(this.cursor, this.current, this.count);
			}
			finally
			{
				Monitor.Exit(syObject);
			}
		}
		public List<object> Dequeue(int n)
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			List<object> result;
			try
			{
				this.UpdateState();
				List<object> list = new List<object>(n);
				int num = 0;
				while (true)
				{
					BinaryEntry binaryEntry = this.dataFile.ReadBinaryEntry();
					if (binaryEntry == null)
					{
						break;
					}
					list.Add(this.Deserialize(binaryEntry.Value));
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
		public List<object> ShadowDequeueAll()
		{
			object syObject;
			Monitor.Enter(syObject = this.SyObject);
			List<object> result;
			try
			{
				List<object> list = new List<object>();
				using (StreamRW streamRW = new StreamRW(this.FileName))
				{
					streamRW.Seek(12L, SeekOrigin.Begin);
					while (true)
					{
						BinaryEntry binaryEntry = streamRW.ReadBinaryEntry();
						if (binaryEntry == null)
						{
							break;
						}
						list.Add(this.Deserialize(binaryEntry.Value));
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
		private byte[] Serialize(object obj)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.mSerialize.Serialize(memoryStream, obj);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Seek(0L, SeekOrigin.Begin);
				memoryStream.Read(array, 0, array.Length);
				result = array;
			}
			return result;
		}
		private object Deserialize(byte[] bytes)
		{
			object result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				result = this.mSerialize.Deserialize(memoryStream);
			}
			return result;
		}
		public void Dispose()
		{
			this.dataFile.Dispose();
		}
	}
}
