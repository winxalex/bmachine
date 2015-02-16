using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.utility;

namespace ws.winx.unity
{
		[Serializable]
		public class Property:ScriptableObject,IProperty
		{

				

				public Type reflectedType {
						get {
								return __memberInfo.ReflectedType;
						}
				}

				public Type type {
						get {

								switch (__memberInfo.MemberType) {
								case MemberTypes.Field:
										return ((FieldInfo)__memberInfo).FieldType;
								case MemberTypes.Property:
										return ((PropertyInfo)__memberInfo).PropertyType;
							
								case MemberTypes.Method:
										return ((MethodInfo)__memberInfo).ReturnType;
								default:
										throw new ArgumentException ("MemberInfo must be if type FieldInfo, PropertyInfo or MethodInfo", "member");
								}
				
								
						}
				}

				public string name;
				public System.Object reflectedInstance;
				[SerializeField]
				private MemberInfo
						__memberInfo;

				public MemberInfo MemberInfo {
						get {
								return __memberInfo;
						}
						set {
								__memberInfo = value;


								if ((__memberInfo.MemberType == MemberTypes.Property) || (__memberInfo.MemberType == MemberTypes.Field))
										this.name = __memberInfo.Name;
					
								
					
								this.__memberInfo = __memberInfo;
								
						}
				}




				//
				// Properties
				//



				/// <summary>
				/// Gets or sets the value.
				/// Property or Field 
				/// Method in form of GetValueOfProperty("property name");
				/// should return primitive
				/// </summary>
				/// <value>The value.</value>
				public  object Value {
						get {
								if (this.__memberInfo == null) {
										//this.Initialize ();
								}
								if (this.__memberInfo is PropertyInfo) {
										return ((PropertyInfo)this.__memberInfo).GetValue (this.reflectedInstance, null);
								}
								if (this.__memberInfo is FieldInfo) {
										return ((FieldInfo)this.__memberInfo).GetValue (this.reflectedInstance);
								}
								
								if (this.__memberInfo is MethodInfo) {
										if (String.IsNullOrEmpty (this.name)) {
												Debug.LogError ("Property name is missing ");
												return default(object);
										}

										object result = ((MethodInfo)this.__memberInfo).Invoke (this.reflectedInstance, new object[]{this.name});

										Type t = result.GetType ();

										if (t.IsPrimitive || t.IsValueType || t == typeof(Decimal) || t == typeof(String))				
												return result;
										else {//when object is returned try to find "Value" field or property to get primitive value from

												MemberInfo m = t.GetField ("Value", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

												if (m == null)
														m = t.GetProperty ("Value", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
												
												if (m != null) {
														this.__memberInfo = m;
														this.reflectedInstance =  result;
														return this.__memberInfo.GetValue (result);

												}

												Debug.LogError("Method "+this.__memberInfo.Name+ " doesn't return object that have 'Value' property");

										}
								}

								Debug.LogError (string.Concat (new object[]
					                               {
						"No property with name '",
						this.name,
						"' in the component '",
						this.reflectedInstance
						
					}));

								return null;
						}
						set {
								if (this.__memberInfo == null) {
										//this.Initialize ();
								}
								if (this.__memberInfo is PropertyInfo) {
										((PropertyInfo)this.__memberInfo).SetValue (this.reflectedInstance, value, null);
								} else {
										if (this.__memberInfo is FieldInfo != null) {
												((FieldInfo)this.__memberInfo).SetValue (this.reflectedInstance, value);
										} else {

												Debug.LogError (string.Concat (new object[]
							                               {
								"No property with name '",
								this.name,
								"' in the component '",
								this.reflectedInstance
								
							}));

										}
								}
						}
				}


				//
				// Constructor
				//
				public Property ()
				{

				}

//		public Property(MemberInfo memberInfo,UnityEngine.Object target){
//
//			if (memberInfo == null)
//								return;
//
//			if(memberInfo is PropertyInfo || memberInfo is FieldInfo)
//			this.name = memberInfo.Name;
//
//			this.reflectedObject = target;
//
//			this.memberInfo = memberInfo;
//			this.reflectedType = memberInfo.ReflectedType;
//
//
//
//		}


				//
				// Methods
				//
				public T GetValue<T> ()
				{
						return (T)this.Value;
				}

				public void OnEnable ()
				{
						hideFlags = HideFlags.HideAndDontSave;
				}

//		public override int GetHashCode ()
//		{
//			return base.GetHashCode ();
//		}



				public override bool Equals (object obj)
				{
						if (obj == null || !(obj is Property))
								return false;

						Property other = (Property)obj;
						return this.reflectedInstance.Equals (other.reflectedInstance) && this.reflectedType.Equals (other.reflectedType)
								&& this.name.Equals (other.name);
				}

				public override string ToString ()
				{
						return "Property[" + name + "] of type " + type + (this.reflectedInstance == null ? " on Static instance" : " on instance of " + this.reflectedInstance);
				}


		}
}

