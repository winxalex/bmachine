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
using ws.winx.csharp.extensions;

namespace ws.winx.editor.bmachine.extensions
{
	
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(MecanimNode), true)]
		public class MecanimNodeCustomEditor : NodeEditor
		{

				

			
				MecanimNode mecanimNode;
				GUIContent[] displayOptions;
				List<MecanimStateInfo> animaInfoValues;
				MecanimStateInfo selectedAnimaStateInfo;
				AvatarPreviewW avatarPreview;
				AnimatorController controller;
				UnityEditorInternal.StateMachine stateMachine;
				float[] eventTimeValues;
				float[] eventTimeValuesPrev;
				bool eventTimeLineInitalized;
				Rect eventTimeLineValuePopUpRect;
				Motion previewedMotion;
				State state;
				bool PrevIKOnFeet;
				bool[] eventTimeValuesSelected;


				//
				// Nested Types
				//
				public class EventComparer : IComparer
				{
						int IComparer.Compare (object objX, object objY)
						{
								SendEventNormalized animationEvent = (SendEventNormalized)objX;
								SendEventNormalized animationEvent2 = (SendEventNormalized)objY;
								float time = animationEvent.timeNormalized;
								float time2 = animationEvent2.timeNormalized;
								if (time != time2) {
										return (int)Mathf.Sign (time - time2);
								}
								int hashCode = animationEvent.GetHashCode ();
								int hashCode2 = animationEvent2.GetHashCode ();
								return hashCode - hashCode2;
						}
				}



				void OnDisable ()
				{

						UnityEngine.Debug.Log ("Disable");
						//OnPreviewDisable();
				}

				private void ClearStateMachine ()
				{
						if (avatarPreview != null && avatarPreview.Animator != null) {
								AnimatorController.SetAnimatorController (avatarPreview.Animator, null);
						}
						UnityEngine.Object.DestroyImmediate (this.controller);
						UnityEngine.Object.DestroyImmediate (this.stateMachine);
						UnityEngine.Object.DestroyImmediate (this.state);
						stateMachine = null;
						controller = null;
						state = null;
				}
		
//		public void ResetStateMachine()
//		{
//			ClearStateMachine();
//			CreateStateMachine();
//		}

				public void SetPreviewMotion (Motion motion)
				{
						if (previewedMotion == motion && motion != null)
								return;
						//			
						previewedMotion = motion;
						//			
						//			ClearStateMachine();
			
						if (avatarPreview == null) {
								avatarPreview = new AvatarPreviewW (null, motion);
								//avatarPreview = new AvatarPreviewWrapper((((MecanimNode)target).animator), motion);


								//avatarPreview.OnAvatarChangeFunc = this.OnPreviewAvatarChanged;
								PrevIKOnFeet = avatarPreview.IKOnFeet;

						}
			
						if (motion != null)
								CreateStateMachine (motion);
						//			
						//			Repaint();
				}

				private void CreateStateMachine (Motion motion)
				{
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

		
			
						if (AnimatorController.GetEffectiveAnimatorController (avatarPreview.Animator) != this.controller) {
								AnimatorController.SetAnimatorController (avatarPreview.Animator, this.controller);

								Debug.Log ("Getting Effective Animator and set avatarPreview.Animator " + avatarPreview.Animator.name + " to temp controller");
						}
				}

				private void CreateParameters (Motion motion)
				{
						int parameterCount = controller.parameterCount;
						for (int i = 0; i < parameterCount; i++) {
								controller.RemoveParameter (0);
						}
			
						if (motion is BlendTree) {
								BlendTree blendTree = motion as BlendTree;
				
								for (int j = 0; j < blendTree.GetRecursiveBlendParamCount(); j++) {
										controller.AddParameter (blendTree.GetRecursiveBlendParam (j), AnimatorControllerParameterType.Float);
								}
						}


				}

				private void UpdateAvatarState (Motion motion)
				{
						if (Event.current.type != EventType.Repaint) {
								return;
						}

						Debug.Log ("UpdateAvatarState");
			
						Animator animator = avatarPreview.Animator;
						if (animator) {
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
								if (animator.layerCount > 0) {

										AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo (0);
										num = currentAnimatorStateInfo.length;
										num2 = currentAnimatorStateInfo.normalizedTime;
								}
				
								avatarPreview.timeControl.startTime = 0f;
								avatarPreview.timeControl.stopTime = num;
								avatarPreview.timeControl.Update ();
				
								float num3 = this.avatarPreview.timeControl.deltaTime;
								if (!motion.isLooping) {
										if (num2 >= 1f) {
												num3 -= num;
										} else {
												if (num2 < 0f) {
														num3 += num;
												}
										}
								}
				
				
								animator.UpdateWrapper (num3);
						}
				}







				//
				// Nested Types
				//
				private class Styles
				{
						public GUIContent playIcon = EditorGUIUtility.IconContent ("PlayButton");
						public GUIContent pauseIcon = EditorGUIUtility.IconContent ("PauseButton");
						public GUIStyle playButton = "TimeScrubberButton";
						public GUIStyle timeScrubber = "TimeScrubber";
				}

				private static Styles s_Styles;
				float m_MouseDrag;
				float nextCurrentTime;
				bool m_WrapForwardDrag;
				string[] displayNames;

				//
				// Methods
				//
				public void DoTimeControl (Rect rect)
				{
						if (MecanimNodeCustomEditor.s_Styles == null) {
								MecanimNodeCustomEditor.s_Styles = new MecanimNodeCustomEditor.Styles ();
						}
						Event current = Event.current;
						//int controlID = GUIUtility.GetControlID (TimeControl.kScrubberIDHash, FocusType.Keyboard);
						int controlID = GUIUtility.GetControlID (FocusType.Passive);
						Rect rect2 = rect;
						rect2.height = 21f;
						Rect rect3 = rect2;
						rect3.xMin += 33f;
						switch (current.GetTypeForControl (controlID)) {
						case EventType.MouseDown:
								if (rect.Contains (current.mousePosition)) {
										GUIUtility.keyboardControl = controlID;
								}
								if (rect3.Contains (current.mousePosition)) {
										EditorGUIUtility.SetWantsMouseJumping (1);
										GUIUtility.hotControl = controlID;
										this.m_MouseDrag = current.mousePosition.x - rect3.xMin;
										//this.nextCurrentTime = this.m_MouseDrag * (this.stopTime - this.startTime) / rect3.width + this.startTime;
										this.m_WrapForwardDrag = false;
										current.Use ();
								}
								break;
						case EventType.MouseUp:
								if (GUIUtility.hotControl == controlID) {
										EditorGUIUtility.SetWantsMouseJumping (0);
										GUIUtility.hotControl = 0;
										current.Use ();
								}
								break;
						case EventType.MouseDrag:
								if (GUIUtility.hotControl == controlID) {
										this.m_MouseDrag += current.delta.x * 1f;//this.playbackSpeed;
										if (false && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width)) {
												//if (this.loop && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width))
												this.m_WrapForwardDrag = true;
												this.m_MouseDrag = Mathf.Repeat (this.m_MouseDrag, rect3.width);
										}
										//this.nextCurrentTime = Mathf.Clamp (this.m_MouseDrag, 0f, rect3.width) * (this.stopTime - this.startTime) / rect3.width + this.startTime;
										current.Use ();
								}
								break;
						case EventType.KeyDown:
//				if (GUIUtility.keyboardControl == controlID)
//				{
//					if (current.keyCode == KeyCode.LeftArrow)
//					{
//						if (this.currentTime - this.startTime > 0.01f)
//						{
//							this.deltaTime = -0.01f;
//						}
//						current.Use ();
//					}
//					if (current.keyCode == KeyCode.RightArrow)
//					{
//						if (this.stopTime - this.currentTime > 0.01f)
//						{
//							this.deltaTime = 0.01f;
//						}
//						current.Use ();
//					}
//				}
								break;
						}

						//GUI.Box (rect2, GUIContent.none, TimeControl.s_Styles.timeScrubber);
						GUI.Box (rect2, GUIContent.none, MecanimNodeCustomEditor.s_Styles.timeScrubber);
						//thisg = GUI.Toggle (rect2, this.playing, (!this.playing) ? TimeControl.s_Styles.playIcon : TimeControl.s_Styles.pauseIcon, TimeControl.s_Styles.playButton);

						//float num = Mathf.Lerp (rect3.x, rect3.xMax, this.normalizedTime);
						float num = Mathf.Lerp (rect3.x, rect3.xMax, mecanimNode.normalizedTimeCurrent);

						if (GUIUtility.keyboardControl == controlID) {
								Handles.color = new Color (1f, 0f, 0f, 1f);
						} else {
								Handles.color = new Color (1f, 0f, 0f, 0.5f);
						}
						Handles.DrawLine (new Vector2 (num, rect3.yMin), new Vector2 (num, rect3.yMax));
						Handles.DrawLine (new Vector2 (num + 1f, rect3.yMin), new Vector2 (num + 1f, rect3.yMax));
				}



				///  TIMELINE EVENTHANDLERS
		 
		 
				/// <summary>
				/// Ons the mecanim event edit.
				/// </summary>
				/// <param name="sender">Sender.</param>
				/// <param name="args">Arguments.</param>
				void onMecanimEventEdit (TimeLineEventArgs<float> args)
				{
				
						SendEventNormalized child = mecanimNode.children [args.selectedIndex] as SendEventNormalized;
					
		
						child.timeNormalized.Value = args.selectedValue;
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

				}




			



				/// <summary>
				/// On the mecanim event close.
				/// </summary>
				/// <param name="sender">Sender.</param>
				/// <param name="args">Arguments.</param>
				void onMecanimEventClose (TimeLineEventArgs<float> args)
				{
						SendEventNormalizedEditor.Hide ();
				}


				/// <summary>
				/// Ons the mecanim event add.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventAdd (TimeLineEventArgs<float> args)
				{
				

						//create and add node to internal bhv tree
						SendEventNormalized child = mecanimNode.tree.AddNode (typeof(SendEventNormalized)) as SendEventNormalized;
				
						//add node to its parent list
						mecanimNode.Insert (args.selectedIndex, child);
						child.timeNormalized.Value = args.selectedValue;

						mecanimNode.tree.SaveNodes ();

						eventTimeValues = (float[])args.values;

						//recreate (not to optimal but doesn't have
						displayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

					

						
						
						//show popup
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

						Undo.RecordObject (target.self, "Add Node");
				}


				/// <summary>
				/// Ons the mecanim event drag end.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventDragEnd (TimeLineEventArgs<float> args)
				{
						int[] indexArray = new int[mecanimNode.children.Length];
						for (int l = 0; l < indexArray.Length; l++) {
								indexArray [l] = l;
						}
			
						Array.Sort (mecanimNode.children, indexArray, new EventComparer ());
			
						bool[] cloneOfSelected = (bool[])eventTimeValuesSelected.Clone ();
						int inx = -1;
						SendEventNormalized ev;
						for (int m = 0; m < indexArray.Length; m++) {
				
								inx = indexArray [m];
								ev = ((SendEventNormalized)mecanimNode.children [m]);	
								this.eventTimeValuesSelected [m] = cloneOfSelected [inx];
								this.eventTimeValues [m] = ev.timeNormalized; 
								this.displayNames [m] = ev.name;
				
						}
			
			
			
			
			
						mecanimNode.tree.HierarchyChanged ();
						StateUtility.SetDirty (mecanimNode.tree);
				}

				/// <summary>
				/// Ons the mecanim event delete.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventDelete (TimeLineEventArgs<float> args)
				{
						float[] timeValues = (float[])args.values;
						int timeValuesNumber = timeValues.Length;



						SendEventNormalized child;
						for (int i=0; i<mecanimNode.children.Length;) {
								child = mecanimNode.children [i] as SendEventNormalized;

								if (i < timeValuesNumber) {

										child.timeNormalized = timeValues [i];
										i++;
								} else {
										//remove localy from node parent
										mecanimNode.Remove (child);
										//remove from internal behaviour tree
										mecanimNode.tree.RemoveNode (child, false);
				
								}


						}



						mecanimNode.tree.SaveNodes ();

						//assign to display new 
						eventTimeValues = timeValues;

						

						//recreate 
						displayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

	
						

						StateUtility.SetDirty (mecanimNode.tree);

						Undo.RecordObject (target.self, "Delete Node");
						SendEventNormalizedEditor.Hide ();


				}

	
          

				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						DrawDefaultInspector ();
			
						mecanimNode = target as MecanimNode;

						Motion motion = mecanimNode.selectedAnimaStateInfo.motion;
	
		
			
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

					
								//mecanimNode.normalizedTimeCurrent=EditorGUILayout.Slider(mecanimNode.normalizedTimeCurrent,0f,1f);

								//Texture image = EditorGUIUtility.IconContent ("Animation.EventMarker").image;



								//	DoTimeControl(rect);
								if (!Application.isPlaying) {
										if (!eventTimeLineInitalized) {
											
							
				
					
												//TODO calculate PopupRect

												eventTimeLineValuePopUpRect = new Rect ((Screen.width - 250) * 0.5f, (Screen.height - 150) * 0.5f, 250, 150);
												//select the time values from nodes
												eventTimeValues = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).timeNormalized.Value).ToArray ();
												displayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();
												eventTimeValuesSelected = new bool[eventTimeValues.Length];
												eventTimeLineInitalized=true;
										}



					EditorGUILayoutEx.CustomTimeLine (ref eventTimeValues,ref eventTimeValuesPrev, ref displayNames, ref eventTimeValuesSelected,mecanimNode.normalizedTimeCurrent,
					                              onMecanimEventAdd,onMecanimEventDelete,onMecanimEventClose,onMecanimEventEdit,onMecanimEventDragEnd
					                              );

										SendEventNormalized ev;

										//update time values 
										int eventTimeValuesNumber = mecanimNode.children.Length;
										for (int i=0; i<eventTimeValuesNumber; i++) {
												ev = ((SendEventNormalized)mecanimNode.children [i]);	
												ev.timeNormalized = eventTimeValues [i];

												//if changes have been made in pop editor or SendEventNormailized inspector
												if (ev.name != displayNames [i])
														displayNames [i] = ((SendEventNormalized)mecanimNode.children [i]).name;
											
												int eventTimeValueSwitchInx = -1;

												
										}
								}

								//NOTES!!! I"ve gone with edit popup but I might draw Nodes here but think would move whole avatar and timeline preview down/up
								//if I draw them all or maybe just selected one(but what if many are selected ???) maybe I would draw it here as popup sucks

			

								

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