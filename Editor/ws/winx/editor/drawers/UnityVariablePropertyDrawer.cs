using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.attributes;
using ws.winx.unity;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace ws.winx.editor.drawers
{
	[CustomPropertyDrawer (typeof(UnityVariablePropertyAttribute))]
	//[CustomPropertyDrawer (typeof(UnityVariable))]
	public class UnityVariablePropertyDrawer:PropertyDrawer
	{



		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (label,new GUILayoutOption[]{GUILayout.MaxWidth(80)});

		 

			property.objectReferenceValue=EditorGUILayoutEx.UnityVariablePopup(null,property.objectReferenceValue as UnityVariable,((UnityVariablePropertyAttribute)attribute).variableType,new List<GUIContent>(),new List<UnityVariable>());
			property.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal ();
		}	
	}
}

