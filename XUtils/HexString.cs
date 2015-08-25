using System;
namespace XUtils
{
	internal class HexString
	{
		private byte[] bytes;
		private bool upperCase;
		private static BinHexEncoding coder;
		static HexString()
		{
			HexString.coder = new BinHexEncoding();
		}
		public HexString(byte[] source)
		{
			this.bytes = source;
		}
		public HexString(byte[] source, HexOptions options)
		{
			this.upperCase = options.IsUpperCase();
			this.bytes = source;
		}
		public HexString(string source)
		{
			this.bytes = HexString.coder.GetBytes(source);
		}
		public static implicit operator string(HexString value)
		{
			string @string = HexString.coder.GetString(value.bytes);
			if (@string == null)
			{
				return null;
			}
			if (value.upperCase)
			{
				return @string;
			}
			return @string.ToLower();
		}
		public static implicit operator byte[](HexString value)
		{
			return value.bytes;
		}
	}
}
