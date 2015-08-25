using System;
using System.Collections.Generic;
using System.Linq;
namespace XUtils.Ioc
{
	public static class ServicesFilters
	{
		public static IServicesImplementationCollection Except(this IServicesImplementationCollection @this, IEnumerable<Type> except)
		{
			return new ServicesImplementationCollection
			{
				Actual = @this.Except(except)
			};
		}
		public static IServicesImplementationCollection Except(this IServicesImplementationCollection @this, params Type[] except)
		{
			return @this.Except((IEnumerable<Type>)except);
		}
		public static IServicesImplementationCollection NotImplementing<TService>(this IServicesImplementationCollection @this)
		{
			return @this.NotImplementing(typeof(TService));
		}
		public static IServicesImplementationCollection NotImplementing(this IServicesImplementationCollection @this, Type tService)
		{
			return new ServicesImplementationCollection
			{
				Actual = 
					from s in @this
					where !tService.IsAssignableFrom(s)
					select s
			};
		}
	}
}
