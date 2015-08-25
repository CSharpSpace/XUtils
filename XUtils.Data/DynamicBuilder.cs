using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
namespace XUtils.Data
{
	internal class DynamicBuilder<T>
	{
		private delegate T Load(IDataRecord dataRecord);
		private static IDictionary<Type, Type> types;
		private static readonly MethodInfo getValueMethod;
		private static readonly MethodInfo isDBNullMethod;
		private DynamicBuilder<T>.Load handler;
		private DynamicBuilder()
		{
		}
		static DynamicBuilder()
		{
			DynamicBuilder<T>.types = new Dictionary<Type, Type>();
			DynamicBuilder<T>.getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[]
			{
				typeof(int)
			});
			DynamicBuilder<T>.isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[]
			{
				typeof(int)
			});
			DynamicBuilder<T>.types.Add(typeof(bool), typeof(bool?));
			DynamicBuilder<T>.types.Add(typeof(byte), typeof(byte?));
			DynamicBuilder<T>.types.Add(typeof(DateTime), typeof(DateTime?));
			DynamicBuilder<T>.types.Add(typeof(decimal), typeof(decimal?));
			DynamicBuilder<T>.types.Add(typeof(double), typeof(double?));
			DynamicBuilder<T>.types.Add(typeof(float), typeof(float?));
			DynamicBuilder<T>.types.Add(typeof(Guid), typeof(Guid?));
			DynamicBuilder<T>.types.Add(typeof(short), typeof(short?));
			DynamicBuilder<T>.types.Add(typeof(int), typeof(int?));
			DynamicBuilder<T>.types.Add(typeof(long), typeof(long?));
		}
		public T Build(IDataRecord dataRecord)
		{
			return this.handler(dataRecord);
		}
		public static DynamicBuilder<T> CreateBuilder(IDataRecord dataRecord)
		{
			Type typeFromHandle = typeof(T);
			DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();
			DynamicMethod dynamicMethod = new DynamicMethod("DynamicCreate", typeFromHandle, new Type[]
			{
				typeof(IDataRecord)
			}, typeFromHandle, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			LocalBuilder local = iLGenerator.DeclareLocal(typeFromHandle);
			iLGenerator.Emit(OpCodes.Newobj, typeFromHandle.GetConstructor(Type.EmptyTypes));
			iLGenerator.Emit(OpCodes.Stloc, local);
			for (int i = 0; i < dataRecord.FieldCount; i++)
			{
				PropertyInfo property = typeFromHandle.GetProperty(dataRecord.GetName(i));
				Label label = iLGenerator.DefineLabel();
				if (property != null && property.GetSetMethod() != null)
				{
					iLGenerator.Emit(OpCodes.Ldarg_0);
					iLGenerator.Emit(OpCodes.Ldc_I4, i);
					iLGenerator.Emit(OpCodes.Callvirt, DynamicBuilder<T>.isDBNullMethod);
					iLGenerator.Emit(OpCodes.Brtrue, label);
					iLGenerator.Emit(OpCodes.Ldloc, local);
					iLGenerator.Emit(OpCodes.Ldarg_0);
					iLGenerator.Emit(OpCodes.Ldc_I4, i);
					iLGenerator.Emit(OpCodes.Callvirt, DynamicBuilder<T>.getValueMethod);
					bool flag = false;
					if (property.PropertyType.Name.ToLower().Contains("nullable"))
					{
						flag = true;
					}
					Type fieldType = dataRecord.GetFieldType(i);
					if (flag)
					{
						iLGenerator.Emit(OpCodes.Unbox_Any, DynamicBuilder<T>.types[fieldType]);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Unbox_Any, fieldType);
					}
					iLGenerator.Emit(OpCodes.Callvirt, property.GetSetMethod());
					iLGenerator.MarkLabel(label);
				}
			}
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Ret);
			dynamicBuilder.handler = (DynamicBuilder<T>.Load)dynamicMethod.CreateDelegate(typeof(DynamicBuilder<T>.Load));
			return dynamicBuilder;
		}
		private static Type GetNullableType(Type type)
		{
			Type result = null;
			if (type == typeof(bool))
			{
				result = typeof(bool?);
			}
			if (type == typeof(byte))
			{
				result = typeof(byte?);
			}
			if (type == typeof(DateTime))
			{
				result = typeof(DateTime?);
			}
			if (type == typeof(decimal))
			{
				result = typeof(decimal?);
			}
			if (type == typeof(double))
			{
				result = typeof(double?);
			}
			if (type == typeof(float))
			{
				result = typeof(float?);
			}
			if (type == typeof(Guid))
			{
				result = typeof(Guid?);
			}
			if (type == typeof(short))
			{
				result = typeof(short?);
			}
			if (type == typeof(int))
			{
				result = typeof(int?);
			}
			if (type == typeof(long))
			{
				result = typeof(long?);
			}
			return result;
		}
	}
}
