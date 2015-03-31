using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace ws.winx.csharp.extensions{
	public static class ReflectionExtension{
		public static void SetValue(this MemberInfo member, object instance, object value)
		{
			if (member.MemberType == MemberTypes.Property)
				((PropertyInfo)member).SetValue(instance, value, null);
			else if (member.MemberType == MemberTypes.Field)
				((FieldInfo)member).SetValue(instance, value);
			else
				throw new Exception("Property must be of type FieldInfo or PropertyInfo");
		}
		
		public static object GetValue(this MemberInfo member, object instance)
		{
			if (member.MemberType == MemberTypes.Property)
				return ((PropertyInfo)member).GetValue(instance, null);
			else if (member.MemberType == MemberTypes.Field)
				return ((FieldInfo)member).GetValue(instance);
			else
				throw new Exception("Property must be of type FieldInfo or PropertyInfo");
		}
		
				public static Type GetUnderlyingType(this MemberInfo member)
				{
					switch (member.MemberType)
					{
					case MemberTypes.Field:
						return ((FieldInfo)member).FieldType;
					case MemberTypes.Property:
						return ((PropertyInfo)member).PropertyType;
					case MemberTypes.Event:
						return ((EventInfo)member).EventHandlerType;
					case MemberTypes.Method:
						return ((MethodInfo)member).ReturnType;
					default:
						throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
					}
				}
	}
}

