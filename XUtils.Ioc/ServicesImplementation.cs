using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace XUtils.Ioc
{
	public static class ServicesImplementation
	{
		private static readonly Type TypeOfIService = typeof(IService);
		public static IServicesImplementationCollection FromAssembly(string file)
		{
			Assembly assembly = Assembly.LoadFrom(file);
			return ServicesImplementation.FromThese(assembly.GetTypes());
		}
		public static IServicesImplementationCollection FromAssembly(Assembly asm)
		{
			return ServicesImplementation.FromThese(asm.GetTypes());
		}
		public static IServicesImplementationCollection FromAssemblyContaining(Type type)
		{
			return ServicesImplementation.FromAssembly(type.Assembly);
		}
		public static IServicesImplementationCollection FromAssemblyContaining<TType>()
		{
			return ServicesImplementation.FromAssembly(typeof(TType).Assembly);
		}
		public static IServicesImplementationCollection FromThese(IEnumerable<Type> types)
		{
			ServicesImplementationCollection servicesImplementationCollection = new ServicesImplementationCollection();
			servicesImplementationCollection.Actual = 
				from x in types
				where x.IsClass && !x.IsAbstract && ServicesImplementation.TypeOfIService.IsAssignableFrom(x)
				select x;
			return servicesImplementationCollection;
		}
		public static IServicesImplementationCollection FromThese(params Type[] types)
		{
			return ServicesImplementation.FromThese((IEnumerable<Type>)types);
		}
	}
}
