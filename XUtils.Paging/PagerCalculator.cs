using System;
namespace XUtils.Paging
{
	public class PagerCalculator : IPagerCalculator
	{
		public void Calculate(Pager pagerData, PagerSettings pagerSettings)
		{
			if (pagerData.CurrentPage < 0)
			{
				pagerData.CurrentPage = 1;
			}
			if (pagerData.CurrentPage > pagerData.TotalPages)
			{
				pagerData.CurrentPage = 1;
			}
			int currentPage = pagerData.CurrentPage;
			pagerData.StartingPage = PagerCalculator.GetStartingPage(pagerData, pagerSettings);
			pagerData.EndingPage = PagerCalculator.GetEndingPage(pagerData, pagerSettings);
			if (currentPage + 1 <= pagerData.TotalPages)
			{
				pagerData.NextPage = currentPage + 1;
			}
			else
			{
				pagerData.NextPage = currentPage;
			}
			if (currentPage - 1 >= 1)
			{
				pagerData.PreviousPage = currentPage - 1;
				return;
			}
			pagerData.PreviousPage = currentPage;
		}
		private static int GetStartingPage(Pager pagerData, PagerSettings settings)
		{
			if (pagerData.CurrentPage <= settings.NumberPagesToDisplay)
			{
				return 1;
			}
			int num = PagerCalculator.GetRange(pagerData.CurrentPage, settings.NumberPagesToDisplay);
			int totalRanges = PagerCalculator.GetTotalRanges(pagerData.TotalPages, settings.NumberPagesToDisplay);
			if (num == totalRanges)
			{
				return pagerData.TotalPages - settings.NumberPagesToDisplay + 1;
			}
			num--;
			return num * settings.NumberPagesToDisplay + 1;
		}
		private static int GetEndingPage(Pager pagerData, PagerSettings settings)
		{
			if (pagerData.TotalPages <= settings.NumberPagesToDisplay)
			{
				return pagerData.TotalPages;
			}
			int range = PagerCalculator.GetRange(pagerData.CurrentPage, settings.NumberPagesToDisplay);
			int totalRanges = PagerCalculator.GetTotalRanges(pagerData.TotalPages, settings.NumberPagesToDisplay);
			if (range == totalRanges)
			{
				return pagerData.TotalPages;
			}
			return range * settings.NumberPagesToDisplay;
		}
		private static int GetTotalRanges(int totalPages, int numberPagesToDisplay)
		{
			return PagerCalculator.GetRange(totalPages, numberPagesToDisplay);
		}
		private static int GetRange(int currentPage, int numberPagesToDisplay)
		{
			if (currentPage <= numberPagesToDisplay)
			{
				return 1;
			}
			int num = currentPage / numberPagesToDisplay;
			int num2 = currentPage % numberPagesToDisplay;
			if (num2 > 0)
			{
				num++;
			}
			return num;
		}
	}
}
