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
				private Type _valueType = typeof(object);


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
				private string
						__memberPath;

				/// <summary>
				/// The name of the property of UnityObject in format: ex. prop1 or prop1.prop2
				/// rotation.x or intensity
				/// to which variable is binded
				/// </summary>
				public string memberPath { 
						get{ return __memberPath; }
						protected	set { 
								__memberPath = value; 

						}
				}


				public enum DisplayMode
				{
						Raw=0,
						Bind,
						List
				}

				

				[HideInInspector]
				public DisplayMode
						displayMode = DisplayMode.Raw;
				[HideInInspector]
				public byte[]
						valueObjectSerialized;
				[NonSerialized]
				private object
						__valueObject;

				private ws.winx.csharp.extensions.ReflectionExtension.MemberInfoSetterDelegate<object,object>  __structSetterDelegate;
				private Func<object,object> __structGetterDelegate;
				private ws.winx.csharp.extensions.ReflectionExtension.MemberInfoSetterDelegate<object,object> __valueSetterDelegate;
		
				ws.winx.csharp.extensions.ReflectionExtension.MemberInfoSetterDelegate<object,object> valueSetterDelegate {
						get {
								if (__valueSetterDelegate == null)
										__valueSetterDelegate = __instanceUnityObject.GetType ().GetSetDelegate (memberPath);
								return __valueSetterDelegate;
						}
				}

				private Func<object,object> __valueGetterDelegate;

				Func<object,object> valueGetterDelegate {
						get {
								if (__valueGetterDelegate == null)
										__valueGetterDelegate = __instanceUnityObject.GetType ().GetGetDelegate (memberPath);
								return __valueGetterDelegate;
						}
				}

				public void Bind (object instance, string memberPath)
				{
						if (instance == null)
								throw new ArgumentException ("Instance");

						if (String.IsNullOrEmpty (memberPath))
								throw new ArgumentException ("memberPath");



						MemberInfo memberInfo = instance.GetType ().GetMemberFromPath (memberPath);

						if (memberInfo == null)
								Debug.LogException (new Exception ("Binding failed! No property " + memberPath + " found on instance " + instance));

						if (!memberInfo.GetUnderlyingType ().IsAssignableFrom (_valueType))
								Debug.LogException (new Exception ("Member type " + memberInfo.GetUnderlyingType () + " isn't assignable from variable type " + _valueType));

						__valueSetterDelegate = null;
						__valueGetterDelegate = null;
						__structSetterDelegate = null;
						__instanceMember = null;
						
				
						this.memberPath = memberPath;
						this.valueObject = instance;

					

					

				}

				/// <summary>
				/// Gets or sets 
				/// </summary>
				/// <value> valueObject conatiner, can be event,UnityEngine.Object or other System.Object
				/// </value>
				object valueObject {
						get {
								return __valueObject;
					
						}
						set {
								__valueObject = value;

								__unityVariableReferencedInstanceID = 0;
								__instanceUnityObject = null;
								__event = null;
				
								if (value is UnityVariable) {//save instanceID so instance can be restored in
										__unityVariableReferencedInstanceID = ((UnityVariable)value).GetInstanceID ();
										return;
								}
								
								__instanceUnityObject = __valueObject as UnityEngine.Object;

								__event = __valueObject as UnityEvent;
					
//								Debug.Log (" UnityInstance:" + __reflectedInstanceUnity + " Reflected instance:" + __reflectedInstance);
						}
				}


				/// <summary>
				/// The __instance unity object (GameObject, Transform, Light...) on which raw value or bind is based
				/// </summary>
				[SerializeField]
				private UnityEngine.Object
						__instanceUnityObject;

				public UnityEngine.Object instanceBinded {
						get {
								return __instanceUnityObject;
						}
				}


				/// <summary>
				/// The __instance on which member in "memberPath" is set or get
				/// ex. __instanceMember is Light in "Light.intensity" or is Quaternion(rotation) in  "Transform.rotaton.x"
				/// </summary>
				[NonSerialized]
				private object
						__instanceMember;
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
						
						return	(Vector3)variable.Value;
						
				}

				public static implicit operator Quaternion (UnityVariable variable)
				{
					
						return	(Quaternion)variable.Value;
					
				}

				public static implicit operator float (UnityVariable variable)
				{
					
						return (float)variable.Value;
				}

				public static implicit operator int (UnityVariable variable)
				{
					
						return (int)variable.Value;
				}


				
				///////////////  VALUE ////////////////

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
								if (String.IsNullOrEmpty (this.memberPath)) {

										return this.valueObject;
								}


								if (__instanceMember == null)
										initInstanceMember ();

								
									
								return valueGetterDelegate (__instanceMember);
				
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

										this.valueObject = null;
										__structSetterDelegate = null;
										__valueGetterDelegate = null;
										__valueSetterDelegate = null;
										return;
									
								}


								if (!value.GetType ().IsAssignableFrom (_valueType))
										Debug.LogException (new Exception ("Value type " + value.GetType () + " isn't assignable from variable type " + _valueType));

								//if nothings is binded => just assign new value
								if (String.IsNullOrEmpty (this.memberPath)) {
						

										this.valueObject = value;

										
								} else {


										if (__instanceMember == null)
												initInstanceMember ();

										
										if(__structGetterDelegate!=null){
											__instanceMember=__structGetterDelegate(__instanceUnityObject);
										}
										
										valueSetterDelegate (ref __instanceMember, value);

										if (__structSetterDelegate != null) {
												
												object unityObject=__instanceUnityObject;
												__structSetterDelegate (ref unityObject, __instanceMember);
												
										
										}

								}



#if UNITY_EDITOR
				UpdateSerializedProperty(value);
