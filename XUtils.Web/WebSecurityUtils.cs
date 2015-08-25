using System;
using System.Globalization;
using System.Web;
namespace XUtils.Web
{
	public class WebSecurityUtils
	{
		public static bool IsSelfRequest(HttpContext ctx, ref string path, string requestDeniedImagePath)
		{
			HttpRequest request = ctx.Request;
			path = request.PhysicalPath;
			if (request.UrlReferrer != null && request.UrlReferrer.Host.Length > 0 && CultureInfo.InvariantCulture.CompareInfo.Compare(request.Url.Host, request.UrlReferrer.Host, CompareOptions.IgnoreCase) != 0)
			{
				path = ctx.Server.MapPath(requestDeniedImagePath);
				return false;
			}
			return true;
		}
	}
}
