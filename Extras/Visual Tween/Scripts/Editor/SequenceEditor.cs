﻿using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using VisualTween.Action.Generic;
using UnityEditorInternal;
using ws.winx.editor;
using System.IO;
using ws.winx.editor.utilities;
using ws.winx.editor.extensions;
using ws.winx.unity.utilities;

namespace VisualTween
{
		public class SequenceEditor : EditorWindow
		{
				
				private static Sequence __sequence;
				private Sequence.SequenceWrap wrap = Sequence.SequenceWrap.ClampForever;
				private static GameObject __sequenceGameObject;
				private Vector2 settingsScroll;
				private bool dragNode;
				private float timeClickOffset;
				private bool resizeNodeStart;
				private bool resizeNodeEnd;
				private bool stop;
				//private bool playForward;
				//private float time;
				private static TimeAreaW __timeAreaW;
				private static SequenceNode __testNode;
				private static ReorderableList __sequenceChannelsReordableList;
				//private static bool __isPlaying;
				private static bool __isRecording;
				private static int __frameRate = 30;
				private const float NODE_RECT_HEIGHT = 40f;

				//20f for timer ruller + 20f for Events Pad
				private const float TIME_LABEL_HEIGHT = 20f;
				private const float EVENT_PAD_HEIGHT = 20f;
				private static GUIContent __frameRateGUIContent = new GUIContent ("fps:");
				string channelLabel;
		
				private static SequenceNode __nodeSelected {
						get {
								if (__sequence != null) {
										return __sequence.selectedNode;
								}
								return null;
						}
						set {
								__sequence.selectedNode = value;
						}
				}

				[MenuItem("Window/Visual Tween/Sequence", false)]
				public static void ShowWindow ()
				{
						SequenceEditor window = EditorWindow.GetWindow<SequenceEditor> (false, "Sequence");

						__sequence = Selection.activeGameObject.GetComponent<Sequence> ();
						__sequenceGameObject = Selection.activeGameObject;

						CreateNewReordableList ();
						window.wantsMouseMove = true;
						UnityEngine.Object.DontDestroyOnLoad (window);


	
				}

				private void OnEnable ()
				{




						if (__timeAreaW == null) {
								__timeAreaW = new TimeAreaW (false);

								
								__timeAreaW.hSlider = true;
								__timeAreaW.vSlider = false;
								__timeAreaW.vRangeLocked = true;
								__timeAreaW.hRangeMin = 0f;
								//__timeAreaW.hRangeMax=3f;

								__timeAreaW.margin = 0f;
								
								__timeAreaW.scaleWithWindow = true;
							
								//__timeAreaW.ignoreScrollWheelUntilClicked = false;
								__timeAreaW.hTicks.SetTickModulosForFrameRate (__frameRate);

								this.Repaint ();
			
								
			
						}

					



						EditorApplication.playmodeStateChanged += OnPlayModeStateChange;
				}



