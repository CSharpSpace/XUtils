using System;
namespace XUtils
{
	public static class InstanceFactory
	{
		private static InstanceContainer iFactoryContainer;
		public static InstanceContainer Container
		{
			get
			{
				return InstanceFactory.iFactoryContainer;
			}
		}
		static InstanceFactory()
		{
			InstanceFactory.iFactoryContainer = new InstanceContainer();
		}
		public static T Cast<T>() where T : class, new()
		{
			return InstanceFactory.iFactoryContainer.Cast<T>(typeof(T).Name);
		}
		public static T Cast<T>(string Key) where T : class, new()
		{
			return InstanceFactory.iFactoryContainer.Cast<T>(Key);
		}
	}
}
