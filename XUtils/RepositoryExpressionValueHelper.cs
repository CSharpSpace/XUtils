using System;
namespace XUtils
{
	internal class RepositoryExpressionValueHelper
	{
		public static string GetVal(object val)
		{
			if (val.GetType() != typeof(bool) && val.GetType() != typeof(bool))
			{
				return val.ToString();
			}
			if (!(bool)val)
			{
				return "0";
			}
			return "1";
		}
	}
}
