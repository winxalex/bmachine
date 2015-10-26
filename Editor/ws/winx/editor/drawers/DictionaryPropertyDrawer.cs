using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ws.winx.unity.attributes;
using UnityEditorInternal;
using System.Linq;
using System;
using ws.winx.csharp.extensions;

namespace ws.winx.editor.drawers
{

	//[CustomPropertyDrawer (typeof(DictionaryPropertyAttribute))]
	[CustomPropertyDrawer (typeof(Dictionary<,>))]
	public class DictionaryPropertyDrawer:PropertyDrawer
	{
		ReorderableList __variablesReordableList;


		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{


			return Math.Max (1, property.arraySize) * 43 + 36f + 2f + 16f;//16f label height , 2f separator
			//return base.GetPropertyHeight (property, label);
		}

		void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
		{

			SerializedProperty valueSerializedProperty = __variablesReordableList.serializedProperty;
			SerializedProperty keysSerializedProperty =valueSerializedProperty.serializedObject.FindProperty ("keys");

			float halfWidth = rect.width*0.5f;

			rect.xMax =rect.xMin + halfWidth-2f;



			//EditorGUI.BeginChangeCheck ();
			 EditorGUI.PropertyField (rect,keysSerializedProperty.GetArrayElementAtIndex(index),GUIContent.none);
			rect.xMin = rect.xMax+2f;
			rect.width = halfWidth - 2f;

		
			EditorGUI.PropertyField (rect, valueSerializedProperty.GetArrayElementAtIndex(index),GUIContent.none);




		}

		void onAddElement(ReorderableList list){
			
			SerializedProperty valueSerializedProperty = list.serializedProperty;
			SerializedProperty keysSerializedProperty = valueSerializedProperty.serializedObject.FindProperty ("keys");

			valueSerializedProperty.arraySize++;
			keysSerializedProperty.arraySize++;

			
		}

		void onRemoveElement(ReorderableList list){
			SerializedProperty valueSerializedProperty = list.serializedProperty;
			SerializedProperty keysSerializedProperty = valueSerializedProperty.serializedObject.FindProperty ("keys");
			valueSerializedProperty.DeleteArrayElementAtIndex (list.index);
			keysSerializedProperty.DeleteArrayElementAtIndex (list.index);

		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if(label!=GUIContent.none)
			EditorGUI.LabelField (position,label);




			if (__variablesReordableList == null) {

			
				
				//SerializedProperty keysList= property.FindPropertyRelative("Keys");
				__variablesReordableList = new ReorderableList (property.serializedObject, property,
			                                                false, false, true, true);
				__variablesReordableList.drawElementCallback = onDrawElement;
				__variablesReordableList.onAddCallback = onAddElement;
				__variablesReordableList.onRemoveCallback=onRemoveElement;

			} else
				__variablesReordableList.serializedProperty = property;



			if (__variablesReordableList != null) {
				//position.width
				position.y+=16f;
				__variablesReordableList.DoList (position);

			}



		}	
	}
}
	