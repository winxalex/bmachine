using BehaviourMachine;
using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using BehaviourMachineEditor;
using System.Linq;
using ws.winx.unity;
using ws.winx.unity.attributes;

namespace ws.winx.editor.bmachine.drawers
{
		[CustomNodePropertyDrawer (typeof(MecanimStateInfoAttribute))]
		public class MecanimNodeStateInfoPropertyDrawer : NodePropertyDrawer
		{

				GUIContent[] animatorStateDisplayOptions;
				AnimatorState[] animatorStateValues;
				AnimatorController aniController;
				AnimatorState animatorStateSelected;
				bool isListDirty = false;
				UnityEngine.Motion motionSelected;



				//
				// Properties
				//
				public new MecanimStateInfoAttribute attribute {
						get {
						
								return  (MecanimStateInfoAttribute)base.attribute;
						}
				}


				//
				// Methods
				//
		
			


				/// <summary>
				/// Regenerates the anima states info list.
				/// </summary>
//				void RegenerateAnimaStatesInfoList ()
//				{
//
//						animaStateInfoValues = MecanimUtility.getAnimaStatesInfo (aniController);
//						displayOptions = animaStateInfoValues.Select (x => x.path).ToArray ();
//
//						isListDirty = false;
//			
//						if (animaStateInfoSelected != null) {
//				
//								animaStateInfoSelected = animaStateInfoValues.FirstOrDefault ((item) => {
//										return item.hash == animaStateInfoSelected.hash;});
//						}
//
//				}
	

			
				

				/// <summary>
				/// Raises the GU event.
				/// </summary>
				/// <param name="property">Property.</param>
				/// <param name="node">Node.</param>
				/// <param name="guiContent">GUI content.</param>
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{

						attribute.serializedObject = node;
					
						animatorStateSelected = property.value as AnimatorState;

						//property.path

						//node.GetType().GetField(property.path).Attributes
						//property.serializedNode


						
						RuntimeAnimatorController runtimeContoller;
				   
						runtimeContoller = attribute.Ani.runtimeAnimatorController;

						if (runtimeContoller is AnimatorOverrideController)
								aniController = ((AnimatorOverrideController)runtimeContoller).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
						else
								aniController = runtimeContoller as UnityEditor.Animations.AnimatorController;

								
								
						animatorStateDisplayOptions = MecanimUtility.GetDisplayOptions (aniController);
						animatorStateValues = MecanimUtility.GetAnimatorStates (aniController);

										
						int selectedIndex;
		
						EditorGUI.BeginChangeCheck ();

						if (animatorStateSelected != null) {
								selectedIndex = Array.FindIndex (animatorStateValues, (item) =>
								item.GetInstanceID () == animatorStateSelected.GetInstanceID ()
								);

						} else {
								if (animatorStateValues.Length > 0) {
										animatorStateSelected = animatorStateValues [0];
										selectedIndex = 0;
											property.value = animatorStateSelected;
											
											attribute.LayerIndex = MecanimUtility.GetLayerIndex (aniController, animatorStateSelected);
											
											
											property.ApplyModifiedValue ();
								} else {
										selectedIndex = -1;
								}
						}

						selectedIndex = EditorGUILayout.Popup (selectedIndex, animatorStateDisplayOptions);

						if (selectedIndex > -1)
								animatorStateSelected = animatorStateValues [selectedIndex];

						//animatorStateSelected = EditorGUILayoutEx.CustomObjectPopup (guiContent, animatorStateSelected, animatorStateDisplayOptions, animatorStateValues);
						
						if (animatorStateSelected.motion == null)
								Debug.LogError ("Selected state doesn't have Motion set");


						if (EditorGUI.EndChangeCheck ()) {
								property.value = animatorStateSelected;

								attribute.LayerIndex = MecanimUtility.GetLayerIndex (aniController, animatorStateSelected);
								

								property.ApplyModifiedValue ();
						}

				}
		

		}
}
