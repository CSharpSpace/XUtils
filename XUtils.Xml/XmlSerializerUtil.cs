using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
namespace XUtils.Xml
{
	public class XmlSerializerUtil
	{
		public static string XmlSerialize<T>(T item)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				xmlSerializer.Serialize(stringWriter, item);
			}
			return stringBuilder.ToString();
		}
		public static string XmlSerialize(object item)
		{
			Type type = item.GetType();
			XmlSerializer xmlSerializer = new XmlSerializer(type);
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				xmlSerializer.Serialize(stringWriter, item);
			}
			return stringBuilder.ToString();
		}
		public static T XmlDeserialize<T>(string xmlData)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			T result;
			using (TextReader textReader = new StringReader(xmlData))
			{
				T t = (T)((object)xmlSerializer.Deserialize(textReader));
				result = t;
			}
			return result;
		}
	}
}
