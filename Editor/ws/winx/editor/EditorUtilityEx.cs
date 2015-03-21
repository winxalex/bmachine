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
								for (int i = 0; i < derivedTypes.Length; i++) {
										typeDrawer = derivedTypes [i];

										CustomPropertyDrawer attribute = AttributeUtility.GetAttribute<CustomPropertyDrawer> (typeDrawer, false);



										if (attribute != null) {
												Type typeProperty = (Type)attribute.GetType ().GetField ("m_Type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (attribute);



												if (!EditorUtilityEx.__drawers.ContainsKey (typeProperty)) {
														EditorUtilityEx.__drawers.Add (typeProperty, Activator.CreateInstance (typeDrawer) as PropertyDrawer);
												}
										}
								}
						}

						EditorUtilityEx.__drawers.TryGetValue (type, out drawer);
						if (drawer != null) {
								return  drawer;

						}

						EditorUtilityEx.__drawers.TryGetValue (type.BaseType, out drawer);
						if (drawer != null) {
								return drawer;
						
						}
			
						if (__drawerDefault == null)
								__drawerDefault = new UnityDefaultPropertyDrawer ();


						return __drawerDefault;
				}
		






















		}
}

