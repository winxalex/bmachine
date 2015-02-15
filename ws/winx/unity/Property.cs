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
								return memberInfo.ReflectedType;
						}
				}

				public Type type {
						get {

								switch (memberInfo.MemberType) {
								case MemberTypes.Field:
										return ((FieldInfo)memberInfo).FieldType;
								case MemberTypes.Property:
										return ((PropertyInfo)memberInfo).PropertyType;
							
								case MemberTypes.Method:
										return ((MethodInfo)memberInfo).ReturnType;
								default:
										throw new ArgumentException ("MemberInfo must be if type FieldInfo, PropertyInfo or MethodInfo", "member");
								}
				
								
						}
				}

				public string name;
				public UnityEngine.Object reflectedInstance;
				public MemberInfo memberInfo;

				public MemberInfo MemberInfo {
						get {
								return memberInfo;
						}
						set {
								memberInfo = value;


								if ((memberInfo.MemberType == MemberTypes.Property) || (memberInfo.MemberType == MemberTypes.Field))
										this.name = memberInfo.Name;
					
								
					
								this.memberInfo = memberInfo;
								
						}
				}




				//
				// Properties
				//
				public  object Value {
						get {
								if (this.memberInfo == null) {
										//this.Initialize ();
								}
								if (this.memberInfo is PropertyInfo) {
										return ((PropertyInfo)this.memberInfo).GetValue (this.reflectedInstance, null);
								}
								if (this.memberInfo is FieldInfo) {
										return ((FieldInfo)this.memberInfo).GetValue (this.reflectedInstance);
								}

								if (this.memberInfo is MethodInfo) {

										return ((MethodInfo)this.memberInfo).Invoke (this.reflectedInstance, new object[]{this.name});
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
								if (this.memberInfo == null) {
										//this.Initialize ();
								}
								if (this.memberInfo is PropertyInfo) {
										((PropertyInfo)this.memberInfo).SetValue (this.reflectedInstance, value, null);
								} else {
										if (this.memberInfo is FieldInfo != null) {
												((FieldInfo)this.memberInfo).SetValue (this.reflectedInstance, value);
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


				public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }

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
						return "Property[" + name  + "] of type " + type + (this.reflectedInstance==null ? " on Static instance":" on instance of "+this.reflectedInstance);
				}


		}
}

