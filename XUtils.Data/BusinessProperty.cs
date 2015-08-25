using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
namespace XUtils.Data
{
	internal static class BusinessProperty
	{
		private static List<PropertyInfo> LoadPropertyMappingInfo(Type objType)
		{
			List<PropertyInfo> list = new List<PropertyInfo>();
			PropertyInfo[] properties = objType.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo item = properties[i];
				list.Add(item);
			}
			return list;
		}
		public static List<PropertyInfo> GetProperties(Type objType)
		{
			List<PropertyInfo> list = PropertyInfoCache.GetCache(objType.Name);
			if (list == null)
			{
				list = BusinessProperty.LoadPropertyMappingInfo(objType);
				PropertyInfoCache.SetCache(objType.Name, list);
			}
			return list;
		}
		private static int[] GetOrdinals(List<PropertyInfo> propMapList, IDataReader dr)
		{
			int[] array = new int[propMapList.Count];
			if (dr != null)
			{
				for (int i = 0; i <= propMapList.Count - 1; i++)
				{
					array[i] = -1;
					try
					{
						array[i] = dr.GetOrdinal(propMapList[i].Name);
					}
					catch (IndexOutOfRangeException)
					{
					}
				}
			}
			return array;
		}
		private static T CreateObject<T>(IDataReader dr, List<PropertyInfo> propInfoList, int[] ordinals) where T : class, new()
		{
			T t = Activator.CreateInstance<T>();
			for (int i = 0; i <= propInfoList.Count - 1; i++)
			{
				if (propInfoList[i].CanWrite)
				{
					Type propertyType = propInfoList[i].PropertyType;
					object value = propInfoList[i].GetValue(t, null);
					if (ordinals[i] != -1 && !dr.IsDBNull(ordinals[i]))
					{
						value = dr.GetValue(ordinals[i]);
					}
					try
					{
						propInfoList[i].SetValue(t, value, null);
					}
					catch
					{
						try
						{
							if (propertyType.BaseType.Equals(typeof(Enum)))
							{
								propInfoList[i].SetValue(t, Enum.ToObject(propertyType, value), null);
							}
							else
							{
								propInfoList[i].SetValue(t, Convert.ChangeType(value, propertyType), null);
							}
						}
						catch
						{
						}
					}
				}
			}
			return t;
		}
		public static T FillObject<T>(Type objType, IDataReader dr) where T : class, new()
		{
			T result = default(T);
			try
			{
				List<PropertyInfo> properties = BusinessProperty.GetProperties(objType);
				int[] ordinals = BusinessProperty.GetOrdinals(properties, dr);
				if (dr.Read())
				{
					result = BusinessProperty.CreateObject<T>(dr, properties, ordinals);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				if (!dr.IsClosed)
				{
					dr.Close();
				}
			}
			return result;
		}
		public static C FillCollection<T, C>(Type objType, IDataReader dr) where T : class, new() where C : ICollection<T>, new()
		{
			C result = (default(C) == null) ? Activator.CreateInstance<C>() : default(C);
			try
			{
				List<PropertyInfo> properties = BusinessProperty.GetProperties(objType);
				int[] ordinals = BusinessProperty.GetOrdinals(properties, dr);
				while (dr.Read())
				{
					T item = BusinessProperty.CreateObject<T>(dr, properties, ordinals);
					result.Add(item);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				if (!dr.IsClosed)
				{
					dr.Close();
				}
			}
			return result;
		}
	}
}
