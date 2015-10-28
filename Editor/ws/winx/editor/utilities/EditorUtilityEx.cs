using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using ws.winx.csharp.utilities;
using System;
using ws.winx.unity;
using ws.winx.editor.extensions;
using System.Reflection;
using ws.winx.editor.drawers;
using System.IO;
using UnityEditor.Animations;
using ws.winx.csharp.extensions;
using System.CodeDom.Compiler;
using System.Linq;


namespace ws.winx.editor.utilities
{
	[InitializeOnLoad]
	public class EditorUtilityEx
		{


				const string showLockIconPrefKey = "Lock_ShowIcon";
				const string addLockUndoRedoPrefKey = "Lock_UndoRedo";
				const string lockMultiSelectionPrefKey = "Lock_MultiSelection";



				static EditorUtilityEx()
				{

					bool drawLockIcon = false;



					if (!EditorPrefs.HasKey (showLockIconPrefKey)) {
								EditorPrefs.SetBool (showLockIconPrefKey, true);
								drawLockIcon=true;
						} else {

								drawLockIcon=EditorPrefs.GetBool(showLockIconPrefKey);

						}

							EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;

						if (drawLockIcon) {
								
								EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
							
								
								EditorApplication.RepaintHierarchyWindow ();
								
								
							}
			
			
			
					if (!EditorPrefs.HasKey(addLockUndoRedoPrefKey))
					{
						EditorPrefs.SetBool(addLockUndoRedoPrefKey, true);
					}
					if (!EditorPrefs.HasKey(lockMultiSelectionPrefKey))
					{
						EditorPrefs.SetBool(lockMultiSelectionPrefKey, false);
					}


//					var stek = ScriptAttributeUtilityW.s_DrawerStack;
//
//
//			IDictionary d= ScriptAttributeUtilityW.s_DrawerTypeForType;
//
//			if(d==null)
//			ScriptAttributeUtilityW.BuildDrawerTypeForTypeDictionary();
//
//			d = ScriptAttributeUtilityW.s_DrawerTypeForType;
//
//			if(!d.Contains(typeof(Dictionary<,>)))
//				d.Add (typeof(Dictionary<,>), ScriptAttributeUtilityW.GetDrawerKeySetInstance(typeof(Dictionary<,>),typeof(DictionaryPropertyDrawer)));
//		
//			var res = d [typeof(Dictionary<,>)];
		}



			
		



		static Assembly _EditorAssembly;

		public static Assembly EditorAssembly{
			get{
				if(_EditorAssembly==null)
					_EditorAssembly=Assembly.GetAssembly (typeof(Editor));

				return _EditorAssembly;
			}
		}
	



