using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace ws.winx.csharp.utilities
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


	public class SwitchUtility{

		public static Action<object> Switch(params Func<object, Action>[] tests)
		{
			return o =>
			{
				var @case = tests
					.Select(f => f(o))
						.FirstOrDefault(a => a != null);
				
				if (@case != null)
				{
					@case();
				}
			};
		}


		public static Action<Type> SwitchExecute (params Func<Type, Action>[] cases)
		{
			return o =>
			{
				var @case = cases
					.Select (f => f (o))
						.FirstOrDefault (a => a != null);
				
				
				if (@case != null)
				{
					@case();
				}
				
			};
		}


		/// <summary>
		/// Switch the specified cases.
		/// </summary>
		/// <param name="cases">Cases of method CaseIsClassOf or CaseIsTypeOf</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <example> switch(</example>
		public static Func<Type,T> Switch<T> (params Func<Type, T>[] cases)
		{
			return o =>
			{
				var @case = cases
					.Select (f => f (o))
						.FirstOrDefault (a => a != null);
				
				
				return @case;

			};
		}


		public static Func<Type, K> CaseIsClassOf<T,K> (K action)
		{
			
			return  o => SwitchUtility.IsSameOrSubclass (typeof(T), o) ? 
				action 
					: default(K);
		}




		
		public static Func<object, Action> CaseIsTypeOf<T>(Action<T> action)
		{
			return o => o is T ? (Action)(() => action((T)o)) : (Action)null;
		}



		public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
		{
			return potentialDescendant.IsSubclassOf(potentialBase)
				|| potentialDescendant == potentialBase;
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

