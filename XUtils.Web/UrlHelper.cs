using System;
using System.Web;
namespace XUtils.Web
{
	public class UrlHelper
	{
		private static bool _isRewritingEnabled = false;
		private static UrlMapper _urlMapper;
		public static bool IsRewritingEnabled
		{
			get
			{
				return UrlHelper._isRewritingEnabled;
			}
			set
			{
				UrlHelper._isRewritingEnabled = value;
				if (UrlHelper._isRewritingEnabled && UrlHelper._urlMapper == null)
				{
					throw new InvalidOperationException("Must provide url mapping if performing url-rewriting.");
				}
			}
		}
		public UrlMapper UrlMapperProvider
		{
			get
			{
				return UrlHelper._urlMapper;
			}
		}
		public static string GetRelativeSiteUrl(string url)
		{
			return UrlHelper.GetRelativeSiteUrl(HttpContext.Current.Request.ApplicationPath, url);
		}
		public static string GetMappedRelativeSiteUrl(string url)
		{
			return UrlHelper.GetMappedRelativeSiteUrl(HttpContext.Current.Request.ApplicationPath, url);
		}
		public static void ConfigureUrlRewriting(bool isUrlRewritingEnabled, UrlMapper urlMapper)
		{
			UrlHelper._isRewritingEnabled = isUrlRewritingEnabled;
			if (UrlHelper._isRewritingEnabled && urlMapper == null)
			{
				throw new InvalidOperationException("Must provide urlmapper if performing url-rewriting.");
			}
			UrlHelper._urlMapper = urlMapper;
		}
		public static string GetSiteRoot()
		{
			string text = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
			if (text == null || text == "0")
			{
				text = "http://";
			}
			else
			{
				text = "https://";
			}
			string text2 = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
			if (text2 == null || text2 == "80" || text2 == "443")
			{
				text2 = "";
			}
			else
			{
				text2 = ":" + text2;
			}
			return text + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + text2 + HttpContext.Current.Request.ApplicationPath;
		}
		public static string GetRequestedFileName(string rawUrl, bool includeExtension)
		{
			string text = rawUrl.Substring(rawUrl.LastIndexOf("/") + 1);
			if (includeExtension)
			{
				return text;
			}
			return text.Substring(0, text.IndexOf("."));
		}
		internal static string GetRelativeSiteUrl(string applicationPath, string url)
		{
			if (!applicationPath.EndsWith("/"))
			{
				applicationPath += "/";
			}
			if (!string.IsNullOrEmpty(url) && url.StartsWith("~/"))
			{
				url = url.Substring(2, url.Length - 2);
			}
			return applicationPath + url;
		}
		internal static string GetMappedRelativeSiteUrl(string applicationPath, string url)
		{
			if (!UrlHelper.IsRewritingEnabled || UrlHelper._urlMapper == null)
			{
				return UrlHelper.GetRelativeSiteUrl(applicationPath, url);
			}
			url = UrlHelper._urlMapper.GetUrl(url);
			return UrlHelper.GetRelativeSiteUrl(applicationPath, url);
		}
	}
}