		static Assembly[] _loadedEditorAssemblies;
		
		
		public static Assembly[] loadedEditorAssemblies
		{
			get
			{
				if (EditorUtilityEx._loadedEditorAssemblies == null)
				{
					EditorUtilityEx._loadedEditorAssemblies = AppDomain.CurrentDomain.GetAssemblies ();
					List<Assembly> list = new List<Assembly> ();
					Assembly[] array = EditorUtilityEx._loadedEditorAssemblies;
					for (int i = 0; i < array.Length; i++)
					{
						Assembly assembly = array [i];
						if (!assembly.GetName ().Name.Contains ("Editor"))
						{
							list.Add (assembly);
						}
					}
					EditorUtilityEx._loadedEditorAssemblies = list.ToArray ();
				}
				return EditorUtilityEx._loadedEditorAssemblies;
			}
		}
		
		
		public static Type[] GetDerivedTypes (Type baseType)
		{
			List<Type> list = new List<Type> ();
			Assembly[] loadedAssemblies = EditorUtilityEx.loadedEditorAssemblies;
			int i = 0;
			while (i < loadedAssemblies.Length)
			{
				Assembly assembly = loadedAssemblies [i];
				Type[] exportedTypes;
				try
				{
					exportedTypes = assembly.GetExportedTypes ();
				}
				catch (ReflectionTypeLoadException)
				{
					Debug.LogWarning (string.Format ("EditorUtilityEx will ignore the following assembly due to type-loading errors: {0}", assembly.FullName));
					goto IL_91;
				}
				goto IL_44;
			IL_91:
					i++;
				continue;
			IL_44:
					for (int j = 0; j < exportedTypes.Length; j++)
				{
					Type type = exportedTypes [j];
					if (!type.IsAbstract&& type.IsSubclassOf (baseType) && type.FullName  != null)
					{
						list.Add (type);
					}
				}
				goto IL_91;
			}
			list.Sort ((Type o1, Type o2) => o1.ToString ().CompareTo (o2.ToString ()));
			return list.ToArray ();
		}

		
		
		


		
		
		
		/// <summary>
		/// Applies the variable value to serialized property.
		/// </summary>
		/// <param name="variable">Variable.</param>
		public static void UpdateSerializedProperty(UnityVariable variable){
			SerializedProperty serializedProperty = variable.serializedProperty as SerializedProperty;

			object value = variable.Value;
			
			if (variable.ValueType == typeof(float)) {
				
				serializedProperty.floatValue = (float)value;
			} else
			if (variable.ValueType == typeof(bool)) {
				
				serializedProperty.boolValue = (bool)value;
			} else
			if (variable.ValueType == typeof(Bounds)) {
				serializedProperty.boundsValue = (Bounds)value;
				
			} else
			if (variable.ValueType == typeof(Color)) {
				serializedProperty.colorValue = (Color)value;
				
			} else
			if (variable.ValueType == typeof(Rect)) {
				serializedProperty.rectValue = (Rect)value;
				
			} else
			if (variable.ValueType == typeof(int)) {
				serializedProperty.intValue = (int)value;
			} else
			if (variable.ValueType == typeof(Vector3)) {
				serializedProperty.vector3Value = (Vector3)value;
			} else
			if (variable.ValueType == typeof(string)) {
				serializedProperty.stringValue = (string)value;
			} else
			if (variable.ValueType == typeof(Quaternion)) {
				serializedProperty.quaternionValue = (Quaternion)value;
			} else if(value is UnityEngine.Object)
				serializedProperty.objectReferenceValue = value as UnityEngine.Object;
			
			//TODO apply changes of array,list,dict happen in code, to serialized prop so can be shown in editor
		}
		
		
		/// <summary>
		/// Applies the modified properties serializedProperty so can be used in PropertyDrawers
		/// </summary>
		public static void ApplySerializedPropertyChangeTo (UnityVariable variable)
		{
			//SerializedObject __seralizedObject = variable.seralizedObject as SerializedObject;
			SerializedProperty seralizedProperty=variable.serializedProperty as SerializedProperty;



			if (seralizedProperty.serializedObject != null) {
				
				if (seralizedProperty.serializedObject.targetObject == null) { //has been destroyed by Unity ????
					
					//__seralizedObject=null;
					seralizedProperty=null;
					
					variable.serializedProperty=seralizedProperty=SerializeObject(variable);

				}
				
				//__seralizedObject.ApplyModifiedProperties ();
				seralizedProperty.serializedObject.ApplyModifiedProperties();

				object targetObject=seralizedProperty.serializedObject.targetObject;

				if(variable.ValueType.IsGenericType && variable.ValueType.GetGenericTypeDefinition()==typeof(Dictionary<,>))
				{
					IDictionary dict=variable.Value as IDictionary;
					dict.Clear();

					IList valuesList=targetObject.GetType().GetField("values").GetValue(targetObject) as IList;
					IList keysList=	targetObject.GetType().GetField("keys").GetValue(targetObject)  as IList;
					int cnt=valuesList.Count;

					for(int i=0;i<cnt;i++)
						if(!dict.Contains(keysList[i]))
						dict.Add(keysList[i],valuesList[i]);
				}
				else
					variable.Value=targetObject.GetType().GetField("value").GetValue(targetObject);
				
//				
//				if (variable.ValueType == typeof(float) && (float)variable.Value != seralizedProperty.floatValue) {
//					
//					variable.Value = seralizedProperty.floatValue;
//					variable.OnBeforeSerialize ();//serialize primitive and objects that aren't subclass of UnityObject or are UnityEvents
//				} else
//				if (variable.ValueType == typeof(bool) && (bool)variable.Value != seralizedProperty.boolValue) {
//					
//					variable.Value = seralizedProperty.boolValue;
//					variable.OnBeforeSerialize ();
//				} else
//				if (variable.ValueType == typeof(Bounds)) {
//					if (((Bounds)variable.Value).center != seralizedProperty.boundsValue.center || ((Bounds)variable.Value).max != seralizedProperty.boundsValue.max || ((Bounds)variable.Value).min != seralizedProperty.boundsValue.min || ((Bounds)variable.Value).size != seralizedProperty.boundsValue.size) {
//						variable.Value = seralizedProperty.boundsValue;
//						variable.OnBeforeSerialize ();
//					}
//					
//				} else
//				if (variable.ValueType == typeof(Color)) {
//					if (((Color)variable.Value).r != seralizedProperty.colorValue.r || ((Color)variable.Value).g != seralizedProperty.colorValue.g || ((Color)variable.Value).b != seralizedProperty.colorValue.b || ((Color)variable.Value).a != seralizedProperty.colorValue.a) {
//						variable.Value = seralizedProperty.colorValue;
//						variable.OnBeforeSerialize ();
//					}
//					
//				} else
//				if (variable.ValueType == typeof(Rect)) {
//					if (((Rect)variable.Value).x != seralizedProperty.rectValue.x || ((Rect)variable.Value).y != seralizedProperty.rectValue.y || ((Rect)variable.Value).width != seralizedProperty.rectValue.width || ((Rect)variable.Value).height != seralizedProperty.rectValue.height) {
//						variable.Value = seralizedProperty.rectValue;
//						variable.OnBeforeSerialize ();
//					}
//					
//				} else
//				if (variable.ValueType == typeof(int) && (int)variable.Value != seralizedProperty.intValue) {
//					
//					variable.Value = seralizedProperty.intValue;
//					variable.OnBeforeSerialize ();
//				} else
//				if (variable.ValueType == typeof(Vector3)) {
//					if (((Vector3)variable.Value).x != seralizedProperty.vector3Value.x || ((Vector3)variable.Value).y != seralizedProperty.vector3Value.y || ((Vector3)variable.Value).z != seralizedProperty.vector3Value.z) {
//						variable.Value = seralizedProperty.vector3Value;
//						variable.OnBeforeSerialize ();
//					}
//				} else
//				if (variable.ValueType == typeof(string) && (string)variable.Value != seralizedProperty.stringValue) {
//					
//					variable.Value = seralizedProperty.stringValue;
//					variable.OnBeforeSerialize ();
//				} else
//				if (variable.ValueType == typeof(Quaternion)) {
//					if (((Quaternion)variable.Value).x != seralizedProperty.quaternionValue.x || ((Quaternion)variable.Value).y != seralizedProperty.quaternionValue.y || ((Quaternion)variable.Value).z != seralizedProperty.quaternionValue.z || ((Quaternion)variable.Value).w != seralizedProperty.quaternionValue.w) {
//						variable.Value = seralizedProperty.quaternionValue;
//						variable.OnBeforeSerialize ();
//					}
//				} else
//				if (variable.ValueType == typeof(AnimationCurve)) {
//					
//					
//					variable.OnBeforeSerialize ();
//					
//				} else
//					if (variable.valueObject is UnityEngine.Object)
//						variable.Value = seralizedProperty.objectReferenceValue;
				
				
			}
		}
		


//#endif


