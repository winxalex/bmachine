using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.unity;
using System.Runtime.Serialization.Formatters.Binary;
using ws.winx.csharp.extensions;
using UnityEngine.Events;
using UnityEditor;
using System.CodeDom.Compiler;

namespace ws.winx.unity
{
		[Serializable]
		public class UnityVariable:ScriptableObject,ISerializationCallbackReceiver
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


				/////////////////////////////////
				// For reusing of Unity drawers		

				private SerializedProperty
						__seralizedProperty;
				
				private SerializedObject
						__seralizedObject;

				public SerializedProperty serializedProperty {
						get {

								if (__seralizedProperty == null) {

										using (Microsoft.CSharp.CSharpCodeProvider foo = 
				       new Microsoft.CSharp.CSharpCodeProvider()) {
					
												CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters ();
					
											
					
												compilerParams.GenerateInMemory = true; 
					
												var assembyExcuting = Assembly.GetExecutingAssembly ();
												
												string assemblyLocationUnity = Assembly.GetAssembly (typeof(ScriptableObject)).Location;
						
												string usingString = "using UnityEngine;";
												
												if (!(this.ValueType.IsPrimitive || this.ValueType == typeof(string))) {
														string assemblyLocationVarable = Assembly.GetAssembly (this.ValueType).Location;
														compilerParams.ReferencedAssemblies.Add (assemblyLocationVarable);

														if (String.Compare (assemblyLocationUnity, assemblyLocationVarable) != 0) {

																usingString += "using " + this.ValueType.Namespace + ";";
														}
												}
							


												compilerParams.ReferencedAssemblies.Add (assemblyLocationUnity);
												compilerParams.ReferencedAssemblies.Add (assembyExcuting.Location);
					
					
				
					
												var res = foo.CompileAssemblyFromSource (
						compilerParams, String.Format (
						
						" {0}" +
						
														"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} field;}}"
							, usingString, this.ValueType.ToString ())
												);
					
					
					
					
												if (res.Errors.Count > 0) {
							
														foreach (CompilerError CompErr in res.Errors) {
																Debug.LogError (
																		"Line number " + CompErr.Line +
																		", Error Number: " + CompErr.ErrorNumber +
																		", '" + CompErr.ErrorText + ";" 
																);
														}
												} else {
					
														var type = res.CompiledAssembly.GetType ("ScriptableObjectTemplate");
							
														ScriptableObject st = ScriptableObject.CreateInstance (type);
								
														type.GetField ("field").SetValue (st, this.Value);
								
								
								
														__seralizedObject = new SerializedObject (st);
								
														__seralizedProperty = __seralizedObject.FindProperty ("field");

												}
			
										}

								}


								

								return __seralizedProperty;
					
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

								__event = __reflectedInstance as UnityEvent;
					
//								Debug.Log (" UnityInstance:" + __reflectedInstanceUnity + " Reflected instance:" + __reflectedInstance);
						}
				}

				[SerializeField]
				private UnityEngine.Object
						__reflectedInstanceUnity;

				[SerializeField]
				private UnityEvent __event;//this filed would have event even is empty

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

								if ((__reflectedInstance != null) && (__reflectedInstanceUnity == null && __event == null) && __reflectedInstance.GetType () != typeof(UnityEngine.Object)) {

						
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
										if (__reflectedInstanceUnity != null)
												__reflectedInstance = __reflectedInstanceUnity;
										else if (__event!=null && this.ValueType==typeof(UnityEvent))
												__reflectedInstance = __event;

								}
						}


			
				}

		#endregion

				public void ApplyModifiedProperties ()
				{
						if (__seralizedObject != null) {
								__seralizedObject.ApplyModifiedProperties ();

								if (this.ValueType == typeof(float)) {

										this.Value = __seralizedProperty.floatValue;
								} else
								if (this.ValueType == typeof(bool)) {
					
										this.Value = __seralizedProperty.boolValue;
								} else
								if (this.ValueType == typeof(Bounds)) {
					
										this.Value = __seralizedProperty.boundsValue;
								} else
								if (this.ValueType == typeof(Color)) {
					
										this.Value = __seralizedProperty.colorValue;
								} else
								if (this.ValueType == typeof(Rect)) {
					
										this.Value = __seralizedProperty.rectValue;
								} else
								if (this.ValueType == typeof(int)) {
									
										this.Value = __seralizedProperty.intValue;
								} else
								if (this.ValueType == typeof(Vector3)) {
									
										this.Value = __seralizedProperty.vector3Value;
								} else
								if (this.ValueType == typeof(string)) {
											
										this.Value = __seralizedProperty.stringValue;
								} else
								if (this.ValueType == typeof(Quaternion)) {
					
										this.Value = __seralizedProperty.quaternionValue;
								} else
									if (this.__reflectedInstance is UnityEngine.Object)
										this.Value = __seralizedProperty.objectReferenceValue;


						}
				}




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

						return this.GetInstanceID () == other.GetInstanceID ();

								
				}

				public override string ToString ()
				{
						return "Property[" + name + "] of type " + ValueType + (this.reflectedInstance == null ? " on Static instance" : " on instance of " + this.reflectedInstance);
				}

			


		}
}

