using System;
using System.Collections.Generic;
using System.Reflection;
namespace XUtils.Plugin
{
	public class PluginInstanceFactory : MarshalByRefObject
	{
		private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
		public T Create<T>(string assemblyFile, string typeName, object[] constructArgs)
		{
			return (T)((object)Activator.CreateInstanceFrom(assemblyFile, typeName, false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, constructArgs, null, null, null).Unwrap());
		}
		public IDictionary<string, string> LoadTypeForAll(string assemblyFile)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			Assembly assembly = Assembly.LoadFrom(assemblyFile);
			Type[] types = assembly.GetTypes();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (type != null && type.IsDefined(typeof(ImplementsAttribute), true))
				{
					object[] customAttributes = type.GetCustomAttributes(true);
					if (customAttributes.Length != 0)
					{
						string text = string.Empty;
						object[] array2 = customAttributes;
						for (int j = 0; j < array2.Length; j++)
						{
							object obj = array2[j];
							ImplementsAttribute implementsAttribute = obj as ImplementsAttribute;
							text = implementsAttribute.OnlyKey;
						}
						if (!string.IsNullOrEmpty(text))
						{
							ConstructorInfo constructor = type.GetConstructor(new Type[0]);
							if (constructor != null && type.IsMarshalByRef)
							{
								dictionary.Add(text, type.FullName);
							}
						}
					}
				}
			}
			return dictionary;
		}
	}
}