				//
				// Static Fields
				//

				//Dictionary of Attribute-Type|PropertyDrawer
				private static Dictionary<Type, PropertyDrawer> __drawers;
				private static UnityDefaultPropertyDrawer __drawerDefault;

				public static PropertyDrawer GetDefaultDrawer ()
				{
						if (__drawerDefault == null)
								__drawerDefault = new UnityDefaultPropertyDrawer ();
						return __drawerDefault;
				}

				//
				// Static Methods
				//
				

		#region GetDrawer of type
				//TODO maybe use ScriptUtilityAttributeW for some
				/// <summary>
				/// Gets the drawer.
				/// </summary>
				/// <returns>The drawer.</returns>
				/// <param name="type">Type.</param>
				public static PropertyDrawer GetDrawer (Type type)
				{
						Type typeDrawer;
						PropertyDrawer drawer = null;
						if (EditorUtilityEx.__drawers == null) {
								

								EditorUtilityEx.__drawers = new Dictionary<Type, PropertyDrawer> ();
								Type[] derivedTypes = ReflectionUtility.GetDerivedTypes (typeof(PropertyDrawer));
								CustomPropertyDrawer[] attributes;
								CustomPropertyDrawer attribute;
								for (int i = 0; i < derivedTypes.Length; i++) {
										typeDrawer = derivedTypes [i];

										attributes = AttributeUtility.GetAttributes<CustomPropertyDrawer> (typeDrawer, false);

										if (attributes != null)
												for (int j=0; j<attributes.Length; j++) {

														attribute = attributes [j];

														if (attribute != null) {
																FieldInfo m_TypeFieldInfo = attribute.GetType ().GetField ("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);




																if (m_TypeFieldInfo != null) {
																		Type typeProperty = (Type)m_TypeFieldInfo.GetValue (attribute);

												

																		if (typeProperty != null && typeProperty.BaseType != typeof(PropertyAttribute))
																		{
																			if(typeProperty.IsGenericType)
																					typeProperty=typeProperty.GetGenericTypeDefinition();

																			if(!EditorUtilityEx.__drawers.ContainsKey (typeProperty)) {
																															EditorUtilityEx.__drawers.Add (typeProperty, Activator.CreateInstance (typeDrawer) as PropertyDrawer);
																				
//																				Debug.Log("  "+typeProperty.Name+" "+typeDrawer.Name+" "+typeProperty.BaseType);
																			}
																		}
																}
														}
												}//attributes
								}//types in dll
						}

						if (type.IsGenericType)
						type = type.GetGenericTypeDefinition ();
						

						EditorUtilityEx.__drawers.TryGetValue (type, out drawer);
						if (drawer != null) {
								return  drawer;

						}
						
						if (type.BaseType != null)
						if (EditorUtilityEx.__drawers.TryGetValue (type.BaseType, out drawer)) {
								return drawer;
						
						}
			
						if (__drawerDefault == null)
								__drawerDefault = new UnityDefaultPropertyDrawer ();


						return __drawerDefault;
				}



		#endregion

		#region SerializedObject
		public static SerializedProperty SerializeObject (object obj)
		{

			//ws.winx.csharp.CSharpCodeCompilerOriginal compOrig = new ws.winx.csharp.CSharpCodeCompilerOriginal ();
			ws.winx.csharp.CSharpCodeCompiler compOrig = new ws.winx.csharp.CSharpCodeCompiler ();
			ws.winx.csharp.CSharpCodeCompiler.unixMcsCommand="/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current/bin/gmcs";

			object value = obj;

			if(obj is UnityVariable){
				UnityVariable var=obj as UnityVariable;

					value=var.Value;
					                                 
			}



							Type ValueType =  obj is UnityVariable? (obj as UnityVariable).ValueType : obj.GetType ();
			//System.CodeDom.Compiler.CodeDomProvider provider = new CodeDomProvider ();			
			CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters ();
			
			
			
			compilerParams.GenerateInMemory = true; 
			
			var assembyExcuting = Assembly.GetExecutingAssembly ();
			
			string assemblyLocationUnity = Assembly.GetAssembly (typeof(ScriptableObject)).Location;
			
			string usingString = "using UnityEngine;";
			
			if (!(ValueType.IsPrimitive || ValueType == typeof(string))) {
				string assemblyLocationVarable = Assembly.GetAssembly (ValueType).Location;
				compilerParams.ReferencedAssemblies.Add (assemblyLocationVarable);
				
				if (String.Compare (assemblyLocationUnity, assemblyLocationVarable) != 0) {
					
					usingString += "using " + ValueType.Namespace + ";";
				}
			}
			
			
			
			compilerParams.ReferencedAssemblies.Add (assemblyLocationUnity);
			compilerParams.ReferencedAssemblies.Add (assembyExcuting.Location);
			


			// Create class with one property value of type same as type of the object we want to serialize

			String sourceCodeString = String.Format (
				
				" {0}" +
				
				"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
				, usingString, ValueType.ToCSharpString ());

			Type[] genericTypes = null;
			if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
				genericTypes=ValueType.GetGenericArguments();

				IDictionary dictionary=value as IDictionary;

				genericTypes[0]=typeof(List<>).MakeGenericType(new Type[]{genericTypes[0]});
				genericTypes[1]=typeof(List<>).MakeGenericType(new Type[]{genericTypes[1]});
					

				sourceCodeString = String.Format (
					
					" {0}" +
					
					"public class ScriptableObjectTemplate:ScriptableObject {{ " +

					"public {1} keys;"+
					"public {2} values;"+

					"}}"
					, usingString,genericTypes[0].ToCSharpString(),genericTypes[1].ToCSharpString() 
					);
			}

		


			var res = compOrig.CompileAssemblyFromSource (
				compilerParams,sourceCodeString )
				;
			
			
			
			
			if (res.Errors.Count > 0) {
				
				foreach (CompilerError CompErr in res.Errors) {
					Debug.LogError (
						"Line number " + CompErr.Line +
						", Error Number: " + CompErr.ErrorNumber +
						", '" + CompErr.ErrorText + ";" 
						);
				}
				
				return null;
				
			} else {
				
				var type = res.CompiledAssembly.GetType ("ScriptableObjectTemplate");
				
				ScriptableObject st = ScriptableObject.CreateInstance (type);

				if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {

					IDictionary valueDict=value as IDictionary;


					int count=valueDict.Keys.Count;

					IList keysList=Activator.CreateInstance(genericTypes[0]) as IList;
					IList valuesList=Activator.CreateInstance(genericTypes[1]) as IList;
					IEnumerator eKeys=valueDict.Keys.GetEnumerator();
					IEnumerator eValues=valueDict.Values.GetEnumerator();

					while (eKeys.MoveNext())
											{
						keysList.Add (eKeys.Current);
						eValues.MoveNext();
							valuesList.Add(eValues.Current);
					}


					st.GetType().GetField("keys").SetValue(st,keysList);
					st.GetType().GetField("values").SetValue(st,valuesList);


				}else
					type.GetField ("value").SetValue (st, value);
				
				SerializedObject seralizedObject=new SerializedObject(st);
				
				SerializedProperty serializedProp=seralizedObject.FindProperty ("value");

				if(serializedProp==null) 
					serializedProp=seralizedObject.FindProperty ("values");

				if(serializedProp==null) 
					Debug.LogError("Can't serialize "+obj);
				
				return serializedProp;
				
				
				
				
				
			}

			//System.CodeDom.Compiler.CodeDomProvider provider = new CodeDomProvider ();
			//provider.CompileAssemblyFromSource ();

			//Mono.CSharp.CSharpCodeCompiler compiler=new Mono.CSharp.CSharpCodeCompiler();


			//return SerializeVariable (value as UnityVariable);
//			
//			using (Microsoft.CSharp.CSharpCodeProvider provider = 
//			       new Microsoft.CSharp.CSharpCodeProvider()) {
//
//				System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters ();
//
//
//				
//
//				Type ValueType = typeof(float);//   value is UnityVariable? (value as UnityVariable).ValueType : value.GetType ();
//				
//				
//				compilerParams.GenerateInMemory = true; 
//				
//				var assembyExcuting = Assembly.GetExecutingAssembly ();
//				
//				string assemblyLocationUnity = Assembly.GetAssembly (typeof(ScriptableObject)).Location;
//				
//				string usingString = "using UnityEngine;";
//				
//				if (!(ValueType.IsPrimitive || ValueType == typeof(string))) {
//					string assemblyLocationVarable = Assembly.GetAssembly (ValueType).Location;
//					compilerParams.ReferencedAssemblies.Add (assemblyLocationVarable);
//					
//					if (String.Compare (assemblyLocationUnity, assemblyLocationVarable) != 0) {
//						
//						usingString += "using " + ValueType.Namespace + ";";
//					}
//				}
//
//			//	MemberInfo windowsMcsPath=typeof(Mono.CSharp.CSharpCodeCompiler).GetField("windowsMcsPath",BindingFlags.Static && BindingFlags.NonPublic);
//			//	MemberInfo windowsMonoPath=typeof(Mono.CSharp.CSharpCodeCompiler).GetField("windowsMonoPath",BindingFlags.Static && BindingFlags.NonPublic);
//
//
//
//
//				if(Application.platform==RuntimePlatform.OSXPlayer && Application.platform==RuntimePlatform.OSXEditor){
//
//				}
//
//				
//				compilerParams.ReferencedAssemblies.Add (assemblyLocationUnity);
//				compilerParams.ReferencedAssemblies.Add (assembyExcuting.Location);
//				
//
//
////				var res = compiler.CompileAssemblyFromSource (
////					compilerParams, String.Format (
////					
////					" {0}" +
////					
////					"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
////					, usingString, ValueType.ToString ())
////					);
//				var res = provider.CompileAssemblyFromSource (
//					compilerParams, String.Format (
//					
//					" {0}" +
//					
//					"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
//					, usingString, ValueType.ToString ())
//					);
//				
//				
//				
//				
//				if (res.Errors.Count > 0) {
//					
//					foreach (System.CodeDom.Compiler.CompilerError CompErr in res.Errors) {
//						Debug.LogError (
//							"Line number " + CompErr.Line +
//							", Error Number: " + CompErr.ErrorNumber +
//							", '" + CompErr.ErrorText + ";" 
//							);
//					}
//					
//					
//					return null;
//					
//					
//				} else {
//					
//					var type = res.CompiledAssembly.GetType ("ScriptableObjectTemplate");
//					
//					ScriptableObject st = ScriptableObject.CreateInstance (type);
//					
//					type.GetField ("value").SetValue (st, value);
//					
//					
//					
//					
//					
//					return new SerializedObject (st).FindProperty ("value");
//					
//				}
//				
//			}
			
			
			
//			using (Microsoft.CSharp.CSharpCodeProvider foo = 
//			       new Microsoft.CSharp.CSharpCodeProvider()) {
//				
//				System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters ();
//				
//				
//				
//				Type ValueType = typeof(float);//   value is UnityVariable? (value as UnityVariable).ValueType : value.GetType ();
//				
//				
//				compilerParams.GenerateInMemory = true; 
//				
//				var assembyExcuting = Assembly.GetExecutingAssembly ();
//				
//				string assemblyLocationUnity = Assembly.GetAssembly (typeof(ScriptableObject)).Location;
//				
//				string usingString = "using UnityEngine;";
//				
//				if (!(ValueType.IsPrimitive || ValueType == typeof(string))) {
//					string assemblyLocationVarable = Assembly.GetAssembly (ValueType).Location;
//					compilerParams.ReferencedAssemblies.Add (assemblyLocationVarable);
//					
//					if (String.Compare (assemblyLocationUnity, assemblyLocationVarable) != 0) {
//						
//						usingString += "using " + ValueType.Namespace + ";";
//					}
//				}
//				
//				
//				
//				compilerParams.ReferencedAssemblies.Add (assemblyLocationUnity);
//				compilerParams.ReferencedAssemblies.Add (assembyExcuting.Location);
//				
//				
//				
//				
//				var res = foo.CompileAssemblyFromSource (
//					compilerParams, String.Format (
//					
//					" {0}" +
//					
//					"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
//					, usingString, ValueType.ToString ())
//					);
//				
//				
//				
//				
//				if (res.Errors.Count > 0) {
//					
//					foreach (System.CodeDom.Compiler.CompilerError CompErr in res.Errors) {
//						Debug.LogError (
//							"Line number " + CompErr.Line +
//							", Error Number: " + CompErr.ErrorNumber +
//							", '" + CompErr.ErrorText + ";" 
//							);
//					}
//					
//					
//					return null;
//					
//					
//				} else {
//					
//					var type = res.CompiledAssembly.GetType ("ScriptableObjectTemplate");
//					
//					ScriptableObject st = ScriptableObject.CreateInstance (type);
//					
//					type.GetField ("value").SetValue (st, value);
//					
//					
//					
//					
//					
//					return new SerializedObject (st).FindProperty ("value");
//					
//				}
//				
//			}
			
			
			
		}
				

			
				
				
		#endregion



