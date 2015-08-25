using System;
namespace XUtils.Ioc
{
	public interface IServicesContainer
	{
		TService Resolve<TService>() where TService : IService;
		IService Resolve(Type tService);
	}
}
