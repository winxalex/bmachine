// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using ws.winx.bmachine.extensions;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;


namespace ws.winx.editor.extensions
{
		public class AnimaStateInfoUtility
		{
		
		/// <summary>
		/// Processes the state machine path inside hierarchy.
		/// </summary>
		/// <param name="stateMachine">State machine.</param>
		/// <param name="parentName">Parent name.</param>
		/// <param name="layer">Layer.</param>
		/// <param name="resultsAnimaInfoList">Results anima info list.</param>
		 static void processStateMachinePath (UnityEditorInternal.StateMachine stateMachine, string parentName, int layer, List<AnimaStateInfo> resultsAnimaInfoList)
		{
			int numStates = 0;
			int numStateMachines = 0;
			
			int currentStateInx;
			int currentStateMachineInx;
			UnityEditorInternal.StateMachine currentStateMachine;
			string path;
			
			UnityEditorInternal.State state;
			
			numStates = stateMachine.stateCount;
			
			
			
			for (currentStateInx=0; currentStateInx<numStates; currentStateInx++) {
				
				
				state = stateMachine.GetState (currentStateInx);
		//	resultsAnimaInfoList.Add (new AnimaStateInfo (state.uniqueNameHash, new GUIContent (parentName + '/' + state.name), layer));
//				
				AnimaStateInfo info=AnimaStateInfo.CreateInstance<AnimaStateInfo>();
				info.hash=state.uniqueNameHash;
				info.label= new GUIContent (parentName + '/' + state.name);
				info.layer = layer;
				info.motion=state.GetMotion();

			

				if(info.motion is BlendTree){
					BlendTree blendTree=info.motion as BlendTree;
					int count=blendTree.GetRecursiveBlendParamCount();

					if(count>0){
						info.blendParamsNames=new string[count];
						info.blendParamsIDs=new int[count];

						for (int j = 0; j < count; j++)
						{
							info.blendParamsNames[j]=blendTree.GetRecursiveBlendParam(j);
							info.blendParamsIDs[j]=Animator.StringToHash(info.blendParamsNames[j]);
						}


					}



				}

				resultsAnimaInfoList.Add (info);

				
			}
			
			
			numStateMachines = stateMachine.stateMachineCount;
			
			if (numStateMachines > 0) {
				for (currentStateMachineInx=0; currentStateMachineInx<numStateMachines; currentStateMachineInx++) {
					currentStateMachine = stateMachine.GetStateMachine (currentStateMachineInx);
					path = parentName + "/" + currentStateMachine.name;
					
					processStateMachinePath (currentStateMachine, path, layer, resultsAnimaInfoList);
					
				}
			} else if (numStates == 0) {
				//statesPathStringBuilder.Append (parentName).Append ("(Empty)|");
				//searchList.Add (new AnimaStateInfo(state.uniqueNameHash,new GUIContent (parentName+"(Empty)"),layer));
			}
			
		}
		
		
		/// <summary>
		/// Gets the anima states info.
		/// </summary>
		/// <returns>The anima states info.</returns>
		/// <param name="aniController">Ani controller.</param>
		public static List<AnimaStateInfo> getAnimaStatesInfo (AnimatorController aniController)
		{
			
			AnimatorControllerLayer layer;
			
			
			int numLayers = aniController.layerCount;
			
			
			int currentLayerInx = 0;
			
			
			List<AnimaStateInfo> animaStatesInfoList = new List<AnimaStateInfo> ();
			
			
			for (; currentLayerInx<numLayers; currentLayerInx++) {
				layer = aniController.GetLayer (currentLayerInx);				                
				processStateMachinePath (layer.stateMachine, layer.name, currentLayerInx, animaStatesInfoList);	
			}
			
			return animaStatesInfoList;
			
		}
		}
}

