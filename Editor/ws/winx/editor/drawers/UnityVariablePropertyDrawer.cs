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

			Type type;
			Type selectedType = ((UnityVariablePropertyAttribute)attribute).variableType;

			//create types selection popup
			//if ( selectedType == typeof(System.Object)) {
			if (property.objectReferenceValue == null)
				type = selectedType;
			else
				type = ((UnityVariable)property.objectReferenceValue).ValueType;


			
			//if change of type create new variable
			if (selectedType != type && !selectedType.IsSubclassOf (type) && type!=typeof(UnityEngine.Object)) {
				
				property.objectReferenceValue = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
				
				if(selectedType== typeof(string))
					((UnityVariable)property.objectReferenceValue).Value=String.Empty;
				else
					((UnityVariable)property.objectReferenceValue).Value=FormatterServices.GetUninitializedObject (selectedType);
			}
			
			property.objectReferenceValue=EditorGUILayoutEx.UnityVariablePopup(null,property.objectReferenceValue as UnityVariable,selectedType,new List<GUIContent>(),new List<UnityVariable>());
			property.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.EndHorizontal ();
		}	
	}
}

