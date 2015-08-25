using System;
namespace XUtils.Ioc
{
	public interface IService
	{
		IServicesContainer Services
		{
			get;
			set;
		}
	}
}
