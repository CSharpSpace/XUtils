using System;
namespace XUtils.Plugin
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ImplementsAttribute : Attribute
	{
		private string _onlyKey;
		public string OnlyKey
		{
			get
			{
				return this._onlyKey;
			}
		}
		public ImplementsAttribute(string onlyKey)
		{
			this._onlyKey = onlyKey;
		}
	}
}
