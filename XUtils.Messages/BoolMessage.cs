using System;
namespace XUtils.Messages
{
	public class BoolMessage
	{
		public static readonly BoolMessage True = new BoolMessage(true, string.Empty);
		public static readonly BoolMessage False = new BoolMessage(false, string.Empty);
		public readonly bool Success;
		public readonly string Message;
		public BoolMessage(bool success, string message)
		{
			this.Success = success;
			this.Message = message;
		}
	}
}
