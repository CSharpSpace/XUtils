using System;
namespace XUtils.Threading.Base.Internal
{
	public interface IHasWorkItemPriority
	{
		WorkItemPriority WorkItemPriority
		{
			get;
		}
	}
}
