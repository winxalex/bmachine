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

namespace ws.winx.editor.bmachine.extensions
{
		[CustomPropertyDrawer (typeof(MecanimStateInfoAttribute))]
		public class MecanimStateInfoPropertyDrawer : PropertyDrawer
		{

				GUIContent[] displayOptions;
				List<MecanimStateInfo> animaStateInfoValues;
				MecanimStateInfo animaStateInfoSelected;
				bool isListDirty = false;
				UnityEngine.Motion motionSelected;


				//public MecanimStateInfoAttribute  get{ return null; };
		
			    
				//
				// Properties
				//
				public MecanimStateInfoAttribute attribute {
						get {
						
								return  (MecanimStateInfoAttribute)base.attribute;
						}
				}




				/// <summary>
				/// Regenerates the anima states info list.
				/// </summary>
				void RegenerateAnimaStatesInfoList (AnimatorController animatorController)
				{
					
						animaStateInfoValues = MecanimStateInfoUtility.getAnimaStatesInfo (animatorController);
						displayOptions = animaStateInfoValues.Select (x => x.label).ToArray ();
					
						isListDirty = false;
					
						if (animaStateInfoSelected != null) {
						
								animaStateInfoSelected = animaStateInfoValues.FirstOrDefault ((item) => {
										return item.hash == animaStateInfoSelected.hash;});
						}
					
				}

				public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
				{
						return 30f;
				}

				public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
	
						

						AnimatorController animatorController = ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<Animator> ().runtimeAnimatorController as AnimatorController;

						if (animatorController != null) {


								RegenerateAnimaStatesInfoList (animatorController);


								
								EditorGUI.indentLevel = 0;


			


								animaStateInfoSelected = property.objectReferenceValue as MecanimStateInfo;


								//EditorGUI.BeginProperty (position, label, property);
								// Check if it was modified this frame, to avoid overwriting the property constantly
								//EditorGUI.BeginChangeCheck ();
								animaStateInfoSelected = EditorGUILayoutEx.CustomObjectPopup (label, animaStateInfoSelected, displayOptions, animaStateInfoValues, null, null, null, position) as MecanimStateInfo;

			
								property.objectReferenceValue = animaStateInfoSelected;

								//if (EditorGUI.EndChangeCheck ()) {


								//}

								//EditorGUI.EndProperty ();
					
						}
								
						
		

						property.serializedObject.ApplyModifiedProperties ();	

		
		


				}

				
		

		}
}
