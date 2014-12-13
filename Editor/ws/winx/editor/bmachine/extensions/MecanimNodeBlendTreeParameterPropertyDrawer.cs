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
		[CustomNodePropertyDrawer (typeof(MecanimNodeBlendParameterAttribute))]
		public class MecanimNodeBlendTreeParameterPropertyDrawer : NodePropertyDrawer
		{

				MecanimStateInfo previousSelectAnimaInfo;
				GUIContent label = new GUIContent ();
				Variable blackBoardVariableX;
				Variable blackBoardVariableY;
				string[] blendParams;
				GUIContent[] displayOptions;
				string caption = "Blend Parameter";
				List<Variable> blackboardFloatVariables;
				MecanimNodeBlendParameterAttribute blendParamAttribute;
				int blackBoardBindingID;
				

				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{


						MecanimNode mecanimNode = node as MecanimNode;


						blackBoardBindingID = (int)property.value;

						blendParams = mecanimNode.animaStateInfoSelected.blendParamsNames;


			
						if (blendParams != null && blendParams.Length > (int)((MecanimNodeBlendParameterAttribute)attribute).axis) {



								label.text = this.caption;
								EditorGUILayout.LabelField (label);

								
					
				


								if (previousSelectAnimaInfo != mecanimNode.animaStateInfoSelected) {
										blackboardFloatVariables = mecanimNode.blackboard.GetVariables (typeof(FloatVar));
										
										//concat global and local blackboards
										blackboardFloatVariables.AddRange (GlobalBlackboard.Instance.GetVariables (typeof(FloatVar)));
										
										displayOptions = blackboardFloatVariables.Select (x => new GUIContent (x.name)).ToArray ();
				
								}
				
				
							
								EditorGUILayout.BeginHorizontal ();

								
								label.text = blendParams [(int)((MecanimNodeBlendParameterAttribute)attribute).axis];

								Variable variable = blackboardFloatVariables.Find ((Item) => {
										return Item.id == blackBoardBindingID;});
										
								variable = EditorGUILayoutEx.CustomObjectPopup (label, variable, displayOptions, blackboardFloatVariables);			

							

								EditorGUILayout.EndHorizontal ();


								property.value = variable.id;	


												
						}




						previousSelectAnimaInfo = mecanimNode.animaStateInfoSelected;

						

		
						property.ApplyModifiedValue ();

				}
		

		}
}
