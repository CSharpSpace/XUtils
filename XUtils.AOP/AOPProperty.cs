using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
namespace XUtils.AOP
{
	public abstract class AOPProperty : IContextProperty, IContributeObjectSink
	{
		public string Name
		{
			get
			{
				return this.GetName();
			}
		}
		protected abstract IMessageSink CreateSink(IMessageSink nextSink);
		protected virtual string GetName()
		{
			return "AOP";
		}
		protected virtual void FreezeImpl(Context newContext)
		{
		}
		protected virtual bool CheckNewContext(Context newCtx)
		{
			return true;
		}
		public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
		{
			return this.CreateSink(nextSink);
		}
		public void Freeze(Context newContext)
		{
			this.FreezeImpl(newContext);
		}
		public bool IsNewContextOK(Context newCtx)
		{
			return this.CheckNewContext(newCtx);
		}
	}
}
