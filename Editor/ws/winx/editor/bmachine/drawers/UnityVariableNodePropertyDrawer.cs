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
		[CustomNodePropertyDrawer (typeof(UnityVariablePropertyAttribute))]
		public class UnityVariableNodePropertyDrawer : NodePropertyDrawer
		{


		//make Unity crash
//				public new UnityVariablePropertyAttribute attribute { 
//						get{ return (UnityVariablePropertyAttribute)attribute;}
//				}


				private void OnEnable() {

						
				}
				


				
		
		

		
				//
				// Methods
				//






		    


				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{


					UnityVariablePropertyAttribute att=(UnityVariablePropertyAttribute)attribute;

					BlackboardCustom blackboard=node.blackboard as BlackboardCustom;

					List<UnityVariable> blackboardLocalList = blackboard.GetVariableBy (att.VariableType);

							
					List<GUIContent> displayOptionsList=blackboardLocalList.Select ((item) => new GUIContent ("Local/"+item.name)).ToList();
								
								
								
								
					property.value=EditorGUILayoutEx.UnityVariablePopup(guiContent,property.value as UnityVariable,att.VariableType,displayOptionsList,blackboardLocalList);
								
	

				}
		

		}
}
