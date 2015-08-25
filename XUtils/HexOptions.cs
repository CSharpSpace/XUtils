using System;
namespace XUtils
{
	[Flags]
	public enum HexOptions
	{
		None = 0,
		Default = 2,
		CaseUpper = 1,
		CaseLower = 0,
		CaseDefault = 0,
		EndianLittle = 2,
		EndianBig = 4,
		EndianDefault = 2,
		Layout8Bit = 0,
		LayoutDefault = 0
	}
}