#endif
								
						}
				}


				//
				// Constructor
				//



				public static UnityVariable CreateInstanceOf (Type T)
				{

						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						variable._valueType = T;
						variable.Value = UnityVariable.Default (T);
							
						
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
								if (__unityVariableReferencedInstanceID == 0 && (__valueObject != null) && (__instanceUnityObject == null || (__instanceUnityObject != null && __instanceUnityObject.GetInstanceID () == 0) && __event == null) 
										&& !__valueObject.GetType ().IsSubclassOf (typeof(UnityEngine.Object)) 
										&& __valueObject.GetType () != typeof(UnityEngine.Object)
										&& __valueObject.GetType () != typeof(UnityEngine.Events.UnityEvent)

				    ) {

						
										try {
												valueObjectSerialized = Utility.Serialize (__valueObject);
										} catch (Exception ex) {

												Debug.LogWarning (ex.Message + " name:" + this.name + __valueObject + " " + __instanceUnityObject);
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
								if (!String.IsNullOrEmpty (__memberPath)) {

										this.memberPath = __memberPath;
								}

								if (valueTypeSerialized != null && valueTypeSerialized.Length > 0)
										_valueType = (Type)Utility.Deserialize (valueTypeSerialized);
								else 
										_valueType = typeof(System.Object);


#if UNITY_EDITOR
								if (__unityVariableReferencedInstanceID != 0) {
										__valueObject = EditorUtility.InstanceIDToObject (__unityVariableReferencedInstanceID);
										return;
								}
#endif

								if (valueObjectSerialized != null && valueObjectSerialized.Length > 0) { 

						
			
										__valueObject = Utility.Deserialize (valueObjectSerialized);

#if UNITY_EDITOR
										__seralizedProperty = null;
										__seralizedObject = null;
#endif

								} else {
										if (__instanceUnityObject != null)
												__valueObject = __instanceUnityObject;
										else if (__event != null && this.ValueType == typeof(UnityEvent))
												__valueObject = __event;
										else 
												__valueObject = null;

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

				public virtual void SetValue<T> (T value)
				{
					
						this.Value = value;
					
				}

				public bool IsBinded ()
				{
						if (this.instanceBinded != null && !String.IsNullOrEmpty (__memberPath))
								return true;

						return false;

				}

				public static object Default (Type T)
				{
						
						if (T == typeof(string))
								return String.Empty;
						else if (T == typeof(AnimationCurve))
								return new AnimationCurve ();
						else if (T == typeof(Texture2D))
								return new Texture2D (2, 2);
						else if (T == typeof(Texture3D))
								return new Texture3D (2, 2, 2, TextureFormat.ARGB32, true);
						else if (T == typeof(Material))
								return new Material (Shader.Find ("Diffuse"));
						else if (T == typeof(UnityEngine.Events.UnityEvent))
								return new UnityEvent ();
						else		
								return FormatterServices.GetUninitializedObject (T);
						
				
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
						string n = String.IsNullOrEmpty (name) ? "no name" : name;
						string print="UnityVariable[" + n + "] of type " + ValueType;

						if (IsBinded()) {
							print += "Value=" + this.Value.ToString ()+" binded "+memberPath+" on instance of "+ instanceBinded.GetType();

						} else {
							if(valueObject==null)
								print +=" Not initialized";
							else
								print +="Value=" +this.Value.ToString ();
						}
						
						

			   
			
						return print;
				}

				private object initInstanceMember ()
				{


					
						if (__instanceMember == null) {
						
								//ex. path1=rotation.x and path2=intensity
								string[] memberPathSegments = memberPath.Split ('.');
						
								// ex. path1 => __instanceMember points to "rotation" on __instanceUnityObject of Transform
								if (memberPathSegments.Length > 1) {
										__instanceMember = __instanceUnityObject.GetType ().GetMember (memberPathSegments [0]) [0].GetValue (__instanceUnityObject);
							
										//if it is property is of type struct => create additional setter (ex. transform.rotation.x where memberPath is "rotation.x" and __instanceUnityObject is "Transform"
										// so 
										if (__instanceMember.GetType ().IsValueType){
												__structSetterDelegate = __instanceUnityObject.GetType ().GetSetDelegate (memberPathSegments [0]);
												__structGetterDelegate=__instanceUnityObject.GetType().GetGetDelegate(memberPathSegments [0]);
										}
								} else //ex.path2 => _instanceMember = "Light" component
										__instanceMember = __instanceUnityObject;
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




		public void UpdateSerializedProperty(object value){


			if (this.ValueType == typeof(float)) {

				this.serializedProperty.floatValue = (float)value;
			}else
			if (this.ValueType == typeof(bool)) {
				
				this.serializedProperty.boolValue=(bool)value;
			} else
			if (this.ValueType == typeof(Bounds)) {
				this.serializedProperty.boundsValue=(Bounds)value;
				
			} else
			if (this.ValueType == typeof(Color)) {
				this.serializedProperty.colorValue=(Color)value;
				
			} else
			if (this.ValueType == typeof(Rect)) {
				this.serializedProperty.rectValue=(Rect)value;
				
			} else
			if (this.ValueType == typeof(int)) {
				this.serializedProperty.intValue=(int)value;
			} else
			if (this.ValueType == typeof(Vector3)) {
				this.serializedProperty.vector3Value=(Vector3)value;
			} else
			if (this.ValueType == typeof(string)) {
				this.serializedProperty.stringValue=(string)value;
			} else
			if (this.ValueType == typeof(Quaternion)) {
				this.serializedProperty.quaternionValue=(Quaternion)value;
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
					if (this.__valueObject is UnityEngine.Object)
						this.Value = __seralizedProperty.objectReferenceValue;
				
				
			}
		}

#endif

			


		}
}

