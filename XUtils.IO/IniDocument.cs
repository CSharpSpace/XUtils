using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
namespace XUtils.IO
{
	public class IniDocument
	{
		public Hashtable m_sections;
		private string m_FileName;
		public ICollection Sections
		{
			get
			{
				return this.m_sections.Values;
			}
		}
		public IniDocument()
		{
			this.m_sections = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
		}
		public IniDocument(string sFileName) : this()
		{
			this.m_FileName = sFileName;
			this.Load(sFileName, false);
		}
		public void Load(string sFileName)
		{
			this.m_FileName = sFileName;
			this.Load(sFileName, false);
		}
		public void Load(string sFileName, bool bMerge)
		{
			this.m_FileName = sFileName;
			if (!bMerge)
			{
				this.RemoveAllSections();
			}
			IniSection iniSection = null;
			StreamReader streamReader = new StreamReader(sFileName);
			Regex regex = new Regex("^([\\s]*#.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Regex regex2 = new Regex("\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Regex regex3 = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				if (text != string.Empty)
				{
					if (regex.Match(text).Success)
					{
						Match match = regex.Match(text);
					}
					else
					{
						if (regex2.Match(text).Success)
						{
							Match match = regex2.Match(text);
							iniSection = this.AddSection(match.Groups[1].Value);
						}
						else
						{
							if (regex3.Match(text).Success && iniSection != null)
							{
								Match match = regex3.Match(text);
								iniSection.AddKey(match.Groups[1].Value).Value = match.Groups[2].Value;
							}
							else
							{
								if (iniSection != null)
								{
									iniSection.AddKey(text);
								}
								else
								{
									Trace.WriteLine(string.Format("Skipping unknown type of data: {0}", text));
								}
							}
						}
					}
				}
			}
			streamReader.Close();
		}
		public void Save()
		{
			StreamWriter streamWriter = new StreamWriter(this.m_FileName, false);
			foreach (IniSection iniSection in this.Sections)
			{
				streamWriter.WriteLine(string.Format("[{0}]", iniSection.Name));
				foreach (IniKey iniKey in iniSection.Keys)
				{
					if (iniKey.Value != string.Empty)
					{
						streamWriter.WriteLine(string.Format("{0}={1}", iniKey.Name, iniKey.Value));
					}
					else
					{
						streamWriter.WriteLine(string.Format("{0}", iniKey.Name));
					}
				}
			}
			streamWriter.Close();
		}
		public IniSection AddSection(string sSection)
		{
			sSection = sSection.Trim();
			IniSection iniSection;
			if (this.m_sections.ContainsKey(sSection))
			{
				iniSection = (IniSection)this.m_sections[sSection];
			}
			else
			{
				iniSection = new IniSection(this, sSection);
				this.m_sections[sSection] = iniSection;
			}
			return iniSection;
		}
		public bool RemoveSection(string sSection)
		{
			sSection = sSection.Trim();
			return this.RemoveSection(this.GetSection(sSection));
		}
		public bool RemoveSection(IniSection Section)
		{
			if (Section != null)
			{
				try
				{
					this.m_sections.Remove(Section.Name);
					return true;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.Message);
				}
				return false;
			}
			return false;
		}
		public bool RemoveAllSections()
		{
			this.m_sections.Clear();
			return this.m_sections.Count == 0;
		}
		public IniSection GetSection(string sSection)
		{
			sSection = sSection.Trim();
			if (this.m_sections.ContainsKey(sSection))
			{
				return (IniSection)this.m_sections[sSection];
			}
			return null;
		}
		public string GetKeyValue(string sSection, string sKey)
		{
			IniSection section = this.GetSection(sSection);
			if (section != null)
			{
				IniKey key = section.GetKey(sKey);
				if (key != null)
				{
					return key.Value;
				}
			}
			return string.Empty;
		}
		public bool SetKeyValue(string sSection, string sKey, string sValue)
		{
			IniSection iniSection = this.AddSection(sSection);
			if (iniSection != null)
			{
				IniKey iniKey = iniSection.AddKey(sKey);
				if (iniKey != null)
				{
					iniKey.Value = sValue;
					return true;
				}
			}
			return false;
		}
		public bool RenameSection(string sSection, string sNewSection)
		{
			bool result = false;
			IniSection section = this.GetSection(sSection);
			if (section != null)
			{
				result = section.SetName(sNewSection);
			}
			return result;
		}
		public bool RenameKey(string sSection, string sKey, string sNewKey)
		{
			IniSection section = this.GetSection(sSection);
			if (section != null)
			{
				IniKey key = section.GetKey(sKey);
				if (key != null)
				{
					return key.SetName(sNewKey);
				}
			}
			return false;
		}
	}
}
