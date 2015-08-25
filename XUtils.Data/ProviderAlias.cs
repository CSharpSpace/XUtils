using System;
namespace XUtils.Data
{
	public class ProviderAlias
	{
		private string _assemblyName;
		private string _typeName;
		public string AssemblyName
		{
			get
			{
				return this._assemblyName;
			}
		}
		public string TypeName
		{
			get
			{
				return this._typeName;
			}
		}
		public ProviderAlias(string assemblyName, string typeName)
		{
			this._assemblyName = assemblyName;
			this._typeName = typeName;
		}
	}
}
