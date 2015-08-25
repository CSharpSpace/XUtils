using System;
using System.Text;
using System.Threading;
namespace XUtils.Paging
{
	public class PagerBuilderWeb : IPagerBuilderWeb
	{
		private static IPagerBuilderWeb _instance;
		private static readonly object _syncRoot = new object();
		public static IPagerBuilderWeb Instance
		{
			get
			{
				if (PagerBuilderWeb._instance == null)
				{
					object syncRoot;
					Monitor.Enter(syncRoot = PagerBuilderWeb._syncRoot);
					try
					{
						if (PagerBuilderWeb._instance == null)
						{
							PagerBuilderWeb._instance = new PagerBuilderWeb();
						}
					}
					finally
					{
						Monitor.Exit(syncRoot);
					}
				}
				return PagerBuilderWeb._instance;
			}
		}
		public string Build(int pageIndex, int totalPages, Func<int, string> urlBuilder)
		{
			Pager pager = Pager.Get(pageIndex, totalPages, PagerSettings.Default);
			return this.Build(pager, PagerSettings.Default, urlBuilder);
		}
		public string Build(int pageIndex, int totalPages, PagerSettings settings, Func<int, string> urlBuilder)
		{
			Pager pager = Pager.Get(pageIndex, totalPages, settings);
			return this.Build(pager, pager.Settings, urlBuilder);
		}
		public string Build(Pager pager, PagerSettings settings, Func<int, string> urlBuilder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			string arg_13_0 = string.Empty;
			string text2 = string.Empty;
			string text3 = string.IsNullOrEmpty(settings.CssClass) ? string.Empty : (" class=\"" + settings.CssClass + "\"");
			if (pager.CanShowPrevious)
			{
				text2 = urlBuilder(pager.CurrentPage - 1);
				stringBuilder.Append(string.Concat(new string[]
				{
					"<a",
					text3,
					" href=\"",
					text2,
					"\">&#171;</a>"
				}));
			}
			if (pager.CanShowFirst)
			{
				text2 = urlBuilder(1);
				stringBuilder.Append(string.Concat(new object[]
				{
					"<a",
					text3,
					" href=\"",
					text2,
					"\">",
					1,
					"</a>"
				}));
				if (pager.CanShowPrevious)
				{
					stringBuilder.Append("&nbsp;&nbsp;&nbsp;");
				}
			}
			for (int i = pager.StartingPage; i <= pager.EndingPage; i++)
			{
				text = ((i == pager.CurrentPage) ? (" class=\"" + settings.CssCurrentPage + "\"") : text3);
				text2 = urlBuilder(i);
				text2 = ((i == pager.CurrentPage) ? string.Empty : (" href=\"" + text2 + "\""));
				stringBuilder.Append(string.Concat(new object[]
				{
					"<a",
					text,
					text2,
					">",
					i,
					"</a>"
				}));
			}
			if (pager.CanShowLast)
			{
				text2 = urlBuilder(pager.TotalPages);
				if (pager.CanShowNext)
				{
					stringBuilder.Append("&nbsp;&nbsp;&nbsp;");
				}
				stringBuilder.Append(string.Concat(new object[]
				{
					"<a",
					text3,
					" href=\"",
					text2,
					"\">",
					pager.TotalPages,
					"</a>"
				}));
			}
			if (pager.CanShowNext)
			{
				text2 = urlBuilder(pager.CurrentPage + 1);
				stringBuilder.Append(string.Concat(new string[]
				{
					"<a",
					text3,
					" href=\"",
					text2,
					"\">&#187;</a>"
				}));
			}
			return stringBuilder.ToString();
		}
	}
}
