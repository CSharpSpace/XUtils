using System;
using System.Collections;
using System.Data;
namespace XUtils.Data
{
	public sealed class ADOHelperParameterCache
	{
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
		internal static IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
		{
			IDataParameter[] array = new IDataParameter[originalParameters.Length];
			int i = 0;
			int num = originalParameters.Length;
			while (i < num)
			{
				array[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
				i++;
			}
			return array;
		}
		public static void CacheParameterSet(string connectionString, string commandText, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string key = connectionString + ":" + commandText;
			ADOHelperParameterCache.paramCache[key] = commandParameters;
		}
		public static IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string key = connectionString + ":" + commandText;
			IDataParameter[] array = ADOHelperParameterCache.paramCache[key] as IDataParameter[];
			if (array == null)
			{
				return null;
			}
			return ADOHelperParameterCache.CloneParameters(array);
		}
	}
}