				/// <summary>
				/// Dos the node.
				/// </summary>
				/// <param name="node">Node.</param>
				/// <param name="rect">Rect.</param>
				/// <param name="channelOrd">Channel ord.</param>
				private static void DoNode (SequenceNode node, Rect rect, int channelOrd)
				{
						

						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.startTime, rect) - 5, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT), MouseCursor.ResizeHorizontal);			
						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - 5, TIME_LABEL_HEIGHT + channelOrd * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT), MouseCursor.ResizeHorizontal);

			
			
						float x = __timeAreaW.TimeToPixel (node.startTime, rect);
						Rect boxRect = new Rect (x, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT, __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - x, NODE_RECT_HEIGHT);

						GUI.Box (boxRect, "", "TL LogicBar 0");

						boxRect.xMin += 5;
						boxRect.xMax -= 5;
						EditorGUIUtility.AddCursorRect (boxRect, MouseCursor.Pan);

			
						GUIStyle style = new GUIStyle ("Label");
						style.fontSize = (__nodeSelected == node ? 12 : style.fontSize);
						style.fontStyle = (__nodeSelected == node ? FontStyle.Bold : FontStyle.Normal);
						Color color = style.normal.textColor;
						color.a = (__nodeSelected == node ? 1.0f : 0.7f);
						style.normal.textColor = color;
						
						//calc draw name of the node
						Vector3 size = style.CalcSize (new GUIContent (node.name));
						Rect rect1 = new Rect (boxRect.x + boxRect.width * 0.5f - size.x * 0.5f, boxRect.y + boxRect.height * 0.5f - size.y * 0.5f, size.x, size.y);
						GUI.Label (rect1, node.name, style);
			

			
				}



				///////////////////// HANDLERS OF CHANNEL LIST ////////////////////


				/// <summary>
				/// Ons the reorder sequence channel callback.
				/// </summary>
				/// <param name="list">List.</param>
				static void onReorderSequenceChannelCallback (ReorderableList list)
				{
						//	Debug.Log (list.index);

//						SequenceChannel channel = null;
//						int channelsNumber = __sequence.channels.Count;
//
//						for (int i=0; i<channelsNumber; i++) {
//								channel = __sequence.channels [i];
//								channel.nodes.ForEach (itm => itm.channelOrd = i);
//						}

						
						
				}
		
			
		
		
			
		
		
				/// <summary>
				/// Handles draw list element.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="index">Index.</param>
				/// <param name="isActive">If set to <c>true</c> is active.</param>
				/// <param name="isFocused">If set to <c>true</c> is focused.</param>
				private static void onDrawSequenceChannelElement (Rect rect, int index, bool isActive, bool isFocused)
				{
						SequenceChannel channel = __sequenceChannelsReordableList.list [index] as SequenceChannel;
						Rect temp = rect;
						

		
						

						if (channel.target != null) {
								temp.yMin = rect.yMax - 16f;
								temp.yMax = rect.yMax;

								EditorGUI.LabelField (temp, channel.target.name, EditorStyles.miniLabel);

						}
						int fontSize = 16;
						temp.yMin = rect.yMin + (rect.height - fontSize) * 0.5f;
						temp.yMax = rect.yMax;// temp.yMin + fontSize;

						GUIStyle styleGUI = new GUIStyle (EditorStyles.label);
						styleGUI.fontSize = fontSize;
						
						channel.name = EditorGUI.TextField (temp, channel.name, styleGUI);



			
				}

				
		
			
		
				/// <summary>
				/// Handles the select list item event.
				/// </summary>
				/// <param name="list">List.</param>
				static void onSelectSequenceChannelCallback (ReorderableList list)
				{

						//Debug.Log ("Select " + list.index);
//			GameObject rootGameObject = (list.list as EditorClipBinding[]) [list.index].gameObject;
//			GameObject rootFirstChildGameObject;
//			
//			if (rootGameObject != null && rootGameObject.transform.childCount>0 && (rootFirstChildGameObject = rootGameObject.transform.GetChild (0).gameObject) != null) {
//				Selection.activeGameObject = rootFirstChildGameObject;
//				
//				
//				EditorWindow.GetWindow<SearchableEditorWindow> ().Focus ();
//			}
				}
		
				/// /////////////////////////////////////////////////////////////////////


				private void OnAddEvent ()
				{
						EditorUtility.DisplayDialog ("Not implemented!", "Events are not availible in this version.", 
			                            "Close");			
				}

				private void OnPlayModeStateChange ()
				{
						if (EditorApplication.isPlaying) {


								//timeline.isRecording = true;
								//__sequence.isPlaying = true;
								//StartRecord ();
						} else {
								//StopRecord ();
								//OnPlay (false);
								//timeline.isRecording = false;
								//__sequence.isPlaying = false;

								//List<Sequence> sequences = GameObjectUtilityEx.FindAllContainComponentOfType<Sequence> ();
								//sequences.ForEach (itm => itm.Stop (__sequence.playForward));

						}
				}

				private void Update ()
				{

						//Debug.Log ("Update SE");

						//GameObject target = __sequence.channels [1].target;



						if (__sequenceGameObject != null) {
								__sequence = __sequenceGameObject.GetComponent<Sequence> ();
						}
				
						if (__sequence != null && __sequence.isPlaying) {
								
								__sequence.UpdateSequence (EditorApplication.timeSinceStartup);

								//this ensure update of MovieTexture (it its bottle neck do some reflection and force render call)
								//GetWindow<SceneView>().Repaint();
								SceneView.RepaintAll ();
						}

				}

