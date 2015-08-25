using System;
using System.Xml;
namespace XUtils.Schedule
{
	public class FileEventStorage : IEventStorage
	{
		private string _FileName;
		private string _XPath;
		private XmlDocument _Doc = new XmlDocument();
		public FileEventStorage(string FileName, string XPath)
		{
			this._FileName = FileName;
			this._XPath = XPath;
		}
		public void RecordLastTime(DateTime Time)
		{
			this._Doc.SelectSingleNode(this._XPath).Value = Time.ToString();
			this._Doc.Save(this._FileName);
		}
		public DateTime ReadLastTime()
		{
			this._Doc.Load(this._FileName);
			string value = this._Doc.SelectSingleNode(this._XPath).Value;
			if (string.IsNullOrEmpty(value))
			{
				return DateTime.Now;
			}
			return DateTime.Parse(value);
		}
	}
}
