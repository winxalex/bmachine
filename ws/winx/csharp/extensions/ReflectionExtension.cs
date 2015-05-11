using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using ws.winx.csharp.utilities;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ws.winx.csharp.extensions
{
		public static class ReflectionExtension
		{

				static Dictionary<MemberInfo,Delegate> _settersCache;
				static Dictionary<MemberInfo,Delegate> _getterCache;

				//static Dictionary<MemberInfo,MemberInfoSetterDelegate<object,object>> _settersWeakCache;
				//static Dictionary<MemberInfo,Func<object,object>> _getterWeakCache;

				public static Dictionary<MemberInfo, Delegate> settersCache {
						get {
								if (_settersCache == null)
										_settersCache = new Dictionary<MemberInfo, Delegate> ();
								return _settersCache;
						}
				}

				public static Dictionary<MemberInfo, Delegate> getterCache {
						get {
								if (_getterCache == null)
										_getterCache = new Dictionary<MemberInfo, Delegate> ();
								return _getterCache;
						}
				}


				public delegate void MemberInfoSetterDelegate<T,in TValue> (ref T target,TValue value);

		
			



				// orginal by Tom Sun
		
		#region Field Access

				/// <summary>
				/// Gets the get delegate. Delegate will be strongly typed but boxed inside Delegate
				/// !!! as in .NET 3.5 there is no "dynamic" use GetGetDelegate<object,object>
				/// </summary>
				/// <returns>The get delegate.</returns>
				/// <param name="type">Type.</param>
				/// <param name="name">Name.</param>
				/// <param name="bindingFlags">Binding flags.</param>
				public static Delegate GetGetDelegateDynamic (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
			
			
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);

						if (getterCache.ContainsKey (memberInfo))
								return getterCache [memberInfo];

						Delegate d = memberInfo.GetGetMemberInfoDelegate ();
						getterCache [memberInfo] = d;
			
						return d;
			
				}

				public static Func<object,object> GetGetDelegate (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
					
					
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);
					
						if (getterCache.ContainsKey (memberInfo))
								return getterCache [memberInfo] as Func<object,object>;
					
						Func<object,object> d = memberInfo.GetGetMemberInfoDelegate ();
						getterCache [memberInfo] = d;
					
						return d;
					
				}


				/// <summary>
				/// Gets the get delegate strong typed.
				/// </summary>
				/// <returns>The get delegate.</returns>
				/// <param name="type">Type.</param>
				/// <param name="name">Name.</param>
				/// <param name="bindingFlags">Binding flags.</param>
				/// <typeparam name="TSource">The 1st type parameter.</typeparam>
				/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
				public static Func<TSource,TValue>  GetGetDelegate<TSource,TValue> (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
			
			
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);

						if (getterCache.ContainsKey (memberInfo))
								return getterCache [memberInfo] as Func<TSource,TValue>;

						Func<TSource,TValue> d = memberInfo.GetGetMemberInfoDelegate<TSource,TValue> ();
						getterCache [memberInfo] = d;
			
						return d;
			
				}



				/// <summary>
				/// Gets the get member info delegate.
				/// </summary>
				/// <returns>The get member info delegate.</returns>
				/// <param name="memberInfo">Member info.</param>
				public static Func<object,object> GetGetMemberInfoDelegate (this MemberInfo memberInfo)
				{
						
						if (memberInfo.MemberType == MemberTypes.Property) {
							
								return (memberInfo as PropertyInfo).GetGetMethod ().GetGetPropertyDelegate<object,object> ();


// v1
//				ParameterExpression sourceParameter =
//					Expression.Parameter (memberInfo.ReflectedType, "source");
//
//				MethodCallExpression methodExpression = Expression.Call (sourceParameter, (memberInfo as PropertyInfo).GetGetMethod ());
//
//				ParameterExpression sourceExpression = Expression.Parameter(typeof(object), "instance");
//				ParameterExpression value = Expression.Parameter(typeof(object), "value");
//			
//											Expression<Func<object,object>> expr =
//												Expression.Lambda<Func<object,object>>(
//						Expression.Call(methodExpression.Method,  Expression.Convert(sourceExpression, memberInfo.DeclaringType),Expression.Convert(value, memberInfo.ReflectedType)),
//													sourceExpression,value);
//
//				return expr.Compile();

// 							v2 call of type strong generic
//							MethodInfo genericMethod=typeof(ReflectionExtension).GetMethod("GetGetPropertyDelegate");
//							MethodInfo method = genericMethod.MakeGenericMethod(new Type[] { memberInfo.DeclaringType,memberInfo.ReflectedType });
//							ParameterExpression source = Expression.Parameter(typeof(object), "source");
//							ParameterExpression value = Expression.Parameter(typeof(object), "value");
//
//							Expression<Func<object,object>> expr =
//								Expression.Lambda<Func<object,object>>(
//								Expression.Call(method,  Expression.Convert(source, memberInfo.DeclaringType),Expression.Convert(value, memberInfo.ReflectedType)),
//									source,value);
//
//								return expr.Compile();
							
							
						} else if (memberInfo.MemberType == MemberTypes.Field) {
							
								//return GetGetFieldDelegate<object,object>(memberInfo as FieldInfo);
								return   (memberInfo as FieldInfo).GetGetFieldDelegate<object,object> ();
						} else if (memberInfo.MemberType == MemberTypes.Method) {
							
							
						}
						
						
						throw new Exception ("Not supported memberInfo of type " + memberInfo.MemberType);
						
				}








				/// <summary>
				/// Gets the get member info delegate.
				/// </summary>
				/// <returns>The get member info delegate.</returns>
				/// <param name="memberInfo">Member info.</param>
				public static Delegate GetGetMemberInfoDelegateDynamic (this MemberInfo memberInfo)
				{
			
						if (memberInfo.MemberType == MemberTypes.Property) {
				
								return (memberInfo as PropertyInfo).GetGetMethod ().GetGetPropertyDelegate ();
				
				
						} else if (memberInfo.MemberType == MemberTypes.Field) {
				
								//return GetGetFieldDelegate<object,object>(memberInfo as FieldInfo);
								return   (memberInfo as FieldInfo).GetGetFieldDelegate ();
						} else if (memberInfo.MemberType == MemberTypes.Method) {
				
				
						}
			
			
						throw new Exception ("Not supported memberInfo of type " + memberInfo.MemberType);
			
				}
		
		
		
				/// <summary>
				/// Gets the member info delegate strongly typed
				/// </summary>
				/// <returns>The member info delegate.</returns>
				/// <param name="memberInfo">Member info.</param>
				/// <typeparam name="TSource">type of instance.</typeparam>
				/// <typeparam name="TValue">type of value.</typeparam>
				public static Func<TSource,TValue> GetGetMemberInfoDelegate<TSource,TValue> (this MemberInfo memberInfo)
				{
			
						if (memberInfo.MemberType == MemberTypes.Property) {
				
								return (memberInfo as PropertyInfo).GetGetMethod ().GetGetPropertyDelegate<TSource,TValue> ();
			
				
						} else if (memberInfo.MemberType == MemberTypes.Field) {
				
								//return GetGetFieldDelegate<object,object>(memberInfo as FieldInfo);
								return   (memberInfo as FieldInfo).GetGetFieldDelegate<TSource,TValue> ();
						} else if (memberInfo.MemberType == MemberTypes.Method) {


						}
			
			
						throw new Exception ("Not supported memberInfo of type " + memberInfo.MemberType);
			
				}
		
		
		
				//
				public static Delegate GetGetPropertyDelegate (this MethodInfo methodInfo)
				{
			
						if (methodInfo == null)
								throw new ArgumentNullException ("methodInfo");
			
						Type propDeclaringType = methodInfo.DeclaringType;

						Type typeDynamic = typeof(Func<,>).MakeGenericType (new Type[] {
				methodInfo.DeclaringType,
				methodInfo.ReturnType
			});
			
						ParameterExpression sourceParameter =
				Expression.Parameter (propDeclaringType, "source");
			
			
						Expression sourceExpression = GetCastOrConvertExpression (
				sourceParameter, propDeclaringType);
			
			
			
						MethodCallExpression methodExpression = Expression.Call (sourceExpression, methodInfo);
			
						Expression resultExpression = GetCastOrConvertExpression (
				methodExpression, methodInfo.ReturnType);
			
						LambdaExpression lambda =
				Expression.Lambda (typeDynamic, resultExpression, sourceParameter);
			
						return  lambda.Compile ();
			
			
			
				}
		
		
		
		
				/// <summary>
				/// Gets the get property delegate.
				/// </summary>
				/// <returns>The get property delegate.</returns>
				/// <param name="methodInfo">Method info.</param>
				/// <typeparam name="TSource">The 1st type parameter.</typeparam>
				/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
				public static Func<TSource,TValue> GetGetPropertyDelegate<TSource,TValue> (this MethodInfo methodInfo)
				{
			
						if (methodInfo == null)
								throw new ArgumentNullException ("methodInfo");
			
						Type propDeclaringType = methodInfo.DeclaringType;

				
			
						ParameterExpression sourceParameter =
				Expression.Parameter (typeof(TSource), "source");
			
			
						Expression sourceExpression = GetCastOrConvertExpression (
				sourceParameter, propDeclaringType);
			
			
			
						MethodCallExpression methodExpression = Expression.Call (sourceExpression, methodInfo);
			
						Expression resultExpression = GetCastOrConvertExpression (
				methodExpression, typeof(TValue));
			
						LambdaExpression lambda =
				Expression.Lambda (typeof(Func<TSource, TValue>), resultExpression, sourceParameter);
			
						return  (Func<TSource, TValue>)lambda.Compile ();
			
		
			
				}
		
		
		
		
				//This is working but everything is handled with use of IL
		
		
				/// <summary>
				/// Gets the set property delegate.
				/// </summary>
				/// <returns>The set property delegate.</returns>
				/// <param name="methodInfo">Method info.</param>
				/// <typeparam name="TSource">instance parameter.</typeparam>
				/// <typeparam name="TValue">value to be set parameter.</typeparam>
