using System;
namespace XUtils.Logging
{
	public interface ILogMulti : ILog
	{
		ILog this[string loggerName]
		{
			get;
		}
		ILog this[int index]
		{
			get;
		}
		int Count
		{
			get;
		}
		void Append(ILog logger);
		bool ContainsKey(string key);
		void Replace(ILog logger);
		void Clear();
	}
}
