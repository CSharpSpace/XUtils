using System;
using System.Globalization;
using System.Resources;
using System.Threading;
namespace XUtils
{
	internal class ResourceStrings
	{
		private static ResourceManager SystemResMgr;
		private static object ResMgrLockObject;
		private static ResourceManager InitResourceManager()
		{
			if (ResourceStrings.SystemResMgr == null)
			{
				Type typeFromHandle;
				Monitor.Enter(typeFromHandle = typeof(Environment));
				try
				{
					if (ResourceStrings.SystemResMgr == null)
					{
						ResourceStrings.ResMgrLockObject = new object();
						ResourceStrings.SystemResMgr = new ResourceManager("mscorlib", typeof(string).Assembly);
					}
				}
				finally
				{
					Monitor.Exit(typeFromHandle);
				}
			}
			return ResourceStrings.SystemResMgr;
		}
		internal static string GetString(string key)
		{
			if (ResourceStrings.SystemResMgr == null)
			{
				ResourceStrings.InitResourceManager();
			}
			object resMgrLockObject;
			Monitor.Enter(resMgrLockObject = ResourceStrings.ResMgrLockObject);
			string @string;
			try
			{
				@string = ResourceStrings.SystemResMgr.GetString(key, null);
			}
			finally
			{
				Monitor.Exit(resMgrLockObject);
			}
			return @string;
		}
		internal static string GetString(string key, params object[] args)
		{
			if (ResourceStrings.SystemResMgr == null)
			{
				ResourceStrings.InitResourceManager();
			}
			object resMgrLockObject;
			Monitor.Enter(resMgrLockObject = ResourceStrings.ResMgrLockObject);
			string @string;
			try
			{
				@string = ResourceStrings.SystemResMgr.GetString(key, null);
			}
			finally
			{
				Monitor.Exit(resMgrLockObject);
			}
			if (args == null || args.Length <= 0)
			{
				return @string;
			}
			for (int i = 0; i < args.Length; i++)
			{
				string text = args[i] as string;
				if (text != null && text.Length > 1024)
				{
					args[i] = text.Substring(0, 1021) + "...";
				}
			}
			return string.Format(CultureInfo.CurrentCulture, @string, args);
		}
	}
}
