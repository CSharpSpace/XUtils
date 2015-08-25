using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace XUtils.Ioc
{
	public class ServicesContainer : IServicesRegistrar, IServicesContainer, ITypeRegistrar
	{
		private readonly IDictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
		public TService Resolve<TService>() where TService : IService
		{
			return (TService)((object)this.Resolve(typeof(TService)));
		}
		public IService Resolve(Type tService)
		{
			IService instance = this.GetInstance(tService);
			instance.Services = this;
			return instance;
		}
		private IService GetInstance(Type tService)
		{
			if (this._services.ContainsKey(tService))
			{
				return this.GetInstance(this._services[tService]);
			}
			Type genericTypeDefinition = tService.GetGenericTypeDefinition();
			if (this._services.ContainsKey(genericTypeDefinition))
			{
				return this.GetGenericInstance(tService, this._services[genericTypeDefinition].ServiceType);
			}
			throw new Exception("Type not registered" + tService);
		}
		private IService GetInstance(ServiceDescriptor serviceDescriptor)
		{
			IService arg_1F_0;
			if ((arg_1F_0 = serviceDescriptor.Instance) == null)
			{
				arg_1F_0 = (serviceDescriptor.Instance = this.CreateInstance(serviceDescriptor.ServiceType));
			}
			return arg_1F_0;
		}
		private IService GetGenericInstance(Type tService, Type genericDefinition)
		{
			Type[] genericArguments = tService.GetGenericArguments();
			Type serviceType = genericDefinition.MakeGenericType(genericArguments);
			IService service = this.CreateInstance(serviceType);
			this._services[tService] = new ServiceDescriptor
			{
				ServiceType = serviceType,
				Instance = service
			};
			return service;
		}
		private IService CreateInstance(Type serviceType)
		{
			ConstructorInfo constructorInfo = serviceType.GetConstructors().First<ConstructorInfo>();
			IService[] parameters = (
				from p in constructorInfo.GetParameters()
				select this.Resolve(p.ParameterType)).ToArray<IService>();
			return (IService)constructorInfo.Invoke(parameters);
		}
		public ITypeRegistrar RegisterForAll(params Type[] implementations)
		{
			return this.RegisterForAll((IEnumerable<Type>)implementations);
		}
		public ITypeRegistrar RegisterForAll(IEnumerable<Type> implementations)
		{
			foreach (Type current in implementations)
			{
				this.RegisterFor(current, current.GetInterfaces());
			}
			return this;
		}
		public ITypeRegistrar RegisterFor(Type implementation, params Type[] interfaces)
		{
			return this.RegisterFor(implementation, (IEnumerable<Type>)interfaces);
		}
		public ITypeRegistrar RegisterFor(Type implementation, IEnumerable<Type> interfaces)
		{
			foreach (Type current in interfaces)
			{
				this._services[ServicesContainer.GetRegistrableType(current)] = new ServiceDescriptor
				{
					ServiceType = implementation
				};
			}
			return this;
		}
		private static Type GetRegistrableType(Type type)
		{
			if (!type.IsGenericType)
			{
				return type;
			}
			return type.GetGenericTypeDefinition();
		}
	}
}
