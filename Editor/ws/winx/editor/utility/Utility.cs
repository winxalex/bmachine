using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace ws.winx.editor.utility
{

	public class ReflectionUtility{

		public static MemberInfo[] GetPublicMembers (Type type, Type propertyType, bool staticMembers, bool canWrite, bool canRead)
		{
			List<MemberInfo> list = new List<MemberInfo> ();
			BindingFlags bindingAttr = (!staticMembers) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Static | BindingFlags.Public);
			FieldInfo[] fields = type.GetFields (bindingAttr);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields [i];
				if (propertyType == null || fieldInfo.FieldType == propertyType)
				{
					list.Add (fieldInfo);
				}
			}
			PropertyInfo[] properties = type.GetProperties (bindingAttr);
			for (int j = 0; j < properties.Length; j++)
			{
				PropertyInfo propertyInfo = properties [j];
				if ((propertyType == null || propertyInfo.PropertyType == propertyType) && ((canRead && propertyInfo.CanRead && (!canWrite || propertyInfo.CanWrite)) || (canWrite && propertyInfo.CanWrite && (!canRead || propertyInfo.CanRead))))
				{
					list.Add (propertyInfo);
				}
			}
			return list.ToArray ();
		}
	}


	public class StringUtility{
		//
		// Static Methods
		//
		public static string GetUniqueNameInList (List<string> names, string name)
		{
			string text = name;
			int num = 1;
			while (names.Contains (text))
			{
				num++;
				text = name + " (" + num.ToString () + ")";
			}
			return text;
		}


	}
}