//				public static Action<TSource,TValue> GetSetPropertyDelegate<TSource,TValue> (this MethodInfo methodInfo)
//				{
//			
//						if (methodInfo == null)
//								throw new ArgumentNullException ("methodInfo");
//			
//						Type propDeclaringType = methodInfo.DeclaringType;
//			
//						ParameterExpression sourceParameter =
//				Expression.Parameter (typeof(TSource), "source");
//			
//			
//						Expression sourceExpression = GetCastOrConvertExpression (
//				sourceParameter, propDeclaringType);
//			
//			
//						ParameterExpression argumentParameter =
//				Expression.Parameter (methodInfo.GetParameters () [0].ParameterType, "argument");
//			
//						Expression argumentExpression = GetCastOrConvertExpression (
//				argumentParameter, typeof(TValue));
//			
//			
//			
//						MethodCallExpression methodExpression = Expression.Call (sourceExpression, methodInfo, argumentExpression);
//			
//						Expression resultExpression = GetCastOrConvertExpression (
//				methodExpression, typeof(TValue));
//			
//						LambdaExpression lambda =
//				Expression.Lambda (typeof(Action<TSource, TValue>), resultExpression, sourceParameter);
//			
//						return  (Action<TSource, TValue>)lambda.Compile ();
//			
//			
//			
//			
//			
//			
//				}
		
		
		
		
		
				/// <summary>
				/// Gets a  delegate to a generated method that allows you to get the field value, that is represented
				/// !!! .NET 3.5 doesn't support "dynamic" for which is planned this function
				/// by the given <paramref name="fieldInfo"/>. The delegate is instance independend, means that you pass the source 
				/// of the field as a parameter to the method and get back the value of it's field.
				/// </summary>
				/// <param name="fieldInfo">Provides the metadata of the field.</param>
				/// <returns>A strong typed delegeate that can be cached to get the field's value with high performance.</returns>
		
				public static Delegate GetGetFieldDelegate (this FieldInfo fieldInfo)
				{
						if (fieldInfo == null)
								throw new ArgumentNullException ("fieldInfo");
			
						Type fieldDeclaringType = fieldInfo.DeclaringType;
			
						Type typeDynamic = typeof(Func<,>).MakeGenericType (new Type[] {
				fieldInfo.DeclaringType,
				fieldInfo.FieldType 
			});
			
						ParameterExpression sourceParameter =
				Expression.Parameter (fieldDeclaringType, "source");
			
			
						MemberExpression fieldExpression = Expression.Field (sourceParameter, fieldInfo);
			
			
			
						LambdaExpression lambda =
				Expression.Lambda (typeDynamic, fieldExpression, sourceParameter);
			
						return  lambda.Compile ();
			
				}
		
		
				/// <summary>
				/// Gets a strong typed delegate to a generated method that allows you to get the field value, that is represented
				/// by the given <paramref name="fieldInfo"/>. The delegate is instance independend, means that you pass the source 
				/// of the field as a parameter to the method and get back the value of it's field.
				/// </summary>
				/// <typeparam name="TSource">The reflecting type. This can be an interface that is implemented by the field's declaring type
				/// or an derrived type of the field's declaring type.</typeparam>
				/// <typeparam name="TValue">The type of the field value.</typeparam>
				/// <param name="fieldInfo">Provides the metadata of the field.</param>
				/// <returns>A strong typed delegeate that can be cached to get the field's value with high performance.</returns>
		
				public static Func<TSource, TValue> GetGetFieldDelegate<TSource, TValue> (this FieldInfo fieldInfo)
				{
						if (fieldInfo == null)
								throw new ArgumentNullException ("fieldInfo");
			
						Type fieldDeclaringType = fieldInfo.DeclaringType;
			
						ParameterExpression sourceParameter =
				Expression.Parameter (typeof(TSource), "source");
			
			
						Expression sourceExpression = GetCastOrConvertExpression (
				sourceParameter, fieldDeclaringType);
			
						MemberExpression fieldExpression = Expression.Field (sourceExpression, fieldInfo);
			
						Expression resultExpression = GetCastOrConvertExpression (
				fieldExpression, typeof(TValue));
			
						LambdaExpression lambda =
				Expression.Lambda (typeof(Func<TSource, TValue>), resultExpression, sourceParameter);
			
						return  (Func<TSource, TValue>)lambda.Compile ();
			
				}
		
		
		
				/// <summary>
				/// Gets the set delegate strongly typed
				/// !!! .NET 3.5 doesn't support "dynamic" for which is planned for
				/// </summary>
				/// <returns>The set delegate.</returns>
				/// <param name="type">Type.</param>
				/// <param name="name">Name of field or property</param>
				/// <param name="bindingFlags">Binding flags.</param>
				public static Delegate GetSetDelegateDynamic (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
			
			
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);

						if (settersCache.ContainsKey (memberInfo))
								return settersCache [memberInfo];
						
						Delegate d = memberInfo.GetSetMemberInfoDelegateDynamic ();
						settersCache [memberInfo] = d;
						
			
						return d;
			
				}


				/// <summary>
				/// Gets the set delegate.
				/// </summary>
				/// <returns>The set delegate.</returns>
				/// <param name="type">Type.</param>
				/// <param name="name">Name.</param>
				/// <param name="bindingFlags">Binding flags.</param>
				/// <typeparam name="T">The 1st type parameter.</typeparam>
				/// <typeparam name="K">The 2nd type parameter.</typeparam>
				public static MemberInfoSetterDelegate<object,object> GetSetDelegate (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
			
			
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);
			
			
			
						if (settersCache.ContainsKey (memberInfo))
								return settersCache [memberInfo] as MemberInfoSetterDelegate<object,object>;
			
						MemberInfoSetterDelegate<object,object> d = memberInfo.GetSetMemberInfoDelegate<object,object> ();
						settersCache [memberInfo] = d;
			
						return d;
			
				}



