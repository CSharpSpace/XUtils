using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Threading;
namespace XUtils.AOP
{
	public abstract class AOPSink : IMessageSink
	{
		private SortedList m_BeforeHandles;
		private SortedList m_AfterHandles;
		private IMessageSink m_NextSink;
		public IMessageSink NextSink
		{
			get
			{
				return this.m_NextSink;
			}
		}
		public AOPSink(IMessageSink nextSink)
		{
			this.m_NextSink = nextSink;
			this.m_BeforeHandles = new SortedList();
			this.m_AfterHandles = new SortedList();
			this.AddAllBeforeAOPHandles();
			this.AddAllAfterAOPHandles();
		}
		protected virtual void AddBeforeAOPHandle(string methodName, BeforeAOPHandle beforeHandle)
		{
			SortedList beforeHandles;
			Monitor.Enter(beforeHandles = this.m_BeforeHandles);
			try
			{
				if (!this.m_BeforeHandles.Contains(methodName))
				{
					this.m_BeforeHandles.Add(methodName, beforeHandle);
				}
			}
			finally
			{
				Monitor.Exit(beforeHandles);
			}
		}
		protected virtual void AddAfterAOPHandle(string methodName, AfterAOPHandle afterHandle)
		{
			SortedList afterHandles;
			Monitor.Enter(afterHandles = this.m_AfterHandles);
			try
			{
				if (!this.m_AfterHandles.Contains(methodName))
				{
					this.m_AfterHandles.Add(methodName, afterHandle);
				}
			}
			finally
			{
				Monitor.Exit(afterHandles);
			}
		}
		public abstract void AddAllBeforeAOPHandles();
		public abstract void AddAllAfterAOPHandles();
		protected BeforeAOPHandle FindBeforeAOPHandle(string methodName)
		{
			SortedList beforeHandles;
			Monitor.Enter(beforeHandles = this.m_BeforeHandles);
			BeforeAOPHandle result;
			try
			{
				result = (BeforeAOPHandle)this.m_BeforeHandles[methodName];
			}
			finally
			{
				Monitor.Exit(beforeHandles);
			}
			return result;
		}
		protected AfterAOPHandle FindAfterAOPHandle(string methodName)
		{
			SortedList afterHandles;
			Monitor.Enter(afterHandles = this.m_AfterHandles);
			AfterAOPHandle result;
			try
			{
				result = (AfterAOPHandle)this.m_AfterHandles[methodName];
			}
			finally
			{
				Monitor.Exit(afterHandles);
			}
			return result;
		}
		protected bool FindbeforeAOPMethod(IMethodCallMessage call)
		{
			bool result = false;
			object[] customAttributes = call.MethodBase.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Attribute attribute = (Attribute)customAttributes[i];
				AOPMethodAttribute aOPMethodAttribute = attribute as AOPMethodAttribute;
				if (aOPMethodAttribute != null && aOPMethodAttribute.UseAspect)
				{
					result = true;
					break;
				}
			}
			return result;
		}
		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
			string methodName = methodCallMessage.MethodName.ToUpper();
			bool flag = this.FindbeforeAOPMethod(methodCallMessage);
			BeforeAOPHandle beforeAOPHandle = this.FindBeforeAOPHandle(methodName);
			if (beforeAOPHandle != null && flag)
			{
				beforeAOPHandle(methodCallMessage);
			}
			IMessage message = this.m_NextSink.SyncProcessMessage(msg);
			IMethodReturnMessage replyMsg = message as IMethodReturnMessage;
			AfterAOPHandle afterAOPHandle = this.FindAfterAOPHandle(methodName);
			if (afterAOPHandle != null && flag)
			{
				afterAOPHandle(replyMsg);
			}
			return message;
		}
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			return this.m_NextSink.AsyncProcessMessage(msg, replySink);
		}
	}
}
