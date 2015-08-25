using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace XUtils.Queues
{
	internal sealed class StreamRW : FileStream
	{
		public StreamRW(string fileName) : base(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
		{
		}
		public void WriteFileHeader(int position, int current, int count)
		{
			long position2 = this.Position;
			this.Seek(0L, SeekOrigin.Begin);
			byte[] bytes = BitConverter.GetBytes(position);
			this.Write(bytes, 0, 4);
			bytes = BitConverter.GetBytes(current);
			this.Write(bytes, 0, 4);
			bytes = BitConverter.GetBytes(count);
			this.Write(bytes, 0, 4);
			this.Seek(position2, SeekOrigin.Begin);
		}
		public FileHeader ReadFileHeader()
		{
			if (this.Position > 0L)
			{
				this.Seek(0L, SeekOrigin.Begin);
			}
			byte[] array = new byte[12];
			this.Read(array, 0, 12);
			return new FileHeader
			{
				Position = BitConverter.ToInt32(array, 0),
				Current = BitConverter.ToInt32(array, 4),
				Count = BitConverter.ToInt32(array, 8)
			};
		}
		public void AppendStringItem(string item)
		{
			long position = this.Position;
			this.Seek(0L, SeekOrigin.End);
			Encoding uTF = Encoding.UTF8;
			byte[] bytes = uTF.GetBytes(item);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			this.Write(bytes2, 0, bytes2.Length);
			this.Write(bytes, 0, bytes.Length);
			this.Seek(position, SeekOrigin.Begin);
		}
		public void FastAppendStringItem(string item)
		{
			Encoding uTF = Encoding.UTF8;
			byte[] bytes = uTF.GetBytes(item);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			this.Write(bytes2, 0, bytes2.Length);
			this.Write(bytes, 0, bytes.Length);
		}
		public void AppendBinaryItem(byte[] item)
		{
			long position = this.Position;
			this.Seek(0L, SeekOrigin.End);
			byte[] bytes = BitConverter.GetBytes(item.Length);
			this.Write(bytes, 0, bytes.Length);
			this.Write(item, 0, item.Length);
			this.Seek(position, SeekOrigin.Begin);
		}
		public void FastAppendStringItem(byte[] item)
		{
			byte[] bytes = BitConverter.GetBytes(item.Length);
			this.Write(bytes, 0, bytes.Length);
			this.Write(item, 0, item.Length);
		}
		public StringEntry ReadStringEntry()
		{
			if (this.Position == this.Length)
			{
				return null;
			}
			StringEntry stringEntry = new StringEntry();
			stringEntry.Pos = this.Position;
			byte[] array = new byte[4];
			this.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			array = new byte[num];
			this.Read(array, 0, array.Length);
			stringEntry.Value = this.ReadString(array);
			return stringEntry;
		}
		public BinaryEntry ReadBinaryEntry()
		{
			if (this.Position == this.Length)
			{
				return null;
			}
			BinaryEntry binaryEntry = new BinaryEntry();
			binaryEntry.Pos = this.Position;
			byte[] array = new byte[4];
			this.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			array = new byte[num];
			this.Read(array, 0, array.Length);
			binaryEntry.Value = array;
			return binaryEntry;
		}
		public LinkedList<StringEntry> ReadAllString()
		{
			LinkedList<StringEntry> linkedList = new LinkedList<StringEntry>();
			long position = this.Position;
			this.Seek(12L, SeekOrigin.Begin);
			while (this.Position < this.Length)
			{
				linkedList.AddLast(this.ReadStringEntry());
			}
			this.Seek(position, SeekOrigin.Begin);
			return linkedList;
		}
		public LinkedList<BinaryEntry> ReadAllBinary()
		{
			LinkedList<BinaryEntry> linkedList = new LinkedList<BinaryEntry>();
			long position = this.Position;
			this.Seek(12L, SeekOrigin.Begin);
			while (this.Position < this.Length)
			{
				linkedList.AddLast(this.ReadBinaryEntry());
			}
			this.Seek(position, SeekOrigin.Begin);
			return linkedList;
		}
		public Queue<string> ReadAllStringInQueue()
		{
			Queue<string> queue = new Queue<string>();
			long position = this.Position;
			this.Seek(12L, SeekOrigin.Begin);
			while (this.Position < this.Length)
			{
				StringEntry stringEntry = this.ReadStringEntry();
				queue.Enqueue(stringEntry.Value);
			}
			this.Seek(position, SeekOrigin.Begin);
			return queue;
		}
		public Queue<byte[]> ReadAllBinaryInQueue()
		{
			Queue<byte[]> queue = new Queue<byte[]>();
			long position = this.Position;
			this.Seek(12L, SeekOrigin.Begin);
			while (this.Position < this.Length)
			{
				BinaryEntry binaryEntry = this.ReadBinaryEntry();
				queue.Enqueue(binaryEntry.Value);
			}
			this.Seek(position, SeekOrigin.Begin);
			return queue;
		}
		private string ReadString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}
		private string ReadString(byte[] bytes, int index, int count)
		{
			return Encoding.UTF8.GetString(bytes, index, count);
		}
	}
}
