using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.attributes;
using ws.winx.unity;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using UnityEngine.Events;
using System.Linq;
using ws.winx.unity.ai.behaviours;


namespace ws.winx.editor.drawers
{
	[CustomPropertyDrawer (typeof(UnityVariablePropertyAttribute))]
	//[CustomPropertyDrawer (typeof(UnityVariable))]
	public class UnityVariablePropertyDrawer:PropertyDrawer
	{

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			UnityVariable variable = property.objectReferenceValue as UnityVariable;

			if (variable != null && variable.serializedProperty != null)
			{

				SerializedProperty variableSerializadProperty = variable.serializedProperty as SerializedProperty;

				if (variable.ValueType == typeof(UnityEvent)) {
					SerializedProperty elements = variableSerializadProperty.FindPropertyRelative ("m_PersistentCalls.m_Calls");
					

					return Math.Max (1, elements.arraySize) * 43 + 36f + 2f + EditorGUIUtility.singleLineHeight;//16f label height , 2f separator
				}
				else if((variable.ValueType.IsGenericType && variable.ValueType.GetGenericTypeDefinition()==typeof(List<>)
				        ) || variable.ValueType.IsArray){

					if(variableSerializadProperty.isExpanded)
						return EditorGUIUtility.singleLineHeight + (variableSerializadProperty.arraySize+1) * EditorGUIUtility.singleLineHeight+2f;//label + (size label) + list elements
				}else if (variable.ValueType.IsGenericType && variable.ValueType.GetGenericTypeDefinition()==typeof(Dictionary<,>)) {

					
					return Math.Max (1, variableSerializadProperty.arraySize) * 43 + 36f + 2f + EditorGUIUtility.singleLineHeight;//16f label height , 2f separator
				}

			

			
			}

			return base.GetPropertyHeight (property, label);
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
		

			Type selectedType = ((UnityVariablePropertyAttribute)attribute).variableType;

			UnityVariablePropertyAttribute att = (UnityVariablePropertyAttribute)attribute;

			Blackboard blackboard = GameObject.FindObjectsOfType<Blackboard> ().FirstOrDefault(itm=> itm.gameObject.name==att.blackboardName);



			List<UnityVariable> blackboardLocalList=null;
			List<GUIContent> displayOptionsList=null;

			if(blackboard!=null){
			blackboardLocalList = blackboard.GetVariableBy (att.variableType);
			
			
			displayOptionsList = blackboardLocalList.Select (item => new GUIContent (att.blackboardName + "/" + item.name)).ToList ();
			}

			
			property.objectReferenceValue=EditorGUILayoutEx.UnityVariablePopup(label,property.objectReferenceValue as UnityVariable,selectedType,displayOptionsList,blackboardLocalList,position);
			property.serializedObject.ApplyModifiedProperties();

		}	
	}
}

