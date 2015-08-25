using System;
using System.Reflection;
using System.Reflection.Emit;
namespace XUtils.Reflection
{
	public static class DynamicMethodHelper
	{
		public delegate object GetHandler(object source);
		public delegate void SetHandler(object source, object value);
		public delegate object InstantiateObjectHandler();
		public sealed class Compiler
		{
			private Compiler()
			{
			}
			public static DynamicMethodHelper.InstantiateObjectHandler CreateInstantiateObjectHandler(Type type)
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				if (constructor == null)
				{
					throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
				}
				DynamicMethod dynamicMethod = new DynamicMethod("InstantiateObject", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object), null, type, true);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Newobj, constructor);
				iLGenerator.Emit(OpCodes.Ret);
				return (DynamicMethodHelper.InstantiateObjectHandler)dynamicMethod.CreateDelegate(typeof(DynamicMethodHelper.InstantiateObjectHandler));
			}
			public static DynamicMethodHelper.GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
			{
				MethodInfo getMethod = propertyInfo.GetGetMethod(true);
				DynamicMethod dynamicMethod = DynamicMethodHelper.Compiler.CreateGetDynamicMethod(type);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Call, getMethod);
				DynamicMethodHelper.Compiler.BoxIfNeeded(getMethod.ReturnType, iLGenerator);
				iLGenerator.Emit(OpCodes.Ret);
				return (DynamicMethodHelper.GetHandler)dynamicMethod.CreateDelegate(typeof(DynamicMethodHelper.GetHandler));
			}
			public static DynamicMethodHelper.GetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
			{
				DynamicMethod dynamicMethod = DynamicMethodHelper.Compiler.CreateGetDynamicMethod(type);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
				DynamicMethodHelper.Compiler.BoxIfNeeded(fieldInfo.FieldType, iLGenerator);
				iLGenerator.Emit(OpCodes.Ret);
				return (DynamicMethodHelper.GetHandler)dynamicMethod.CreateDelegate(typeof(DynamicMethodHelper.GetHandler));
			}
			public static DynamicMethodHelper.SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
			{
				MethodInfo setMethod = propertyInfo.GetSetMethod(true);
				DynamicMethod dynamicMethod = DynamicMethodHelper.Compiler.CreateSetDynamicMethod(type);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				DynamicMethodHelper.Compiler.UnboxIfNeeded(setMethod.GetParameters()[0].ParameterType, iLGenerator);
				iLGenerator.Emit(OpCodes.Call, setMethod);
				iLGenerator.Emit(OpCodes.Ret);
				return (DynamicMethodHelper.SetHandler)dynamicMethod.CreateDelegate(typeof(DynamicMethodHelper.SetHandler));
			}
			public static DynamicMethodHelper.SetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
			{
				DynamicMethod dynamicMethod = DynamicMethodHelper.Compiler.CreateSetDynamicMethod(type);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				DynamicMethodHelper.Compiler.UnboxIfNeeded(fieldInfo.FieldType, iLGenerator);
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
				iLGenerator.Emit(OpCodes.Ret);
				return (DynamicMethodHelper.SetHandler)dynamicMethod.CreateDelegate(typeof(DynamicMethodHelper.SetHandler));
			}
			private static DynamicMethod CreateGetDynamicMethod(Type type)
			{
				return new DynamicMethod("DynamicGet", typeof(object), new Type[]
				{
					typeof(object)
				}, type, true);
			}
			private static DynamicMethod CreateSetDynamicMethod(Type type)
			{
				return new DynamicMethod("DynamicSet", typeof(void), new Type[]
				{
					typeof(object),
					typeof(object)
				}, type, true);
			}
			private static void BoxIfNeeded(Type type, ILGenerator generator)
			{
				if (type.IsValueType)
				{
					generator.Emit(OpCodes.Box, type);
				}
			}
			private static void UnboxIfNeeded(Type type, ILGenerator generator)
			{
				if (type.IsValueType)
				{
					generator.Emit(OpCodes.Unbox_Any, type);
				}
			}
		}
		public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	}
}