		/// <summary>
		/// Objects public members/submembers of type to display options - values.
		/// </summary>
		/// <param name="object">Object.</param>
		/// <param name="type">Type.</param>
		/// <param name="displayOptions">Display options.</param>
		/// <param name="membersUniquePath">Members unique path.</param>
		public static void ObjectToDisplayOptionsValues (UnityEngine.Object @object,Type type,out GUIContent[] displayOptions,out string[] membersUniquePath)
		{
			
			
			Type target = null;
			List<GUIContent> guiContentList = new List<GUIContent> ();
			List<string> membersUniquePathList = new List<string> ();
			
			MemberInfo memberInfo;
			
			
			
			target = @object.GetType ();
			
			
			
			
			
			List<string> list = new List<string> ();
			
			
			
			
			
			
			MemberInfo[] publicMembers ;
			int numMembers = 0;
			int numSubMembers = 0;
			
			
			
			
			//GET OBJECT NON STATIC PROPERTIES
			publicMembers = target.GetPublicMembersOfType (type,false, true, true);
			
			numMembers = publicMembers.Length;
			
			for (int j = 0; j < numMembers; j++)
			{
				memberInfo=publicMembers [j];
				
				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));
				
				membersUniquePathList.Add (memberInfo.Name+"@"+@object.GetInstanceID());
				
				
			}
			
