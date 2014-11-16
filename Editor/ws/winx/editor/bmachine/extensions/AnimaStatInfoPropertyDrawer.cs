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


namespace ws.winx.editor.bmachine.extensions
{
	[CustomNodePropertyDrawer (typeof(AnimaStateInfoAttribute))]
	public class AnimaStatInfoPropertyDrawer : NodePropertyDrawer
	{

		GUIContent[] displayOptions;
		List<AnimaStateInfo> animaInfoValues;
		AnimatorController aniController;
		AnimaStateInfo selectedAnimaStateInfo;
		bool isListDirty=false;
		
		/// <summary>
		/// Ons the assets re imported.
		/// </summary>
		/// <param name="importedAssetsPath">Imported assets path.</param>
		void onAssetsReImported (string[] importedAssetsPath)
		{

		
			//check if the reimported asset is our aniController
			if (Array.FindIndex (importedAssetsPath, (path) => {
				return System.IO.Path.GetFileNameWithoutExtension (path) == aniController.name;}) > -1) {

				isListDirty=true;

			}
			
		}



		void RegenerateAnimaStatesInfoList(){

			animaInfoValues = AnimaStateInfoUtility.getAnimaStatesInfo (aniController);
			displayOptions = animaInfoValues.Select (x => x.label).ToArray ();

			isListDirty=false;
			
			if(selectedAnimaStateInfo!=null){
				
				selectedAnimaStateInfo=animaInfoValues.FirstOrDefault((item)=>{ return item.hash==selectedAnimaStateInfo.hash;});
			}

		}
	
		//
		// Methods
		//
		public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
		{

			MecanimNode mc = node as MecanimNode;

				selectedAnimaStateInfo= property.value as AnimaStateInfo;


			if (displayOptions == null || isListDirty) {
			
				aniController = mc.animator.runtimeAnimatorController as AnimatorController;

				RegenerateAnimaStatesInfoList();



                //add handler to modification of AnimatorController
				AssetPostProcessorEventDispatcher.Imported += new AssetPostProcessorEventDispatcher.ImporetedEventHandler(onAssetsReImported);
			}
		
			property.value = EditorGUILayoutEx.CustomObjectPopup (guiContent, selectedAnimaStateInfo, displayOptions, animaInfoValues);


		
			property.ApplyModifiedValue ();

		}
		

	}
}
