using System;
namespace XUtils.Threading.Base.Internal
{
	internal class CanceledWorkItemsGroup
	{
		public static readonly CanceledWorkItemsGroup NotCanceledWorkItemsGroup = new CanceledWorkItemsGroup();
		public bool IsCanceled
		{
			get;
			set;
		}
		public CanceledWorkItemsGroup()
		{
			this.IsCanceled = false;
		}
	}
}
