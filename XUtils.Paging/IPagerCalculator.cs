using System;
namespace XUtils.Paging
{
	public interface IPagerCalculator
	{
		void Calculate(Pager pager, PagerSettings settings);
	}
}
