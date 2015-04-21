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
		[CustomNodePropertyDrawer (typeof(AnimatorStateAttribute))]
		public class AnimatorStateNodePropertyDrawer : NodePropertyDrawer
		{

				GUIContent[] animatorStateDisplayOptions;
				UnityEditor.Animations.AnimatorState[] animatorStateValues;
				AnimatorController aniController;
				UnityEditor.Animations.AnimatorState animatorStateSelected;
				UnityEditor.Animations.AnimatorState animatorStateSelectedPrev;
				UnityEngine.Motion motionSelected;
				//SerializedNodeProperty animatorSerialized;



				//
				// Properties
				//
				public new AnimatorStateAttribute attribute {
						get {
						
								return  (AnimatorStateAttribute)base.attribute;
						}
				}


				//
				// Methods
				//

				
		

				/// <summary>
				/// Handles the onGUI event.
				/// </summary>
				/// <param name="property">Property.</param>
				/// <param name="node">Node.</param>
				/// <param name="guiContent">GUI content.</param>
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{

						
					
		
						

						//if (animatorSerialized == null || aniController == null) {
						if(aniController == null) {

								//!!! Serialization never serialized Animator cos its initialized in Reset after
//								NodePropertyIterator iter= property.serializedNode.GetIterator();
//								iter.Find(attribute.animatorFieldName);
//								 animatorSerialized=iter.current;
								//				//								if(animatorSerialized==null || animatorSerialized.value==null){
								//										Debug.LogError("AnimatorStateNodePropertyDrawer> No Animator component set on node parent GameObject");
								//									return;
								//								}
				
								//runtimeContoller =( (Animator)animatorSerialized.value).runtimeAnimatorController;
								Animator animator = node.GetType ().GetField (attribute.animatorFieldName).GetValue (node) as Animator;
				                        
				               

								RuntimeAnimatorController runtimeContoller;


								runtimeContoller = animator.runtimeAnimatorController;
								
								if (runtimeContoller is AnimatorOverrideController)
										aniController = ((AnimatorOverrideController)runtimeContoller).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
								else
										aniController = runtimeContoller as UnityEditor.Animations.AnimatorController;
				


						}
					
						
				
						



						
								
								
						animatorStateDisplayOptions = MecanimUtility.GetDisplayOptions (aniController);
						animatorStateValues = MecanimUtility.GetAnimatorStates (aniController);

										

			
						if(property.value!=null){
							
							
							if(animatorStateValues.Length>0){
								animatorStateSelected=animatorStateValues.FirstOrDefault((itm)=>itm.nameHash==((ws.winx.unity.AnimatorState)property.value).nameHash);
								
								
								
							}
						}
						

						animatorStateSelected = EditorGUILayoutEx.CustomObjectPopup (guiContent, animatorStateSelected, animatorStateDisplayOptions, animatorStateValues);//,compare);
						
						


						//TODO try Begin/End Check
						if (animatorStateSelectedPrev != animatorStateSelected) {
								//property.value = animatorStateSelected;

								NodePropertyIterator iter = property.serializedNode.GetIterator ();
								iter.Find (attribute.layerIndexFieldName);
								SerializedNodeProperty layerIndexSerialized = iter.current;
								
								layerIndexSerialized.value = MecanimUtility.GetLayerIndex (aniController, animatorStateSelected);
								layerIndexSerialized.ApplyModifiedValue ();



								if(animatorStateSelected!=null){
									
									ws.winx.unity.AnimatorState state=property.value as ws.winx.unity.AnimatorState;
									if(state==null) state=ScriptableObject.CreateInstance<ws.winx.unity.AnimatorState>();
					
									state.motion=animatorStateSelected.motion;
									state.nameHash=animatorStateSelected.nameHash;
									
									if(state.motion is UnityEditor.Animations.BlendTree){
										BlendTree tree =(BlendTree)state.motion;
										int blendParamsNum= tree.GetRecursiveBlendParamCount();
										
										state.blendParamsHashes=new int[blendParamsNum];
										
										for(int i=0;i<blendParamsNum;i++)
											state.blendParamsHashes[i]=Animator.StringToHash(tree.GetRecursiveBlendParam(i));
										
									}
									
									property.value=state;
									property.ValueChanged();
								}

								property.ApplyModifiedValue ();

								animatorStateSelectedPrev = animatorStateSelected;
						}



						if (animatorStateSelected.motion == null)
								Debug.LogError ("Selected state doesn't have Motion set");

				}



				
		

		}
}
