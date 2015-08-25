using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
namespace XUtils.Reflection
{
	public class ReflectionUtils
	{
		public static void SetProperties<T>(T obj, IList<KeyValuePair<string, string>> properties) where T : class
		{
			if (obj == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> current in properties)
			{
				ReflectionUtils.SetProperty<T>(obj, current.Key, current.Value);
			}
		}
		public static void SetProperty<T>(object obj, string propName, object propVal) where T : class
		{
			Guard.IsNotNull(obj, "Object containing properties to set is null");
			Guard.IsTrue(!string.IsNullOrEmpty(propName), "Property name not supplied.");
			propName = propName.Trim();
			if (string.IsNullOrEmpty(propName))
			{
				throw new ArgumentException("Property name is empty.");
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(propName);
			if (property != null && property.CanWrite && ReflectionTypeChecker.CanConvertToCorrectType(property, propVal))
			{
				object value = ReflectionTypeChecker.ConvertToSameType(property, propVal);
				property.SetValue(obj, value, null);
			}
		}
		public static void SetProperty(object obj, string propName, object propVal)
		{
			propName = propName.Trim();
			if (string.IsNullOrEmpty(propName))
			{
				throw new ArgumentException("Property name is empty.");
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(propName);
			if (property != null && property.CanWrite)
			{
				property.SetValue(obj, propVal, null);
			}
		}
		public static void SetProperty(object obj, PropertyInfo prop, object propVal, bool catchException)
		{
			if (prop != null && prop.CanWrite)
			{
				if (!catchException)
				{
					prop.SetValue(obj, propVal, null);
					return;
				}
				Try.Catch(delegate
				{
					prop.SetValue(obj, propVal, null);
				}, null);
			}
		}
		public static void SetProperty(object obj, PropertyInfo prop, string propVal)
		{
			Guard.IsNotNull(obj, "Object containing properties to set is null");
			Guard.IsNotNull(prop, "Property not supplied.");
			if (prop != null && prop.CanWrite && ReflectionTypeChecker.CanConvertToCorrectType(prop, propVal))
			{
				object value = ReflectionTypeChecker.ConvertToSameType(prop, propVal);
				prop.SetValue(obj, value, null);
			}
		}
		public static object GetPropertyValue(object obj, string propName)
		{
			Guard.IsNotNull(obj, "Must provide object to get it's property.");
			Guard.IsTrue(!string.IsNullOrEmpty(propName), "Must provide property name to get property value.");
			propName = propName.Trim();
			PropertyInfo property = obj.GetType().GetProperty(propName);
			if (property == null)
			{
				return null;
			}
			return property.GetValue(obj, null);
		}
		public static IList<object> GetPropertyValues(object obj, IList<string> properties)
		{
			IList<object> list = new List<object>();
			foreach (string current in properties)
			{
				PropertyInfo property = obj.GetType().GetProperty(current);
				object value = property.GetValue(obj, null);
				list.Add(value);
			}
			return list;
		}
		public static IList<PropertyInfo> GetProperties(object obj, string propsDelimited)
		{
			return ReflectionUtils.GetProperties(obj.GetType(), propsDelimited.Split(new char[]
			{
				','
			}));
		}
		public static IList<PropertyInfo> GetProperties(Type type, string[] props)
		{
			PropertyInfo[] properties = type.GetProperties();
			List<PropertyInfo> list = new List<PropertyInfo>();
			IDictionary<string, string> dictionary = props.ToDictionary<string>();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				if (dictionary.ContainsKey(propertyInfo.Name))
				{
					list.Add(propertyInfo);
				}
			}
			return list;
		}
		public static IList<PropertyInfo> GetProperties(Type type, string[] props, BindingFlags flags)
		{
			PropertyInfo[] properties = type.GetProperties(flags);
			List<PropertyInfo> list = new List<PropertyInfo>();
			IDictionary<string, string> dictionary = props.ToDictionary<string>();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				if (dictionary.ContainsKey(propertyInfo.Name))
				{
					list.Add(propertyInfo);
				}
			}
			return list;
		}
		public static object GetPropertyValueSafely(object obj, PropertyInfo propInfo)
		{
			Guard.IsNotNull(obj, "Must provide object to get it's property.");
			if (propInfo == null)
			{
				return null;
			}
			object result = null;
			try
			{
				result = propInfo.GetValue(obj, null);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}
		public static IList<PropertyInfo> GetAllProperties(object obj, Predicate<PropertyInfo> criteria)
		{
			if (obj == null)
			{
				return null;
			}
			return ReflectionUtils.GetProperties(obj.GetType(), criteria);
		}
		public static IList<PropertyInfo> GetProperties(Type type, Predicate<PropertyInfo> criteria)
		{
			IList<PropertyInfo> list = new List<PropertyInfo>();
			PropertyInfo[] properties = type.GetProperties();
			if (properties == null || properties.Length == 0)
			{
				return null;
			}
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				bool flag = criteria == null || criteria(propertyInfo);
				if (flag)
				{
					list.Add(propertyInfo);
				}
			}
			return list;
		}
		public static IList<PropertyInfo> GetWritableProperties(object obj, StringDictionary propsToExclude)
		{
			return ReflectionUtils.GetWritableProperties(obj.GetType(), (PropertyInfo property) => (propsToExclude == null) ? property.CanWrite : (property.CanWrite && !propsToExclude.ContainsKey(property.Name)));
		}
		public static IList<PropertyInfo> GetProperties(StringDictionary propsToExclude, Type typ)
		{
			return ReflectionUtils.GetWritableProperties(typ, (PropertyInfo property) => propsToExclude == null || !propsToExclude.ContainsKey(property.Name));
		}
		public static IDictionary<string, PropertyInfo> GetPropertiesAsMap(object obj, Predicate<PropertyInfo> criteria)
		{
			IList<PropertyInfo> properties = ReflectionUtils.GetProperties(obj.GetType(), criteria);
			IDictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
			foreach (PropertyInfo current in properties)
			{
				dictionary.Add(current.Name, current);
			}
			return dictionary;
		}
		public static IDictionary<string, PropertyInfo> GetPropertiesAsMap(Type type, BindingFlags flags, bool isCaseSensitive)
		{
			PropertyInfo[] properties = type.GetProperties(flags);
			IDictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				if (isCaseSensitive)
				{
					dictionary[propertyInfo.Name] = propertyInfo;
				}
				else
				{
					dictionary[propertyInfo.Name.Trim().ToLower()] = propertyInfo;
				}
			}
			return dictionary;
		}
		public static IDictionary<string, PropertyInfo> GetPropertiesAsMap<T>(BindingFlags flags, bool isCaseSensitive)
		{
			Type typeFromHandle = typeof(T);
			return ReflectionUtils.GetPropertiesAsMap(typeFromHandle, flags, isCaseSensitive);
		}
		public static PropertyInfo GetProperty(Type type, string propertyName)
		{
			IList<PropertyInfo> properties = ReflectionUtils.GetProperties(type, (PropertyInfo property) => property.Name == propertyName);
			return properties[0];
		}
		public static IList<PropertyInfo> GetWritableProperties(Type type, Predicate<PropertyInfo> criteria)
		{
			return ReflectionUtils.GetProperties(type, (PropertyInfo property) => (criteria == null) ? property.CanWrite : (property.CanWrite && criteria(property)));
		}
		public static object InvokeMethod(object obj, string methodName, object[] parameters)
		{
			Guard.IsNotNull(methodName, "Method name not provided.");
			Guard.IsNotNull(obj, "Can not invoke method on null object");
			methodName = methodName.Trim();
			if (string.IsNullOrEmpty(methodName))
			{
				throw new ArgumentException("Method name not provided.");
			}
			MethodInfo method = obj.GetType().GetMethod(methodName);
			return method.Invoke(obj, parameters);
		}
		public static void CopyPropertyValue(object source, object destination, PropertyInfo prop)
		{
			object value = prop.GetValue(source, null);
			prop.SetValue(destination, value, null);
		}
	}
}
