using System;
using System.Threading;
namespace XUtils
{
	public static class Retry
	{
		public static void Task(RetryMode mode, Action<int> callback, Func<int, string, bool> errorCallback = null)
		{
			Retry.Task(mode, new RetryTrigger().MaxRuns(3), callback, errorCallback);
		}
		public static void Task(RetryMode mode, RetryTrigger trigger, Action<int> callback, Func<int, string, bool> errorCallback = null)
		{
			if (mode == RetryMode.Async)
			{
				Retry.AsyncExecute(delegate
				{
					Retry.Task(RetryMode.Sync, trigger, callback, errorCallback);
				});
				return;
			}
			for (int i = 1; i <= trigger.MaxRun; i++)
			{
				if (Retry.ActionExecute(i, callback, errorCallback))
				{
					return;
				}
				if (i <= trigger.MaxRun)
				{
					Thread.Sleep(trigger.Wait);
				}
			}
		}
		private static bool ActionExecute(int number, Action<int> callback, Func<int, string, bool> errorCallback)
		{
			bool result;
			try
			{
				callback(number);
				result = true;
			}
			catch (Exception ex)
			{
				if (errorCallback != null)
				{
					result = errorCallback(number, ex.Message);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		private static void AsyncExecute(Action action)
		{
			action.BeginInvoke(delegate(IAsyncResult mCallback)
			{
				action.EndInvoke(mCallback);
			}, null);
		}
	}
}