			//GET properties in COMPONENTS IF GAME OBJECT
			GameObject gameObject = @object as GameObject;
			if (gameObject != null)
			{
				Component currentComponent=null;
				Component[] components = gameObject.GetComponents<Component> ();
				for (int k = 0; k < components.Length; k++)
				{
					currentComponent = components [k];
					Type compType = currentComponent.GetType ();
					string uniqueNameInList = StringUtility.GetIndexNameInList (list, compType.Name);
					list.Add (uniqueNameInList);
					
					
					
					
					//NONSTATIC PROPERTIES
					publicMembers = compType.GetPublicMembersOfType (type, false, true, true);
					for (int m = 0; m < publicMembers.Length; m++)
					{
						memberInfo=publicMembers [m];
						
						guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name));
						
						membersUniquePathList.Add(memberInfo.Name+"@"+currentComponent.GetInstanceID());
					}
					
					
					publicMembers = compType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
					
					MemberInfo[] publicSubMembers=null;
					MemberInfo memberSubCurrent=null;
					numMembers=publicMembers.Length;
					
					for (int m = 0; m < numMembers; m++)
					{
						
						memberInfo=publicMembers [m];
						if(memberInfo.MemberType!=MemberTypes.Property && memberInfo.MemberType!=MemberTypes.Field)
							continue;
						
						publicSubMembers=memberInfo.GetUnderlyingType().GetPublicMembersOfType (type, false, true, true);
						numSubMembers=publicSubMembers.Length;
						
						for(int r=0; r<numSubMembers; r++){
							
							memberSubCurrent=publicSubMembers[r];
							guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name+"."+memberSubCurrent.Name));
							
