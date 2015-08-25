using System;
namespace XUtils
{
	public static class TypeParsers
	{
		public static T ConvertTo<T>(object input)
		{
			object obj = default(T);
			if (typeof(T) == typeof(int))
			{
				obj = Convert.ToInt32(input);
			}
			else
			{
				if (typeof(T) == typeof(long))
				{
					obj = Convert.ToInt64(input);
				}
				else
				{
					if (typeof(T) == typeof(string))
					{
						obj = Convert.ToString(input);
					}
					else
					{
						if (typeof(T) == typeof(bool))
						{
							obj = Convert.ToBoolean(input);
						}
						else
						{
							if (typeof(T) == typeof(double))
							{
								obj = Convert.ToDouble(input);
							}
							else
							{
								if (typeof(T) == typeof(DateTime))
								{
									obj = Convert.ToDateTime(input);
								}
								else
								{
									if (typeof(T) == typeof(decimal))
									{
										obj = Convert.ToDecimal(input);
									}
								}
							}
						}
					}
				}
			}
			return (T)((object)obj);
		}
	}
}
