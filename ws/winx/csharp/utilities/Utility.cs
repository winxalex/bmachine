using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace ws.winx.csharp.utilities
{
	/// <summary>
	/// Reflection utility.
	/// </summary>
	public class ReflectionUtility{

		public static MemberInfo[] GetPublicMembers (Type type, Type propertyType, bool staticMembers, bool canWrite, bool canRead, bool isSubClass=false)
		{
			List<MemberInfo> list = new List<MemberInfo> ();
			BindingFlags bindingAttr = (!staticMembers) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Static | BindingFlags.Public);
			FieldInfo[] fields = type.GetFields (bindingAttr);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields [i];
				if (propertyType == null || fieldInfo.FieldType == propertyType || (isSubClass && fieldInfo.FieldType.IsSubclassOf(propertyType)))
				{
					list.Add (fieldInfo);
				}
			}
			PropertyInfo[] properties = type.GetProperties (bindingAttr);
			for (int j = 0; j < properties.Length; j++)
			{
				PropertyInfo propertyInfo = properties [j];
				if ((propertyType == null || propertyInfo.PropertyType == propertyType || (isSubClass && propertyInfo.PropertyType.IsSubclassOf(propertyType))) 
				    && ((canRead && propertyInfo.CanRead && (!canWrite || propertyInfo.CanWrite)) || (canWrite && propertyInfo.CanWrite && (!canRead || propertyInfo.CanRead))))
				{
					list.Add (propertyInfo);
				}
			}


			return list.ToArray ();
		}




		private static Dictionary<string, Type> s_LoadedType = new Dictionary<string, Type> ();
		
		private static Assembly[] s_LoadedAssemblies;
		
	
		public static Assembly[] loadedAssemblies
		{
			get
			{
				if (ReflectionUtility.s_LoadedAssemblies == null)
				{
					//Assembly.GetExecutingAssembly();
					ReflectionUtility.s_LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}
				return ReflectionUtility.s_LoadedAssemblies;
			}
		}
		
	
		
		public static Type[] GetDerivedTypes (Type baseType)
		{
			List<Type> list = new List<Type> ();
			Assembly[] loadedAssemblies = ReflectionUtility.loadedAssemblies;
			int i = 0;
			while (i < loadedAssemblies.Length)
			{
				Assembly assembly = loadedAssemblies [i];
				Type[] exportedTypes;
				try
				{
					exportedTypes =assembly.GetTypes();// assembly.GetExportedTypes ();
				}
				catch (ReflectionTypeLoadException)
				{
					Debug.LogWarning (string.Format ("Following assembly will be ignored due to type-loading errors: {0} ({1})", assembly.FullName , assembly.Location ));
					goto IL_97;
				}
				goto IL_4A;
			IL_97:
					i++;
				continue;
			IL_4A:
					for (int j = 0; j < exportedTypes.Length; j++)
				{
					Type type = exportedTypes [j];
					if (!type.IsAbstract && type.IsSubclassOf (baseType) && type.FullName  != null)
					{
						list.Add (type);
					}
				}
				goto IL_97;
			}
			list.Sort ((Type o1, Type o2) => o1.ToString ().CompareTo (o2.ToString ()));
			return list.ToArray ();
		}
		
		public static Type GetType (string name)
		{
			Type type = null;
			if (ReflectionUtility.s_LoadedType.TryGetValue (name, out type))
			{
				return type;
			}
			//type = (Type.GetType (name + ",Assembly-CSharp-Editor-firstpass") ?? Type.GetType (name + ",Assembly-CSharp-Editor"));

			type = Type.GetType (name);

			if (type == null)
			{
				Assembly[] loadedAssemblies = ReflectionUtility.loadedAssemblies;
				for (int i = 0; i < loadedAssemblies.Length; i++)
				{
					Assembly assembly = loadedAssemblies [i];
					type = assembly.GetType (name);
					if (type != null)
					{
						break;
					}
				}
			}
			ReflectionUtility.s_LoadedType.Add (name, type);
			return type;
		}
	



	}

	/// <summary>
	/// Attribute utility.
	/// </summary>
	public class AttributeUtility
	{
		//
		// Static Methods
		//
		public static T GetAttribute<T> (MemberInfo memberInfo, bool inherite) where T : Attribute
		{
			if (memberInfo != null)
			{
				T[] array = memberInfo.GetCustomAttributes (typeof(T), inherite) as T[];
				if (array != null && array.Length > 0)
				{
					return array [0];
				}
			}
			return (T)((object)null);
		}
		
		public static T GetAttribute<T> (Type type, bool inherite) where T : Attribute
		{
			if (type != null)
			{
				T[] array = type.GetCustomAttributes (typeof(T), inherite) as T[];
				if (array != null && array.Length > 0)
				{
					return array [0];
				}
			}
			return (T)((object)null);
		}


		public static T[] GetAttributes<T> (Type type, bool inherite) where T : Attribute
		{
			if (type != null)
			{
				T[] array = type.GetCustomAttributes (typeof(T), inherite) as T[];
				if (array != null && array.Length > 0)
				{
					return array;
				}
			}
			return (T[])((object)null);
		}

	}


	/// <summary>
	/// Switch utility.
	/// </summary>
	public class SwitchUtility{



		public static Action<object> Switch(IList<Func<object, Action>> tests)
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


		public static Action<Type> SwitchExecute (IList<Func<Type,Action>> cases)
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
		public static Func<Type,T> Switch<T> (IList<Func<Type, T>> cases)
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
			return  potentialDescendant.IsSubclassOf(potentialBase)
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

