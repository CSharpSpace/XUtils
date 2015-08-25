using System;
using System.Collections;
using System.Diagnostics;
namespace XUtils.IO
{
	public class IniSection
	{
		private IniDocument m_pIniDocument;
		private string m_sSection;
		public Hashtable m_keys;
		public ICollection Keys
		{
			get
			{
				return this.m_keys.Values;
			}
		}
		public string Name
		{
			get
			{
				return this.m_sSection;
			}
		}
		protected internal IniSection(IniDocument parent, string sSection)
		{
			this.m_pIniDocument = parent;
			this.m_sSection = sSection;
			this.m_keys = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}
		public IniKey AddKey(string sKey)
		{
			sKey = sKey.Trim();
			IniKey iniKey = null;
			if (sKey.Length != 0)
			{
				if (this.m_keys.ContainsKey(sKey))
				{
					iniKey = (IniKey)this.m_keys[sKey];
				}
				else
				{
					iniKey = new IniKey(this, sKey);
					this.m_keys[sKey] = iniKey;
				}
			}
			return iniKey;
		}
		public void Set(string sKey, string sValue)
		{
			this.AddKey(sKey).SetValue(sValue);
		}
		public bool RemoveKey(string sKey)
		{
			return this.RemoveKey(this.GetKey(sKey));
		}
		public bool RemoveKey(IniKey Key)
		{
			if (Key != null)
			{
				try
				{
					this.m_keys.Remove(Key.Name);
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
		public bool RemoveAllKeys()
		{
			this.m_keys.Clear();
			return this.m_keys.Count == 0;
		}
		public IniKey GetKey(string sKey)
		{
			sKey = sKey.Trim();
			if (this.m_keys.ContainsKey(sKey))
			{
				return (IniKey)this.m_keys[sKey];
			}
			return null;
		}
		public T Get<T>(string sKey)
		{
			return this.GetKey(sKey).GetValue<T>();
		}
		public bool SetName(string sSection)
		{
			sSection = sSection.Trim();
			if (sSection.Length != 0)
			{
				IniSection section = this.m_pIniDocument.GetSection(sSection);
				if (section != this && section != null)
				{
					return false;
				}
				try
				{
					this.m_pIniDocument.m_sections.Remove(this.m_sSection);
					this.m_pIniDocument.m_sections[sSection] = this;
					this.m_sSection = sSection;
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
		public string GetName()
		{
			return this.m_sSection;
		}
	}
}
