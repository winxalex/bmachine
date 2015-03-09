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

namespace ws.winx.editor.bmachine.drawers
{
		[CustomNodePropertyDrawer (typeof(MecanimStateInfoAttribute))]
		public class MecanimNodeStateInfoPropertyDrawer : NodePropertyDrawer
		{

				GUIContent[] displayOptions;
				List<MecanimStateInfo> animaStateInfoValues;
				UnityEditor.Animations.AnimatorController aniController;
				MecanimStateInfo animaStateInfoSelected;
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
				/// Ons the assets re imported.
				/// </summary>
				/// <param name="importedAssetsPath">Imported assets path.</param>
				void onAssetsReImported (string[] importedAssetsPath)
				{

		
						//check if the reimported asset is our aniController
						if (Array.FindIndex (importedAssetsPath, (path) => {
								return System.IO.Path.GetFileNameWithoutExtension (path) == aniController.name;}) > -1) {

								isListDirty = true;

						}
			
				}


				/// <summary>
				/// Regenerates the anima states info list.
				/// </summary>
				void RegenerateAnimaStatesInfoList ()
				{

						animaStateInfoValues = MecanimStateInfoUtility.getAnimaStatesInfo (aniController);
						displayOptions = animaStateInfoValues.Select (x => x.label).ToArray ();

						isListDirty = false;
			
						if (animaStateInfoSelected != null) {
				
								animaStateInfoSelected = animaStateInfoValues.FirstOrDefault ((item) => {
										return item.hash == animaStateInfoSelected.hash;});
						}

				}
	

			
				

				/// <summary>
				/// Raises the GU event.
				/// </summary>
				/// <param name="property">Property.</param>
				/// <param name="node">Node.</param>
				/// <param name="guiContent">GUI content.</param>
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{

						attribute.serializedObject = node;
					
						animaStateInfoSelected = property.value as MecanimStateInfo;


						if (displayOptions == null || isListDirty) {
								RuntimeAnimatorController runtimeContoller;
				   
								runtimeContoller = attribute.Ani.runtimeAnimatorController;

								if (runtimeContoller is AnimatorOverrideController)
										aniController = ((AnimatorOverrideController)runtimeContoller).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
								else
										aniController = runtimeContoller as UnityEditor.Animations.AnimatorController;

								RegenerateAnimaStatesInfoList ();



								//add handler to modification of AnimatorController
								AssetPostProcessorEventDispatcher.Imported += new AssetPostProcessorEventDispatcher.ImportedEventHandler (onAssetsReImported);
						}
		
						
						animaStateInfoSelected = EditorGUILayoutEx.CustomObjectPopup (guiContent, animaStateInfoSelected, displayOptions, animaStateInfoValues);
						
						if (animaStateInfoSelected.motion == null)
								Debug.LogError ("Selected state doesn't have Motion set");



						property.value = animaStateInfoSelected;

		
						property.ApplyModifiedValue ();

				}
		

		}
}
