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

	public class UniUnityVariablePropertyDrawer:PropertyDrawer
	{



		Type selectedType;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{


			Rect typePos = new Rect (position.x, position.y, 80, position.height);
			position.xMin = typePos.xMax;
			Rect varPos = new Rect (position.x, position.y, position.width, position.height);


			Type type;

						//create types selection popup
						//if ( selectedType == typeof(System.Object)) {
			if (property.objectReferenceValue == null)
				type = EditorGUILayoutEx.unityTypes [0];
			else
				type = ((UnityVariable)property.objectReferenceValue).ValueType;

			
			String name = ((UnityVariable)property.objectReferenceValue).name;

			selectedType = EditorGUILayoutEx.CustomObjectPopup<Type> (null, selectedType,EditorGUILayoutEx.unityTypesDisplayOptions , EditorGUILayoutEx.unityTypes,null,null,null,null,typePos);
			
								//if change of type create new variable
								if (selectedType != type && !selectedType.IsSubclassOf (type) && type!=typeof(UnityEngine.Object)) {
										
										property.objectReferenceValue = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();

										if(selectedType== typeof(string))
											((UnityVariable)property.objectReferenceValue).Value=String.Empty;
										else
											((UnityVariable)property.objectReferenceValue).Value=FormatterServices.GetUninitializedObject (selectedType);
								}
					//	} 

			property.objectReferenceValue=EditorGUILayoutEx.UnityVariablePopup(null,property.objectReferenceValue as UnityVariable,selectedType,new List<GUIContent>(),new List<UnityVariable>(),varPos);
			((UnityVariable)property.objectReferenceValue).drawer = this;
			((UnityVariable)property.objectReferenceValue).name = name;
			property.serializedObject.ApplyModifiedProperties();

		}	
	}
}

