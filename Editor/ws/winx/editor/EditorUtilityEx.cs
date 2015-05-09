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

namespace ws.winx.editor
{
		public class EditorUtilityEx
		{
				public delegate Rect SwitchDrawerDelegate (Rect rect,UnityVariable variable);


				//into static editor Utilityor somthing
				static List<Func<Type,SwitchDrawerDelegate>> _drawers;
				static Func<Type,SwitchDrawerDelegate> _defaultSwitchDrawer;
		
				public static void AddCustomDrawer<T> (SwitchDrawerDelegate drawer)
				{
						if (_drawers == null)
								_drawers = new List<Func<Type, SwitchDrawerDelegate>> ();

						_drawers.Add (SwitchUtility.CaseIsClassOf<T,SwitchDrawerDelegate> (drawer));
				}
		
				public static Func<Type, SwitchDrawerDelegate> GetDefaultSwitchDrawer ()
				{
						if (_defaultSwitchDrawer == null) {

								if (_drawers == null)
										_drawers = new List<Func<Type, SwitchDrawerDelegate>> ();

								_drawers.AddRange (
					
					new Func<Type, SwitchDrawerDelegate>[]{
					
					SwitchUtility.CaseIsClassOf<float,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawFloatVar),
					SwitchUtility.CaseIsClassOf<int,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawIntVar),
					SwitchUtility.CaseIsClassOf<bool,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawBoolVar),
					SwitchUtility.CaseIsClassOf<string,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawStringVar),
					SwitchUtility.CaseIsClassOf<Quaternion,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawQuaternionVar),
					SwitchUtility.CaseIsClassOf<Vector3,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawVector3Var),
					SwitchUtility.CaseIsClassOf<Rect,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawRectVar),
					SwitchUtility.CaseIsClassOf<Color,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawColorVar),
					SwitchUtility.CaseIsClassOf<Material,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
					SwitchUtility.CaseIsClassOf<UnityEngine.Texture,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
					SwitchUtility.CaseIsClassOf<GameObject,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
					SwitchUtility.CaseIsClassOf<AnimationCurve,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawCurveVar),
					SwitchUtility.CaseIsClassOf<AnimationClip,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
					SwitchUtility.CaseIsClassOf<UnityEngine.Object,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
				}
								);
				
				
								_defaultSwitchDrawer = SwitchUtility.Switch (_drawers);
				
				
						}
			
			
						return _defaultSwitchDrawer;
				}



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

												

																		if (typeProperty != null && typeProperty.BaseType != typeof(PropertyAttribute) && !EditorUtilityEx.__drawers.ContainsKey (typeProperty)) {
																				EditorUtilityEx.__drawers.Add (typeProperty, Activator.CreateInstance (typeDrawer) as PropertyDrawer);
												
//																Debug.Log("  "+typeProperty.Name+" "+typeDrawer.Name+" "+typeProperty.BaseType);
																		}
																}
														}
												}//attributes
								}//types in dll
						}

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




		#region SerializedObject

			
				public static SerializedObject Serialize (object value)
				{

						using (Microsoft.CSharp.CSharpCodeProvider foo = 
				      new Microsoft.CSharp.CSharpCodeProvider()) {
				
								System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters ();

								Type ValueType = value.GetType ();
				
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
				
				
				
				
								var res = foo.CompileAssemblyFromSource (
					compilerParams, String.Format (
					
					" {0}" +
					
										"public class ScriptableObjectTemplate:ScriptableObject {{ public {1} value;}}"
					, usingString, ValueType.ToString ())
								);
				
				
				
				
								if (res.Errors.Count > 0) {
					
										foreach (System.CodeDom.Compiler.CompilerError CompErr in res.Errors) {
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
					
										type.GetField ("value").SetValue (st, value);
					
					
					
										return new SerializedObject (st);
					

					
								}
				
						}
			
			
			
				}
		#endregion

				
				/// ////////////////////////////////   MENU EXTENSIONS /////////////////////////
				
		#region RemoveSubAsset
				[MenuItem("Assets/Delete/Remove SubAsset")]
				public static void RemoveSelectedSubAsset ()
				{
						if (Selection.activeObject is ScriptableObject && AssetDatabase.IsSubAsset (Selection.activeObject.GetInstanceID ())) {
								String path = AssetDatabase.GetAssetPath (Selection.activeObject);			
								UnityEngine.Object.DestroyImmediate (Selection.activeObject, true);

								AssetDatabase.ImportAsset (path);
						}

				}
		#endregion


		#region CreateAssetFromSelected
				[MenuItem("Assets/Create/Asset From Selected")]
				public static void CreateAssetFromSelected ()
				{
						if (Selection.activeObject != null && Selection.activeObject is MonoScript && ((MonoScript)Selection.activeObject).GetClass ().IsSubclassOf (typeof(ScriptableObject))) 
								CreateAssetFromName (Selection.activeObject.name);
				}

				public static void CreateAssetFromName (String name)
				{

						if (File.Exists (Path.Combine (Path.Combine (Application.dataPath, "Resources"), name + ".asset"))) {
							
								if (EditorUtility.DisplayDialog ("UnityClipboard Asset Exists!",
							                                 "Are you sure you overwrite?", "Yes", "Cancel")) {
										AssetDatabase.CreateAsset (ScriptableObject.CreateInstance (Selection.activeObject.name), "Assets/Resources/" + name + ".asset");
								}
						} else {
				
								AssetDatabase.CreateAsset (ScriptableObject.CreateInstance (Selection.activeObject.name), "Assets/Resources/" + name + ".asset");
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
														CreateNew (go, localPath);
										} else
												CreateNew (go, localPath);
								}
						}
				}

				[MenuItem("GameObject/Create/ Prefab From Selected", true)]
				static bool ValidateCreatePrefab ()
				{
						return Selection.activeGameObject != null;
				}
		
				static void CreateNew (GameObject obj, string localPath)
				{
						var prefab = PrefabUtility.CreateEmptyPrefab (localPath);
						PrefabUtility.ReplacePrefab (obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
				}


		#endregion

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




	






		}
}

