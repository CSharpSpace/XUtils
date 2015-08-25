using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
namespace XUtils.Net.Sockets.Tcp
{
	public class NetObjectStream : NetBasePayloadStream<NetObject>
	{
		public NetObjectStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
		}
		public override void Send(NetObject netObj)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, netObj);
			byte[] data = memoryStream.ToArray();
			base.SendPayload(data);
		}
		protected override void ReceivedPayload(byte[] data)
		{
			MemoryStream serializationStream = new MemoryStream(data);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			NetObject data2 = (NetObject)binaryFormatter.Deserialize(serializationStream);
			base.RaiseOnReceived(data2);
		}
	}
}
