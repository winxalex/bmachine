using BehaviourMachine;
using System;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using BehaviourMachineEditor;
using UnityEditorInternal;
using System.Linq;
using ws.winx.unity;
using ws.winx.unity.attributes;
using ws.winx.bmachine;

namespace ws.winx.editor.bmachine.drawers
{
		[CustomPropertyDrawer (typeof(UnityVariablePropertyAttribute))]
		public class UnityVariablePropertyDrawer : PropertyDrawer
		{



				public new UnityVariablePropertyAttribute attribute { 
						get{ return (UnityVariablePropertyAttribute)attribute;}
				}


				private void OnEnable() {

						
				}
				


				
		
		

		
				//
				// Methods
				//





		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{

					
//						List<UnityVariable> blackboardLocalList=((BlackboardCustom)node.blackboard).GetVariableBy (attribute.type);
//
//			List<GUIContent> displayOptionsList=blackboardLocalList.Select ((item) => new GUIContent ("Local/"+item.name)).ToList();
//						
//						
//						
//						
//			property.value=EditorGUILayoutEx.UnityVariablePopup(new GUIContent("Var:"),property.value as UnityVariable,typeof(float),displayOptionsList,blackboardLocalList);
//						

						

		
						

				}
		

		}
}