//		public static MemberInfoSetterDelegate<object,object> GetSetDelegate (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
//		{
//			if (name == null)
//				throw new ArgumentNullException ("name");
//			
//			
//			MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);
//			
//			
//			
//			if (settersCache.ContainsKey (memberInfo))
//				return settersCache [memberInfo] as MemberInfoSetterDelegate<object,object>;
//			
//			MemberInfoSetterDelegate<object,object> d = memberInfo.GetSetMemberInfoDelegate<object,object> ();
//			settersCache [memberInfo] = d;
//			
//			return d;
//			
//		}
		
				/// <summary>
				/// Gets the set delegate strongly typed
				/// </summary>
				/// <returns>The set delegate.</returns>
				/// <param name="type">Type.</param>
				/// <param name="name">Name.</param>
				/// <param name="bindingFlags">Binding flags.</param>
				/// <typeparam name="T">The 1st type parameter.</typeparam>
				/// <typeparam name="K">The 2nd type parameter.</typeparam>
				public static MemberInfoSetterDelegate<T,K> GetSetDelegate<T,K> (this Type type, string name, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				{
						if (name == null)
								throw new ArgumentNullException ("name");
			
			
						MemberInfo memberInfo = type.GetMemberFromPath (name, bindingFlags);

						
						
						if (settersCache.ContainsKey (memberInfo))
								return settersCache [memberInfo] as MemberInfoSetterDelegate<T,K>;
						
						MemberInfoSetterDelegate<T,K> d = memberInfo.GetSetMemberInfoDelegate<T,K> ();
						settersCache [memberInfo] = d;
			
						return d;
			
				}




		
				/// <summary>
				/// Gets the set member info delegate.
				/// </summary>
				/// <returns>The set member info delegate.</returns>
				/// <param name="memberInfo">Member info.</param>
				public static Delegate GetSetMemberInfoDelegateDynamic (this MemberInfo memberInfo)
				{
			
						Type typeDynamic = typeof(MemberInfoSetterDelegate<,>).MakeGenericType (new Type[] {
				memberInfo.DeclaringType,
				memberInfo.GetUnderlyingType ()
			});
			
			
			
						Type instanceType = memberInfo.DeclaringType;
						Type valueType = memberInfo.GetUnderlyingType ();
			
						var paramType = instanceType.MakeByRefType ();
			
			
						var setter = new DynamicMethod ("", typeof(void),
			                                new[] { paramType, valueType },
			memberInfo.DeclaringType.Module, true);
			
						var generator = setter.GetILGenerator ();
			
			
						if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).IsStatic) {
								generator.Emit (OpCodes.Ldarg_1);
								generator.Emit (OpCodes.Stsfld, (FieldInfo)memberInfo);
						} else {
				
								generator.Emit (OpCodes.Ldarg_0);
								if (!instanceType.IsValueType)//this line is so we can use one SetterDelegate with ref in case of cls
										generator.Emit (OpCodes.Ldind_Ref);
								generator.Emit (OpCodes.Ldarg_1);
								if (memberInfo.MemberType == MemberTypes.Field)
										generator.Emit (OpCodes.Stfld, (FieldInfo)memberInfo);
								else
										generator.Emit (OpCodes.Callvirt, ((PropertyInfo)memberInfo).GetSetMethod ());
						}
			
						generator.Emit (OpCodes.Ret);
			
			
			
						return setter.CreateDelegate (typeDynamic);
			
			
				}


