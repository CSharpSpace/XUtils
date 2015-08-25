using System;
namespace XUtils.Messages
{
	public static class BoolMessageExtensions
	{
		public static int AsExitCode(this BoolMessage result)
		{
			if (!result.Success)
			{
				return 0;
			}
			return 1;
		}
	}
}
