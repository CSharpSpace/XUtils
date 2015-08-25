using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
namespace XUtils.Net.Sockets.Udp
{
	public class UdpObjectServer : UdpBaseServer
	{
		public event ReceivedNetObjectHandler PacketReceived;
		public void Send(NetObject netObj, IPEndPoint remoteEP)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, netObj);
			byte[] array = memoryStream.ToArray();
			base.SendPacket(null, array, 0, array.Length, remoteEP);
		}
		protected override void OnUdpPacketReceived(UdpPacket packet)
		{
			if (this.PacketReceived != null)
			{
				MemoryStream serializationStream = new MemoryStream(packet.Data);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				NetObject data = (NetObject)binaryFormatter.Deserialize(serializationStream);
				this.PacketReceived(new UdpNetObjectPacketEventArgs(this, packet.Socket, packet.RemoteEndPoint, data));
			}
		}
	}
}