//		public static MemberInfoSetterDelegate<object,object> GetSetMemberInfoDelegate(this MemberInfo memberInfo)
//		{
//			Type instanceType = memberInfo.DeclaringType;
//			Type valueType = memberInfo.GetUnderlyingType ();
//			
//			var paramType = instanceType.MakeByRefType ();
//			
//			
//			var setter = new DynamicMethod ("", typeof(void),
//			                                new[] { paramType, valueType },
//			memberInfo.DeclaringType.Module, true);
//			
//			var generator = setter.GetILGenerator ();
//			
//			
//			if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).IsStatic) {
//				generator.Emit (OpCodes.Ldarg_1);
//				generator.Emit (OpCodes.Stsfld, (FieldInfo)memberInfo);
//			} else {
//				
//				generator.Emit (OpCodes.Ldarg_0);
//				if (!instanceType.IsValueType)//this line is so we can use one SetterDelegate with ref in case of cls
//					generator.Emit (OpCodes.Ldind_Ref);
//				generator.Emit (OpCodes.Ldarg_1);
//				if (memberInfo.MemberType == MemberTypes.Field)
//					generator.Emit (OpCodes.Stfld, (FieldInfo)memberInfo);
//				else
//					generator.Emit (OpCodes.Callvirt, ((PropertyInfo)memberInfo).GetSetMethod ());
//			}
//			
//			generator.Emit (OpCodes.Ret);
//			
//			
//			
//			
//			return (MemberInfoSetterDelegate<object,object>)setter.CreateDelegate (typeof(MemberInfoSetterDelegate<object,object>));
//		}
		
		
				/// <summary>
				/// Gets set memberinfo delegate strongly typed
				/// </summary>
				/// <returns>The set member info delegate.</returns>
				/// <param name="memberInfo">Member info.</param>
				/// <typeparam name="T">The 1st type parameter.</typeparam>
				/// <typeparam name="K">The 2nd type parameter.</typeparam>
				public static MemberInfoSetterDelegate<T,K> GetSetMemberInfoDelegate<T,K> (this MemberInfo memberInfo)
				{
						Type instanceType = typeof(T);// 
						Type valueType = typeof(K);//
			
						Type valueTypeRef = instanceType.MakeByRefType ();

						Type reflectingType = memberInfo.GetUnderlyingType ();
						Type declaringType = memberInfo.DeclaringType;
			
			
						var setter = new DynamicMethod ("", typeof(void),
			                                new[] { valueTypeRef, valueType },
			memberInfo.DeclaringType.Module, true);
			
						var generator = setter.GetILGenerator ();
			
			
						if (memberInfo.MemberType == MemberTypes.Field && ((FieldInfo)memberInfo).IsStatic) {
								generator.Emit (OpCodes.Ldarg_1);

								if ((valueType.IsInterface || valueType == typeof(object)) && reflectingType.IsValueType)//if value is boxed => unbox
									generator.Emit(OpCodes.Unbox_Any,reflectingType);
										
								generator.Emit (OpCodes.Stsfld, (FieldInfo)memberInfo);
						} else {
				
								generator.Emit (OpCodes.Ldarg_0);
								if (!instanceType.IsValueType)//this line is so we can use one SetterDelegate with ref in case of cls
										generator.Emit (OpCodes.Ldind_Ref);

								if ((instanceType.IsInterface || instanceType == typeof(object)) && declaringType.IsValueType)//if instance of valuetype is boxed => unbox
								{
									generator.Emit(OpCodes.Unbox, declaringType);
								}


								generator.Emit (OpCodes.Ldarg_1);


								if ((valueType.IsInterface || valueType == typeof(object)) && reflectingType.IsValueType)//if value is boxed => unbox
									generator.Emit(OpCodes.Unbox_Any,reflectingType);
				
								if (memberInfo.MemberType == MemberTypes.Field)
										generator.Emit (OpCodes.Stfld, (FieldInfo)memberInfo);
								else
										generator.Emit (OpCodes.Callvirt, ((PropertyInfo)memberInfo).GetSetMethod ());
						}
			
						generator.Emit (OpCodes.Ret);
			
			
			
			
						return (MemberInfoSetterDelegate<T,K>)setter.CreateDelegate (typeof(MemberInfoSetterDelegate<T,K>));
				}
	
		
				///!!! This wasn't working 
		
				/// <summary>
				/// Gets a strong typed delegate to a generated method that allows you to get the field value, that is represented
				/// by the given <paramref name="fieldName"/>. The delegate is instance independend, means that you pass the source 
				/// of the field as a parameter to the method and get back the value of it's field.
				/// </summary>
				/// <typeparam name="TSource">The reflecting type. This can be an interface that is implemented by the field's declaring type
				/// or an derrived type of the field's declaring type.</typeparam>
				/// <typeparam name="TValue">The type of the field value.</typeparam>
				/// <param name="fieldName">The name of the field.</param>
				/// <returns>A strong typed delegeate that can be cached to get the field's value with high performance.</returns>
