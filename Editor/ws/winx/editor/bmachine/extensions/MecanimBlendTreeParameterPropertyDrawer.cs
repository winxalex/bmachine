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

namespace ws.winx.editor.bmachine.extensions
{
		[CustomNodePropertyDrawer (typeof(MecanimBlendParameterAttribute))]
		public class MecanimBlendTreeParameterPropertyDrawer : NodePropertyDrawer
		{

				MecanimStateInfo previousSelectAnimaInfo;
				GUIContent label = new GUIContent ();
				Variable blackBoardVariableX;
				Variable blackBoardVariableY;
				string[] blendParams;
				GUIContent[] displayOptions;
				string caption = "Blend Parameter";
				List<Variable> blackboardFloatVariables;
				MecanimBlendParameterAttribute blendParamAttribute;
				int blackBoardBindingID;
				

				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{


						MecanimNode mecanimNode = node as MecanimNode;


						blackBoardBindingID = (int)property.value;

						blendParams = mecanimNode.selectedAnimaStateInfo.blendParamsNames;


			
						if (blendParams != null && blendParams.Length > (int)((MecanimBlendParameterAttribute)attribute).axis) {



								label.text = this.caption;
								EditorGUILayout.LabelField (label);

								
					
				


								if (previousSelectAnimaInfo != mecanimNode.selectedAnimaStateInfo) {
										blackboardFloatVariables = mecanimNode.blackboard.GetVariables (typeof(FloatVar));
										
										//concat global and local blackboards
										blackboardFloatVariables.AddRange (GlobalBlackboard.Instance.GetVariables (typeof(FloatVar)));
										
										displayOptions = blackboardFloatVariables.Select (x => new GUIContent (x.name)).ToArray ();
				
								}
				
				
							
								EditorGUILayout.BeginHorizontal ();

								
								label.text = blendParams [(int)((MecanimBlendParameterAttribute)attribute).axis];

								Variable variable = blackboardFloatVariables.Find ((Item) => {
										return Item.id == blackBoardBindingID;});
										
								variable = EditorGUILayoutEx.CustomObjectPopup (label, variable, displayOptions, blackboardFloatVariables);			

							

								EditorGUILayout.EndHorizontal ();


								property.value = variable.id;	


												
						}




						previousSelectAnimaInfo = mecanimNode.selectedAnimaStateInfo;

						

		
						property.ApplyModifiedValue ();

				}
		

		}
}
