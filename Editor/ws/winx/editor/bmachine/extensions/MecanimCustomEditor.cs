﻿//----------------------------------------------
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


//
//		public void EventLineGUI (Rect rect, AnimationSelection selection, AnimationWindowState state, CurveEditor curveEditor)
//		{
//			AnimationClip activeAnimationClip = state.m_ActiveAnimationClip;
//			GameObject rootGameObject = state.m_RootGameObject;
//			GUI.BeginGroup (rect);
//			Color color = GUI.color;
//			Rect rect2 = new Rect (0f, 0f, rect.width, rect.height);
//			float time = (float)Mathf.RoundToInt (state.PixelToTime (Event.current.mousePosition.x, rect) * state.frameRate) / state.frameRate;
//			if (activeAnimationClip != null)
//			{
//				AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents (activeAnimationClip);
//				Texture image = EditorGUIUtility.IconContent ("Animation.EventMarker").image;
//				Rect[] array = new Rect[animationEvents.Length];
//				Rect[] array2 = new Rect[animationEvents.Length];
//				int num = 1;
//				int num2 = 0;
//				for (int i = 0; i < animationEvents.Length; i++)
//				{
//					AnimationEvent animationEvent = animationEvents [i];
//					if (num2 == 0)
//					{
//						num = 1;
//						while (i + num < animationEvents.Length && animationEvents [i + num].time == animationEvent.time)
//						{
//							num++;
//						}
//						num2 = num;
//					}
//					num2--;
//					float num3 = Mathf.Floor (state.FrameToPixel (animationEvent.time * activeAnimationClip.frameRate, rect));
//					int num4 = 0;
//					if (num > 1)
//					{
//						float num5 = (float)Mathf.Min ((num - 1) * (image.width - 1), (int)(state.FrameDeltaToPixel (rect) - (float)(image.width * 2)));
//						num4 = Mathf.FloorToInt (Mathf.Max (0f, num5 - (float)((image.width - 1) * num2)));
//					}
//					Rect rect3 = new Rect (num3 + (float)num4 - (float)(image.width / 2), (rect.height - 10f) * (float)(num2 - num + 1) / (float)Mathf.Max (1, num - 1), (float)image.width, (float)image.height);
//					array [i] = rect3;
//					array2 [i] = rect3;
//				}
//				if (this.m_DirtyTooltip)
//				{
//					if (this.m_HoverEvent >= 0 && this.m_HoverEvent < array.Length)
//					{
//						this.m_InstantTooltipText = AnimationEventPopup.FormatEvent (rootGameObject, animationEvents [this.m_HoverEvent]);
//						this.m_InstantTooltipPoint = new Vector2 (array [this.m_HoverEvent].xMin + (float)((int)(array [this.m_HoverEvent].width / 2f)) + rect.x - 30f, rect.yMax);
//					}
//					this.m_DirtyTooltip = false;
//				}
//				if (this.m_EventsSelected == null || this.m_EventsSelected.Length != animationEvents.Length)
//				{
//					this.m_EventsSelected = new bool[animationEvents.Length];
//					AnimationEventPopup.ClosePopup ();
//				}
//				Vector2 zero = Vector2.zero;
//				int num6;
//				float num7;
//				float num8;
//				HighLevelEvent highLevelEvent = EditorGUIExt.MultiSelection (rect, array2, new GUIContent (image), array, ref this.m_EventsSelected, null, out num6, out zero, out num7, out num8, GUIStyleX.none);
//				if (highLevelEvent != HighLevelEvent.None)
//				{
//					switch (highLevelEvent)
//					{
//					case HighLevelEvent.DoubleClick:
//						if (num6 != -1)
//						{
//							AnimationEventPopup.Edit (rootGameObject, selection.clip, num6, this.m_Owner);
//						}
//						else
//						{
//							this.EventLineContextMenuAdd (new AnimationEventTimeLine.EventLineContextMenuObject (rootGameObject, activeAnimationClip, time, -1));
//						}
//						break;
//					case HighLevelEvent.ContextClick:
//					{
//						GenericMenu genericMenu = new GenericMenu ();
//						AnimationEventTimeLine.EventLineContextMenuObject userData = new AnimationEventTimeLine.EventLineContextMenuObject (rootGameObject, activeAnimationClip, animationEvents [num6].time, num6);
//						genericMenu.AddItem (new GUIContent ("Edit Animation Event"), false, new GenericMenu.MenuFunction2 (this.EventLineContextMenuEdit), userData);
//						genericMenu.AddItem (new GUIContent ("Add Animation Event"), false, new GenericMenu.MenuFunction2 (this.EventLineContextMenuAdd), userData);
//						genericMenu.AddItem (new GUIContent ("Delete Animation Event"), false, new GenericMenu.MenuFunction2 (this.EventLineContextMenuDelete), userData);
//						genericMenu.ShowAsContext ();
//						this.m_InstantTooltipText = null;
//						this.m_DirtyTooltip = true;
//						state.Repaint ();
//						break;
//					}
//					case HighLevelEvent.BeginDrag:
//						this.m_EventsAtMouseDown = animationEvents;
//						this.m_EventTimes = new float[animationEvents.Length];
//						for (int j = 0; j < animationEvents.Length; j++)
//						{
//							this.m_EventTimes [j] = animationEvents [j].time;
//						}
//						break;
//					case HighLevelEvent.Drag:
//					{
//						for (int k = animationEvents.Length - 1; k >= 0; k--)
//						{
//							if (this.m_EventsSelected [k])
//							{
//								AnimationEvent animationEvent2 = this.m_EventsAtMouseDown [k];
//								animationEvent2.time = this.m_EventTimes [k] + zero.x * state.PixelDeltaToTime (rect);
//								animationEvent2.time = Mathf.Max (0f, animationEvent2.time);
//								animationEvent2.time = (float)Mathf.RoundToInt (animationEvent2.time * activeAnimationClip.frameRate) / activeAnimationClip.frameRate;
//							}
//						}
//						int[] array3 = new int[this.m_EventsSelected.Length];
//						for (int l = 0; l < array3.Length; l++)
//						{
//							array3 [l] = l;
//						}
//						Array.Sort (this.m_EventsAtMouseDown, array3, new AnimationEventTimeLine.EventComparer ());
//						bool[] array4 = (bool[])this.m_EventsSelected.Clone ();
//						float[] array5 = (float[])this.m_EventTimes.Clone ();
//						for (int m = 0; m < array3.Length; m++)
//						{
//							this.m_EventsSelected [m] = array4 [array3 [m]];
//							this.m_EventTimes [m] = array5 [array3 [m]];
//						}
//						Undo.RegisterCompleteObjectUndo (activeAnimationClip, "Move Event");
//						AnimationUtility.SetAnimationEvents (activeAnimationClip, this.m_EventsAtMouseDown);
//						this.m_DirtyTooltip = true;
//						break;
//					}
//					case HighLevelEvent.Delete:
//						this.DeleteEvents (activeAnimationClip, this.m_EventsSelected);
//						break;
//					case HighLevelEvent.SelectionChanged:
//						curveEditor.SelectNone ();
//						if (num6 != -1)
//						{
//							AnimationEventPopup.UpdateSelection (rootGameObject, selection.clip, num6, this.m_Owner);
//						}
//						break;
//					}
//				}
//				this.CheckRectsOnMouseMove (rect, animationEvents, array);
//			}
//			if (Event.current.type == EventType.ContextClick && rect2.Contains (Event.current.mousePosition) && selection.EnsureClipPresence ())
//			{
//				Event.current.Use ();
//				GenericMenu genericMenu2 = new GenericMenu ();
//				genericMenu2.AddItem (new GUIContent ("Add Animation Event"), false, new GenericMenu.MenuFunction2 (this.EventLineContextMenuAdd), new AnimationEventTimeLine.EventLineContextMenuObject (rootGameObject, activeAnimationClip, time, -1));
//				genericMenu2.ShowAsContext ();
//			}
//			GUI.color = color;
//			GUI.EndGroup ();
//		}

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

					
				mecanimNode.normalizedTimeCurrent=EditorGUILayout.Slider(mecanimNode.normalizedTimeCurrent,0f,1f);

				//mecanimNode.animator.Update(mecanimNode.normalizedTimeStart

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