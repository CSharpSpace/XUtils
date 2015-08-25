using System;
namespace XUtils.Paging
{
	public class PageNumber
	{
		private int _pageNumber;
		private string _cssClass;
		public int Page
		{
			get
			{
				return this._pageNumber;
			}
		}
		public string CssClass
		{
			get
			{
				return this._cssClass;
			}
		}
		public PageNumber(int number, string cssClass)
		{
			this._pageNumber = number;
			this._cssClass = cssClass;
		}
	}
}
