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
using System.Reflection;
using ws.winx.unity.attributes;
using UnityEditor.Animations;


namespace ws.winx.editor.drawers
{
		[CustomPropertyDrawer (typeof(MecanimStateInfoAttribute))]
		public class MecanimStateInfoPropertyDrawer : PropertyDrawer
		{

				GUIContent[] displayOptions;
				AnimatorState[] animaStateInfoValues;
				AnimatorState animaStateInfoSelected;
				
				UnityEngine.Motion motionSelected;


				//public MecanimStateInfoAttribute  get{ return null; };
		
			    
				//
				// Properties
				//
				public new MecanimStateInfoAttribute attribute {
						get {
						
								return  (MecanimStateInfoAttribute)base.attribute;
						}
				}




//				/// <summary>
//				/// Regenerates the anima states info list.
//				/// </summary>
//				void RegenerateAnimaStatesInfoList (UnityEditor.Animations.AnimatorController animatorController)
//				{
//					
//						animaStateInfoValues = MecanimUtility.GetAnimatorStates (animatorController);
//						displayOptions = MecanimUtility.GetDisplayOptions (animatorController);
//					
//					
//					
//						if (animaStateInfoSelected != null) {
//						
//								animaStateInfoSelected = animaStateInfoValues.FirstOrDefault ((item) => {
//										return item.hash == animaStateInfoSelected.hash;});
//						}
//					
//				}

				public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
				{
						return 30f;
				}

				public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
	
						

						UnityEditor.Animations.AnimatorController animatorController = ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<Animator> ().runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

						if (animatorController != null) {


								//RegenerateAnimaStatesInfoList (animatorController);

										animaStateInfoValues = MecanimUtility.GetAnimatorStates (animatorController);
										displayOptions = MecanimUtility.GetDisplayOptions (animatorController);

								
								EditorGUI.indentLevel = 0;


			
			

								animaStateInfoSelected = property.objectReferenceValue as AnimatorState;


								//EditorGUI.BeginProperty (position, label, property);
								// Check if it was modified this frame, to avoid overwriting the property constantly
								//EditorGUI.BeginChangeCheck ();
								animaStateInfoSelected = EditorGUILayoutEx.CustomObjectPopup (label, animaStateInfoSelected, displayOptions, animaStateInfoValues,null, null, null, null, position) as AnimatorState;

			
								property.objectReferenceValue = animaStateInfoSelected;

								//if (EditorGUI.EndChangeCheck ()) {


								//}

								//EditorGUI.EndProperty ();
					
						}
								
						
		

						property.serializedObject.ApplyModifiedProperties ();	

		
		


				}

				
		

		}
}