//				public static Func<TSource, TValue> GetGetFieldDelegate<TSource, TValue> (this Type type, string fieldName, BindingFlags bindingFlags=  BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
//				{
//						if (fieldName == null)
//								throw new ArgumentNullException ("fieldName");
//			
//			
//						FieldInfo fieldInfo = type.GetField (fieldName, bindingFlags);
//			
//						return GetGetFieldDelegate<TSource, TValue> (fieldInfo);
//			
//				}
		


	
		
				/// <summary>
				/// Gets a strong typed delegate to a generated method that allows you to set the field value, that is represented
				/// by the given <paramref name="fieldInfo"/>. The delegate is instance independend, means that you pass the source 
				/// of the field as a parameter to the generated method and get back the value of it's field.
				/// </summary>
				/// <typeparam name="TSource">The reflecting type. This can be an interface that is implemented by the field's declaring type
				/// or an derrived type of the field's declaring type.</typeparam>
				/// <typeparam name="TValue">The type of the field value.</typeparam>
				/// <param name="fieldInfo">Provides the metadata of the field.</param>
				/// <returns>A strong typed delegeate that can be cached to set the field's value with high performance.</returns>
//				public static Action<TSource, TValue> GetSetFieldDelegate<TSource, TValue> (this FieldInfo fieldInfo)
//				{
//						if (fieldInfo == null)
//								throw new ArgumentNullException ("fieldInfo");
//			
//						Type fieldDeclaringType = fieldInfo.DeclaringType;
//			
//			
//						// Define the parameters of the lambda expression: (source,value) =>
//						ParameterExpression sourceParameter = Expression.Parameter (typeof(TSource), "source");
//						ParameterExpression valueParameter = Expression.Parameter (typeof(TValue), "value");
//			
//			
//			
//						// Add cast or convert expression if necessary. (e.g. when fieldDeclaringType is not assignable from typeof(TSource)
//						Expression sourceExpression = sourceParameter.GetCastOrConvertExpression (fieldDeclaringType);
//			
//						// Get the field access expression.
//						Expression fieldExpression = Expression.Field (sourceExpression, fieldInfo);
//			
//			
//						// Add cast or convert expression if necessary.
//						Expression valueExpression = valueParameter.GetCastOrConvertExpression (fieldExpression.Type);
//			
//						// Get the generic method that assigns the field value.
//						var genericSetFieldMethodInfo = setFieldMethod.MakeGenericMethod (fieldExpression.Type);
//			
//						// get the set field expression 
//						// e.g. source.SetField(ref (arg as MyClass).integerProperty, Convert(value)
//						MethodCallExpression setFieldMethodCallExpression = Expression.Call (
//				null, genericSetFieldMethodInfo, fieldExpression, valueExpression);
//			
//						// Create the final lambda expression
//						// e.g. (source,value) => SetField(ref (arg as MyClass).integerProperty, Convert(value))
//						LambdaExpression lambda = Expression.Lambda (typeof(Action<TSource, TValue>),
//			                                            setFieldMethodCallExpression, sourceParameter, valueParameter);
//			
//						return (Action<TSource, TValue>)lambda.Compile ();
//			
//				}
		
				/// <summary>
				/// Gets a strong typed delegate to a generated method that allows you to set the field value, that is represented
				/// by the given <paramref name="fieldName"/>. The delegate is instance independend, means that you pass the source 
				/// of the field as a parameter to the generated method and get back the value of it's field.
				/// </summary>
				/// <typeparam name="TSource">The reflecting type. This can be an interface that is implemented by the field's declaring type
				/// or an derrived type of the field's declaring type.</typeparam>
				/// <typeparam name="TValue">The type of the field value.</typeparam>
				/// <param name="fieldName">The name of the field.</param>
				/// <param name="fieldType">The type of the field.</param>
				/// <param name="fieldDeclaringType">The type that declares the field.</param>
				/// <returns>A strong typed delegeate that can be cached to set the field's value with high performance.</returns>
