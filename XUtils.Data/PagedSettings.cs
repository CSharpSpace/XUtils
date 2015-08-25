using System;
namespace XUtils.Data
{
	public class PagedSettings
	{
		private string fields = "*";
		public string TableName
		{
			get;
			set;
		}
		public string SortField
		{
			get;
			set;
		}
		public string Fields
		{
			get
			{
				return this.fields;
			}
			set
			{
				this.fields = value;
			}
		}
		public string Where
		{
			get;
			set;
		}
		public OrderBy OrderType
		{
			get;
			set;
		}
	}
}
