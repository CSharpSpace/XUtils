using System;
namespace XUtils.Ioc
{
	internal class ServiceDescriptor
	{
		public Type ServiceType
		{
			get;
			set;
		}
		public IService Instance
		{
			get;
			set;
		}
	}
}
