using System;
namespace XUtils.Messages
{
	public class BoolMessageEx : BoolMessage
	{
		private Exception _ex;
		public new static readonly BoolMessageEx True = new BoolMessageEx(true, null, string.Empty);
		public new static readonly BoolMessageEx False = new BoolMessageEx(false, null, string.Empty);
		public Exception Ex
		{
			get
			{
				return this._ex;
			}
		}
		public BoolMessageEx(bool success, Exception ex, string message) : base(success, message)
		{
			this._ex = ex;
		}
	}
}
