//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using BehaviourMachine;
using UnityEditorInternal;
using System.Text;
using StateMachine=UnityEditorInternal.StateMachine;
using System.Linq;
using BehaviourMachineEditor;
using ws.winx.editor.extensions;
using ws.winx.bmachine.extensions;
using Motion=UnityEngine.Motion;
using ws.winx.unity;

namespace ws.winx.editor.bmachine.extensions
{
	
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(MecanimNode), true)]
		public class MecanimCustomEditor : NodeEditor
		{

				

			
				MecanimNode mecanimNode;
				GUIContent[] displayOptions;
				List<AnimaStateInfo> animaInfoValues;
				AnimaStateInfo selectedAnimaStateInfo;
				AvatarPreviewW avatarPreview;
				AnimatorController controller;
				UnityEditorInternal.StateMachine stateMachine;

		Motion previewedMotion;
		

		State state;

		bool PrevIKOnFeet;



		void OnDisable() {

			UnityEngine.Debug.Log ("Disable");
			//OnPreviewDisable();
		}

		private void ClearStateMachine()
		{
			if (avatarPreview != null && avatarPreview.Animator != null)
			{
				AnimatorController.SetAnimatorController(avatarPreview.Animator, null);
			}
			UnityEngine.Object.DestroyImmediate(this.controller);
			UnityEngine.Object.DestroyImmediate(this.stateMachine);
			UnityEngine.Object.DestroyImmediate(this.state);
			stateMachine = null;
			controller = null;
			state = null;
		}
		
//		public void ResetStateMachine()
//		{
//			ClearStateMachine();
//			CreateStateMachine();
//		}

		public void SetPreviewMotion(Motion motion) {
			if (previewedMotion == motion && motion!=null)
							return;
			//			
						previewedMotion = motion;
			//			
			//			ClearStateMachine();
			
			if (avatarPreview == null)
			{
				avatarPreview = new AvatarPreviewW(null, motion);
				//avatarPreview = new AvatarPreviewWrapper((((MecanimNode)target).animator), motion);


				//avatarPreview.OnAvatarChangeFunc = this.OnPreviewAvatarChanged;
				PrevIKOnFeet = avatarPreview.IKOnFeet;

			}
			
						if (motion != null)
						CreateStateMachine(motion);
			//			
			//			Repaint();
		}



		private void CreateStateMachine(Motion motion) {
			if (avatarPreview == null || avatarPreview.Animator == null)
				return;

			Debug.Log ("CreateStateMachine");
			
			if (controller == null) {
								controller = new AnimatorController ();
								controller.AddLayer ("preview");
								controller.hideFlags = HideFlags.DontSave;
				
								stateMachine = controller.GetLayer (0).stateMachine;
								CreateParameters (motion);
				
								state = stateMachine.AddState ("preview");

								state.SetMotion (motion);
								state.iKOnFeet = avatarPreview.IKOnFeet;
								state.hideFlags = HideFlags.DontSave;

								
								AnimatorController.SetAnimatorController (avatarPreview.Animator, controller);
								Debug.Log ("Setting avatarPreview.Animator " + avatarPreview.Animator.name + " to temp controller");
						} else {
								Debug.Log ("Reset of root position and rotation cos of new motion");
								//Vector3 rootPosition = avatarPreview.Animator.rootPosition;
								//Quaternion rootRotation = avatarPreview.Animator.rootRotation;
								state.SetMotion (motion);
								CreateParameters (motion);
								avatarPreview.timeControl.currentTime = 0f;
								avatarPreview.Animator.UpdateWrapper (0f);
								//avatarPreview.Animator.rootPosition = rootPosition;
								//avatarPreview.Animator.rootRotation = rootRotation;
								Debug.Log ("Reseted");
						}

		
			
			if (AnimatorController.GetEffectiveAnimatorController(avatarPreview.Animator) != this.controller)
			{
				AnimatorController.SetAnimatorController(avatarPreview.Animator, this.controller);

				Debug.Log ("Getting Effective Animator and set avatarPreview.Animator "+avatarPreview.Animator.name+" to temp controller");
			}
		}


		private void CreateParameters(Motion motion)
		{
			int parameterCount = controller.parameterCount;
			for (int i = 0; i < parameterCount; i++)
			{
				controller.RemoveParameter(0);
			}
			
			if (motion is BlendTree)
			{
				BlendTree blendTree = motion as BlendTree;
				
				for (int j = 0; j < blendTree.GetRecursiveBlendParamCount(); j++)
				{
					controller.AddParameter(blendTree.GetRecursiveBlendParam(j), AnimatorControllerParameterType.Float);
				}
			}
		}


		private void UpdateAvatarState(Motion motion)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			Debug.Log ("UpdateAvatarState");
			
			Animator animator = avatarPreview.Animator;
			if (animator)
			{
//				if (PrevIKOnFeet != avatarPreview.IKOnFeet)
//				{
//					PrevIKOnFeet = avatarPreview.IKOnFeet;
//					Vector3 rootPosition = avatarPreview.Animator.rootPosition;
//					Quaternion rootRotation = avatarPreview.Animator.rootRotation;
//
//					ClearStateMachine();
//					CreateStateMachine(motion);
//					//ResetStateMachine();
//					avatarPreview.Animator.UpdateWrapper(avatarPreview.timeControl.currentTime);
//					avatarPreview.Animator.UpdateWrapper(0f);
//					avatarPreview.Animator.rootPosition = rootPosition;
//					avatarPreview.Animator.rootRotation = rootRotation;
//				}
				
				avatarPreview.timeControl.loop = true;
				float num = 1f;
				float num2 = 0f;
				if (animator.layerCount > 0)
				{

					AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					num = currentAnimatorStateInfo.length;
					num2 = currentAnimatorStateInfo.normalizedTime;
				}
				
				avatarPreview.timeControl.startTime = 0f;
				avatarPreview.timeControl.stopTime = num;
				avatarPreview.timeControl.Update();
				
				float num3 = this.avatarPreview.timeControl.deltaTime;
				if (!motion.isLooping)
				{
					if (num2 >= 1f)
					{
						num3 -= num;
					}
					else
					{
						if (num2 < 0f)
						{
							num3 += num;
						}
					}
				}
				
				
				animator.UpdateWrapper(num3);
			}
		}

          

				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						DrawDefaultInspector ();
			
						mecanimNode = target as MecanimNode;

						Motion motion =  mecanimNode.selectedAnimaStateInfo.motion;

		
			
						if (mecanimNode != null) {
								// Get children nodes
								//ActionNode[] children = randomChild.children;
				
								// Update serialized node data
								if (Event.current.type == EventType.Layout) {
										this.serializedNode.Update ();
								}
				
								// Get an iterator
								//var iterator = serializedNode.GetIterator ();
				
								// Cache the indent level
								int indentLevel = EditorGUI.indentLevel;
				
								//iterator.Find ("selectedStateHash")
								//		SerializedNodeProperty current = iterator.current;

					
										
//										if (selectedAnimaStateInfo == null) {
//											mecanimNode.layer = selectedAnimaStateInfo.layer;
//											mecanimNode.selectedStateHash = selectedAnimaStateInfo.hash;
//										}


										//selectedAnimaStateInfo=EditorGUILayoutEx.CustomObjectPopup(new GUIContent("State"),selectedAnimaStateInfo,displayOptions,animaInfoValues.ToArray());
										//selectedAnimaStateInfo = EditorGUILayoutEx.CustomObjectPopup (new GUIContent ("State"), selectedAnimaStateInfo, displayOptions, animaInfoValues);

										//selectedObject2=ws.winx.editor.extensions.EditorGUILayoutEx.CustomObjectPopup(new GUIContent("State1"),selectedObject2,new GUIContent[]{new GUIContent("mile"),new GUIContent("mile/kitic")},new string[]{"mile","kitic"});

//										if (selectedAnimaStateInfo != null) {
//												mecanimNode.layer = selectedAnimaStateInfo.layer;
//												mecanimNode.selectedStateHash = selectedAnimaStateInfo.hash;
//										}

					


									//	GUILayoutHelper.DrawNodeProperty (new GUIContent (current.label, current.tooltip), current, target);
					

//								if (!Application.isPlaying) {
//									
//									
//										SetPreviewMotion (motion);
//										
//									//if(Event.current.type==EventType.Repaint){	
//										UpdateAvatarState (motion);
//
//					//GUILayout.BeginHorizontal();
//							avatarPreview.DoAvatarPreview2 (new Rect (0, 0, 100, 150), GUIStyle.none);
//										//avatarPreview.DoAvatarPreview (new Rect (0, 0, Screen.width, 200), GUIStyle.none);
//					//GUILayout.EndHorizontal();	
//									
//								} else {
//									AnimatorController cont=mecanimNode.animator.runtimeAnimatorController as AnimatorController;
//								}
				
										// Restore the indent level
										EditorGUI.indentLevel = indentLevel;
				
										// Apply modified properties
										this.serializedNode.ApplyModifiedProperties ();
								
						}
				}
		}
}