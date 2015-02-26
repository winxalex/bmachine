using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using ws.winx.csharp.utilities;
using System;
using ws.winx.unity;
using ws.winx.editor.extensions;

namespace ws.winx.editor{
	public class EditorUtilityEx
	{
		public delegate Rect SwitchDrawerDelegate (Rect rect,UnityVariable variable);


		//into static editor Utilityor somthing
		static List<Func<Type,SwitchDrawerDelegate>> _drawers;
		static Func<Type,SwitchDrawerDelegate> _defaultSwitchDrawer;
		
		public static void AddCustomDrawer (SwitchDrawerDelegate drawer)
		{
			_drawers.Add (SwitchUtility.CaseIsClassOf<float,SwitchDrawerDelegate> (drawer));
		}
		
		public static Func<Type, SwitchDrawerDelegate> GetDefaultSwitchDrawer ()
		{
			if (_defaultSwitchDrawer == null) {
				_drawers = new List<Func<Type,SwitchDrawerDelegate>> (
					
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

	}
}