//				private void Update ()
//				{
//						if (__sequenceGameObject != null) {
//								__sequence = __sequenceGameObject.GetComponent<Sequence> ();
//						}
//
//						if (__sequence != null) {
//
//								//remove all with channels without target
//								__sequence.channels.RemoveAll (x => x.target == null);
//
////								if (!__sequence.nodes.Contains (__nodeSelected)) {
////										__nodeSelected = null;
////								}
////								if (!EditorApplication.isPlaying) {
////										if (lastRecordState) {
////												EditorUpdate (timeline.CurrentTime, false);
////										}
////								}
//
//								if (__sequence.lastPlayState && !stop) {
//										if ((float)EditorApplication.timeSinceStartup > __sequence.time) {
//												switch (wrap) {
//												case Sequence.SequenceWrap.PingPong:
//														__sequence.playForward = !__sequence.playForward;
//														__sequence.time = (float)EditorApplication.timeSinceStartup + __sequence.endTime;
//														if (__sequence.playForward) {
//																timeline.CurrentTime = 0;
//																playStartTime = (float)EditorApplication.timeSinceStartup;
//														}
//														break;
//												case Sequence.SequenceWrap.Once:
//														__sequence.Stop (false);
//														playStartTime = (float)EditorApplication.timeSinceStartup;
//														timeline.CurrentTime = 0;
//														stop = true;
//														break;
//												case Sequence.SequenceWrap.ClampForever:
//														__sequence.Stop (true);
//														stop = true;
//														break;
//												case Sequence.SequenceWrap.Loop:
//														__sequence.Stop (false);
//														playStartTime = (float)EditorApplication.timeSinceStartup;
//														timeline.CurrentTime = 0;
//														stop = false;
//														time = (float)EditorApplication.timeSinceStartup + endTime ();
//														break;
//												}		
//										}
//
//										timeline.CurrentTime = (playForward ? ((float)EditorApplication.timeSinceStartup - playStartTime) : time - (float)EditorApplication.timeSinceStartup);
//
//										EditorUpdate (timeline.CurrentTime, false);
//										Repaint ();
//								}
//
//								if (EditorApplication.isPlaying) {
//										timeline.CurrentTime = __sequence.passedTime;
//										Repaint ();
//								}
//						} else {
//								__sequence = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<Sequence> () : null;			
//						}
//				}

				

				public void EditorUpdate (float time, bool forceUpdate)
				{
//						__sequence.nodes = __sequence.nodes.OrderBy (x => x.startTime).ToList ();
			
//						foreach (var kvp in GetGroupTargets()) {
//								SequenceNode mNode = kvp.Value.OrderBy (x => x.startTime).First ();
//								mNode.RecordAction ();
//						}

//						foreach(SequenceChannel channel in __sequence.channels)
//							foreach (SequenceNode node in channel.nodes) {
//									if (((time - node.startTime) / node.duration) > 0.0f && ((time - node.startTime) / node.duration) < 1.0f || forceUpdate && ((time - node.startTime) / node.duration) > 0.0f) {
//											node.StartTween ();
//									}
//					
//									node.UpdateNode (time);
//					
//									if (((time - node.startTime) / node.duration) < 0.0f || ((time - node.startTime) / node.duration) > 1.0f || forceUpdate) {
//											node.CompleteTween ();
//									}
//							}
			
//						foreach (var kvp in GetGroupTargets()) {
//								SequenceNode mNode = kvp.Value.OrderBy (x => x.startTime).ToList ().Find (y => ((time - y.startTime) / y.duration) < 0.0f);
//								if (mNode != null)
//										mNode.UndoAction ();
//						}
			
				}

				private void OnPlay (bool isPlaying)
				{
						if (EditorApplication.isPlaying) {
								Debug.Log ("You can't stop preview playing in play mode.");
								__sequence.isPlaying = true;
								return;			
						}

						if (isPlaying) {
								//AudioUtilW.PlayClip(
								//__sequence.Play(AudioUtilW.PlayClip);
								__sequence.Play (EditorApplication.timeSinceStartup);

								__sequence.SequenceNodeStart -= onSequenceNodeStart;
								__sequence.SequenceNodeStart += onSequenceNodeStart;


								
						} else {
								__sequence.Stop (__sequence.playForward);
						}

						


						//lastPlayState = isPlaying;	

						//__sequence.playStartTime = (float)EditorApplication.timeSinceStartup;
						//__sequence.time = (float)EditorApplication.timeSinceStartup + __sequence.endTime;
						//timeline.CurrentTime = 0;

//						if (isPlaying) {
//								stop = false;
//								__sequence.playForward = true;
//								//StartRecord ();	
//						} else {
//								//StopRecord ();			
//						}
				}

				private void OnTimelineClick (float time)
				{
//						if (EditorApplication.isPlaying) {
//								Debug.Log ("You can't change time in play mode.");
//								//timeline.isRecording = true;
//								return;			
//						}

//						if (lastPlayState) {
//								__sequence.isPlaying = false;
//								OnPlay (false);
//								timeline.CurrentTime = time;
//						}

//						if (!lastRecordState) {
//								StartRecord ();
//						}
		

						//timeline.isRecording = true;
//						if (__sequence != null) {
//								EditorUpdate (time, true);
//						}
				}

			

			


				/// <summary>
				/// Handles OnGUI event
				/// </summary>
				private void OnGUI ()
				{
						if (__sequence == null)
								return;
						//timeline.DoTimeline (new Rect(0,0,this.position.width,this.position.height));

						Rect rect = this.position;



						rect.x = this.position.width * 0.2f;
						rect.width = this.position.width * 0.8f;//80% for timeArea

						rect.y = 0;

						

						OnTimelineGUI (rect, __frameRate);

						rect.width = rect.x;//20% for settings
						rect.x = 0;
						rect.y = 0;
			
						DoChannelsGUI (rect);

						
				}



				/// <summary>
				/// Sequences the drop channel target event handler.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="channel">Channel.</param>
				static void sequenceDropChannelTargetEventHandler (Rect rect, int channel)
				{
						Event evt = Event.current;


						
						switch (evt.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
								if (!rect.Contains (evt.mousePosition))
										return;
								DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							
								Type draggedType;
							
								if (evt.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag ();
										if (EditorApplication.isPlaying) {
												Debug.Log ("Can't add tween object in play mode. Stop the play mode and readd it.");
										} else {
												GameObject target = DragAndDrop.objectReferences [0] as GameObject;

												if (target != null) {

														__sequence.channels [channel].target = target;
												}
										}




								}
								break;
						}
				}

				/// <summary>
				/// Dos the toolbar GUI.
				/// </summary>
				/// <param name="rect">Rect.</param>
				void DoChannelsGUI (Rect rect)
				{
						GUIStyle style = new GUIStyle ("ProgressBarBack");
						style.padding = new RectOffset (0, 0, 0, 0);
						
						GUILayout.BeginArea (rect, GUIContent.none, style);
			
						GUILayout.BeginHorizontal (EditorStyles.toolbar);
						GUI.backgroundColor = __isRecording ? Color.red : Color.white;
						if (GUILayout.Button (EditorGUIUtility.FindTexture ("d_Animation.Record"), EditorStyles.toolbarButton)) {
								__isRecording = !__isRecording;
//				if(onRecord != null){
//					onRecord();
//				}
						}
						GUI.backgroundColor = Color.white;
			
						if (GUILayout.Button (__sequence.isPlaying ? EditorGUIUtility.FindTexture ("d_PlayButton On") : EditorGUIUtility.FindTexture ("d_PlayButton"), EditorStyles.toolbarButton)) {
								__sequence.isPlaying = !__sequence.isPlaying;

								//AudioUtilW.PlayClip(__sequence.channels[0].nodes[0].source as AudioClip);
								OnPlay (__sequence.isPlaying);

						}
						GUILayout.FlexibleSpace ();
						if (GUILayout.Button (EditorGUIUtility.FindTexture ("d_Animation.AddEvent"), EditorStyles.toolbarButton)) {
//				if(onAddEvent!= null){
//					onAddEvent();
//				}
						}
						GUILayout.EndHorizontal ();
			
						GUILayout.BeginHorizontal ();
						GUILayout.BeginVertical ();

						OnSettingsGUI (rect.width - 1.5f);

						Rect rectLastControl = GUILayoutUtility.GetLastRect ();

						rect.yMin = rectLastControl.yMax;

						//Draw Channels
						if (__sequenceChannelsReordableList != null)
								__sequenceChannelsReordableList.DoLayoutList ();


						rectLastControl = GUILayoutUtility.GetLastRect ();
						rect.yMax = rectLastControl.yMax - 16f;//16f for +/- buttons


						sequenceDropChannelTargetEventHandler (rect, (int)((Event.current.mousePosition.y - rect.y) / NODE_RECT_HEIGHT));

						GUILayout.EndVertical ();
						GUILayout.Space (1.5f);
						GUILayout.EndHorizontal ();



						


						GUILayout.EndArea ();
			
				}


				/// <summary>
				/// Handles drawing settings OnGUI .
				/// </summary>
				/// <param name="width">Width.</param>
				private void OnSettingsGUI (float width)
				{
						GUILayout.BeginHorizontal ();
						if (GUILayout.Button (__sequence != null ? __sequence.name : "[None Selected]", EditorStyles.toolbarDropDown, GUILayout.Width (width * 0.3f))) {
								GenericMenu toolsMenu = new GenericMenu ();
				
								List<Sequence> sequences = GameObjectUtilityEx.FindAllContainComponentOfType<Sequence> ();
								foreach (Sequence sequence in sequences) {
										toolsMenu.AddItem (new GUIContent (sequence.name), false, OnGameObjectSelectionChanged, sequence);
								}
								toolsMenu.AddItem (new GUIContent ("[New Sequence]"), false, CreateNewSequence);
								toolsMenu.AddItem (new GUIContent ("[New Animation Clip]"), false, CreateNewAnimationClip);
				
								toolsMenu.DropDown (new Rect (3, 37, 0, 0));
								EditorGUIUtility.ExitGUI ();
						}
			
						if (__sequence != null) {
								wrap = __sequence.wrap;			
						}
						wrap = (Sequence.SequenceWrap)EditorGUILayout.EnumPopup (wrap, EditorStyles.toolbarDropDown, GUILayout.Width (width * 0.4f));
						if (__sequence != null) {
								__sequence.wrap = wrap;			
						}


						//Handle FRAMERATE input and changes		
						EditorGUI.BeginChangeCheck ();

						EditorGUILayout.LabelField (__frameRateGUIContent, GUILayout.Width (width * 0.1f));
						__frameRate = Mathf.Max (EditorGUILayout.IntField (__frameRate, GUILayout.Width (width * 0.2f)), 1);
			
						if (EditorGUI.EndChangeCheck ()) {

								float duration = 0f;
								foreach (SequenceChannel channel in __sequence.channels)
										foreach (SequenceNode n in channel.nodes) {
										
												if (n.source is AnimationClip) {
									
												
														(n.source as AnimationClip).frameRate = __frameRate;
												} 

												//resnap to new framerate
												n.duration = TimeAreaW.SnapTimeToWholeFPS (n.duration, __frameRate);

					
										}

								//now tick drawn on new frame rate
								__timeAreaW.hTicks.SetTickModulosForFrameRate (__frameRate);

						}
			

						GUILayout.EndHorizontal ();






				}

				private void OnEventGUI (Rect rect)
				{
				
				}



				/// <summary>
				/// Raises the timeline GU event.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="frameRate">Frame rate.</param>
				private void OnTimelineGUI (Rect rect, float frameRate)
				{
						Handles.color = new Color (0.5f, 0.5f, 0.5f, 0.2f);
					

						int rows = __sequence.channels.Count + 1;
						int y = 0;
						float lineY = 0f;
						for (; y<rows; y++) {
								//Handles.DrawLine (new Vector3 (0, y, 0), new Vector3 (rect.width, y, 0));	
								lineY = TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + y * NODE_RECT_HEIGHT;
								Handles.DrawLine (new Vector3 (rect.xMin, lineY, 0), new Vector3 (rect.xMax, lineY, 0));	
								
								
						}

										

						//make all area from end of label and Event pad to botton droppable
						sequenceDropSourceEventHandler (new Rect (rect.x, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.width, rect.height - TIME_LABEL_HEIGHT - EVENT_PAD_HEIGHT), Math.Min ((int)((Event.current.mousePosition.y - rect.y - TIME_LABEL_HEIGHT - EVENT_PAD_HEIGHT) / NODE_RECT_HEIGHT), __sequence.channels.Count));
			
						Handles.color = Color.white;

						//TimeArea
						__timeAreaW.DoTimeArea (rect, __frameRate);

					
						if (Event.current.type == EventType.Repaint) 
								EditorGUILayoutEx.ANIMATION_STYLES.eventBackground.Draw (new Rect (rect.x, EVENT_PAD_HEIGHT, rect.width, EVENT_PAD_HEIGHT), GUIContent.none, 0);

						//DRAW NODES 
						int channelNumber = __sequence.channels.Count;
						SequenceChannel channel = null;
						for (int i=0; i<channelNumber; i++) {
								channel = __sequence.channels [i];
								foreach (SequenceNode node in channel.nodes) {
									
										

										sequenceNodeClickEventHandler (node, rect, i);
										DoNode (node, rect, i);
								}
						}

						//check timeline click
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && new Rect (rect.x, 0, rect.width, TIME_LABEL_HEIGHT).Contains (Event.current.mousePosition)) {
								
									
								//OnTimelineClick(__timeAreaW.TimeToPixel(time,rect));
								Debug.Log ("Click on TimeLine");
								Event.current.Use ();
						}
						


						sequenceNodeDragEventHandler (rect);
				}

				private void sequenceNodeClickEventHandler (SequenceNode node, Rect rect, int channelOrd)
				{

						Event ev = Event.current;
						Rect clickRect;
			
						float startTime;
						switch (ev.rawType) {
						case EventType.MouseDown:
				
			
					
								clickRect = new Rect (__timeAreaW.TimeToPixel (node.startTime, rect) - 5, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT);
					
								//check start of the node rect width=5
								if (clickRect.Contains (Event.current.mousePosition)) {
										
										__nodeSelected = node;
										resizeNodeStart = true;
										ev.Use ();
								}
					
								clickRect.x = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - 5;
					
					
								//check the end of node rect width=5
								if (clickRect.Contains (Event.current.mousePosition)) {
										__nodeSelected = node;
										resizeNodeEnd = true;
										ev.Use ();
								}
					
					
								clickRect.x = __timeAreaW.TimeToPixel (node.startTime, rect) + 5;
								clickRect.width = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - clickRect.x - 5;
					
								if (clickRect.Contains (Event.current.mousePosition)) {
										
										if (ev.button == 0) {
												//timeClickOffset = node.startTime - timeline.GUIToSeconds (Event.current.mousePosition.x);
												timeClickOffset = node.startTime - __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
												dragNode = true;
												__nodeSelected = node;
										} 
										if (ev.button == 1) {
												GenericMenu genericMenu = new GenericMenu ();
												genericMenu.AddItem (new GUIContent ("Remove"), false, this.RemoveNode, node);
												genericMenu.ShowAsContext ();
										}
										ev.Use ();
								}

								break;
				
						case EventType.ContextClick:
				
								clickRect = new Rect (0, 0, 0, NODE_RECT_HEIGHT);
				

					
								clickRect.x = __timeAreaW.TimeToPixel (node.startTime, rect);
								clickRect.y = TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT;
								clickRect.width = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - clickRect.x;
					
								if (clickRect.Contains (Event.current.mousePosition)) {
										GenericMenu genericMenu = new GenericMenu ();
										genericMenu.AddItem (new GUIContent ("Remove"), false, this.RemoveNode, node);
										genericMenu.ShowAsContext ();
								}

								break;
						}



				}


				/// <summary>
				/// Handles the events, resize, pan.
				/// </summary>
				/// <param name="rect">Rect.</param>
				private void sequenceNodeDragEventHandler (Rect rect)
				{
						Event ev = Event.current;
						Rect clickRect;

						float startTime;
						switch (ev.rawType) {

						case EventType.MouseDrag:
								if (resizeNodeStart) {
										//selectedNode.startTime = timeline.GUIToSeconds (Event.current.mousePosition.x);
										float prevStartTime = __nodeSelected.startTime;
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
										

										if (startTime >= 0) {
												__nodeSelected.startTime = TimeAreaW.SnapTimeToWholeFPS (startTime, __frameRate);
												__nodeSelected.duration += TimeAreaW.SnapTimeToWholeFPS (prevStartTime - __nodeSelected.startTime, __frameRate);
												
												ev.Use ();
										}
										//selectedNode.duration += __timeAreaW.PixelToTime (Event.current.mousePosition.x+ev.delta.x, rect)-selectedNode.startTime;
										//selectedNode.duration -= timeline.GUIToSeconds (ev.delta.x);

										
								}

								if (resizeNodeEnd) {
										//selectedNode.duration = (timeline.GUIToSeconds (Event.current.mousePosition.x) - selectedNode.startTime);
										__nodeSelected.duration = TimeAreaW.SnapTimeToWholeFPS (__timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) - __nodeSelected.startTime, __frameRate);
										ev.Use ();
								}

								if (dragNode && !resizeNodeStart && !resizeNodeEnd) {
										//selectedNode.startTime = timeline.GUIToSeconds (Event.current.mousePosition.x) + timeClickOffset;
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) + timeClickOffset;
										if (startTime >= 0)
												__nodeSelected.startTime = TimeAreaW.SnapTimeToWholeFPS (startTime, __frameRate);
										

										//change channel 
//										if (Event.current.mousePosition.y > selectedNode.channel * __nodeRectHeight + 25) {
//												selectedNode.channel += 1;
//										}
//										if (Event.current.mousePosition.y < selectedNode.channel * __nodeRectHeight - 5) {
//												selectedNode.channel -= 1;
//										}
//										selectedNode.channel = Mathf.Clamp (selectedNode.channel, 0, int.MaxValue);
										ev.Use ();
								}
								break;
						case EventType.MouseUp:
								dragNode = false;
								resizeNodeStart = false;
								resizeNodeEnd = false;
								break;
						}
				}





				/// <summary>
				/// Removes the node.
				/// </summary>
				/// <param name="data">Data.</param>
				private void RemoveNode (object data)
				{
						SequenceNode node = data as SequenceNode;
						


						SequenceChannel sequenceChannel = __sequence.channels.Find (itm => itm.nodes.Exists (nd => nd.GetInstanceID () == node.GetInstanceID ()));

						//remove node
						sequenceChannel.nodes.Remove (node);



						//if this removed node was last node => remove channel and move channels up(reindex)
						if (sequenceChannel.nodes.Count == 0) {

								//remove channel
								__sequence.channels.Remove (sequenceChannel);



						}

						

						Repaint ();
				}



				/// <summary>
				/// Raises the game object selection changed event.
				/// </summary>
				/// <param name="data">Data.</param>
				private void OnGameObjectSelectionChanged (object data)
				{
						Sequence sequence = data as Sequence;
						Selection.activeGameObject = sequence.gameObject;
						__sequenceGameObject = sequence.gameObject;
						__sequence = sequence;
				}


				/// <summary>
				/// Sequences the drop source event handler.
				/// </summary>
				/// <param name="area">Area.</param>
				/// <param name="channel">Channel.</param>
				public void sequenceDropSourceEventHandler (Rect area, int channel)
				{
			
						Event evt = Event.current;
			
						switch (evt.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
								if (!area.Contains (evt.mousePosition))
										return;
								DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

								Type draggedType;
				
								if (evt.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag ();
										if (EditorApplication.isPlaying) {
												Debug.Log ("Can't add tween object in play mode. Stop the play mode and readd it.");
										} else {
												foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences) {

														//Debug.Log ("Dropped object of type:" + dragged_object);
														
														//handle sound, video, animation clip
														draggedType = dragged_object.GetType ();



														if (draggedType == typeof(UnityEngine.AnimationClip) || 
																draggedType == typeof(UnityEngine.AudioClip) ||
																draggedType == typeof(UnityEngine.MovieTexture)) {


																//allow only dropping multiply items of same type in same channel
																SequenceChannel sequenceChannel = null;

																
																if (channel < __sequence.channels.Count) {
																		sequenceChannel = __sequence.channels [channel];
																} else {
																		sequenceChannel = new SequenceChannel ();



										
																		if (draggedType == typeof(AnimationClip)) {
																				sequenceChannel.name = "Animation";
																		} else if (draggedType == typeof(AudioClip)) {
																				sequenceChannel.name = "Audio";
																		} else if (draggedType == typeof(MovieTexture)) {
																				sequenceChannel.name = "Video";
																		}


																		__sequence.channels.Add (sequenceChannel);
																}

																if (sequenceChannel.nodes.Count > 0) {
																		if (sequenceChannel.nodes [0].source.GetType () != draggedType) {
																				Debug.LogWarning ("You can have only same type nodes in same channel");	
																				continue;
																		}
																}


																//Vector2
																SequenceNode node = CreateNewSequenceNode (Event.current.mousePosition, dragged_object, channel);
														

																__sequence.channels [channel].nodes.Add (node);
																__nodeSelected = node;
														}
												}
												EditorUtility.SetDirty (__sequence);
										}
								}
								break;
						}
			
				}
				

				/// <summary>
				/// Creates the sequence node.
				/// </summary>
				/// <returns>The sequence node.</returns>
				/// <param name="pos">Position.</param>
				/// <param name="source">Source.</param>
				/// <param name="channel">Channel.</param>
				private SequenceNode CreateNewSequenceNode (Vector2 pos, UnityEngine.Object source, int channelOrd)
				{
						SequenceNode node = ScriptableObject.CreateInstance<SequenceNode> ();
						
						float duration = node.duration;
						node.source = source;
						if (source is AnimationClip) {
								
								duration = (source as AnimationClip).length;
						} else if (source is AudioClip) {
								
								duration = (source as AudioClip).length;
						} else if (source is MovieTexture) {
								
								duration = (source as MovieTexture).duration;


						}

						//node.onStart = new UnityEngine.Events.UnityEvent ().AddListener (new UnityEngine.Events.UnityAction (this, this.GetType ().GetMethod ("fdsaffa").MethodHandle.GetFunctionPointer ()));
						
						
					
						node.name = source.name;
						
						//node.channelOrd = channelOrd;

						node.channel = __sequence.channels [channelOrd];
					
						node.startTime = TimeAreaW.SnapTimeToWholeFPS (__timeAreaW.PixelToTime (pos.x, __timeAreaW.rect), __frameRate);
						node.duration = TimeAreaW.SnapTimeToWholeFPS (duration, __frameRate);
						
			
						return node;
				}

				
				public static void onSequenceNodeStart (SequenceNode node)
				{
						GameObject target = node.channel.target;
						UnityEngine.Object source = node.source;
						AudioClip audioClip;
					
			
						if (target != null) {

								if (source is AudioClip) {
										audioClip = source as AudioClip;


										//AudioUtilW.PlayClip (audioClip, 0, node.loop);
					

					
					
					
					
								} else if (source is MovieTexture) {
										Renderer renderer = target.GetComponent<Renderer> ();
										if (renderer != null) {
												MovieTexture movieTexture = (source as MovieTexture);
						


							
												audioClip = movieTexture.audioClip;
							
												if (audioClip != null)
														AudioUtilW.PlayClip (audioClip, 0, node.loop);
							
										}
						
						
						
						
						
						
								} else
										Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
						} else if (source is  AnimationClip) {
								//										Animator animator = target.GetComponent<Animator> ();
								//										
								//										if (animator){
								//												animator.enabled=true;
								//												animator.CrossFade (stateNameHash, 0f, 0, 0f);
								//				}
					
					
					
						}

				}




		public static void onSequenceNodeEnd (SequenceNode node)
		{
			GameObject target = node.channel.target;
			UnityEngine.Object source = node.source;
			AudioClip audioClip;
			
			
			if (target != null) {
				
				if (source is AudioClip) {
					audioClip = source as AudioClip;
					AudioUtilW.PlayClip (audioClip, 0, node.loop);
			
					
				} else if (source is MovieTexture) {
					Renderer renderer = target.GetComponent<Renderer> ();
					if (renderer != null) {
						MovieTexture movieTexture = (source as MovieTexture);
						
						
						
						
						audioClip = movieTexture.audioClip;
						
						if (audioClip != null)
							AudioUtilW.PlayClip (audioClip, 0, node.loop);
						
					}
					
					
					
					
					
					
				} else
					Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
			} else if (source is  AnimationClip) {
				//										Animator animator = target.GetComponent<Animator> ();
				//										
				//										if (animator){
				//												animator.enabled=true;
				//												animator.CrossFade (stateNameHash, 0f, 0, 0f);
				//				}
				
				
				
			}
			
		}

				private static void CreateNewReordableList ()
				{
						if (__sequenceChannelsReordableList == null) {
								__sequenceChannelsReordableList = new ReorderableList (__sequence.channels, typeof(SequenceNode), true, false, false, false);
								__sequenceChannelsReordableList.elementHeight = NODE_RECT_HEIGHT;
								__sequenceChannelsReordableList.headerHeight = 2f;
				
								__sequenceChannelsReordableList.drawElementCallback = onDrawSequenceChannelElement;

								__sequenceChannelsReordableList.onSelectCallback = onSelectSequenceChannelCallback;
								__sequenceChannelsReordableList.onReorderCallback = onReorderSequenceChannelCallback;
						} else
								__sequenceChannelsReordableList.list = __sequence.channels;
								

				}
				




				/// <summary>
				/// Creates the new animation clip.
				/// </summary>
				private static void CreateNewAnimationClip ()
				{

						string path = EditorUtility.SaveFilePanel (
					"Create New Clip",
					"Assets",
					"",
					"anim");

						if (!String.IsNullOrEmpty (path)) {
				
								AnimationClip clip = new AnimationClip ();//UnityEditor.Animations.AnimatorController.AllocateAnimatorClip ();
								clip.name = Path.GetFileNameWithoutExtension (path);		
								AssetDatabase.CreateAsset (clip, AssetDatabaseUtility.AbsoluteUrlToAssets (path));
								AssetDatabase.SaveAssets ();
								clip.frameRate = __frameRate;
					
						}
		
				}


					
		
		
				////   CREATE NEW SEQUENCE GAME OBJECT WITH SEQUENCE BEHAVIOUR ////
				/// <summary>
				/// Creates the new sequence.
				/// </summary>
				private void CreateNewSequence ()
				{
						
						List<Sequence> sequences = GameObjectUtilityEx.FindAllContainComponentOfType<Sequence> ();
						int count = 0;

						while (sequences.Find(x=>x.name == "Sequence "+count.ToString())!=null) {
								count++;
						}

						__sequenceGameObject = new GameObject ("Sequence " + count.ToString ());
						__sequence = __sequenceGameObject.AddComponent<Sequence> ();

						//add Animator component
						__sequenceGameObject.AddComponent<Animator> ();


						Selection.activeGameObject = __sequenceGameObject;
				}



