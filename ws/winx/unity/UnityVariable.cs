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


				// this UnityVariable can reference other variable (ex in blackboard)
				[SerializeField]
				private int __unityVariableReferencedInstanceID=0;


				[HideInInspector]
				public byte[]
						valueTypeSerialized;

				public Type ValueType {
						get {

								return _valueType;	
				
								
						}
				}


				//////////////////////////////////////////
				// !!!! For reusing of Unity Drawers	//	
				[NonSerialized]
				private SerializedProperty
						__seralizedProperty;

				[NonSerialized]
				private SerializedObject
						__seralizedObject;

				
				public SerializedProperty serializedProperty {
						get {

								if (__seralizedProperty == null && this.Value != null) {

										CreateSerializedProperty();

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
						__instanceSystemObject;

				/// <summary>
				/// Gets or sets 
				/// </summary>
				/// <value>The instance system object (event,UnityEngine.Object or other System.Object)
				/// </value>
				public System.Object instanceSystemObject {
						get {
								return __instanceSystemObject;
					
						}
						set {
								__instanceSystemObject = value;

								__unityVariableReferencedInstanceID=0;
								__instanceUnityObject=null;
								__event=null;
				
								if(value is UnityVariable){//save instanceID so instance can be restored in
									__unityVariableReferencedInstanceID=((UnityVariable)value).GetInstanceID();
									return;
								}
								
								__instanceUnityObject = __instanceSystemObject as UnityEngine.Object;

								__event = __instanceSystemObject as UnityEvent;
					
//								Debug.Log (" UnityInstance:" + __reflectedInstanceUnity + " Reflected instance:" + __reflectedInstance);
						}
				}

				[SerializeField]
				private UnityEngine.Object
						__instanceUnityObject;
				[SerializeField]
				private UnityEvent
						__event;//this filed would have event even is empty

				[NonSerialized]
				private MemberInfo
						__memberInfo;
		
				public MemberInfo MemberInfo {
						get {
								return __memberInfo;
						}
						set {
								
								
								

								if(__memberInfo!=null && value!=null &&   __memberInfo.GetUnderlyingType() !=value.GetUnderlyingType()){
									Debug.LogError("MemberInfo should continue firstime established Type of :"+_valueType);
								}

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

										return this.instanceSystemObject;
								}
								if (this.__memberInfo.MemberType == MemberTypes.Property) {
										return ((PropertyInfo)this.__memberInfo).GetValue (this.instanceSystemObject, null);
								}
								if (this.__memberInfo.MemberType == MemberTypes.Field) {
										return ((FieldInfo)this.__memberInfo).GetValue (this.instanceSystemObject);
								}
								
								

								Debug.LogError (string.Concat (new object[]
					                               {
						"No property with name '",
						this.name,
						"' in the component '",
						this.instanceSystemObject
						
					}));

								return null;
						}
						set {

								
								if(value==null){
									__seralizedProperty=null;
									__seralizedObject=null;
									__unityVariableReferencedInstanceID=0;
									
									//value=default(this.ValueType);
									
									//_valueType=typeof(System.Object);
									//__memberInfo=null;

									
								}


								if (this.__memberInfo == null) {
										
										this.instanceSystemObject = value;

				
					
										if (value != null)
												_valueType = value.GetType ();


										return;
								}


								if (this.__memberInfo.MemberType == MemberTypes.Property) {
										((PropertyInfo)this.__memberInfo).SetValue (this.instanceSystemObject, value, null);
								} else {
										if (this.__memberInfo.MemberType == MemberTypes.Field) {
												((FieldInfo)this.__memberInfo).SetValue (this.instanceSystemObject, value);
										} else {

												Debug.LogError (string.Concat (new object[]
							                               {
																"No property with name '",
																this.name,
																"' in the component '",
																this.instanceSystemObject
																
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

								//if it is not reference to other UnityVariable isn't null and not reference to UnityObject
								if (__unityVariableReferencedInstanceID!=0 && (__instanceSystemObject != null) && (__instanceUnityObject == null && __event == null) && __instanceSystemObject.GetType () != typeof(UnityEngine.Object)) {

						
										try {
												reflectedInstanceSerialized = Utility.Serialize (__instanceSystemObject);
										} catch (Exception ex) {

												Debug.LogWarning (ex.Message + " name:" + this.name + " memInfo:" + __memberInfo + " " + __instanceSystemObject + " " + __instanceUnityObject);
										}
								}
						}

		
				}

				public void OnAfterDeserialize ()
				{
						if (serializable) {
								if (memberInfoSerialized != null && memberInfoSerialized.Length > 0)
										__memberInfo = (MemberInfo)Utility.Deserialize (memberInfoSerialized);
								else
									__memberInfo=null;

								if (valueTypeSerialized != null && valueTypeSerialized.Length > 0)
										_valueType = (Type)Utility.Deserialize (valueTypeSerialized);
								else 
									_valueType=typeof(System.Object);


								if(__unityVariableReferencedInstanceID!=0){
									__instanceSystemObject=EditorUtility.InstanceIDToObject(__unityVariableReferencedInstanceID);
									return;
								}

								if (reflectedInstanceSerialized != null && reflectedInstanceSerialized.Length > 0) { 

						
			
										__instanceSystemObject = Utility.Deserialize (reflectedInstanceSerialized);

										__seralizedProperty=null;
										__seralizedObject=null;

								} else {
										if (__instanceUnityObject != null)
												__instanceSystemObject = __instanceUnityObject;
										else if (__event != null && this.ValueType == typeof(UnityEvent))
												__instanceSystemObject = __event;
										else 
											__instanceSystemObject=null;

								}
						}


			
				}

		#endregion


				/// <summary>
				/// Applies the modified properties. !!!! For reusing of Unity Drawers
				/// </summary>
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
								} 
								else
								if (this.ValueType == typeof(Rect)) {
									
									this.Value = __seralizedProperty.rectValue;
								} 
								else
								if (this.ValueType == typeof(Bounds)) {
									
									this.Value = __seralizedProperty.boundsValue;
								} 
								else
								if (this.ValueType == typeof(AnimationCurve)) {
										this.Value = __seralizedProperty.animationCurveValue;
								}
								else
									if (this.__instanceSystemObject is UnityEngine.Object)
										  this.Value = __seralizedProperty.objectReferenceValue;


						}
				}

		[NonSerialized]
		public PropertyDrawer drawer;

			

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

//				public static bool operator ==(UnityVariable a, UnityVariable b)
//				{
//
//					
//					// Return true if the fields match:
//					return a.x == b.x && a.y == b.y && a.z == b.z;
//				}


				public override bool Equals (object obj)
				{
						if (obj == null || !(obj is UnityVariable))
								return false;

						UnityVariable other = (UnityVariable)obj;

						return this.GetInstanceID () == other.GetInstanceID ();

								
				}

				public override string ToString ()
				{
						return "Property[" + name + "] of type " + ValueType + (this.instanceSystemObject == null ? " on Static instance" : (this.instanceSystemObject.GetType().IsPrimitive || this.instanceSystemObject.GetType()==typeof(string)) ? " value of " : " on instance of " + this.instanceSystemObject.ToString());
				}

				void CreateSerializedProperty(){

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
							
							"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
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
							
							type.GetField ("value").SetValue (st, this.Value);
							
							
							
							__seralizedObject = new SerializedObject (st);
							
							__seralizedProperty = __seralizedObject.FindProperty ("value");
							
						}
						
					}


				}

			


		}
}

