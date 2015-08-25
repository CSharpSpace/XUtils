using System;
using System.Collections;
using System.IO;
using System.Text;
namespace XUtils.Serialization
{
	public class MarshallingMethods
	{
		public static Hashtable ReadMethods;
		public static Hashtable WriteMethods;
		static MarshallingMethods()
		{
			MarshallingMethods.ReadMethods = new Hashtable();
			MarshallingMethods.WriteMethods = new Hashtable();
			MarshallingMethods.ReadMethods.Add(typeof(bool), typeof(BinaryReader).GetMethod("ReadBoolean"));
			MarshallingMethods.ReadMethods.Add(typeof(byte), typeof(BinaryReader).GetMethod("ReadByte"));
			MarshallingMethods.ReadMethods.Add(typeof(sbyte), typeof(BinaryReader).GetMethod("ReadSByte"));
			MarshallingMethods.ReadMethods.Add(typeof(float), typeof(BinaryReader).GetMethod("ReadSingle"));
			MarshallingMethods.ReadMethods.Add(typeof(byte[]), typeof(BinaryReader).GetMethod("ReadBytes"));
			MarshallingMethods.ReadMethods.Add(typeof(char[]), typeof(BinaryReader).GetMethod("ReadChars"));
			MarshallingMethods.ReadMethods.Add(typeof(short), typeof(BinaryReader).GetMethod("ReadInt16"));
			MarshallingMethods.ReadMethods.Add(typeof(int), typeof(BinaryReader).GetMethod("ReadInt32"));
			MarshallingMethods.ReadMethods.Add(typeof(ushort), typeof(BinaryReader).GetMethod("ReadUInt16"));
			MarshallingMethods.ReadMethods.Add(typeof(uint), typeof(BinaryReader).GetMethod("ReadUInt32"));
			MarshallingMethods.ReadMethods.Add(typeof(string), typeof(MarshallingMethods).GetMethod("ReadString"));
			MarshallingMethods.ReadMethods.Add(typeof(DateTime), typeof(MarshallingMethods).GetMethod("ReadDateTime"));
			MarshallingMethods.ReadMethods.Add(typeof(short[]), typeof(MarshallingMethods).GetMethod("ReadInt16Array"));
			MarshallingMethods.ReadMethods.Add(typeof(int[]), typeof(MarshallingMethods).GetMethod("ReadInt32Array"));
			MarshallingMethods.ReadMethods.Add(typeof(ushort[]), typeof(MarshallingMethods).GetMethod("ReadUInt16Array"));
			MarshallingMethods.ReadMethods.Add(typeof(uint[]), typeof(MarshallingMethods).GetMethod("ReadUInt32Array"));
			MarshallingMethods.ReadMethods.Add(typeof(CustomMarshaler), typeof(CustomMarshaler).GetMethod("ReadFromStream"));
			MarshallingMethods.WriteMethods.Add(typeof(bool), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(bool)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(byte), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(byte)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(sbyte), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(sbyte)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(float), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(float)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(short), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(short)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(int), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(int)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(ushort), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(ushort)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(uint), typeof(BinaryWriter).GetMethod("Write", new Type[]
			{
				typeof(uint)
			}));
			MarshallingMethods.WriteMethods.Add(typeof(string), typeof(MarshallingMethods).GetMethod("WriteString"));
			MarshallingMethods.WriteMethods.Add(typeof(CustomMarshaler), typeof(CustomMarshaler).GetMethod("WriteToStream"));
			MarshallingMethods.WriteMethods.Add(typeof(bool[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(bool[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(char[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(char[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(short[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(short[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(ushort[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(ushort[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(int[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(int[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(uint[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(uint[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(long[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(long[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(ulong[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(ulong[])
			}));
			MarshallingMethods.WriteMethods.Add(typeof(float[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[]
			{
				typeof(BinaryWriter),
				typeof(float[])
			}));
		}
		public static short[] ReadInt16Array(BinaryReader reader, int count)
		{
			short[] array = new short[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = reader.ReadInt16();
			}
			return array;
		}
		public static int[] ReadInt32Array(BinaryReader reader, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = reader.ReadInt32();
			}
			return array;
		}
		public static ushort[] ReadUInt16Array(BinaryReader reader, int count)
		{
			ushort[] array = new ushort[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = reader.ReadUInt16();
			}
			return array;
		}
		public static uint[] ReadUInt32Array(BinaryReader reader, int count)
		{
			uint[] array = new uint[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = reader.ReadUInt32();
			}
			return array;
		}
		public static string ReadString(BinaryReader reader, int count)
		{
			if (count == 0)
			{
				count = 255;
			}
			char[] value = reader.ReadChars(count);
			string arg_26_0 = new string(value);
			char[] trimChars = new char[1];
			return arg_26_0.TrimEnd(trimChars);
		}
		public static void WriteString(BinaryWriter writer, string value, int size)
		{
			if (value != null)
			{
				byte[] bytes = Encoding.Unicode.GetBytes(value.Substring(0, size));
				writer.Write(bytes);
			}
		}
		public static DateTime ReadDateTime(BinaryReader reader)
		{
			return DateTime.FromFileTime(reader.ReadInt64());
		}
		public static void WriteArray(BinaryWriter writer, bool[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, char[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, byte[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, short[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, ushort[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, int[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, uint[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, long[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, ulong[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteArray(BinaryWriter writer, float[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				writer.Write(arr[i]);
			}
		}
		public static void WriteSerializers(BinaryWriter writer, CustomMarshaler[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i].WriteToStream(writer);
			}
		}
	}
}
