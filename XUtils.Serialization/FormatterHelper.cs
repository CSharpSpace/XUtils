using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace XUtils.Serialization
{
	public static class FormatterHelper
	{
		public static byte[] Serialize(object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream(10240);
			byte[] result;
			try
			{
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[(int)memoryStream.Length];
				memoryStream.Read(array, 0, array.Length);
				result = array;
			}
			finally
			{
				memoryStream.Close();
			}
			return result;
		}
		public static T Deserialize<T>(byte[] buffer)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, false);
			T result;
			try
			{
				object obj = binaryFormatter.Deserialize(memoryStream);
				result = (T)((object)obj);
			}
			finally
			{
				memoryStream.Close();
			}
			return result;
		}
	}
}
