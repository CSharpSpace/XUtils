using System;
using System.Collections.Generic;
using System.Text;
namespace XUtils
{
	public class TypeHelper
	{
		private static IDictionary<string, bool> _numericTypes;
		private static IDictionary<string, bool> _basicTypes;
		static TypeHelper()
		{
			TypeHelper._numericTypes = new Dictionary<string, bool>();
			TypeHelper._numericTypes[typeof(int).Name] = true;
			TypeHelper._numericTypes[typeof(long).Name] = true;
			TypeHelper._numericTypes[typeof(float).Name] = true;
			TypeHelper._numericTypes[typeof(double).Name] = true;
			TypeHelper._numericTypes[typeof(decimal).Name] = true;
			TypeHelper._numericTypes[typeof(sbyte).Name] = true;
			TypeHelper._numericTypes[typeof(short).Name] = true;
			TypeHelper._numericTypes[typeof(int).Name] = true;
			TypeHelper._numericTypes[typeof(long).Name] = true;
			TypeHelper._numericTypes[typeof(double).Name] = true;
			TypeHelper._numericTypes[typeof(decimal).Name] = true;
			TypeHelper._basicTypes = new Dictionary<string, bool>();
			TypeHelper._basicTypes[typeof(int).Name] = true;
			TypeHelper._basicTypes[typeof(long).Name] = true;
			TypeHelper._basicTypes[typeof(float).Name] = true;
			TypeHelper._basicTypes[typeof(double).Name] = true;
			TypeHelper._basicTypes[typeof(decimal).Name] = true;
			TypeHelper._basicTypes[typeof(sbyte).Name] = true;
			TypeHelper._basicTypes[typeof(short).Name] = true;
			TypeHelper._basicTypes[typeof(int).Name] = true;
			TypeHelper._basicTypes[typeof(long).Name] = true;
			TypeHelper._basicTypes[typeof(double).Name] = true;
			TypeHelper._basicTypes[typeof(decimal).Name] = true;
			TypeHelper._basicTypes[typeof(bool).Name] = true;
			TypeHelper._basicTypes[typeof(DateTime).Name] = true;
			TypeHelper._basicTypes[typeof(string).Name] = true;
		}
		public static bool IsNumeric(object val)
		{
			return TypeHelper._numericTypes.ContainsKey(val.GetType().Name);
		}
		public static bool IsNumeric(Type type)
		{
			return TypeHelper._numericTypes.ContainsKey(type.Name);
		}
		public static bool IsBasicType(Type type)
		{
			return TypeHelper._basicTypes.ContainsKey(type.Name);
		}
		public static string Join(object[] vals)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Type type = vals[0].GetType();
			if (type == typeof(int[]))
			{
				int[] array = vals[0] as int[];
				stringBuilder.Append(array[0].ToString());
				for (int i = 1; i < array.Length; i++)
				{
					stringBuilder.Append(", " + array[i]);
				}
			}
			else
			{
				if (type == typeof(long[]))
				{
					long[] array2 = vals[0] as long[];
					stringBuilder.Append(array2[0].ToString());
					for (int j = 1; j < array2.Length; j++)
					{
						stringBuilder.Append(", " + array2[j]);
					}
				}
				else
				{
					if (type == typeof(float[]))
					{
						float[] array3 = vals[0] as float[];
						stringBuilder.Append(array3[0].ToString());
						for (int k = 1; k < array3.Length; k++)
						{
							stringBuilder.Append(", " + array3[k]);
						}
					}
					else
					{
						if (type == typeof(double[]))
						{
							double[] array4 = vals[0] as double[];
							stringBuilder.Append(array4[0].ToString());
							for (int l = 1; l < array4.Length; l++)
							{
								stringBuilder.Append(", " + array4[l]);
							}
						}
						else
						{
							stringBuilder.Append(vals[0].ToString());
							for (int m = 1; m < vals.Length; m++)
							{
								stringBuilder.Append(", " + vals[m]);
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
