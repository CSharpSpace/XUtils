using System;
using XUtils.Paging;
namespace XUtils
{
	public class Paged<T>
	{
		public static readonly Paged<T> Empty = new Paged<T>(1, 1, 0, default(T));
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
		public T Items
		{
			get;
			private set;
		}
		public Paged(int pageNumber, int pageSize, int totalRecords, T items)
		{
			this.PageNumber = pageNumber;
			this.PageSize = pageSize;
			this.TotalCount = totalRecords;
			this.TotalPages = (int)Math.Ceiling((double)this.TotalCount / (double)this.PageSize);
			if (items != null)
			{
				this.Items = items;
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
