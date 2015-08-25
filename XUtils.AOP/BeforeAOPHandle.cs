using System;
using System.Runtime.Remoting.Messaging;
namespace XUtils.AOP
{
	public delegate void BeforeAOPHandle(IMethodCallMessage callMsg);
}