//		int num = AudioUtil.GetSampleCount (audioClip) / (int)r.width;
//		switch (current.type)
//		{
//		case EventType.MouseDown:
//		case EventType.MouseDrag:
//			if (r.Contains (current.mousePosition) && !AudioUtil.IsMovieAudio (audioClip))
//			{
//				if (this.m_PlayingClip != audioClip)
//				{
//					AudioUtil.StopAllClips ();
//					AudioUtil.PlayClip (audioClip, 0, AudioClipInspector.m_bLoop);
//					this.m_PlayingClip = audioClip;
//				}
//				AudioUtil.SetClipSamplePosition (audioClip, num * (int)current.mousePosition.x);
//				current.Use ();
//			}
//			break;
//		}

		
		
		public static Texture2D GetAudioClipTexture(AudioClip clip, float width, float height)
		{
			if (clip == null)
			{
				return null;
			}
			AudioImporter audioImporter = (AudioImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip));

			Texture2D[] array = new Texture2D[clip.channels];
			for (int i = 0; i < clip.channels; i++)
			{
				array[i] = (Texture2D)AudioUtilW.GetWaveForm(
				                                    
					clip,
					audioImporter,
					i,
					width,
					height / (float)clip.channels
				);
			}
			return CombineWaveForms(array);
		}


		public static Texture2D CombineWaveForms(Texture2D[] waveForms)
		{
			if (waveForms.Length == 1)
			{
				return waveForms[0];
			}
			int width = waveForms[0].width;
			int num = 0;
			for (int i = 0; i < waveForms.Length; i++)
			{
				Texture2D texture2D = waveForms[i];
				num += texture2D.height;
			}
			Texture2D texture2D2 = new Texture2D(width, num, TextureFormat.ARGB32, false);
			int num2 = 0;
			for (int j = 0; j < waveForms.Length; j++)
			{
				Texture2D texture2D3 = waveForms[j];
				num2 += texture2D3.height;
				texture2D2.SetPixels(0, num - num2, width, texture2D3.height, texture2D3.GetPixels());
				GameObject.DestroyImmediate(texture2D3);
			}
			texture2D2.Apply();
			return texture2D2;
		}


				/// <summary>
				/// Handle the selection(gameobject) change event.
				/// </summary>
				private void OnSelectionChange ()
				{
						if (Selection.activeGameObject != null && Selection.activeGameObject != __sequenceGameObject) {
								Sequence sequence = Selection.activeGameObject.GetComponent<Sequence> ();
								if (sequence != null) {
										__sequenceGameObject = sequence.gameObject;
										__sequence = sequence;
										CreateNewReordableList ();
										Repaint ();
								}
						}
				}

				
		}
}