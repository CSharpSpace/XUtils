using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.HtmlControls;
namespace XUtils.Web
{
	public class WebUtils
	{
		private static IDictionary<string, ImageFormat> _imageFormatsLookup;
		static WebUtils()
		{
			WebUtils._imageFormatsLookup = new Dictionary<string, ImageFormat>();
			WebUtils._imageFormatsLookup.Add("jpeg", ImageFormat.Jpeg);
			WebUtils._imageFormatsLookup.Add("jpg", ImageFormat.Jpeg);
			WebUtils._imageFormatsLookup.Add("gif", ImageFormat.Gif);
			WebUtils._imageFormatsLookup.Add("tiff", ImageFormat.Tiff);
			WebUtils._imageFormatsLookup.Add("png", ImageFormat.Png);
		}
		public static string GetContentOfFile(HtmlInputFile inputFile)
		{
			byte[] buffer = new byte[inputFile.PostedFile.ContentLength];
			int contentLength = inputFile.PostedFile.ContentLength;
			inputFile.PostedFile.InputStream.Read(buffer, 0, contentLength);
			MemoryStream stream = new MemoryStream(buffer);
			StreamReader streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}
		public static byte[] GetContentOfFileAsBytes(HtmlInputFile inputFile)
		{
			byte[] array = new byte[inputFile.PostedFile.ContentLength];
			int contentLength = inputFile.PostedFile.ContentLength;
			inputFile.PostedFile.InputStream.Read(array, 0, contentLength);
			return array;
		}
		public static string GetFileExtension(HtmlInputFile inputFile)
		{
			if (inputFile == null || string.IsNullOrEmpty(inputFile.PostedFile.FileName))
			{
				return string.Empty;
			}
			string fileName = inputFile.PostedFile.FileName;
			int num = fileName.LastIndexOf(".");
			if (num < 0)
			{
				return string.Empty;
			}
			string text = fileName.Substring(num + 1);
			return text.Trim().ToLower();
		}
		public static ImageFormat GetFileExtensionAsFormat(HtmlInputFile inputFile)
		{
			string fileExtension = WebUtils.GetFileExtension(inputFile);
			if (string.IsNullOrEmpty(fileExtension))
			{
				return null;
			}
			if (!WebUtils._imageFormatsLookup.ContainsKey(fileExtension))
			{
				return null;
			}
			return WebUtils._imageFormatsLookup[fileExtension];
		}
	}
}
