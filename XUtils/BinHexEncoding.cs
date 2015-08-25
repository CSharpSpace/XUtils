using System;
using System.Globalization;
using System.Security;
using System.Text;
namespace XUtils
{
	internal class BinHexEncoding : Encoding
	{
		private static byte[] char2val = new byte[]
		{
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			10,
			11,
			12,
			13,
			14,
			15,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			10,
			11,
			12,
			13,
			14,
			15,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255,
			255
		};
		private static string val2char = "0123456789ABCDEF";
		public override int GetByteCount(char[] chars, int index, int count)
		{
			return this.GetMaxByteCount(count);
		}
		[SecurityCritical, SecurityTreatAsSafe]
		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charIndex < 0)
			{
				throw new ArgumentOutOfRangeException("charIndex", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", ResourceStrings.GetString("OffsetExceedsBufferSize", new object[]
				{
					chars.Length
				}));
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", ResourceStrings.GetString("SizeExceedsRemainingBufferSpace", new object[]
				{
					chars.Length - charIndex
				}));
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0)
			{
				throw new ArgumentOutOfRangeException("byteIndex", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", ResourceStrings.GetString("OffsetExceedsBufferSize", new object[]
				{
					bytes.Length
				}));
			}
			int byteCount = this.GetByteCount(chars, charIndex, charCount);
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentException(ResourceStrings.GetString("XmlArrayTooSmall"), "bytes");
			}
			if (charCount > 0)
			{
				fixed (byte* ptr = BinHexEncoding.char2val)
				{
					fixed (byte* ptr2 = &bytes[byteIndex])
					{
						fixed (char* ptr3 = &chars[charIndex])
						{
							char* ptr4 = ptr3;
							char* ptr5 = ptr3 + (IntPtr)charCount;
							byte* ptr6 = ptr2;
							while (ptr4 < ptr5)
							{
								char c = *ptr4;
								char c2 = ptr4[(IntPtr)2 / 2];
								if ((c | c2) >= '\u0080')
								{
									throw new FormatException(ResourceStrings.GetString("XmlInvalidBinHexSequence", new object[]
									{
										new string(ptr4, 0, 2),
										charIndex + (ptr4 - ptr3 / 2) / 2 / 2L
									}));
								}
								byte b = ptr[(IntPtr)((byte)c) / 1];
								byte b2 = ptr[(IntPtr)((byte)c2) / 1];
								if ((b | b2) == 255)
								{
									throw new FormatException(ResourceStrings.GetString("XmlInvalidBinHexSequence", new object[]
									{
										new string(ptr4, 0, 2),
										charIndex + (ptr4 - ptr3 / 2) / 2 / 2L
									}));
								}
								*ptr6 = (byte)(((int)b << 4) + (int)b2);
								ptr4 += (IntPtr)4 / 2;
								ptr6 += (IntPtr)1 / 1;
							}
						}
					}
				}
			}
			return byteCount;
		}
		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return this.GetMaxCharCount(count);
		}
		[SecurityCritical, SecurityTreatAsSafe]
		public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0)
			{
				throw new ArgumentOutOfRangeException("byteIndex", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", ResourceStrings.GetString("OffsetExceedsBufferSize", new object[]
				{
					bytes.Length
				}));
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", ResourceStrings.GetString("SizeExceedsRemainingBufferSpace", new object[]
				{
					bytes.Length - byteIndex
				}));
			}
			int charCount = this.GetCharCount(bytes, byteIndex, byteCount);
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charIndex < 0)
			{
				throw new ArgumentOutOfRangeException("charIndex", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", ResourceStrings.GetString("OffsetExceedsBufferSize", new object[]
				{
					chars.Length
				}));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentException(ResourceStrings.GetString("XmlArrayTooSmall"), "chars");
			}
			if (byteCount > 0)
			{
				fixed (char* ptr = BinHexEncoding.val2char)
				{
					char* ptr2 = ptr;
					fixed (byte* ptr3 = &bytes[byteIndex])
					{
						fixed (char* ptr4 = &chars[charIndex])
						{
							char* ptr5 = ptr4;
							byte* ptr6 = ptr3;
							byte* ptr7 = ptr3 + (IntPtr)byteCount / 1;
							while (ptr6 < ptr7)
							{
								*ptr5 = ptr2[(IntPtr)(*ptr6 >> 4)];
								ptr5[(IntPtr)2 / 2] = *(ptr2 + (IntPtr)(*ptr6 & 15));
								ptr6 += (IntPtr)1 / 1;
								ptr5 += (IntPtr)4 / 2;
							}
						}
					}
				}
			}
			return charCount;
		}
		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", ResourceStrings.GetString("ValueMustBeNonNegative"));
			}
			if (charCount % 2 != 0)
			{
				throw new FormatException(ResourceStrings.GetString("XmlInvalidBinHexLength", new object[]
				{
					charCount.ToString(NumberFormatInfo.CurrentInfo)
				}));
			}
			return charCount / 2;
		}
		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0 || byteCount > 1073741823)
			{
				throw new ArgumentOutOfRangeException("byteCount", ResourceStrings.GetString("ValueMustBeInRange", new object[]
				{
					0,
					1073741823
				}));
			}
			return byteCount * 2;
		}
	}
}
