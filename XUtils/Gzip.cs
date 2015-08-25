using System;
using System.IO;
using System.IO.Compression;
namespace XUtils
{
	public class Gzip
	{
		public static byte[] Compress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					Gzip.Compress(memoryStream, memoryStream2);
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}
		public static void Compress(Stream source, Stream dest)
		{
			using (GZipStream gZipStream = new GZipStream(dest, CompressionMode.Compress, true))
			{
				byte[] array = new byte[1024];
				int count;
				while ((count = source.Read(array, 0, array.Length)) > 0)
				{
					gZipStream.Write(array, 0, count);
				}
			}
		}
		public static byte[] Decompress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					Gzip.Decompress(memoryStream, memoryStream2);
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}
		public static void Decompress(Stream source, Stream dest)
		{
			using (GZipStream gZipStream = new GZipStream(source, CompressionMode.Decompress, true))
			{
				byte[] array = new byte[1024];
				int count;
				while ((count = gZipStream.Read(array, 0, array.Length)) > 0)
				{
					dest.Write(array, 0, count);
				}
			}
		}
	}
}
