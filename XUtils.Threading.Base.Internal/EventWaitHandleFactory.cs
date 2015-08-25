using System;
using System.Threading;
namespace XUtils.Threading.Base.Internal
{
	public static class EventWaitHandleFactory
	{
		public static AutoResetEvent CreateAutoResetEvent()
		{
			return new AutoResetEvent(false);
		}
		public static ManualResetEvent CreateManualResetEvent(bool initialState)
		{
			return new ManualResetEvent(initialState);
		}
	}
}
