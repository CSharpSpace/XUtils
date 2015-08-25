using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
namespace XUtils.Threading.Base.Internal
{
	internal class CallerThreadContext
	{
		private static readonly MethodInfo getLogicalCallContextMethodInfo = typeof(Thread).GetMethod("GetLogicalCallContext", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo setLogicalCallContextMethodInfo = typeof(Thread).GetMethod("SetLogicalCallContext", BindingFlags.Instance | BindingFlags.NonPublic);
		private static string HttpContextSlotName = CallerThreadContext.GetHttpContextSlotName();
		private HttpContext _httpContext;
		private LogicalCallContext _callContext;
		public bool CapturedCallContext
		{
			get
			{
				return null != this._callContext;
			}
		}
		public bool CapturedHttpContext
		{
			get
			{
				return null != this._httpContext;
			}
		}
		private static string GetHttpContextSlotName()
		{
			FieldInfo field = typeof(HttpContext).GetField("CallContextSlotName", BindingFlags.Static | BindingFlags.NonPublic);
			if (field != null)
			{
				return (string)field.GetValue(null);
			}
			return "HttpContext";
		}
		private CallerThreadContext()
		{
		}
		public static CallerThreadContext Capture(bool captureCallContext, bool captureHttpContext)
		{
			CallerThreadContext callerThreadContext = new CallerThreadContext();
			if (captureCallContext && CallerThreadContext.getLogicalCallContextMethodInfo != null)
			{
				callerThreadContext._callContext = (LogicalCallContext)CallerThreadContext.getLogicalCallContextMethodInfo.Invoke(Thread.CurrentThread, null);
				if (callerThreadContext._callContext != null)
				{
					callerThreadContext._callContext = (LogicalCallContext)callerThreadContext._callContext.Clone();
				}
			}
			if (captureHttpContext && HttpContext.Current != null)
			{
				callerThreadContext._httpContext = HttpContext.Current;
			}
			return callerThreadContext;
		}
		public static void Apply(CallerThreadContext callerThreadContext)
		{
			if (callerThreadContext == null)
			{
				throw new ArgumentNullException("callerThreadContext");
			}
			if (callerThreadContext._callContext != null && CallerThreadContext.setLogicalCallContextMethodInfo != null)
			{
				CallerThreadContext.setLogicalCallContextMethodInfo.Invoke(Thread.CurrentThread, new object[]
				{
					callerThreadContext._callContext
				});
			}
			if (callerThreadContext._httpContext != null)
			{
				HttpContext.Current = callerThreadContext._httpContext;
			}
		}
	}
}
