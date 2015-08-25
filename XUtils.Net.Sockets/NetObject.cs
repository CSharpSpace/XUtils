using System;
namespace XUtils.Net.Sockets
{
	[Serializable]
	public class NetObject
	{
		public string Name
		{
			get;
			set;
		}
		public object Object
		{
			get;
			set;
		}
		public T Cast<T>()
		{
			return (T)((object)this.Object);
		}
		public NetObject(string name, object obj)
		{
			this.Name = name;
			this.Object = obj;
		}
		public NetObject()
		{
		}
	}
}
