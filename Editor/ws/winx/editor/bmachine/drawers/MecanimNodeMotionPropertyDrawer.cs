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

namespace ws.winx.editor.bmachine.drawers
{
		[CustomNodePropertyDrawer (typeof(MecanimNodeMotionPropertyAttribute))]
		public class MecanimNodeMotionPropertyDrawer : NodePropertyDrawer
		{

				
				GUIContent label = new GUIContent ();
				GUIContent[] displayOptions;
				string caption = "Motion";
				List<Variable> blackboardMotionVariables;
				//Type MotionType = typeof(UnityEngine.Motion);
				int blackBoardBindingID;
				//UnityEngine.Motion motion;
		
				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{


						MecanimNode mecanimNode = node as MecanimNode;


						
						//motion = (UnityEngine.Motion)property.value;

						label.text = this.caption;
								
					

						ObjectVar[] objectVars = mecanimNode.blackboard.objectVars;
						int len = objectVars.Length;
						int inx;
						ObjectVar currentObjectVar;
						blackboardMotionVariables = new List<Variable> ();
						List<GUIContent> displayOptionsList = new List<GUIContent> ();
						for (inx=0; inx < len; inx++) {
								currentObjectVar = objectVars [inx];
								if (currentObjectVar.Value is UnityEngine.Motion) {
										blackboardMotionVariables.Add (currentObjectVar);
										displayOptionsList.Add (new GUIContent (currentObjectVar.name));
								}
				
						}

						objectVars = GlobalBlackboard.Instance.objectVars;
						


						for (inx = 0; inx < len; inx++) {
								currentObjectVar = objectVars [inx];
								if (currentObjectVar.Value is UnityEngine.Motion) {
										blackboardMotionVariables.Add (currentObjectVar);
										displayOptionsList.Add (new GUIContent ("Global/" + currentObjectVar.name));
								}
						}

			

						//displayOptions = blackboardMotionVariables.Select (x => new GUIContent (x.name)).ToArray ();
						displayOptions = displayOptionsList.ToArray ();

			
			
				
							
						EditorGUILayout.BeginHorizontal ();

				
								
						Variable variable = null;
						//= blackboardMotionVariables.Find ((Item) => {
//								return Item.id == blackBoardBindingID;});
										
						variable = EditorGUILayoutEx.CustomObjectPopup (label, variable, displayOptions, blackboardMotionVariables);			

						//motion = variable.genericValue as UnityEngine.Motion;
						//	motion=EditorGUILayout.ObjectField (motion, MotionType, false) as UnityEngine.Motion;

						// if(mecanimNode.motionOverride==null)
						

						EditorGUILayout.EndHorizontal ();


						//property.value = variable.id;	


					

		
						property.ApplyModifiedValue ();

				}
		

		}
}
