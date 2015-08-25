using System;
namespace XUtils.Serialization
{
	public sealed class CustomMarshalAsAttribute : Attribute
	{
		public int SizeConst;
		public string SizeField;
	}
}
