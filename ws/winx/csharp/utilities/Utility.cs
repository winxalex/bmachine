using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Linq.Expressions;
using ws.winx.csharp.extensions;
//using System.CodeDom.Compiler

namespace ws.winx.csharp.utilities
{
	/// <summary>
	/// Reflection utility.
	/// </summary>
	public class ReflectionUtility{





	


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




		/// <summary>
		/// Converts Delegate boxed to Getter Func<K,T>
		/// </summary>
		/// <param name="del">Del.</param>
		/// <param name="instance">Instance.</param>
		/// <param name="memberInfoValue">Member info value returned.</param>
		/// <typeparam name="K">type of instance </typeparam>
		/// <typeparam name="T">type of value returned.</typeparam>
		public static void GetThruDelegate<K,T>(Delegate del,K instance,out T memberInfoValue){
			
			//!!! Engage type check if this is prone to errors
			//		Type instanceType =	del.Method.GetParameters () [1].ParameterType;
			//		if (!typeof(K).IsAssignableFrom(instanceType))
			//						throw new Exception ("Type of parameter 'instance' should be " +instanceType);
			//		Type memberInfoValueType = del.Method.ReturnType;
			//		if(!typeof(T).IsAssignableFrom(memberInfoValueType))
			//		   throw new Exception ("Type of parameter 'memberInfoValue' should be " +memberInfoValueType);

		
			memberInfoValue = ( del as Func<K,T>)(instance);

		}







		/// <summary>
		/// Converts Delegate boxed to Setter MemberInfoSetterDelegate(as Func,Action generics can't contain "ref")
		/// </summary>
		/// <param name="del">Del.</param>
		/// <param name="instance">Instance.</param>
		/// <param name="value">Value to be set.</param>
		/// <typeparam name="K">type of instance</typeparam>
		/// <typeparam name="T">type of value to be set</typeparam>
		public static void SetThruDelegate<K,T>(Delegate del,ref K instance,T value){
			//!!! Engage type check if this is prone to errors
			Type instanceType =	del.Method.GetParameters () [0].ParameterType;
			if (!typeof(K).IsAssignableFrom(instanceType))
				throw new Exception ("Type of parameter 'instance' should be " +instanceType);
			Type valueType = del.Method.GetParameters () [1].ParameterType;
			if(!typeof(T).IsAssignableFrom(valueType))
				throw new Exception ("Type of parameter 'value' should be " +valueType);
			
			//((ws.winx.csharp.extensions.ReflectionExtension.MemberInfoSetterDelegate<K,T>)del)(ref instance,value);
			( del as ws.winx.csharp.extensions.ReflectionExtension.MemberInfoSetterDelegate<K,T>)(ref instance,value);
		}
	



//		public static string GetMemberName<T>(T argument)
//		{
//			Expression<Func<T>> lamda=()=>argument;
//			return 
//
//			ParameterExpression argumentParameter=Expression.Constant(argument,typeof(T));
//			Expression<Func<T>> lamda2 = Expression.Lambda (typeof(Func<T>),argumentParameter);
//			return ((MemberExpression)lamda2.Body).Member.Name;
//
//
//		}


		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			BinaryExpression b;
			//b.Method.GetParameters()[0].Member.Name
			//MemberExpression expressionBody = (MemberExpression)memberExpression.Body;

			BinaryExpression expresssionBody = (BinaryExpression)memberExpression.Body;
			return "";
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
		public static string GetIndexNameInList (List<string> names, string name)
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

