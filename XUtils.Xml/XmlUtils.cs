using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
namespace XUtils.Xml
{
	public sealed class XmlUtils
	{
		private XmlUtils()
		{
		}
		public static XmlDocument LoadXMLFromFile(string file)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(file);
			}
			catch (XmlException)
			{
				return null;
			}
			return xmlDocument;
		}
		public static void RemoveAllChildrenFrom(XmlNode n)
		{
			while (n.HasChildNodes)
			{
				n.RemoveChild(n.FirstChild);
			}
		}
		public static string GetAttributeValue(XPathNavigator xNav, string attrName)
		{
			string attribute = xNav.GetAttribute(attrName, "");
			if (attribute == string.Empty)
			{
				throw new Exception("GetAttributeValue:: Could not find Required attribute: " + attrName + " in node:" + xNav.Value);
			}
			return attribute;
		}
		public static string GetAttributeValue(XPathNavigator xNav, string attrName, string defaultValue)
		{
			string attribute = xNav.GetAttribute(attrName, "");
			if (attribute == string.Empty)
			{
				return defaultValue;
			}
			return attribute;
		}
		public static XmlNode FragmentToNode(string xmlFragment)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (StringReader stringReader = new StringReader(xmlFragment))
			{
				xmlDocument.Load(stringReader);
			}
			return xmlDocument.FirstChild;
		}
		public static string EscapeXml(string xml)
		{
			if (xml.IndexOf("&") >= 0)
			{
				xml = xml.Replace("&", "&amp;");
			}
			if (xml.IndexOf("'") >= 0)
			{
				xml = xml.Replace("'", "&apos;");
			}
			if (xml.IndexOf("\"") >= 0)
			{
				xml = xml.Replace("\"", "&quot;");
			}
			if (xml.IndexOf("<") >= 0)
			{
				xml.Replace("<", "&lt;");
			}
			if (xml.IndexOf(">") >= 0)
			{
				xml.Replace(">", "&gt;");
			}
			return xml;
		}
		public static string FormatNicely(string xmlText)
		{
			if (xmlText == null || xmlText.Trim().Length == 0)
			{
				return "";
			}
			string result = "";
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(xmlText);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlDocument.WriteContentTo(xmlTextWriter);
				xmlTextWriter.Flush();
				memoryStream.Flush();
				memoryStream.Position = 0L;
				StreamReader streamReader = new StreamReader(memoryStream);
				string text = streamReader.ReadToEnd();
				result = text;
			}
			catch (Exception)
			{
				result = xmlText;
			}
			finally
			{
				memoryStream.Close();
				xmlTextWriter.Close();
			}
			return result;
		}
		public static TextWriter TransformXml(TextReader inXml, TextReader styleSheet, TextWriter outXml)
		{
			if (inXml == null || styleSheet == null)
			{
				return outXml;
			}
			Guard.IsNotNull(outXml, "outXml not null");
			try
			{
				XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
				XsltSettings settings = new XsltSettings(false, false);
				using (XmlReader xmlReader = XmlReader.Create(styleSheet))
				{
					xslCompiledTransform.Load(xmlReader, settings, null);
				}
				using (XmlReader xmlReader2 = XmlReader.Create(inXml))
				{
					xslCompiledTransform.Transform(xmlReader2, null, outXml);
				}
			}
			catch (Exception innerException)
			{
				throw new ApplicationException("Error occured while performing xsl tranformation", innerException);
			}
			return outXml;
		}
		public static string TransformXml(string xmlToTransform, string pathToXsl)
		{
			if (xmlToTransform == null || xmlToTransform.Length == 0 || !File.Exists(pathToXsl))
			{
				return "";
			}
			string result = "";
			try
			{
				using (TextReader textReader = new StreamReader(new FileStream(pathToXsl, FileMode.Open, FileAccess.Read, FileShare.Read)))
				{
					TextReader inXml = new StringReader(xmlToTransform);
					TextWriter textWriter = new StringWriter();
					XmlUtils.TransformXml(inXml, textReader, textWriter);
					result = textWriter.ToString();
				}
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}
		public static string Serialize(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter textWriter = new StringWriter(stringBuilder);
			xmlSerializer.Serialize(textWriter, obj);
			return stringBuilder.ToString();
		}
	}
}
