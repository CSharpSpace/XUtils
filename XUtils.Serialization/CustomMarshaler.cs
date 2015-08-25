using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace XUtils.Serialization
{
	public abstract class CustomMarshaler
	{
		internal byte[] data;
		private MemoryStream stream;
		private BinaryReader binReader;
		private BinaryWriter binWriter;
		public byte[] ByteArray
		{
			get
			{
				return this.data;
			}
		}
		public CustomMarshaler()
		{
		}
		public void Deserialize()
		{
			if (this.data != null)
			{
				if (this.binReader != null)
				{
					this.binReader.Close();
					this.stream.Close();
				}
				this.stream = new MemoryStream(this.data);
				this.binReader = new BinaryReader(this.stream, Encoding.Unicode);
				this.ReadFromStream(this.binReader);
				this.binReader.Close();
			}
		}
		public void Serialize()
		{
			if (this.data != null)
			{
				this.stream = new MemoryStream(this.data);
				this.binWriter = new BinaryWriter(this.stream, Encoding.Unicode);
				this.WriteToStream(this.binWriter);
				this.binWriter.Close();
			}
		}
		public int GetSize()
		{
			int num = 0;
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.FieldType.IsArray)
				{
					num += this.GetFieldSize(fieldInfo);
				}
				else
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						num += this.GetFieldSize(fieldInfo) * 2;
					}
					else
					{
						if (fieldInfo.FieldType.IsPrimitive)
						{
							num += Marshal.SizeOf(fieldInfo.FieldType);
						}
					}
				}
			}
			return num;
		}
		public virtual void ReadFromStream(BinaryReader reader)
		{
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				MethodInfo methodInfo = (MethodInfo)MarshallingMethods.ReadMethods[fieldInfo.FieldType];
				if (fieldInfo.FieldType.IsArray)
				{
					Type elementType = fieldInfo.FieldType.GetElementType();
					if (elementType.IsValueType && elementType.IsPrimitive)
					{
						if (elementType == typeof(char) || elementType == typeof(byte))
						{
							fieldInfo.SetValue(this, methodInfo.Invoke(reader, new object[]
							{
								this.GetFieldSize(fieldInfo)
							}));
						}
						else
						{
							fieldInfo.SetValue(this, methodInfo.Invoke(null, new object[]
							{
								reader,
								this.GetFieldSize(fieldInfo)
							}));
						}
					}
					else
					{
						int fieldSize = this.GetFieldSize(fieldInfo);
						methodInfo = (MethodInfo)MarshallingMethods.ReadMethods[typeof(CustomMarshaler)];
						Array array2 = Array.CreateInstance(elementType, fieldSize);
						for (int j = 0; j < fieldSize; j++)
						{
							array2.SetValue(Activator.CreateInstance(elementType), j);
							methodInfo.Invoke(array2.GetValue(j), new object[]
							{
								reader
							});
						}
						fieldInfo.SetValue(this, array2);
					}
				}
				else
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						fieldInfo.SetValue(this, methodInfo.Invoke(null, new object[]
						{
							reader,
							this.GetFieldSize(fieldInfo)
						}));
					}
					else
					{
						if (fieldInfo.FieldType.IsValueType && fieldInfo.FieldType.IsPrimitive)
						{
							fieldInfo.SetValue(this, methodInfo.Invoke(reader, null));
						}
						else
						{
							CustomMarshaler customMarshaler = (CustomMarshaler)Activator.CreateInstance(fieldInfo.FieldType);
							customMarshaler.ReadFromStream(reader);
						}
					}
				}
			}
		}
		public virtual void WriteToStream(BinaryWriter writer)
		{
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				object value = fieldInfo.GetValue(this);
				MethodInfo methodInfo = (MethodInfo)MarshallingMethods.WriteMethods[fieldInfo.FieldType];
				if (fieldInfo.FieldType.IsArray)
				{
					Type elementType = fieldInfo.FieldType.GetElementType();
					if (elementType.IsValueType && elementType.IsPrimitive)
					{
						Array array2 = (Array)fieldInfo.GetValue(this);
						methodInfo.Invoke(null, new object[]
						{
							writer,
							array2
						});
					}
					else
					{
						int fieldSize = this.GetFieldSize(fieldInfo);
						methodInfo = (MethodInfo)MarshallingMethods.WriteMethods[typeof(CustomMarshaler)];
						Array array3 = (Array)fieldInfo.GetValue(this);
						for (int j = 0; j < fieldSize; j++)
						{
							methodInfo.Invoke(array3.GetValue(j), new object[]
							{
								writer
							});
						}
					}
				}
				else
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						methodInfo.Invoke(null, new object[]
						{
							writer,
							fieldInfo.GetValue(this),
							this.GetFieldSize(fieldInfo)
						});
					}
					else
					{
						if (fieldInfo.FieldType.IsValueType && fieldInfo.FieldType.IsPrimitive)
						{
							methodInfo.Invoke(writer, new object[]
							{
								value
							});
						}
					}
				}
			}
		}
		protected int GetFieldSize(FieldInfo field)
		{
			int result = 0;
			CustomMarshalAsAttribute customMarshalAsAttribute = (CustomMarshalAsAttribute)field.GetCustomAttributes(typeof(CustomMarshalAsAttribute), true)[0];
			if (customMarshalAsAttribute != null)
			{
				if (customMarshalAsAttribute.SizeField != null)
				{
					FieldInfo field2 = base.GetType().GetField(customMarshalAsAttribute.SizeField);
					result = (int)field2.GetValue(this);
				}
				else
				{
					result = customMarshalAsAttribute.SizeConst;
				}
			}
			return result;
		}
		private static bool CompareByteArrays(byte[] data1, byte[] data2)
		{
			if (data1 == null && data2 == null)
			{
				return true;
			}
			if (data1 == null || data2 == null)
			{
				return false;
			}
			if (data1.Length != data2.Length)
			{
				return false;
			}
			for (int i = 0; i < data1.Length; i++)
			{
				if (data1[i] != data2[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
