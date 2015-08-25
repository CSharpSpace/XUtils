using System;
using System.Diagnostics;
namespace XUtils.IO
{
	public class IniKey
	{
		private string m_sKey;
		private string m_sValue;
		private IniSection m_section;
		public string Name
		{
			get
			{
				return this.m_sKey;
			}
		}
		public string Value
		{
			get
			{
				return this.m_sValue;
			}
			set
			{
				this.m_sValue = value;
			}
		}
		protected internal IniKey(IniSection parent, string sKey)
		{
			this.m_section = parent;
			this.m_sKey = sKey;
		}
		public void SetValue(string sValue)
		{
			this.m_sValue = sValue;
		}
		public T GetValue<T>()
		{
			return TypeParsers.ConvertTo<T>(this.m_sValue);
		}
		public bool SetName(string sKey)
		{
			sKey = sKey.Trim();
			if (sKey.Length != 0)
			{
				IniKey key = this.m_section.GetKey(sKey);
				if (key != this && key != null)
				{
					return false;
				}
				try
				{
					this.m_section.m_keys.Remove(this.m_sKey);
					this.m_section.m_keys[sKey] = this;
					this.m_sKey = sKey;
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
			return this.m_sKey;
		}
	}
}
