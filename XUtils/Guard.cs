using System;
namespace XUtils
{
	public sealed class Guard
	{
		public static void IsTrue(bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException("The condition supplied is false");
			}
		}
		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				throw new ArgumentException(message);
			}
		}
		public static void IsFalse(bool condition)
		{
			if (condition)
			{
				throw new ArgumentException("The condition supplied is true");
			}
		}
		public static void IsFalse(bool condition, string message)
		{
			if (condition)
			{
				throw new ArgumentException(message);
			}
		}
		public static void IsNotNull(object obj, string message)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(message);
			}
		}
		public static void IsNotNull(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("The argument provided cannot be null.");
			}
		}
		public static void IsNull(object obj, string message)
		{
			if (obj != null)
			{
				throw new ArgumentNullException(message);
			}
		}
		public static void IsNull(object obj)
		{
			if (obj != null)
			{
				throw new ArgumentNullException("The argument provided cannot be null.");
			}
		}
	}
}
