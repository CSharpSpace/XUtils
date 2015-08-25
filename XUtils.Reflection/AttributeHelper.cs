using System;
using System.Collections.Generic;
using System.Reflection;
namespace XUtils.Reflection
{
	public class AttributeHelper
	{
		public static string GetAssemblyInfoDescription(Type type, string defaultVal)
		{
			Assembly assembly = type.Assembly;
			bool flag = Attribute.IsDefined(assembly, typeof(AssemblyDescriptionAttribute));
			string result = defaultVal;
			if (flag)
			{
				AssemblyDescriptionAttribute assemblyDescriptionAttribute = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute));
				if (assemblyDescriptionAttribute != null)
				{
					result = assemblyDescriptionAttribute.Description;
				}
			}
			return result;
		}
		public static IList<T> GetClassAttributes<T>(object obj)
		{
			if (obj == null)
			{
				return new List<T>();
			}
			object[] customAttributes = obj.GetType().GetCustomAttributes(typeof(T), false);
			IList<T> list = new List<T>();
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				object obj2 = array[i];
				list.Add((T)((object)obj2));
			}
			return list;
		}
		public static IList<KeyValuePair<Type, T>> GetClassAttributesFromAssembly<T>(string assemblyName, Action<KeyValuePair<Type, T>> action)
		{
			Assembly assembly = Assembly.Load(assemblyName);
			Type[] types = assembly.GetTypes();
			List<KeyValuePair<Type, T>> list = new List<KeyValuePair<Type, T>>();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				object[] customAttributes = type.GetCustomAttributes(typeof(T), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					KeyValuePair<Type, T> keyValuePair = new KeyValuePair<Type, T>(type, (T)((object)customAttributes[0]));
					list.Add(keyValuePair);
					action(keyValuePair);
				}
			}
			return list;
		}
		public static IDictionary<string, KeyValuePair<T, PropertyInfo>> GetPropsWithAttributes<T>(object obj) where T : Attribute
		{
			if (obj == null)
			{
				return new Dictionary<string, KeyValuePair<T, PropertyInfo>>();
			}
			Dictionary<string, KeyValuePair<T, PropertyInfo>> dictionary = new Dictionary<string, KeyValuePair<T, PropertyInfo>>();
			PropertyInfo[] properties = obj.GetType().GetProperties();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(T), true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					dictionary[propertyInfo.Name] = new KeyValuePair<T, PropertyInfo>(customAttributes[0] as T, propertyInfo);
				}
			}
			return dictionary;
		}
		public static List<PropertyInfo> GetPropsOnlyWithAttributes<T>(object obj) where T : Attribute
		{
			if (obj == null)
			{
				return new List<PropertyInfo>();
			}
			List<PropertyInfo> list = new List<PropertyInfo>();
			PropertyInfo[] properties = obj.GetType().GetProperties();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(T), true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					list.Add(propertyInfo);
				}
			}
			return list;
		}
		public static List<KeyValuePair<T, PropertyInfo>> GetPropsWithAttributesList<T>(object obj) where T : Attribute
		{
			if (obj == null)
			{
				return new List<KeyValuePair<T, PropertyInfo>>();
			}
			List<KeyValuePair<T, PropertyInfo>> list = new List<KeyValuePair<T, PropertyInfo>>();
			IList<PropertyInfo> allProperties = ReflectionUtils.GetAllProperties(obj, null);
			foreach (PropertyInfo current in allProperties)
			{
				object[] customAttributes = current.GetCustomAttributes(typeof(T), true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					list.Add(new KeyValuePair<T, PropertyInfo>(customAttributes[0] as T, current));
				}
			}
			return list;
		}
		public static IList<KeyValuePair<PropertyInfo, TPropAttrib>> GetPropertiesWithAttributesOnTypes<TPropAttrib>(IList<Type> types, Action<Type, KeyValuePair<PropertyInfo, TPropAttrib>> action) where TPropAttrib : Attribute
		{
			List<KeyValuePair<PropertyInfo, TPropAttrib>> list = new List<KeyValuePair<PropertyInfo, TPropAttrib>>();
			foreach (Type current in types)
			{
				PropertyInfo[] properties = current.GetProperties();
				PropertyInfo[] array = properties;
				for (int i = 0; i < array.Length; i++)
				{
					PropertyInfo propertyInfo = array[i];
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(TPropAttrib), true);
					if (customAttributes != null && customAttributes.Length > 0)
					{
						KeyValuePair<PropertyInfo, TPropAttrib> keyValuePair = new KeyValuePair<PropertyInfo, TPropAttrib>(propertyInfo, customAttributes[0] as TPropAttrib);
						list.Add(keyValuePair);
						action(current, keyValuePair);
					}
				}
			}
			return list;
		}
	}
}
