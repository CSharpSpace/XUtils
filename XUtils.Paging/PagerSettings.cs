using System;
namespace XUtils.Paging
{
	public class PagerSettings
	{
		public static readonly PagerSettings Default = new PagerSettings(7, "current", "");
		public int NumberPagesToDisplay = 5;
		public string CssCurrentPage = string.Empty;
		public string CssClass = string.Empty;
		public PagerSettings()
		{
		}
		public PagerSettings(int numberPagesToDisplay, string cssClassForCurrentPage, string cssClassForPage)
		{
			this.NumberPagesToDisplay = numberPagesToDisplay;
			this.CssCurrentPage = cssClassForCurrentPage;
			this.CssClass = cssClassForPage;
		}
	}
}
