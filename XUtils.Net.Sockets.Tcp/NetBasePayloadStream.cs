using System;
using System.Net;
using System.Net.Sockets;
namespace XUtils.Net.Sockets.Tcp
{
	public abstract class NetBasePayloadStream<T> : NetBaseStream<T>
	{
		private byte[] buffer;
		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}
		public NetBasePayloadStream(NetworkStream stream, EndPoint endpoint) : base(stream, endpoint)
		{
			this.buffer = new byte[0];
		}
		protected void SendPayload(byte[] data)
		{
			base.SendRaw(this.GetPayload(data));
		}
		protected abstract void ReceivedPayload(byte[] data);
		public byte[] GetPayload(byte[] bytes)
		{
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			byte[] array = new byte[bytes2.Length + bytes.Length];
			System.Buffer.BlockCopy(bytes2, 0, array, 0, bytes2.Length);
			System.Buffer.BlockCopy(bytes, 0, array, bytes2.Length, bytes.Length);
			return array;
		}
		protected override void ReceivedRaw(byte[] bytes)
		{
			int num = this.buffer.Length + bytes.Length;
			byte[] dst = new byte[num];
			System.Buffer.BlockCopy(this.buffer, 0, dst, 0, this.buffer.Length);
			System.Buffer.BlockCopy(bytes, 0, dst, this.buffer.Length, bytes.Length);
			this.buffer = dst;
			while (this.buffer.Length >= 4)
			{
				byte[] array = new byte[4];
				System.Buffer.BlockCopy(this.buffer, 0, array, 0, 4);
				int num2 = BitConverter.ToInt32(array, 0);
				if (this.buffer.Length < 4 + num2)
				{
					break;
				}
				byte[] array2 = new byte[num2];
				System.Buffer.BlockCopy(this.buffer, 4, array2, 0, num2);
				num = this.buffer.Length - 4 - num2;
				dst = new byte[num];
				int srcOffset = 4 + num2;
				System.Buffer.BlockCopy(this.buffer, srcOffset, dst, 0, num);
				this.buffer = dst;
				this.ReceivedPayload(array2);
			}
		}
	}
}
