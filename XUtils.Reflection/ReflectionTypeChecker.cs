using System;
using System.Reflection;
namespace XUtils.Reflection
{
	public class ReflectionTypeChecker
	{
		public static bool CanConvertTo<T>(string val)
		{
			return ReflectionTypeChecker.CanConvertTo(typeof(T), val);
		}
		public static bool CanConvertTo(Type type, string val)
		{
			try
			{
				if (type == typeof(int))
				{
					int num = 0;
					bool result;
					if (int.TryParse(val, out num))
					{
						result = true;
						return result;
					}
					result = false;
					return result;
				}
				else
				{
					if (type == typeof(string))
					{
						bool result = true;
						return result;
					}
					if (type == typeof(double))
					{
						double num2 = 0.0;
						bool result;
						if (double.TryParse(val, out num2))
						{
							result = true;
							return result;
						}
						result = false;
						return result;
					}
					else
					{
						if (type == typeof(long))
						{
							long num3 = 0L;
							bool result;
							if (long.TryParse(val, out num3))
							{
								result = true;
								return result;
							}
							result = false;
							return result;
						}
						else
						{
							if (type == typeof(float))
							{
								float num4 = 0f;
								bool result;
								if (float.TryParse(val, out num4))
								{
									result = true;
									return result;
								}
								result = false;
								return result;
							}
							else
							{
								if (type == typeof(bool))
								{
									bool flag = false;
									bool result;
									if (bool.TryParse(val, out flag))
									{
										result = true;
										return result;
									}
									result = false;
									return result;
								}
								else
								{
									if (type == typeof(DateTime))
									{
										DateTime minValue = DateTime.MinValue;
										bool result;
										if (DateTime.TryParse(val, out minValue))
										{
											result = true;
											return result;
										}
										result = false;
										return result;
									}
									else
									{
										if (type.BaseType == typeof(Enum))
										{
											Enum.Parse(type, val, true);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
				bool result = false;
				return result;
			}
			return true;
		}
		public static bool CanConvertToCorrectType(PropertyInfo propInfo, object val)
		{
			try
			{
				if (propInfo.PropertyType == typeof(int))
				{
					Convert.ToInt32(val);
				}
				else
				{
					if (propInfo.PropertyType == typeof(double))
					{
						Convert.ToDouble(val);
					}
					else
					{
						if (propInfo.PropertyType == typeof(long))
						{
							Convert.ToInt64(val);
						}
						else
						{
							if (propInfo.PropertyType == typeof(float))
							{
								Convert.ToSingle(val);
							}
							else
							{
								if (propInfo.PropertyType == typeof(bool))
								{
									Convert.ToBoolean(val);
								}
								else
								{
									if (propInfo.PropertyType == typeof(DateTime))
									{
										Convert.ToDateTime(val);
									}
									else
									{
										if (propInfo.PropertyType.BaseType == typeof(Enum) && val is string)
										{
											Enum.Parse(propInfo.PropertyType, (string)val, true);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
		public static bool CanConvertToCorrectType(PropertyInfo propInfo, string val)
		{
			try
			{
				if (propInfo.PropertyType == typeof(int))
				{
					int num = 0;
					bool result;
					if (int.TryParse(val, out num))
					{
						result = true;
						return result;
					}
					result = false;
					return result;
				}
				else
				{
					if (propInfo.PropertyType == typeof(string))
					{
						bool result = true;
						return result;
					}
					if (propInfo.PropertyType == typeof(double))
					{
						double num2 = 0.0;
						bool result;
						if (double.TryParse(val, out num2))
						{
							result = true;
							return result;
						}
						result = false;
						return result;
					}
					else
					{
						if (propInfo.PropertyType == typeof(long))
						{
							long num3 = 0L;
							bool result;
							if (long.TryParse(val, out num3))
							{
								result = true;
								return result;
							}
							result = false;
							return result;
						}
						else
						{
							if (propInfo.PropertyType == typeof(float))
							{
								float num4 = 0f;
								bool result;
								if (float.TryParse(val, out num4))
								{
									result = true;
									return result;
								}
								result = false;
								return result;
							}
							else
							{
								if (propInfo.PropertyType == typeof(bool))
								{
									bool flag = false;
									bool result;
									if (bool.TryParse(val, out flag))
									{
										result = true;
										return result;
									}
									result = false;
									return result;
								}
								else
								{
									if (propInfo.PropertyType == typeof(DateTime))
									{
										DateTime minValue = DateTime.MinValue;
										bool result;
										if (DateTime.TryParse(val, out minValue))
										{
											result = true;
											return result;
										}
										result = false;
										return result;
									}
									else
									{
										if (propInfo.PropertyType.BaseType == typeof(Enum))
										{
											Enum.Parse(propInfo.PropertyType, val, true);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
				bool result = false;
				return result;
			}
			return true;
		}
		public static object ConvertToSameType(PropertyInfo propInfo, object val)
		{
			object result = null;
			if (propInfo.PropertyType == typeof(int))
			{
				result = Convert.ChangeType(val, typeof(int));
			}
			else
			{
				if (propInfo.PropertyType == typeof(double))
				{
					result = Convert.ChangeType(val, typeof(double));
				}
				else
				{
					if (propInfo.PropertyType == typeof(long))
					{
						result = Convert.ChangeType(val, typeof(long));
					}
					else
					{
						if (propInfo.PropertyType == typeof(float))
						{
							result = Convert.ChangeType(val, typeof(float));
						}
						else
						{
							if (propInfo.PropertyType == typeof(bool))
							{
								result = Convert.ChangeType(val, typeof(bool));
							}
							else
							{
								if (propInfo.PropertyType == typeof(DateTime))
								{
									result = Convert.ChangeType(val, typeof(DateTime));
								}
								else
								{
									if (propInfo.PropertyType == typeof(string))
									{
										result = Convert.ChangeType(val, typeof(string));
									}
									else
									{
										if (propInfo.PropertyType.BaseType == typeof(Enum) && val is string)
										{
											result = Enum.Parse(propInfo.PropertyType, (string)val, true);
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static bool IsSameType(PropertyInfo propInfo, object val)
		{
			return (propInfo.PropertyType == typeof(int) && val is int) || (propInfo.PropertyType == typeof(bool) && val is bool) || (propInfo.PropertyType == typeof(string) && val is string) || (propInfo.PropertyType == typeof(double) && val is double) || (propInfo.PropertyType == typeof(long) && val is long) || (propInfo.PropertyType == typeof(float) && val is float) || (propInfo.PropertyType == typeof(DateTime) && val is DateTime) || (propInfo.PropertyType != null && propInfo.PropertyType.GetType() == val.GetType());
		}
	}
}
