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


		Type typeSelected;


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

			typeSelected = EditorGUILayoutEx.CustomObjectPopup<Type> (null, typeSelected,EditorGUILayoutEx.unityTypesDisplayOptions , EditorGUILayoutEx.unityTypes,null,null,null,null,typePos);
			
								//if change of type create new variable
								if (typeSelected != type && !typeSelected.IsSubclassOf (type) /*&& type!=typeof(UnityEngine.Object)*/) {
										
										property.objectReferenceValue = UnityVariable.CreateInstanceOf(typeSelected);

//										if(typeSelected== typeof(string))
//											((UnityVariable)property.objectReferenceValue).Value=String.Empty;
//										else
//											((UnityVariable)property.objectReferenceValue).Value=UnityVariable.Default(type);
								}
					//	} 

			property.objectReferenceValue=EditorGUILayoutEx.UnityVariablePopup(null,property.objectReferenceValue as UnityVariable,typeSelected,new List<GUIContent>(),new List<UnityVariable>(),varPos);
			((UnityVariable)property.objectReferenceValue).drawer = this;
			((UnityVariable)property.objectReferenceValue).name = name;
			property.serializedObject.ApplyModifiedProperties();

		}	
	}
}

