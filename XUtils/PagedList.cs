using System;
using System.Collections.Generic;
using XUtils.Paging;
namespace XUtils
{
	public class PagedList<T> : List<T>
	{
		public static readonly PagedList<T> Empty = new PagedList<T>(1, 1, 0, null);
		public int PageSize
		{
			get;
			private set;
		}
		public int PageNumber
		{
			get;
			private set;
		}
		public int TotalCount
		{
			get;
			private set;
		}
		public int TotalPages
		{
			get;
			private set;
		}
		public PagedList(int pageNumber, int pageSize, int totalRecords, IList<T> items)
		{
			this.PageNumber = pageNumber;
			this.PageSize = pageSize;
			this.TotalCount = totalRecords;
			this.TotalPages = (int)Math.Ceiling((double)this.TotalCount / (double)this.PageSize);
			if (items != null && items.Count > 0)
			{
				base.AddRange(items);
			}
		}
		public string ToPageHtml(Func<int, string> urlBuilder)
		{
			return this.ToPageHtml(7, "current", string.Empty, urlBuilder);
		}
		public string ToPageHtml(int numberPagesToDisplay, Func<int, string> urlBuilder)
		{
			return this.ToPageHtml(numberPagesToDisplay, "current", string.Empty, urlBuilder);
		}
		public string ToPageHtml(int numberPagesToDisplay, string cssClassForCurrentPage, string cssClassForPage, Func<int, string> urlBuilder)
		{
			Pager pager = new Pager(this.PageNumber, this.TotalPages, new PagerSettings(numberPagesToDisplay, cssClassForCurrentPage, cssClassForPage));
			return pager.ToHtml(urlBuilder);
		}
	}
}
