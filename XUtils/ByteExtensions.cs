using System;
using System.Text;
namespace XUtils
{
	public static class ByteExtensions
	{
		public class BytesBuilder
		{
			private StringBuilder builder;
			private static BinHexEncoding coder;
			public HexOptions Options;
			static BytesBuilder()
			{
				ByteExtensions.BytesBuilder.coder = new BinHexEncoding();
			}
			public BytesBuilder()
			{
				this.builder = new StringBuilder();
			}
			public BytesBuilder(int capacity)
			{
				this.builder = new StringBuilder(capacity * 2);
			}
			public BytesBuilder(byte[] value)
			{
				this.builder = new StringBuilder(value.ToHex());
			}
			public BytesBuilder(byte[] value, int startIndex, int length, int capacity)
			{
				this.builder = new StringBuilder(value.ToHex(), startIndex * 2, length * 2, capacity * 2);
			}
			public BytesBuilder(int capacity, int maxCapacity)
			{
				this.builder = new StringBuilder(capacity * 2, maxCapacity * 2);
			}
			public BytesBuilder(byte[] value, int capacity)
			{
				this.builder = new StringBuilder(value.ToHex(), capacity * 2);
			}
			public void AppendBytes(byte[] value)
			{
				this.builder.Append(value.ToHex());
			}
			public void AppendHexString(string source)
			{
				this.builder.Append(ByteExtensions.BytesBuilder.coder.GetBytes(source));
			}
			public void AppendByte(byte value)
			{
				this.builder.Append(value.ToHex());
			}
			public void AppendSByte(sbyte value)
			{
				this.builder.Append(value.ToHex());
			}
			public void AppendInt16(short value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt16(ushort value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendInt32(int value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt32(uint value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendInt64(long value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt64(ulong value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendByte(byte? value)
			{
				this.builder.Append(value.ToHex());
			}
			public void AppendSByte(sbyte? value)
			{
				this.builder.Append(value.ToHex());
			}
			public void AppendInt16(short? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt16(ushort? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendInt32(int? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt32(uint? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendInt64(long? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void AppendUInt64(ulong? value)
			{
				this.builder.Append(value.ToHex(this.Options));
			}
			public void InsertBytes(int index, byte[] value)
			{
				this.builder.Insert(index * 2, value.ToHex());
			}
			public void InsertHexString(int index, string source)
			{
				this.builder.Insert(index * 2, ByteExtensions.BytesBuilder.coder.GetBytes(source));
			}
			public void InsertByte(int index, byte value)
			{
				this.builder.Insert(index, value.ToHex());
			}
			public void InsertSByte(int index, sbyte value)
			{
				this.builder.Insert(index, value.ToHex());
			}
			public void InsertInt16(int index, short value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public void InsertUInt16(int index, ushort value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public void InsertInt32(int index, int value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public void InsertUInt32(int index, uint value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public void InsertInt64(int index, long value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public void InsertUInt64(int index, ulong value)
			{
				this.builder.Insert(index, value.ToHex(this.Options));
			}
			public new string ToString()
			{
				return this.builder.ToString();
			}
			public byte[] ToBytes()
			{
				return new HexString(this.builder.ToString());
			}
			public void Test()
			{
				new ulong?(93485uL);
			}
		}
		public static string ToHex(this byte[] bytes, HexOptions options)
		{
			return new HexString(bytes, options);
		}
		public static string ToHex(this byte[] bytes)
		{
			return new HexString(bytes, HexOptions.Default);
		}
		public static byte[] Reverse(this byte[] bytes)
		{
			byte[] array = (byte[])bytes.Clone();
			Array.Reverse(array);
			return array;
		}
		public static bool IsUpperCase(this HexOptions options)
		{
			return (options & HexOptions.CaseUpper) != HexOptions.None;
		}
		public static bool IsEndianBig(this HexOptions options)
		{
			return (options & HexOptions.EndianBig) != HexOptions.None;
		}
		public static byte[] GetBytes(this byte @byte)
		{
			return new byte[]
			{
				@byte
			};
		}
		public static string ToHex(this byte @byte)
		{
			return new HexString(@byte.GetBytes());
		}
		public static string ToHex(this byte @byte, HexOptions options)
		{
			return new HexString(@byte.GetBytes(), options);
		}
		public static byte[] GetBytes(this byte? @byte)
		{
			if (!@byte.HasValue)
			{
				return null;
			}
			return new byte[]
			{
				@byte.Value
			};
		}
		public static string ToHex(this byte? @byte)
		{
			return new HexString(@byte.GetBytes());
		}
		public static string ToHex(this byte? @byte, HexOptions options)
		{
			return new HexString(@byte.GetBytes(), options);
		}
		public static byte[] GetBytes(this sbyte @sbyte)
		{
			return new byte[]
			{
				(byte)@sbyte
			};
		}
		public static string ToHex(this sbyte @sbyte)
		{
			return new HexString(@sbyte.GetBytes());
		}
		public static string ToHex(this sbyte @sbyte, HexOptions options)
		{
			return new HexString(@sbyte.GetBytes(), options);
		}
		public static byte[] GetBytes(this sbyte? @sbyte)
		{
			if (!@sbyte.HasValue)
			{
				return null;
			}
			return @sbyte.Value.GetBytes();
		}
		public static string ToHex(this sbyte? @sbyte)
		{
			return new HexString(@sbyte.GetBytes());
		}
		public static string ToHex(this sbyte? @sbyte, HexOptions options)
		{
			return new HexString(@sbyte.GetBytes(), options);
		}
		public static byte[] GetBytes(this short @short, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@short);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this short @short)
		{
			return @short.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this short @short, HexOptions options)
		{
			return new HexString(@short.GetBytes(options), options);
		}
		public static string ToHex(this short @short)
		{
			return new HexString(@short.GetBytes());
		}
		public static byte[] GetBytes(this short? @short, HexOptions options)
		{
			byte[] array = @short.HasValue ? BitConverter.GetBytes(@short.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this short? @short)
		{
			return @short.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this short? @short, HexOptions options)
		{
			return new HexString(@short.GetBytes(options), options);
		}
		public static string ToHex(this short? @short)
		{
			return new HexString(@short.GetBytes());
		}
		public static byte[] GetBytes(this ushort @ushort, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@ushort);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this ushort @ushort)
		{
			return @ushort.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this ushort @ushort, HexOptions options)
		{
			return new HexString(@ushort.GetBytes(options), options);
		}
		public static string ToHex(this ushort @ushort)
		{
			return new HexString(@ushort.GetBytes());
		}
		public static byte[] GetBytes(this ushort? @ushort, HexOptions options)
		{
			byte[] array = @ushort.HasValue ? BitConverter.GetBytes(@ushort.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this ushort? @ushort)
		{
			return @ushort.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this ushort? @ushort, HexOptions options)
		{
			return new HexString(@ushort.GetBytes(options), options);
		}
		public static string ToHex(this ushort? @ushort)
		{
			return new HexString(@ushort.GetBytes());
		}
		public static byte[] GetBytes(this int @int, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@int);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this int @int)
		{
			return @int.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this int @int, HexOptions options)
		{
			return new HexString(@int.GetBytes(options), options);
		}
		public static string ToHex(this int @int)
		{
			return new HexString(@int.GetBytes());
		}
		public static byte[] GetBytes(this int? @int, HexOptions options)
		{
			byte[] array = @int.HasValue ? BitConverter.GetBytes(@int.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this int? @int)
		{
			return @int.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this int? @int, HexOptions options)
		{
			return new HexString(@int.GetBytes(options), options);
		}
		public static string ToHex(this int? @int)
		{
			return new HexString(@int.GetBytes());
		}
		public static byte[] GetBytes(this uint @uint, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@uint);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this uint @uint)
		{
			return @uint.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this uint @uint, HexOptions options)
		{
			return new HexString(@uint.GetBytes(options), options);
		}
		public static string ToHex(this uint @uint)
		{
			return new HexString(@uint.GetBytes());
		}
		public static byte[] GetBytes(this uint? @uint, HexOptions options)
		{
			byte[] array = @uint.HasValue ? BitConverter.GetBytes(@uint.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this uint? @uint)
		{
			return @uint.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this uint? @uint, HexOptions options)
		{
			return new HexString(@uint.GetBytes(options), options);
		}
		public static string ToHex(this uint? @uint)
		{
			return new HexString(@uint.GetBytes());
		}
		public static byte[] GetBytes(this long @long, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@long);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this long @long)
		{
			return @long.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this long @long, HexOptions options)
		{
			return new HexString(@long.GetBytes(options), options);
		}
		public static string ToHex(this long @long)
		{
			return new HexString(@long.GetBytes());
		}
		public static byte[] GetBytes(this long? @long, HexOptions options)
		{
			byte[] array = @long.HasValue ? BitConverter.GetBytes(@long.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this long? @long)
		{
			return @long.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this long? @long, HexOptions options)
		{
			return new HexString(@long.GetBytes(options), options);
		}
		public static string ToHex(this long? @long)
		{
			return new HexString(@long.GetBytes());
		}
		public static byte[] GetBytes(this ulong @ulong, HexOptions options)
		{
			byte[] bytes = BitConverter.GetBytes(@ulong);
			if (options.IsEndianBig())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public static byte[] GetBytes(this ulong @ulong)
		{
			return @ulong.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this ulong @ulong, HexOptions options)
		{
			return new HexString(@ulong.GetBytes(options), options);
		}
		public static string ToHex(this ulong @ulong)
		{
			return new HexString(@ulong.GetBytes());
		}
		public static byte[] GetBytes(this ulong? @ulong, HexOptions options)
		{
			byte[] array = @ulong.HasValue ? BitConverter.GetBytes(@ulong.Value) : null;
			if (options.IsEndianBig())
			{
				Array.Reverse(array);
			}
			return array;
		}
		public static byte[] GetBytes(this ulong? @ulong)
		{
			return @ulong.GetBytes(HexOptions.Default);
		}
		public static string ToHex(this ulong? @ulong, ulong defaultValue, HexOptions options)
		{
			return new HexString((@ulong ?? defaultValue).GetBytes(options), options);
		}
		public static string ToHex(this ulong? @ulong, HexOptions options)
		{
			return new HexString(@ulong.GetBytes(options), options);
		}
		public static string ToHex(this ulong? @ulong, ulong defaultValue)
		{
			return new HexString((@ulong ?? defaultValue).GetBytes());
		}
		public static string ToHex(this ulong? @ulong)
		{
			return new HexString(@ulong.GetBytes());
		}
	}
}
