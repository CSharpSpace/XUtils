using System;
using System.Web;
using XUtils.Configuration;
namespace XUtils.Web
{
	public static class CookieHelper
	{
		static CookieHelper()
		{
		}
		public static void CreateCookieValue(string cookieName, string cookieValue)
		{
			CookieHelper.Insert(new HttpCookie(cookieName)
			{
				Value = cookieValue,
				Domain = Config.Get<string>("domain"),
				Path = "/"
			});
		}
		public static void CreateCookieValue(string cookieName, string cookieValue, DateTime cookieTime)
		{
			CookieHelper.Insert(new HttpCookie(cookieName)
			{
				Value = cookieValue,
				Domain = Config.Get<string>("domain"),
				Path = "/",
				Expires = cookieTime
			});
		}
		public static void CreateCookieValue(string cookieName, string cookieValue, string subCookieName, string subCookieValue)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName);
			httpCookie.Value = cookieValue;
			httpCookie.Domain = Config.Get<string>("domain");
			httpCookie.Path = "/";
			httpCookie[subCookieName] = subCookieValue;
			CookieHelper.Insert(httpCookie);
		}
		public static void CreateCookieValue(string cookieName, string cookieValue, string subCookieName, string subCookieValue, DateTime cookieTime)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName);
			httpCookie.Value = cookieValue;
			httpCookie.Domain = Config.Get<string>("domain");
			httpCookie.Path = "/";
			httpCookie[subCookieName] = subCookieValue;
			httpCookie.Expires = cookieTime;
			CookieHelper.Insert(httpCookie);
		}
		public static string GetCookieValue(string cookieName)
		{
			string result = string.Empty;
			HttpCookie cookie = CookieHelper.GetCookie(cookieName);
			if (cookie != null)
			{
				result = cookie.Value;
			}
			return result;
		}
		public static string GetCookieValue(string cookieName, string subCookieName)
		{
			string result = string.Empty;
			HttpCookie cookie = CookieHelper.GetCookie(cookieName);
			if (cookie != null)
			{
				result = cookie.Value.Split(new char[]
				{
					'&'
				})[1].ToString().Split(new char[]
				{
					'='
				})[1];
			}
			return result;
		}
		public static void RemoveCookieValue(string cookieName)
		{
			CookieHelper.CreateCookieValue(cookieName, string.Empty, DateTime.MinValue);
		}
		private static void Insert(HttpCookie _cookie)
		{
			HttpContext.Current.Response.Cookies.Add(_cookie);
		}
		private static HttpCookie GetCookie(string cookieName)
		{
			return HttpContext.Current.Request.Cookies[cookieName];
		}
	}
}
