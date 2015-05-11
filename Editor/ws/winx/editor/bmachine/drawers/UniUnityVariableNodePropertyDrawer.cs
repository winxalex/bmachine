using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.attributes;
using ws.winx.unity;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using BehaviourMachineEditor;
using System.Linq;
using ws.winx.bmachine;

namespace ws.winx.editor.bmachine.drawers
{
	[CustomNodePropertyDrawer(typeof(UniUnityVariablePropertyAttribute))]
	public class UniUnityVariableNodePropertyDrawer:NodePropertyDrawer
	{



		Type typeSelected;


		public override void OnGUI (SerializedNodeProperty property, BehaviourMachine.ActionNode node, GUIContent guiContent)
		{

			
			Rect position=	GUILayoutUtility.GetRect(Screen.width-32f,32f);



			Rect typePos = new Rect (position.x, position.y, 80, position.height);
			position.xMin = typePos.xMax;
			Rect varPos = new Rect (position.x, position.y, position.width, position.height);


			Type type;

			GUIContent[] displayOptionsTypes;
			Type[] types;

			UniUnityVariablePropertyAttribute attributeUni = attribute as UniUnityVariablePropertyAttribute;


			if (property.value == null)
				type = EditorGUILayoutEx.unityTypes [0];
			else
				type = ((UnityVariable)property.value).ValueType;

			//blackboard vars LOCAL
			BlackboardCustom blackboard = node.blackboard as BlackboardCustom;
			
			List<UnityVariable> blackboardVariablesLocalList = blackboard.GetVariableBy (type);
			
			
			List<GUIContent> displayOptionsVariablesLocal=	 blackboardVariablesLocalList.Select ((item) => new GUIContent ("Local/" + item.name)).ToList();


			//blackboard vars GLOBAL

			
			if (attributeUni.typesCustom != null) {

				GUIContent[] displayOptionsCustom=attributeUni.typesCustom.Select((itm)=>new GUIContent(itm.Name)).ToArray();

				if(attributeUni.only){
					types=attributeUni.typesCustom;
					displayOptionsTypes=displayOptionsCustom;
				}else{

					types=attributeUni.typesCustom.Concat<Type>(EditorGUILayoutEx.unityTypes).ToArray();

					displayOptionsTypes = displayOptionsCustom.Concat<GUIContent>(EditorGUILayoutEx.unityTypesDisplayOptions).ToArray();

				}


		    }else{
			
					displayOptionsTypes=EditorGUILayoutEx.unityTypesDisplayOptions;
					types=EditorGUILayoutEx.unityTypes;



			}



						



			
			//String name = attributeUni.name;

			//create types selection popup
			typeSelected = EditorGUILayoutEx.CustomObjectPopup<Type> (null, type,displayOptionsTypes , types,null,null,null,null,typePos);
			
								//if change of type create new variable
								if (typeSelected != type && !typeSelected.IsSubclassOf (type) /*&& type!=typeof(UnityEngine.Object)*/) {
										
									property.value = UnityVariable.CreateInstanceOf(typeSelected);
								}
					

			property.value=EditorGUILayoutEx.UnityVariablePopup(null,property.value as UnityVariable,typeSelected,displayOptionsVariablesLocal,blackboardVariablesLocalList,varPos);




			property.serializedNode.ApplyModifiedProperties();

		}	
	}
}

