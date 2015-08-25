using System;
using System.Runtime.Remoting.Messaging;
namespace XUtils.AOP
{
	public delegate void AfterAOPHandle(IMethodReturnMessage replyMsg);
}
