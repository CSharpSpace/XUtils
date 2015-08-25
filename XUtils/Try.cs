using System;
namespace XUtils
{
	public class Try
	{
		private static LamdaLogger _logger = new LamdaLogger();
		public static LamdaLogger Logger
		{
			get
			{
				return Try._logger;
			}
			set
			{
				Try._logger = value;
			}
		}
		public static void CatchLog(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Try._logger.Error(null, ex, null);
			}
		}
		public static void CatchLog(string errorMessage, Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Try._logger.Error(errorMessage, ex, null);
			}
		}
		public static void CatchLogRethrow(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Try._logger.Error(null, ex, null);
				throw ex;
			}
		}
		public static void CatchLog(string errorMessage, Action action, Action<object, Exception, object[]> logger)
		{
			try
			{
				action();
			}
			catch (Exception arg)
			{
				if (logger != null)
				{
					logger(errorMessage, arg, null);
				}
			}
		}
		public static void Catch(Action action, Action<Exception> exceptionHandler = null)
		{
			try
			{
				action();
			}
			catch (Exception obj)
			{
				if (exceptionHandler != null)
				{
					exceptionHandler(obj);
				}
			}
		}
		public static void CatchHandle(string errorMessage, Action action, Action<Exception> exceptionHandler, Action finallyHandler)
		{
			try
			{
				action();
			}
			catch (Exception obj)
			{
				if (exceptionHandler != null)
				{
					exceptionHandler(obj);
				}
			}
			finally
			{
				if (finallyHandler != null)
				{
					finallyHandler();
				}
			}
		}
		public static T CatchLogGet<T>(string errorMessage, Func<T> action)
		{
			return Try.CatchLogGet<T>(errorMessage, false, action, null);
		}
		public static T CatchLogGet<T>(string errorMessage, bool rethrow, Func<T> action, Action<object, Exception, object[]> logger)
		{
			T result = default(T);
			try
			{
				result = action();
			}
			catch (Exception ex)
			{
				if (logger != null)
				{
					logger(errorMessage, ex, null);
				}
				else
				{
					if (Try._logger != null)
					{
						Try._logger.Error(errorMessage, ex, null);
					}
				}
				if (rethrow)
				{
					throw ex;
				}
			}
			return result;
		}
	}
}
