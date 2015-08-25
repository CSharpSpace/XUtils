using System;
namespace XUtils.Paging
{
	public class Pager : ICloneable
	{
		private int _currentPage;
		private int _totalPages;
		private int _previousPage;
		private int _startingPage;
		private int _endingPage;
		private int _nextPage;
		private PagerSettings _pagerSettings;
		private static IPagerCalculator _instance = new PagerCalculator();
		private static readonly object _syncRoot = new object();
		public int CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			set
			{
				this._currentPage = value;
			}
		}
		public int TotalPages
		{
			get
			{
				return this._totalPages;
			}
			set
			{
				this._totalPages = value;
			}
		}
		public int FirstPage
		{
			get
			{
				return 1;
			}
		}
		public int PreviousPage
		{
			get
			{
				return this._previousPage;
			}
			set
			{
				this._previousPage = value;
			}
		}
		public int StartingPage
		{
			get
			{
				return this._startingPage;
			}
			set
			{
				this._startingPage = value;
			}
		}
		public int EndingPage
		{
			get
			{
				return this._endingPage;
			}
			set
			{
				this._endingPage = value;
			}
		}
		public int NextPage
		{
			get
			{
				return this._nextPage;
			}
			set
			{
				this._nextPage = value;
			}
		}
		public int LastPage
		{
			get
			{
				return this._totalPages;
			}
		}
		public bool IsMultiplePages
		{
			get
			{
				return this._totalPages > 1;
			}
		}
		public PagerSettings Settings
		{
			get
			{
				return this._pagerSettings;
			}
			set
			{
				this._pagerSettings = value;
			}
		}
		public bool CanShowFirst
		{
			get
			{
				return this._startingPage != 1;
			}
		}
		public bool CanShowPrevious
		{
			get
			{
				return this._startingPage > 2;
			}
		}
		public bool CanShowNext
		{
			get
			{
				return this._endingPage < this._totalPages - 1;
			}
		}
		public bool CanShowLast
		{
			get
			{
				return this._endingPage != this._totalPages;
			}
		}
		public Pager() : this(1, 1, PagerSettings.Default)
		{
		}
		public Pager(int currentPage, int totalPages) : this(currentPage, totalPages, PagerSettings.Default)
		{
		}
		public Pager(int currentPage, int totalPages, PagerSettings settings)
		{
			this._pagerSettings = settings;
			this.SetCurrentPage(currentPage, totalPages);
		}
		public void Init(IPagerCalculator pager)
		{
			Pager._instance = pager;
		}
		public void SetCurrentPage(int currentPage)
		{
			this.SetCurrentPage(currentPage, this._totalPages);
		}
		public void SetCurrentPage(int currentPage, int totalPages)
		{
			if (totalPages < 0)
			{
				totalPages = 1;
			}
			if (currentPage < 0 || currentPage > totalPages)
			{
				currentPage = 1;
			}
			this._currentPage = currentPage;
			this._totalPages = totalPages;
			this.Calculate();
		}
		public void MoveFirst()
		{
			this._currentPage = 1;
			this.Calculate();
		}
		public void MovePrevious()
		{
			this._currentPage = this._previousPage;
			this.Calculate();
		}
		public void MoveNext()
		{
			this._currentPage = this._nextPage;
			this.Calculate();
		}
		public void MoveLast()
		{
			this._currentPage = this._totalPages;
			this.Calculate();
		}
		public void MoveToPage(int selectedPage)
		{
			this._currentPage = selectedPage;
			this.Calculate();
		}
		public void Calculate()
		{
			Pager.Calculate(this, this._pagerSettings);
		}
		public static void Calculate(Pager pagerData, PagerSettings pagerSettings)
		{
			Pager._instance.Calculate(pagerData, pagerSettings);
		}
		public string ToHtml(Func<int, string> urlBuilder)
		{
			return PagerBuilderWeb.Instance.Build(this, this.Settings, urlBuilder);
		}
		public string ToHtml(Func<int, string> urlBuilder, PagerSettings settings)
		{
			return PagerBuilderWeb.Instance.Build(this, settings, urlBuilder);
		}
		public static Pager Get(int currentPage, int totalPages, PagerSettings settings)
		{
			return new Pager(currentPage, totalPages, settings);
		}
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
