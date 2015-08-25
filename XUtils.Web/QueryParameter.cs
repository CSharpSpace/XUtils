using System;
using System.Text;
using System.Web;
namespace XUtils.Web
{
	public static class QueryParameter
	{
		public static T Get<T>(string key)
		{
			T result;
			try
			{
				string text = string.Empty;
				HttpRequest request = HttpContext.Current.Request;
				if (request.HttpMethod.ToUpper() == "GET")
				{
					text = request.QueryString[key];
				}
				else
				{
					text = request[key];
				}
				text = HttpUtility.UrlDecode(text, Encoding.UTF8);
				T t = TypeParsers.ConvertTo<T>(text);
				result = t;
			}
			catch
			{
				throw;
			}
			return result;
		}
	}
}
