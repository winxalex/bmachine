using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.unity;
using System.Runtime.Serialization.Formatters.Binary;
using ws.winx.csharp.extensions;
using UnityEngine.Events;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using ws.winx.csharp.utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ws.winx.unity
{
		[Serializable]
		public class UnityVariable:ScriptableObject,ISerializationCallbackReceiver
		{
				
				public bool serializable = true;
				private Type _valueType = typeof(System.Object);


				// this UnityVariable can reference other variable (ex in blackboard)
				[SerializeField]
				private int
						__unityVariableReferencedInstanceID = 0;
				[HideInInspector]
				public byte[]
						valueTypeSerialized;

				public Type ValueType {
						get {

								return _valueType;	
				
								
						}


				}

				

				
				[SerializeField]
				private string __memberName;

				/// <summary>
				/// The name of the property of UnityObject in format: ex. prop1 or prop1.prop2
				/// rotation.x or intensity
				/// to which variable is binded
				/// </summary>
				public string memberName { 
					get{ return __memberName; }
					set { 
						__memberName = value; 

					}
				}

				


			
				[HideInInspector]
				public byte[]
						reflectedInstanceSerialized;

				

				[NonSerialized]
				private object
						__instanceBinded;

				private Delegate __structSetterDelegate;

				private Delegate __valueSetterDelegate;

				public Delegate valueSetterDelegate {
					get {
						if(__valueSetterDelegate==null) __valueSetterDelegate=__instanceUnityObject.GetType().GetSetDelegate(memberName);
						return __valueSetterDelegate;
					}
				}


			

				private Delegate __valueGetterDelegate;

				public Delegate valueGetterDelegate {
					get {
						if(__valueGetterDelegate==null) __valueGetterDelegate=__instanceUnityObject.GetType().GetGetDelegate(memberName);
						return __valueGetterDelegate;
					}
				}
				

				/// <summary>
				/// Gets or sets 
				/// </summary>
				/// <value>The instance system object (event,UnityEngine.Object or other System.Object)
				/// </value>
				public System.Object instanceBinded {
						get {
								return __instanceBinded;
					
						}
						set {
								__instanceBinded = value;

								__unityVariableReferencedInstanceID = 0;
								__instanceUnityObject = null;
								__event = null;
				
								if (value is UnityVariable) {//save instanceID so instance can be restored in
										__unityVariableReferencedInstanceID = ((UnityVariable)value).GetInstanceID ();
										return;
								}
								
								__instanceUnityObject = __instanceBinded as UnityEngine.Object;

								__event = __instanceBinded as UnityEvent;
					
//								Debug.Log (" UnityInstance:" + __reflectedInstanceUnity + " Reflected instance:" + __reflectedInstance);
						}
				}

				[SerializeField]
				private UnityEngine.Object
						__instanceUnityObject;


				[NonSerialized]
				private object __instanceMember;
					

 

				[SerializeField]
				private UnityEvent
						__event;//this filed would have event even is empty




				




				//
				// Properties
				//

		//TODO

				// User-defined conversion from UnityVariable to Vector3 
				public static implicit operator Vector3 (UnityVariable variable)
				{
						Vector3 val;
						variable.GetValue(out val);
						return val;
				}


				


				/// <summary>
				/// Gets or sets the value.
				/// Property or Field 
				/// Method in form of GetValueOfProperty("property name");
				/// should return primitive
				/// </summary>
				/// <value>The value.</value>
				public  object Value {
						get {

								//if UnityVariable isn't binded to some memeber thru delegate
							if (String.IsNullOrEmpty(this.memberName)) {

										return this.instanceBinded;
								}


								if(__instanceMember==null) initInstanceMember();

								 object val;
								 ReflectionUtility.GetThruDelegate(valueGetterDelegate,__instanceMember,out val);
								 return val;

						}
						set {

								
								if (value == null) {

#if UNITY_EDITOR
										__seralizedProperty = null;
										__seralizedObject = null;

#endif
										__unityVariableReferencedInstanceID = 0;
									
										//value=default(this.ValueType);
									
										//_valueType=typeof(System.Object);
										//__memberInfo=null;

									
								}


								if (String.IsNullOrEmpty(this.memberName)) {
										
										this.instanceBinded = value;

				
					
										if (value != null)
												_valueType = value.GetType ();


										return;
								}


								if(__instanceMember==null) initInstanceMember();

								//ReflectionUtility.SetThruDelegate(valueSetterDelegate,ref __instanceMember,value);
				ReflectionUtility.SetThruDelegate(valueSetterDelegate,ref __instanceMember,value);

								if(__structSetterDelegate!=null)
									ReflectionUtility.SetThruDelegate(__structSetterDelegate,ref __instanceUnityObject,__instanceMember);
								
						}
				}


				//
				// Constructor
				//



				public static UnityVariable CreateInstanceOf (Type T)
				{

						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						variable._valueType = T;
						if (T != typeof(UnityEngine.Object)) {
								if (T == typeof(string))
										variable.Value = String.Empty;
								else if (T == typeof(AnimationCurve))
										variable.Value = new AnimationCurve ();
								else if (T == typeof(Texture2D))
										variable.Value = new Texture2D (2, 2);
								else if (T == typeof(Texture3D))
										variable.Value = new Texture3D (2, 2, 2, TextureFormat.ARGB32, true);
								else if (T == typeof(Material))
										variable.Value = new Material (Shader.Find ("Diffuse"));
								else if (T == typeof(UnityEngine.Events.UnityEvent))
										variable.Value = new UnityEvent ();
								else		
										variable.Value = FormatterServices.GetUninitializedObject (T);
						}
							
						
						variable.OnBeforeSerialize ();


						return variable;
				}

		#region ISerializationCallbackReceiver implementation

				public void OnBeforeSerialize ()
				{
						if (serializable) {
//								if (__memberInfo != null)
//										memberInfoSerialized = Utility.Serialize (this.__memberInfo);

								valueTypeSerialized = Utility.Serialize (_valueType);

								//if it is not reference to other UnityVariable isn't null and not reference to UnityObject
								if (__unityVariableReferencedInstanceID == 0 && (__instanceBinded != null) && (__instanceUnityObject == null || (__instanceUnityObject != null && __instanceUnityObject.GetInstanceID () == 0) && __event == null) 
										&& !__instanceBinded.GetType ().IsSubclassOf (typeof(UnityEngine.Object)) 
										&& __instanceBinded.GetType () != typeof(UnityEngine.Object)
										&& __instanceBinded.GetType () != typeof(UnityEngine.Events.UnityEvent)

				    ) {

						
										try {
												reflectedInstanceSerialized = Utility.Serialize (__instanceBinded);
										} catch (Exception ex) {

												Debug.LogWarning (ex.Message + " name:" + this.name  + __instanceBinded + " " + __instanceUnityObject);
										}
								}
						}

		
				}

				public void OnAfterDeserialize ()
				{
						if (serializable) {
//								if (memberInfoSerialized != null && memberInfoSerialized.Length > 0)
//										__memberInfo = (MemberInfo)Utility.Deserialize (memberInfoSerialized);
//								else
//										__memberInfo = null;

								//check if something was binded to this variable
								if(!String.IsNullOrEmpty(__memberName)){

									this.memberName=__memberName;
								}

								if (valueTypeSerialized != null && valueTypeSerialized.Length > 0)
										_valueType = (Type)Utility.Deserialize (valueTypeSerialized);
								else 
										_valueType = typeof(System.Object);


#if UNITY_EDITOR
								if (__unityVariableReferencedInstanceID != 0) {
										__instanceBinded = EditorUtility.InstanceIDToObject (__unityVariableReferencedInstanceID);
										return;
								}
#endif

								if (reflectedInstanceSerialized != null && reflectedInstanceSerialized.Length > 0) { 

						
			
										__instanceBinded = Utility.Deserialize (reflectedInstanceSerialized);

#if UNITY_EDITOR
										__seralizedProperty = null;
										__seralizedObject = null;
#endif

								} else {
										if (__instanceUnityObject != null)
												__instanceBinded = __instanceUnityObject;
										else if (__event != null && this.ValueType == typeof(UnityEvent))
												__instanceBinded = __event;
										else 
												__instanceBinded = null;

								}
						}


			
				}

		#endregion


				
				

			

				//
				// Methods
				//
				public virtual void GetValue<T> (out T result)
				{
						//if UnityVariable isn't binded to some memeber thru delegate
						if (String.IsNullOrEmpty(this.memberName)) {
							
							result=(T)this.instanceBinded;
							return;
						}
						
						
						if(__instanceMember==null) initInstanceMember();
						
						
						ReflectionUtility.GetThruDelegate(valueGetterDelegate,__instanceMember,out result);
						
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
						return "Property[" + name + "] of type " + ValueType + (this.instanceBinded == null ? (!String.IsNullOrEmpty(this.memberName) ? "Value=" + this.Value.ToString () + " on Static instance" : " Not initialized") : (this.instanceBinded.GetType ().IsPrimitive || this.instanceBinded.GetType () == typeof(string)) ? " Value=" + this.Value.ToString () : " Value=" + this.Value.ToString () + " on instance of " + this.instanceBinded.ToString ());
				}


				private object initInstanceMember(){


					
					if(__instanceMember==null){
						
						string[] propertyPath = memberName.Split ('.');
						
						if(propertyPath.Length>2){
							__instanceMember=__instanceUnityObject.GetType().GetMember(propertyPath[0])[0].GetValue(__instanceUnityObject);
							
							//if it is property is of type struct => create additional setter
							if(__instanceMember.GetType().IsValueType) __structSetterDelegate=__instanceUnityObject.GetType().GetSetDelegate(propertyPath[0]);
						}
						else
							__instanceMember=__instanceUnityObject;
					}
					
					return __instanceMember;
				
				}


				////////////////////////////////////////////////////////////////////////////////////
				//                            !!!! For reusing of Unity Drawers	                 //	

#if UNITY_EDITOR
		[NonSerialized]
		public PropertyDrawer
			drawer;
		
		[NonSerialized]
		private SerializedProperty
			__seralizedProperty;
		[NonSerialized]
		private SerializedObject
			__seralizedObject;
		
		public SerializedProperty serializedProperty {
			get {
				
				if (__seralizedProperty == null && this.Value != null) {
					
					CreateSerializedProperty ();

				
					
				}
				
				
				
				
				return __seralizedProperty;
				
			}
		}
		
		void CreateSerializedProperty ()
				{

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





		/// <summary>
		/// Applies the modified properties. !!!! For reusing of Unity Drawers
		/// </summary>
		public void ApplyModifiedProperties ()
		{
			if (__seralizedObject != null) {
				
				
				__seralizedObject.ApplyModifiedProperties ();
				
				
				
				if (this.ValueType == typeof(float) && (float)this.Value != __seralizedProperty.floatValue) {
					
					this.Value = __seralizedProperty.floatValue;
					this.OnBeforeSerialize ();//serialize primitive and objects that aren't subclass of UnityObject or are UnityEvents
				} else
				if (this.ValueType == typeof(bool) && (bool)this.Value != __seralizedProperty.boolValue) {
					
					this.Value = __seralizedProperty.boolValue;
					this.OnBeforeSerialize ();
				} else
				if (this.ValueType == typeof(Bounds)) {
					if (((Bounds)this.Value).center != __seralizedProperty.boundsValue.center || ((Bounds)this.Value).max != __seralizedProperty.boundsValue.max || ((Bounds)this.Value).min != __seralizedProperty.boundsValue.min || ((Bounds)this.Value).size != __seralizedProperty.boundsValue.size) {
						this.Value = __seralizedProperty.boundsValue;
						this.OnBeforeSerialize ();
					}
					
				} else
				if (this.ValueType == typeof(Color)) {
					if (((Color)this.Value).r != __seralizedProperty.colorValue.r || ((Color)this.Value).g != __seralizedProperty.colorValue.g || ((Color)this.Value).b != __seralizedProperty.colorValue.b || ((Color)this.Value).a != __seralizedProperty.colorValue.a) {
						this.Value = __seralizedProperty.colorValue;
						this.OnBeforeSerialize ();
					}
					
				} else
				if (this.ValueType == typeof(Rect)) {
					if (((Rect)this.Value).x != __seralizedProperty.rectValue.x || ((Rect)this.Value).y != __seralizedProperty.rectValue.y || ((Rect)this.Value).width != __seralizedProperty.rectValue.width || ((Rect)this.Value).height != __seralizedProperty.rectValue.height) {
						this.Value = __seralizedProperty.rectValue;
						this.OnBeforeSerialize ();
					}
					
				} else
				if (this.ValueType == typeof(int) && (int)this.Value != __seralizedProperty.intValue) {
					
					this.Value = __seralizedProperty.intValue;
					this.OnBeforeSerialize ();
				} else
				if (this.ValueType == typeof(Vector3)) {
					if (((Vector3)this.Value).x != __seralizedProperty.vector3Value.x || ((Vector3)this.Value).y != __seralizedProperty.vector3Value.y || ((Vector3)this.Value).z != __seralizedProperty.vector3Value.z) {
						this.Value = __seralizedProperty.vector3Value;
						this.OnBeforeSerialize ();
					}
				} else
				if (this.ValueType == typeof(string) && (string)this.Value != __seralizedProperty.stringValue) {
					
					this.Value = __seralizedProperty.stringValue;
					this.OnBeforeSerialize ();
				} else
				if (this.ValueType == typeof(Quaternion)) {
					if (((Quaternion)this.Value).x != __seralizedProperty.quaternionValue.x || ((Quaternion)this.Value).y != __seralizedProperty.quaternionValue.y || ((Quaternion)this.Value).z != __seralizedProperty.quaternionValue.z || ((Quaternion)this.Value).w != __seralizedProperty.quaternionValue.w) {
						this.Value = __seralizedProperty.quaternionValue;
						this.OnBeforeSerialize ();
					}
				} else
				if (this.ValueType == typeof(AnimationCurve)) {
					
					
					this.OnBeforeSerialize ();
					
				} else
					if (this.__instanceBinded is UnityEngine.Object)
						this.Value = __seralizedProperty.objectReferenceValue;
				
				
			}
		}

#endif

			


		}
}