							membersUniquePathList.Add(memberInfo.Name+"."+memberSubCurrent.Name+"@"+currentComponent.GetInstanceID());
							
						}
						
						
					}
					
				}
			}
			
			
			displayOptions=guiContentList.ToArray();
			membersUniquePath=membersUniquePathList.ToArray();
			
			
			
			
		}//end function


		#region UnityClipboard

		private static UnityClipboard __clipboard;
		
		public static UnityClipboard Clipboard {
			get {
				if (__clipboard == null) {
					
					__clipboard = Resources.Load<UnityClipboard> ("UnityClipboard");
					
					if (__clipboard == null)
						AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<UnityClipboard> (), "Assets/Resources/UnityClipboard.asset");
					
					__clipboard = Resources.Load<UnityClipboard> ("UnityClipboard");
				}
				
				return __clipboard;
			}
		}

		#endregion



				
				/// ////////////////////////////////   MENU EXTENSIONS //////////////////////////////////////
				
		#region RemoveSubAsset
				[MenuItem("Assets/Delete/Remove SubAsset")]
				public static void RemoveSelectedSubAsset ()
				{
						var objectsSelected = Selection.objects;

					foreach (var obj in objectsSelected) {
						if (obj is ScriptableObject && AssetDatabase.IsSubAsset (Selection.activeObject.GetInstanceID ())) {
									
							UnityEngine.Object.DestroyImmediate (Selection.activeObject, true);

							
						}
					}

				}
		#endregion


		#region CreateAssetFromSelected





				[MenuItem("Assets/Create/Asset From Scriptable Object")]
				public static void CreateAssetFromSelected ()
				{
						if (Selection.activeObject != null && Selection.activeObject is MonoScript && ((MonoScript)Selection.activeObject).GetClass ().IsSubclassOf (typeof(ScriptableObject))) 
						CreateAssetFromName (((MonoScript)Selection.activeObject).GetClass ().Name,Selection.activeObject.name);
				}

				public static void CreateAssetFromName (String className,String name="")
				{

							string pathBase = EditorUtility.SaveFilePanel ("Choose save folder", "Assets", name,"asset");
							
							if (!String.IsNullOrEmpty (pathBase)) {
								
								pathBase = pathBase.Remove (0, pathBase.IndexOf ("Assets"));
								
								
								
								
								if (AssetDatabase.LoadAssetAtPath (pathBase, typeof(GameObject))) {
									if (EditorUtility.DisplayDialog ("Are you sure?", 
									                                 "The Assets already exists. Do you want to overwrite it?", 
									                                 "Yes", 
									                                 "No"))

										AssetDatabase.CreateAsset (ScriptableObject.CreateInstance (className), pathBase);
										
								} else
									AssetDatabase.CreateAsset (ScriptableObject.CreateInstance (className), pathBase);
								
							}

			
				}
		#endregion


		#region CreatePrefab
				// Creates a prefab from the selected GameObjects.
				// if the prefab already exists it asks if you want to replace it

				[MenuItem("GameObject/Create/Prefab From Selected")]
				static void CreatePrefab ()
				{
						var objs = Selection.gameObjects;



						string pathBase = EditorUtility.SaveFolderPanel ("Choose save folder", "Assets", "");

						if (!String.IsNullOrEmpty (pathBase)) {

								pathBase = pathBase.Remove (0, pathBase.IndexOf ("Assets")) + Path.DirectorySeparatorChar;

								foreach (var go in objs) {
										String localPath = pathBase + go.name + ".prefab";

										if (AssetDatabase.LoadAssetAtPath (localPath, typeof(GameObject))) {
												if (EditorUtility.DisplayDialog ("Are you sure?", 
						                                "The prefab already exists. Do you want to overwrite it?", 
						                                "Yes", 
						                                "No"))
														CreateNewPrefab (go, localPath);
										} else
												CreateNewPrefab (go, localPath);
								}
						}
				}

				[MenuItem("GameObject/Create/Prefab From Selected", true)]
				static bool ValidateCreatePrefab ()
				{
						return Selection.activeGameObject != null;
				}
		
				public static void CreateNewPrefab (GameObject go, string localPath)
				{
						var prefab = PrefabUtility.CreateEmptyPrefab (localPath);
						PrefabUtility.ReplacePrefab (go, prefab, ReplacePrefabOptions.ConnectToPrefab);
				}


		#endregion

		#region lock/unlock

		const string lockMenuItem = "GameObject/UnityLock/Lock GameObject";
		const string lockRecursivelyMenuItem = "GameObject/UnityLock/Lock GameObject and Children %#l";
		const string unlockMenuItem = "GameObject/UnityLock/Unlock GameObject";
		const string unlockRecursivelyMenuItem = "GameObject/UnityLock/Unlock GameObject and Children %#u";

		private static Texture2D _lockIcon;


		//	// Have we loaded the prefs yet
		//	private static var prefsLoaded : boolean = false;
		//	
		//	// The Preferences
		//	public static var boolPreference : boolean = false;
		//	
		//	// Add preferences section named "My Preferences" to the Preferences Window
		//	@PreferenceItem ("My Preferences")
		//	static function PreferencesGUI () {
		//		// Load the preferences
		//		if (!prefsLoaded) {
		//			boolPreference = EditorPrefs.GetBool ("BoolPreferenceKey", false);
		//			prefsLoaded = true;
		//		}
		//		
		//		// Preferences GUI
		//		boolPreference = EditorGUILayout.Toggle ("Bool Preference", boolPreference);
		//		
		//		// Save the preferences
		//		if (GUI.changed)
		//			EditorPrefs.SetBool ("BoolPreferenceKey", boolPreference);
		//	}


		[PreferenceItem("Lock")]
		static void LockPreferencesGUI(){






			//Preferences GUI

			EditorGUILayout.BeginVertical();

			EditorGUI.BeginChangeCheck ();
			bool drawIcon = ShowLockIconPrefsBoolOption(showLockIconPrefKey, "Show lock icon in hierarchy");

			if (EditorGUI.EndChangeCheck ()) {
								if (drawIcon) {
										EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
								} else {
										EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
								}
			
								EditorApplication.RepaintHierarchyWindow ();
						}

			EditorGUILayout.HelpBox(
				"When enabled a small lock icon will appear in the hierarchy view for all locked objects.",
				MessageType.None);
			
			EditorGUILayout.Space();
			
			bool wasSelectionDisabled = EditorPrefs.GetBool(lockMultiSelectionPrefKey);
			bool isSelectionDisabled = ShowLockIconPrefsBoolOption(lockMultiSelectionPrefKey, "Disable selecting locked objects");
			EditorGUILayout.HelpBox(
				"When enabled locked objects will not be selectable in the scene view with a left click. Some objects can still be selected by using a selection rectangle; it doesn't appear to be possible to prevent this.\n\nObjects represented only with gizmos will not be drawn as gizmos aren't rendered when selection is disabled.",
				MessageType.None);
			
			if (wasSelectionDisabled != isSelectionDisabled)
			{
				ToggleSelectionOfLockedObjects(isSelectionDisabled);
			}
			
			EditorGUILayout.Space();
			
			ShowLockIconPrefsBoolOption(addLockUndoRedoPrefKey, "Support undo/redo for lock and unlock");
			EditorGUILayout.HelpBox(
				"When enabled the lock and unlock operations will be properly inserted into the undo stack just like any other action.\n\nIf this is disabled the Undo button will never lock or unlock an object. This can cause other operations to silently fail, such as trying to undo a translation on a locked object.",
				MessageType.None);
			
			EditorGUILayout.EndVertical();


		}


		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceID);
			if (obj && (obj.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
			{
				if (!_lockIcon)
				{
					_lockIcon = AssetDatabase.LoadAssetAtPath("Assets" + Directory.GetFiles(Application.dataPath, "LockHierarchyIcon.png", SearchOption.AllDirectories)[0].Substring(Application.dataPath.Length).Replace('\\', '/'), typeof(Texture2D)) as Texture2D;
				}
				
				GUI.Box(new Rect(selectionRect.xMax - 16f, selectionRect.center.y - (16f / 2f), 16f, 16f), _lockIcon, GUIStyle.none);
			}
		}

		static bool ShowLockIconPrefsBoolOption(string key, string name)
		{
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField(name, GUILayout.ExpandWidth(true));
			bool oldValue = EditorPrefs.GetBool(key);
			bool newValue = EditorGUILayout.Toggle(oldValue, GUILayout.Width(20));
			if (newValue != oldValue)
			{
				EditorPrefs.SetBool(key, newValue);
			}
			
			EditorGUILayout.EndHorizontal();
			
			return newValue;
		}



		public static void ToggleSelectionOfLockedObjects(bool disableSelection)
		{
			foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if ((go.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
				{
					foreach (Component comp in go.GetComponents(typeof(Component)))
					{
						if (!(comp is Transform))
						{
							if (disableSelection)
							{
								comp.hideFlags |= HideFlags.NotEditable;
								comp.hideFlags |= HideFlags.HideInHierarchy;
							}
							else
							{
								comp.hideFlags &= ~HideFlags.NotEditable;
								comp.hideFlags &= ~HideFlags.HideInHierarchy;
							}
						}
					}
					
					EditorUtility.SetDirty(go);
				}
			}
		}


	/// <summary>
	/// Lock the specified gameObject and includeChildren.
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="includeChildren">If set to <c>true</c> include children.</param>
		public static void Lock(GameObject gameObject, bool includeChildren=false)
		{
			if (EditorPrefs.GetBool(addLockUndoRedoPrefKey))
			{
				Undo.RecordObject(gameObject,"Lock Object");
			}
			gameObject.hideFlags |= HideFlags.NotEditable;
			foreach (Component comp in gameObject.GetComponents(typeof(Component)))
			{
				if (!(comp is Transform))
				{
					if (EditorPrefs.GetBool(lockMultiSelectionPrefKey))
					{
						comp.hideFlags |= HideFlags.NotEditable;
						comp.hideFlags |= HideFlags.HideInHierarchy;
					}
				}
			}
			EditorUtility.SetDirty(gameObject);


			if (includeChildren) {
				foreach (Transform childTransform in gameObject.transform)
				{
					Lock(childTransform.gameObject,true);
				}


			}
		}

		/// <summary>
		/// Lock the specified gameObjects and includeChildren.
		/// </summary>
		/// <param name="gameObjects">Game objects.</param>
		/// <param name="includeChildren">If set to <c>true</c> include children.</param>
		public static void Lock(GameObject[] gameObjects,bool includeChildren=false)
		{
			foreach (var go in gameObjects)
			{
				Lock(go,includeChildren);
			}
		}


		public static void Unlock(GameObject gameObject,bool includeChildren=false)
		{
			if (EditorPrefs.GetBool(addLockUndoRedoPrefKey))
			{
				Undo.RecordObject(gameObject,"Unlock Object");
			}
			gameObject.hideFlags &= ~HideFlags.NotEditable;
			foreach (Component comp in gameObject.GetComponents(typeof(Component)))
			{
				if (!(comp is Transform))
				{
					// Don't check pref key; no harm in removing flags that aren't there
					comp.hideFlags &= ~HideFlags.NotEditable;
					comp.hideFlags &= ~HideFlags.HideInHierarchy;
				}
			}
			EditorUtility.SetDirty(gameObject);


			if (includeChildren) {
				foreach (Transform childTransform in gameObject.transform)
				{
					Unlock(childTransform.gameObject,true);
				}
				
				
			}
		}


		/// <summary>
		/// Unlock the specified gameObjects and includeChildren.
		/// </summary>
		/// <param name="gameObjects">Game objects.</param>
		/// <param name="includeChildren">If set to <c>true</c> include children.</param>
		public static void Unlock(GameObject[] gameObjects,bool includeChildren=false)
		{
			foreach (var go in gameObjects)
			{
				Unlock(go,includeChildren);
			}
		}


		
		[MenuItem(lockMenuItem)]
		static void LockSelection()
		{
			Lock (Selection.gameObjects);
		}
		
		[MenuItem(lockMenuItem, true)]
		static bool CanLock()
		{
			return Selection.gameObjects.Length > 0;
		}
		
		[MenuItem(lockRecursivelyMenuItem)]
		static void LockSelectionRecursively()
		{
			Lock (Selection.gameObjects,true);
		}
		
		[MenuItem(lockRecursivelyMenuItem, true)]
		static bool CanLockRecursively()
		{
			return Selection.gameObjects.Length > 0;
		}
		
		[MenuItem(unlockMenuItem)]
		static void UnlockSelection()
		{
			Unlock(Selection.gameObjects);
		}
		
		[MenuItem(unlockMenuItem, true)]
		static bool CanUnlock()
		{
			return Selection.gameObjects.Length > 0;
		}
		
		[MenuItem(unlockRecursivelyMenuItem)]
		static void UnlockSelectionRecursively()
		{
			Unlock (Selection.gameObjects, true);
		}
		
		[MenuItem(unlockRecursivelyMenuItem, true)]
		static bool CanUnlockRecursively()
		{
			return Selection.gameObjects.Length > 0;
		}








#endregion




	






		}
}

