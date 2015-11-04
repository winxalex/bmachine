using System;
using UnityEditor;
using UnityEngine;

using ws.winx.editor.extensions;
using System.Collections.Generic;
using System.Linq;
using ws.winx.unity;
using System.Reflection;
using ws.winx.unity.attributes;
using UnityEditor.Animations;
using ws.winx.editor.utilities;


namespace ws.winx.editor.drawers
{
		[CustomPropertyDrawer (typeof(AnimatorStateAttribute))]
		public class AnimatorStatePropertyDrawer : PropertyDrawer
		{

				GUIContent[] displayOptions;
				UnityEditor.Animations.AnimatorState[] animatorStateValues;
				UnityEditor.Animations.AnimatorState animatorStateSelected;
				
				UnityEngine.Motion motionSelected;


				//public MecanimStateInfoAttribute  get{ return null; };
		
			    
				//
				// Properties
				//
				public new AnimatorStateAttribute attribute {
						get {
						
								return  (AnimatorStateAttribute)base.attribute;
						}
				}





				public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
				{
						return 30f;
				}

				public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
					
						
			//TODO change to obtain controller thru property.serializedObject.Find(attribute.animator)....
						UnityEditor.Animations.AnimatorController animatorController = ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<Animator> ().runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

						if (animatorController != null) {


				//property.serializedObject.Update();	

										animatorStateValues = MecanimUtility.GetAnimatorStates (animatorController);
										displayOptions = MecanimUtility.GetDisplayOptions (animatorController);

								
								EditorGUI.indentLevel = 0;


								//if already have been serialzied before
								if(property.objectReferenceValue!=null){
									

									if(animatorStateValues.Length>0){
										animatorStateSelected=animatorStateValues.FirstOrDefault((itm)=>itm.nameHash==((ws.winx.unity.AnimatorState)property.objectReferenceValue).nameHash) as UnityEditor.Animations.AnimatorState;
									}
								}

								//animaStateInfoSelected = property.objectReferenceValue as UnityEditor.Animations.AnimatorState;


								
								// Check if it was modified this frame, to avoid overwriting the property constantly
								
								UnityEditor.Animations.AnimatorState prevAnimatorStateSelected=animatorStateSelected;
								animatorStateSelected = EditorGUILayoutEx.CustomObjectPopup (label, animatorStateSelected, displayOptions, animatorStateValues,null, null, null, null, position) as UnityEditor.Animations.AnimatorState;

								

									if(animatorStateSelected!=null && animatorStateSelected.nameHash!=prevAnimatorStateSelected.nameHash){
			
								 	ws.winx.unity.AnimatorState state=property.objectReferenceValue as ws.winx.unity.AnimatorState;

									if(state==null) 
											state=ScriptableObject.CreateInstance<ws.winx.unity.AnimatorState>();

									state.motion=animatorStateSelected.motion;
									state.nameHash=animatorStateSelected.nameHash;
									state.name=animatorStateSelected.name;

									if(state.motion is UnityEditor.Animations.BlendTree){
										BlendTree tree =(BlendTree)state.motion;
										int blendParamsNum= tree.GetRecursiveBlendParamCount();

										state.blendParamsHashes=new int[blendParamsNum];

											for(int i=0;i<blendParamsNum;i++)
												state.blendParamsHashes[i]=Animator.StringToHash(tree.GetRecursiveBlendParam(i));

									}else{

										state.blendParamsHashes=null;
									}


									

									property.objectReferenceValue=state;
									property.serializedObject.ApplyModifiedProperties ();


									


								}



								

								
					
						}
								
						
		

							

		
		


				}

				
		

		}
}
