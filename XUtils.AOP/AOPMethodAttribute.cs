using System;
using System.Runtime.Remoting.Contexts;
namespace XUtils.AOP
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public abstract class AOPMethodAttribute : ContextAttribute
	{
		public bool UseAspect
		{
			get;
			set;
		}
		public AOPMethodAttribute() : base("MethodAOP")
		{
		}
	}
}
