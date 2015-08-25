using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
namespace XUtils.Serialization
{
	public static class JsonNT
	{
		public static string JsonNTSerializer<T>(this T entities)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream();
			string @string;
			try
			{
				dataContractJsonSerializer.WriteObject(memoryStream, entities);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			finally
			{
				memoryStream.Close();
			}
			return @string;
		}
		public static string JsonNTSerializer<T>(this List<T> entities)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(List<T>));
			MemoryStream memoryStream = new MemoryStream();
			string @string;
			try
			{
				dataContractJsonSerializer.WriteObject(memoryStream, entities);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			finally
			{
				memoryStream.Close();
			}
			return @string;
		}
		public static T JsonNTDeserializeToEntity<T>(this string str)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(str));
			T result;
			try
			{
				T t = (T)((object)dataContractJsonSerializer.ReadObject(memoryStream));
				result = t;
			}
			finally
			{
				memoryStream.Close();
			}
			return result;
		}
		public static List<T> JsonNTDeserialize<T>(this string str)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(List<T>));
			MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(str));
			List<T> result;
			try
			{
				List<T> list = (List<T>)dataContractJsonSerializer.ReadObject(memoryStream);
				result = list;
			}
			finally
			{
				memoryStream.Close();
			}
			return result;
		}
	}
}
