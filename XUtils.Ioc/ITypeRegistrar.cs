using System;
using System.Collections.Generic;
namespace XUtils.Ioc
{
	public interface ITypeRegistrar
	{
		ITypeRegistrar RegisterFor(Type implementation, IEnumerable<Type> interfaces);
		ITypeRegistrar RegisterFor(Type implementation, params Type[] interfaces);
		ITypeRegistrar RegisterForAll(IEnumerable<Type> implementations);
		ITypeRegistrar RegisterForAll(params Type[] implementations);
	}
}