//				public static Action<TSource, TValue> GetSetFieldDelegate<TSource, TValue> (string fieldName, Type fieldType, Type fieldDeclaringType)
//				{
//						if (fieldName == null)
//								throw new ArgumentNullException ("fieldName");
//						if (fieldType == null)
//								throw new ArgumentNullException ("fieldType");
//						if (fieldDeclaringType == null)
//								throw new ArgumentNullException ("fieldDeclaringType");
//			
//						// Define the parameters of the lambda expression: (source,value) =>
//						ParameterExpression sourceParameter = Expression.Parameter (typeof(TSource), "source");
//						ParameterExpression valueParameter = Expression.Parameter (typeof(TValue), "value");
//			
//			
//						// Add cast or convert expression if necessary. (e.g. when fieldDeclaringType is not assignable from typeof(TSource)
//						Expression sourceExpression = sourceParameter.GetCastOrConvertExpression (fieldDeclaringType);
//						Expression valueExpression = valueParameter.GetCastOrConvertExpression (fieldType);
//			
//						// Get the field access expression.
//						MemberExpression fieldExpression = Expression.Field (sourceExpression, fieldName);
//			
//						// Get the generic method that assigns the field value.
//						var genericSetFieldMethodInfo = setFieldMethod.MakeGenericMethod (fieldType);
//			
//						// get the set field expression 
//						// e.g. source.SetField(ref (arg as MyClass).integerProperty, Convert(value)
//						MethodCallExpression setFieldMethodCallExpression = Expression.Call (
//				null, genericSetFieldMethodInfo, fieldExpression, valueExpression);
//			
//						// Create the final lambda expression
//						// e.g. (source,value) => SetField(ref (arg as MyClass).integerProperty, Convert(value))
//						LambdaExpression lambda = Expression.Lambda (typeof(Action<TSource, TValue>),
//			                                            setFieldMethodCallExpression, sourceParameter, valueParameter);
//			
//						Action<TSource, TValue> result = (Action<TSource, TValue>)lambda.Compile ();
//						return result;
//				}
		
				/// <summary>
				/// Gets an expression that can be assigned to the given target type. 
				/// Creates a new expression when a cast or conversion is required, 
				/// or returns the given <paramref name="expression"/> if no cast or conversion is required.
				/// </summary>
				/// <param name="expression">The expression which resulting value should be passed to a 
				/// parameter with a different type.</param>
				/// <param name="targetType">The target parameter type.</param>
				/// <returns>The <paramref name="expression"/> if no cast or conversion is required, 
				/// otherwise a new expression instance that wraps the the given <paramref name="expression"/> 
				/// inside the required cast or conversion.</returns>
				public static Expression GetCastOrConvertExpression (this Expression expression, Type targetType)
				{
						Expression result;
						Type expressionType = expression.Type;
			
						// Check if a cast or conversion is required.
						//if (targetType.IsAssignableFrom(expressionType))
						// object.IsAssignableForm(System.Single) gives true;
						if (targetType != typeof(object) && targetType.IsAssignableFrom (expressionType)) {
								result = expression;
						} else {
								// Check if we can use the as operator for casting or if we must use the convert method
								if (targetType.IsValueType && !IsNullableType (targetType)) {
										result = Expression.Convert (expression, targetType);
								} else {
										result = Expression.TypeAs (expression, targetType);
								}
						}
			
						return result;
				}
		
		
		#region Called by reflection - Don't delete.
		
				/// <summary>
				/// Stores the method info for the method that performs the assignment of the field value.
				/// Note: There is no assign expression in .NET 3.0/3.5. With .NET 4.0 this method becomes obsolete.
				/// </summary>
				private static readonly MethodInfo setFieldMethod =
			typeof(ReflectionUtility).GetMethod ("SetField",
			                                    BindingFlags.NonPublic | BindingFlags.Static);
		
				/// <summary>
				/// A strong type method that assigns the given value to the field that is represented by the given field reference.
				/// Note: .NET 4.0 provides an assignment expression. This method is just required for .NET 3.0/3.5.
				/// </summary>
				/// <typeparam name="TValue">The type of the field.</typeparam>
				/// <param name="field">A reference to the field.</param>
				/// <param name="newValue">The new value that should be assigned to the field.</param>
				private static void SetField<TValue> (ref TValue field, TValue newValue)
				{
						field = newValue;
				}
		
		#endregion Called by reflection - Don't delete.
		
		#endregion Field Access
		
				/// <summary>
				/// Determines whether the given type is a nullable type.
				/// </summary>
				/// <param name="type">The type to check.</param>
				/// <returns>true if the given type is a nullable type, otherwise false.</returns>
				public static bool IsNullableType (this Type type)
				{
						if (type == null)
								throw new ArgumentNullException ("type");
			
						bool result = false;
						if (type.IsGenericType && type.GetGenericTypeDefinition ().Equals (typeof(Nullable<>))) {
								result = true;
						}
			
						return result;
				}



			




				/// <summary>
				/// Gets the public members: fields and properties
				/// </summary>
				/// <returns>The public members.</returns>
				/// <param name="propertyType">Property type.</param>
				/// <param name="staticMembers">If set to <c>true</c> static members.</param>
				/// <param name="canWrite">If set to <c>true</c> can write.</param>
				/// <param name="canRead">If set to <c>true</c> can read.</param>
				/// <param name="isSubClass">If set to <c>true</c> is sub class.</param>
				public static MemberInfo[] GetPublicMembersOfType (this Type type, Type propertyType, bool staticMembers, bool canWrite, bool canRead, bool isSubClass=false)
				{
						List<MemberInfo> list = new List<MemberInfo> ();
						BindingFlags bindingAttr = (!staticMembers) ? (BindingFlags.Instance | BindingFlags.Public) : (BindingFlags.Static | BindingFlags.Public);
						FieldInfo[] fields = type.GetFields (bindingAttr);
						for (int i = 0; i < fields.Length; i++) {
								FieldInfo fieldInfo = fields [i];
								if (propertyType == null || fieldInfo.FieldType == propertyType || (isSubClass && fieldInfo.FieldType.IsSubclassOf (propertyType))) {
										list.Add (fieldInfo);
								}
						}
						PropertyInfo[] properties = type.GetProperties (bindingAttr);
						for (int j = 0; j < properties.Length; j++) {
								PropertyInfo propertyInfo = properties [j];
								if ((propertyType == null || propertyInfo.PropertyType == propertyType || (isSubClass && propertyInfo.PropertyType.IsSubclassOf (propertyType))) 
										&& ((canRead && propertyInfo.CanRead && (!canWrite || propertyInfo.CanWrite)) || (canWrite && propertyInfo.CanWrite && (!canRead || propertyInfo.CanRead)))) {
										list.Add (propertyInfo);
								}
						}
			
			
						return list.ToArray ();
				}




				/// <summary>
				/// Gets the member info.
				/// </summary>
				/// <returns>The member info.</returns>
				/// <param name="instance">Instance.</param>
				/// <param name="propertiesPath">Properties path on 'Instance' like a.b.c.prop </param>
				public static MemberInfo GetMemberFromPath (this Type type, string propertiesPath, BindingFlags flags=BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				{
			
						if (!propertiesPath.Contains ("."))
								return type.GetField (propertiesPath, flags) as MemberInfo ??
										type.GetProperty (propertiesPath, flags) as MemberInfo;
			
			
						string[] properties = propertiesPath.Split ('.');
			
			
			


						MemberInfo memberInfo = type.GetField (properties [0]) as MemberInfo ??
								type.GetProperty (properties [0]) as MemberInfo;

						var typeCurrent = memberInfo.GetUnderlyingType ();
			
						for (int propertyIndex = 1; propertyIndex < properties.Length; propertyIndex++) {
								propertiesPath = properties [propertyIndex];
								if (!string.IsNullOrEmpty (propertiesPath)) {

										memberInfo = typeCurrent.GetField (propertiesPath, flags) as MemberInfo ??
												typeCurrent.GetProperty (propertiesPath, flags) as MemberInfo;
										typeCurrent = memberInfo.GetUnderlyingType ();
								}
						}
			
			
						return memberInfo;
				}




		/// <summary>
		/// [ <c>public static object GetDefault(this Type type)</c> ]
		/// <para></para>
		/// Retrieves the default value for a given Type
		/// </summary>
		/// <param name="type">The Type for which to get the default value</param>
		/// <returns>The default value for <paramref name="type"/></returns>
		/// <remarks>
		/// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
		/// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
		/// exception.
		/// </remarks>
		/// <example>
		/// To use this method in its native, non-extension form, make a call like:
		/// <code>
		///     object Default = DefaultValue.GetDefault(someType);
		/// </code>
		/// To use this method in its Type-extension form, make a call like:
		/// <code>
		///     object Default = someType.GetDefault();
		/// </code>
		/// </example>
		/// <seealso cref="GetDefault&lt;T&gt;"/>
		public static object GetDefault(this Type type)
		{
			// If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
			if (type == null || !type.IsValueType || type == typeof(void))
				return null;
			
			// If the supplied Type has generic parameters, its default value cannot be determined
			if (type.ContainsGenericParameters)
				throw new ArgumentException(
					"{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
					"> contains generic parameters, so the default value cannot be retrieved");
			
			// If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
			//  default instance of the value type
			if (type.IsPrimitive || !type.IsNotPublic)
			{
				try
				{
			
					return Activator.CreateInstance(type);;
				}
				catch (Exception e)
				{
					throw new ArgumentException(
						"{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
						"create a default instance of the supplied value type <" + type +
						"> (Inner Exception message: \"" + e.Message + "\")", e);
				}
			}
			
			// Fail with exception
			throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type + 
			                            "> is not a publicly-visible type, so the default value cannot be retrieved");
		}
		
		public static void SetValue (this MemberInfo member, object instance, object value)
		{
			if (member.MemberType == MemberTypes.Property)
				((PropertyInfo)member).SetValue (instance, value, null);
			else if (member.MemberType == MemberTypes.Field)
								((FieldInfo)member).SetValue (instance, value);
						else
								throw new Exception ("Property must be of type FieldInfo or PropertyInfo");
				}
		
				public static object GetValue (this MemberInfo member, object instance)
				{
						if (member.MemberType == MemberTypes.Property)
								return ((PropertyInfo)member).GetValue (instance, null);
						else if (member.MemberType == MemberTypes.Field)
								return ((FieldInfo)member).GetValue (instance);
						else
								throw new Exception ("Property must be of type FieldInfo or PropertyInfo");
				}
		
				public static Type GetUnderlyingType (this MemberInfo member)
				{
						switch (member.MemberType) {
						case MemberTypes.Field:
								return ((FieldInfo)member).FieldType;
						case MemberTypes.Property:
								return ((PropertyInfo)member).PropertyType;
						case MemberTypes.Method:
								return ((MethodInfo)member).ReturnType;
						case MemberTypes.Event:
								return ((EventInfo)member).EventHandlerType;
					
						default:
								throw new ArgumentException ("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
						}
				}
		}
}

