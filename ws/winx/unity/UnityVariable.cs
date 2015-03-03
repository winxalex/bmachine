using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.unity;
using System.Runtime.Serialization.Formatters.Binary;
using ws.winx.csharp.extensions;

namespace ws.winx.unity
{
		[Serializable]
		public class UnityVariable:ScriptableObject,ISerializationCallbackReceiver//,IProperty
		{
				
				public bool serializable = true;
				private Type _valueType = typeof(System.Object);
				[HideInInspector]
				public byte[]
						valueTypeSerialized;

				public Type ValueType {
						get {

								return _valueType;	
				
								
						}
				}

				[HideInInspector]
				public byte[]
						memberInfoSerialized;
				[HideInInspector]
				public byte[]
						reflectedInstanceSerialized;
				
				[NonSerialized]
				private object
						__reflectedInstance;

				public System.Object reflectedInstance {
						get {
								return __reflectedInstance;
					
						}
						set {
								__reflectedInstance = value;

						   
								__reflectedInstanceUnity = __reflectedInstance as UnityEngine.Object;
					
//								Debug.Log (" UnityInstance:" + __reflectedInstanceUnity + " Reflected instance:" + __reflectedInstance);
						}
				}

				[SerializeField]
				private UnityEngine.Object
						__reflectedInstanceUnity;
				[NonSerialized]
				private MemberInfo
						__memberInfo;
		
				public MemberInfo MemberInfo {
						get {
								return __memberInfo;
						}
						set {
								__memberInfo = value;

								if (value != null) {
									
										this.name = __memberInfo.Name;


										if (__memberInfo.MemberType == MemberTypes.Field) {
												_valueType = ((FieldInfo)__memberInfo).FieldType;

										} else if (__memberInfo.MemberType == MemberTypes.Property)
												_valueType = ((PropertyInfo)__memberInfo).PropertyType;
										else
												throw new Exception ("Unsupported MemberInfo type. Only Properties and Fields supported");
						
									
								}
									

								
								
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

										return this.reflectedInstance;
								}
								if (this.__memberInfo.MemberType == MemberTypes.Property) {
										return ((PropertyInfo)this.__memberInfo).GetValue (this.reflectedInstance, null);
								}
								if (this.__memberInfo.MemberType == MemberTypes.Field) {
										return ((FieldInfo)this.__memberInfo).GetValue (this.reflectedInstance);
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
										
										this.reflectedInstance = value;

										if (value != null)
												_valueType = this.reflectedInstance.GetType ();

										return;
								}
								if (this.__memberInfo.MemberType == MemberTypes.Property) {
										((PropertyInfo)this.__memberInfo).SetValue (this.reflectedInstance, value, null);
								} else {
										if (this.__memberInfo.MemberType == MemberTypes.Field) {
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
				public UnityVariable ()
				{

				}

		#region ISerializationCallbackReceiver implementation

				public void OnBeforeSerialize ()
				{
						if (serializable) {
								if (__memberInfo != null)
										memberInfoSerialized = Utility.Serialize (this.__memberInfo);

								valueTypeSerialized = Utility.Serialize (_valueType);

								if ((__reflectedInstance != null) && (__reflectedInstanceUnity == null) && __reflectedInstance.GetType () != typeof(UnityEngine.Object)) {

						
										try {
												reflectedInstanceSerialized = Utility.Serialize (__reflectedInstance);
										} catch (Exception ex) {

												Debug.Log (ex.Message + " name:" + this.name + " memInfo:" + __memberInfo + " " + __reflectedInstance + " " + __reflectedInstanceUnity);
										}
								}
						}

		
				}

				public void OnAfterDeserialize ()
				{
						if (serializable) {
								if (memberInfoSerialized != null && memberInfoSerialized.Length > 0)
										__memberInfo = (MemberInfo)Utility.Deserialize (memberInfoSerialized);

								if (valueTypeSerialized != null && valueTypeSerialized.Length > 0)
										_valueType = (Type)Utility.Deserialize (valueTypeSerialized);

								if (reflectedInstanceSerialized != null && reflectedInstanceSerialized.Length > 0) { 

						
			
										__reflectedInstance = Utility.Deserialize (reflectedInstanceSerialized);

								} else {
										__reflectedInstance = __reflectedInstanceUnity;

								}
						}


			
				}

		#endregion




				//
				// Methods
				//
				public virtual T GetValue<T> ()
				{
		
						return (T)this.Value;
				}
		
				public void OnEnable ()
				{
						//	hideFlags = HideFlags.HideAndDontSave;

					
				}

				public override int GetHashCode ()
				{
					return base.GetHashCode ();
				}



				public override bool Equals (object obj)
				{
						if (obj == null || !(obj is UnityVariable))
								return false;

						UnityVariable other = (UnityVariable)obj;

						return 
						(this.MemberInfo != null && this.MemberInfo.Equals (other.MemberInfo))
								|| (this.reflectedInstance == null && this.reflectedInstance.Equals (other.reflectedInstance));
								
				}

				public override string ToString ()
				{
						return "Property[" + name + "] of type " + ValueType + (this.reflectedInstance == null ? " on Static instance" : " on instance of " + this.reflectedInstance);
				}


		}
}

