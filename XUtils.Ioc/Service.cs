using System;
namespace XUtils.Ioc
{
	public class Service : IService
	{
		public IServicesContainer Services
		{
			get;
			set;
		}
	}
}
