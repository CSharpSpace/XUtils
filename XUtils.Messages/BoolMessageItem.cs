using System;
namespace XUtils.Messages
{
	public class BoolMessageItem : BoolMessage
	{
		private object _item;
		public new static readonly BoolMessageItem True = new BoolMessageItem(null, true, string.Empty);
		public new static readonly BoolMessageItem False = new BoolMessageItem(null, false, string.Empty);
		public object Item
		{
			get
			{
				return this._item;
			}
		}
		public BoolMessageItem(object item, bool success, string message) : base(success, message)
		{
			this._item = item;
		}
	}
	public class BoolMessageItem<T> : BoolMessageItem
	{
		public new T Item
		{
			get
			{
				return (T)((object)base.Item);
			}
		}
		public BoolMessageItem(T item, bool success, string message) : base(item, success, message)
		{
		}
	}
}
