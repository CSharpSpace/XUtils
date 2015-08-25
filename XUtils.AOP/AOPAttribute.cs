using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
namespace XUtils.AOP
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class AOPAttribute : ContextAttribute
	{
		public AOPAttribute() : base("AOP")
		{
		}
		public sealed override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			ctorMsg.ContextProperties.Add(this.GetAOPProperty());
		}
		protected abstract AOPProperty GetAOPProperty();
	}
}
