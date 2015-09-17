using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using ws.winx.editor;
using System.IO;
using ws.winx.editor.utilities;
using ws.winx.editor.extensions;
using ws.winx.unity.utilities;
using UnityEditor.Animations;
using ws.winx.unity;
using ws.winx.unity.sequence;
using ws.winx.ik;

namespace ws.winx.editor.windows
{





		/// <summary>
		/// Sequence editor window.
		/// !!! IMPORTANT
		/// 
		/// You need to select node(you are pointing which animation clip will contain records) in record mode then modify gameobject component values you want to record

		/// Can't mix vidoe sound and audio=> crazyness happening
		/// Can't stop all the channels with sounds (it stops just first StopAll doens't work seem all are on diff threads)
		/// Bake rotation and position can affect your injected bones animation so 
		/// for custome bone animation I used uncheked Bake rotation in pose
		/// 
		/// FFBIKAnimatedValues should be in channel after channels containing animation nodes manipulated by IK
		/// FFBIKAnimatedValues animation node should have been same or less length then animation node manipulated by IK
		/// If you animate target of IAnimatedValues double check if animation have saved it
		/// 
		/// changes of transform in channel.traget inspector doesn't trigger ReOffest and Resaving(use scene rot,pos,scl)
		/// 
		/// 
		/// TODOS:
		/// bug fix AnimationUtility.DeleteFrame got -1 index Set Editor
		/// 
		/// 
		/// </summary>
		public class SequenceEditorWindow : EditorWindow
		{

				
				private static Sequence __sequence;
				private static bool __isPlayMode = false;
				private Sequence.SequenceWrap wrap = Sequence.SequenceWrap.ClampForever;
				private static GameObject __sequenceGameObject;
				private static EditorCurveBinding __nodeSelectedSourceEditorCurveBinding;
				private static Vector2 __channelsTranslation;
		

				// Node interactivity
				private static bool __isDragged;
				private static bool __isEventDragged;
				private static float __clickDelta;
				private bool __wasResizingNodeStart;
				private bool __wasResizingNodeEnd;
			
				//private bool playForward;
				//private float time;
				private static TimeAreaW __timeAreaW;
				private static SequenceNode __testNode;
				private static ReorderableList __sequenceChannelsReordableList;
				private const float NODE_RECT_HEIGHT = 40f;
				//20f for timer ruller + 20f for Events Pad
				private const float TIME_LABEL_HEIGHT = 20f;
				private const float EVENT_PAD_HEIGHT = 20f;
				//private const float SCROLLER_SIZE =16f;
				private static GUIContent __frameRateGUIContent = new GUIContent ("fps:");
				private static GUIContent __nodeLabelGUIContent = new GUIContent ();
				private static GUIContent __nodeGUIContent = new GUIContent ();
				private static GUIContent __nodeTransitionGUIContent = new GUIContent ();

				//Keyframe
				private static float[] keyframeTimeValuesNormalized;
				private static float[] keyframeTimeValuesNormalizePrev;
				private static bool[] keyframeTimeValuesNormalizedSelected;
				private static string[] keyframesDisplayNames;
				private static GUIContent __keyframeMarker;

				//Event
				
				private static GUIContent __eventMarker;
				
				private static SequenceNode __nodeSelected {
						get {
								if (__sequence != null) {
										return __sequence.nodeSelected;
								}
								return null;
						}
						set {

								if (value == __nodeSelected)
										return;
								
								__sequence.nodeSelected = value;


								if (__sequence.nodeSelected.channel.type == SequenceChannel.SequenceChannelType.Animation) {
										__nodeSelectedSourceEditorCurveBinding = default(EditorCurveBinding);// EditorCurveBinding.FloatCurve ("", typeof(float), "");


				
										AnimationClip clip = __nodeSelected.source as AnimationClip;
								
										//if there is no curveBinding selected try to find at least one and select it as default

										EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings (clip);
										if (curveBindings != null && curveBindings.Length > 0) {
												__nodeSelectedSourceEditorCurveBinding = curveBindings.FirstOrDefault (itm => itm.type != typeof(Animator));
										}
					
								}



								EditorUtility.SetDirty (__sequence);
						}
				}


				//
				// Nested Types
				//
			

				private static SequenceEditorWindow __window;

				public static SequenceEditorWindow window {
						get {
								if (__window == null)
										__window = EditorWindow.GetWindow<SequenceEditorWindow> (false, "Sequence");
								return __window;
						}
				}
			
				[MenuItem("Window/Sequence", false)]
				public static void ShowWindow ()
				{
						__window = EditorWindow.GetWindow<SequenceEditorWindow> (false, "Sequence");

						

						if (__sequence != null) {
								__sequence = Selection.activeGameObject.GetComponent<Sequence> ();
								__sequenceGameObject = Selection.activeGameObject;

								
						
								if (__timeAreaW != null)
										__timeAreaW.hTicks.SetTickModulosForFrameRate (__sequence.frameRate);
							
						} else {
								selectionChangeEventHandler ();

						}

						

						__window.wantsMouseMove = true;
						UnityEngine.Object.DontDestroyOnLoad (__window);


	
				}



				/// <summary>
				/// Hanlde the enable event raized by Mono.
				/// </summary>
				private void OnEnable ()
				{


						

						if (__timeAreaW == null) {
								__timeAreaW = new TimeAreaW (false);

								
								__timeAreaW.hSlider = true;
								__timeAreaW.vSlider = true;
								__timeAreaW.vRangeMax = 1f;
								__timeAreaW.vRangeLocked = true;
								__timeAreaW.hRangeMin = 0f;
								

								__timeAreaW.margin = 0f;
								
								__timeAreaW.scaleWithWindow = true;
							
								//__timeAreaW.ignoreScrollWheelUntilClicked = false;
								if (__sequence != null)
										__timeAreaW.hTicks.SetTickModulosForFrameRate (__sequence.frameRate);
								else
										__timeAreaW.hTicks.SetTickModulosForFrameRate (30);

								this.Repaint ();
			
								
			
						}

						if (__sequenceChannelsReordableList == null) {

								__sequenceChannelsReordableList = new ReorderableList (new List<SequenceChannel> (), typeof(SequenceChannel), true, false, false, true);
								__sequenceChannelsReordableList.elementHeight = NODE_RECT_HEIGHT;
								__sequenceChannelsReordableList.headerHeight = 2f;
							
								__sequenceChannelsReordableList.drawElementCallback = onDrawSequenceChannelElement;
							
								__sequenceChannelsReordableList.onSelectCallback = onSelectSequenceChannelCallback;
								__sequenceChannelsReordableList.onReorderCallback = onReorderSequenceChannelCallback;
								__sequenceChannelsReordableList.onRemoveCallback = onRemoveSequenceChannelCallback;
					
					
					
						}
				
						
			
						EditorApplication.playmodeStateChanged -= OnPlayModeStateChange;
						EditorApplication.playmodeStateChanged += OnPlayModeStateChange;

						SceneView.onSceneGUIDelegate -= OnSceneGUI;
						SceneView.onSceneGUIDelegate += OnSceneGUI;
				}





				/// <summary>
				/// Ons the DragHandle event.
				/// </summary>
				/// <param name="controlID">Control I.</param>
				/// <param name="e">E.</param>
				/// <param name="userData">User data.</param>
				private static void onDragHandleEvent (int controlID, Event e, int keyframeInx)
				{
			if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace) && !String.IsNullOrEmpty(__nodeSelectedSourceEditorCurveBinding.propertyName)) {

								AnimationClip clip = __sequence.nodeSelected.source as AnimationClip;
				
				
				
								AnimationUtilityEx.RemoveKeyframeFrom (clip,__nodeSelectedSourceEditorCurveBinding, keyframeInx);
				
								window.Repaint ();
				
								//Debug.Log ("remove:" + bindingInx + " " + keyframeInx);
				
				
								if (SceneView.currentDrawingSceneView != null)
										SceneView.currentDrawingSceneView.Repaint ();
				
						} else if (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1) {


			
								AnimationClip clip = __sequence.nodeSelected.source as AnimationClip;

				
								//Debug.Log ("bind:" + bindingInx + " " + keyframeInx);
				
				
								//make animation jump to selected keyframe represented by Handle
								__sequence.timeCurrent = __sequence.nodeSelected.timeStart + AnimationUtilityEx.GetTimeAt (clip, keyframeInx, AnimationUtilityEx.EditorCurveBinding_PosX);


								if (!__sequence.isRecording) { 
										StartRecording ();
				
					
									
								}

								SampleClipNodesAt (__sequence.timeCurrent);
								
								if (SceneView.currentDrawingSceneView != null)
										SceneView.currentDrawingSceneView.Repaint ();
				
								window.Repaint ();
						}
			
				}
		
				/// <summary>
				/// Handles the scene GUI event.
				/// </summary>
				/// <param name="sceneView">Scene view.</param>
				private static void OnSceneGUI (SceneView sceneView)
				{

						List<SequenceChannel> channels;

						if (
								(Tools.current == Tool.Move || Tools.current == Tool.Rotate) && //Move or Rotate tool is selected
								Selection.activeGameObject != null && //something is selected
								currentlyDragging && //while drag something 
								__sequence != null && 
								!__sequence.isPlaying && !__sequence.isRecording && //Not playing or recording
						//__sequence.timeCurrent > 0f && //<< ????
								(channels = __sequence.channels.FindAll (itm => itm.type == SequenceChannel.SequenceChannelType.Animation && itm.target == Selection.activeGameObject)) != null

														
				) {  //active object is target of Animation type of channel
								
								foreach (SequenceChannel channel in channels) {

										//if change has been made to pos or rotation of the target => change orginal and save nodes animation position offest
										if (channel.targetPositionOriginal != channel.target.transform.position
												|| channel.targetRotationOriginal != channel.target.transform.rotation) {

//												if (__sequence.timeCurrent > 0f) {//paused animation at some point in time (target is moved/rotated)
//														Stop ();//stop "paused" and then reset
//														__sequence.timeCurrent = 0f;//rewind to begin and reset
//														ResetChannelTarget (channel);//target is reset to Original pos/rot before animation is applied
//												}
								
								
												Debug.Log ("Target object position have been changed");

												//take new transform
												channel.targetPositionOriginal = channel.target.transform.position;
												channel.targetRotationOriginal = channel.target.transform.rotation;


												if (!AnimationMode.InAnimationMode ())
														AnimationMode.StartAnimationMode ();
												ReOffsetNodesAnimation (channel);
												AnimationMode.StopAnimationMode ();
										}
								}
						}
			    


		



						Color clr = Color.red;
						Vector3[] positionsInKeyframe = null;
						


						if (Tools.current == Tool.Move && Selection.activeGameObject != null && __sequence != null 
								&& !__sequence.isPlaying
								&& __sequence.nodeSelected != null
								&& __sequence.nodeSelected.channel.type == SequenceChannel.SequenceChannelType.Animation
								&& __sequence.nodeSelected.channel.target != null 
								&& (Selection.activeGameObject == __sequence.nodeSelected.channel.target || Selection.activeGameObject.transform.IsChildOf (__sequence.nodeSelected.channel.target.transform))

			    ) {
					
								Transform transformFirstChild = Selection.activeGameObject.transform;

								Transform transformRoot = __sequence.nodeSelected.channel.target.transform;


								Transform transformSpace = transformRoot;


								if (transformRoot == transformFirstChild)
										transformSpace = null;//no need of transform from local to space defined with transformSpace

								AnimationClip clip = __sequence.nodeSelected.source as AnimationClip;

								positionsInKeyframe = AnimationUtilityEx.GetPositions (clip, AnimationUtility.CalculateTransformPath (transformFirstChild, transformRoot), transformSpace);
				
						
						
						
								if (positionsInKeyframe != null) {			
										for (int j=0; j<positionsInKeyframe.Length; j++) {
								
										
								
												//actualy this is static handle
												HandlesEx.DragHandle (positionsInKeyframe [j], 0.1f, Handles.SphereCap, Color.red, "(" + j + ")" + " " + Decimal.Round (Convert.ToDecimal (AnimationUtilityEx.GetTimeAt (clip, j, AnimationUtilityEx.EditorCurveBinding_PosX)), 2).ToString (), j, onDragHandleEvent, new GUIContent[]{new GUIContent ("Delete")}, new int[]{j}, null);
								
								
								
										}
										//save color
										clr = Handles.color;
							
							
								
							
										Handles.DrawPolyLine (positionsInKeyframe);
										//restore color
										Handles.color = clr;
								}
						
						}

//			
//			//Debug.Log ("hot"+GUIUtility.hotControl);
//			
//			SceneView.RepaintAll ();
				}

				private static bool currentlyDragging {
						get {
								return GUIUtility.hotControl != 0;
						}
				}
		
		
				/// <summary>
				/// Dos the node.
				/// </summary>
				/// <param name="node">Node.</param>
				/// <param name="rect">Rect.</param>
				/// <param name="channelOrd">Channel ord.</param>
				private static void DoNode (SequenceNode node, Rect rect, int channelOrd)
				{
						float keyFramesRectHeight = __nodeSelected == node ? __keyframeMarker.image.height : 0f;

						
						

						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.timeStart, rect) - 5, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT + keyFramesRectHeight, 10, NODE_RECT_HEIGHT - keyFramesRectHeight), MouseCursor.ResizeHorizontal);			
						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.timeStart + node.duration, rect) - 5, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT + keyFramesRectHeight, 10, NODE_RECT_HEIGHT - keyFramesRectHeight), MouseCursor.ResizeHorizontal);

			
			
						float startTimePos = __timeAreaW.TimeToPixel (node.timeStart + node.transition, rect);

						float endTimePos = __timeAreaW.TimeToPixel (node.timeStart + node.duration, rect);

						float startTransitionTimePos = 0f;
//						SequenceNode nodePrev = null;

						//if there is next node with transition
//						if (node.index + 1< node.channel.nodes.Count && (nodePrev = node.channel.nodes [node.index - 1]).transition > 0) {
//								
//								startTimePos = __timeAreaW.TimeToPixel (node.startTime + nodePrev.transition, rect);
//								
//						}

						//find curveBinding property path and name
						String propertyAnimated = String.Empty;
						String propertyName = String.Empty;
						
						if (__nodeSelected == node && __nodeSelected.channel.target != null && __nodeSelected.channel.type == SequenceChannel.SequenceChannelType.Animation) {
								if (!String.IsNullOrEmpty (__nodeSelectedSourceEditorCurveBinding.propertyName)) {

									int backSlashIndx=__nodeSelectedSourceEditorCurveBinding.path.LastIndexOf("/");

									if(backSlashIndx>-1)
										propertyName=__nodeSelectedSourceEditorCurveBinding.path.Substring(backSlashIndx+1);
									else
										propertyName=__nodeSelectedSourceEditorCurveBinding.path;
										
									propertyName="[ "+propertyName+"."+__nodeSelectedSourceEditorCurveBinding.propertyName.Split ('.') [0].Replace ("m_LocalRotation", "Rotation").Replace ("m_LocalPosition", "Position").Replace ("m_LocalScale", "Scale")+"]";
									propertyAnimated = "(" + __nodeSelectedSourceEditorCurveBinding.path + ">" + propertyName + ")";
								
								
								}
						}


						//Draw Transtion rect
						Rect transitionRect = new Rect ();

						if (node.transition > 0) {
								

								Color colorSave = GUI.color;
								GUI.color = Color.red;

								startTransitionTimePos = __timeAreaW.TimeToPixel (node.timeStart, rect);
								
								transitionRect = new Rect (startTransitionTimePos, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT + __timeAreaW.translation.y, startTimePos - startTransitionTimePos, NODE_RECT_HEIGHT);

								if (!__sequence.isPlaying && __nodeSelected == node) {
										transitionRect.yMin += __keyframeMarker.image.height;
								}

								//don't draw if outside visible area
								transitionRect.yMax = Mathf.Clamp (transitionRect.yMax, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								transitionRect.xMax = Mathf.Clamp (transitionRect.xMax, rect.xMin, rect.xMax);
								transitionRect.yMin = Mathf.Clamp (transitionRect.yMin, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								transitionRect.xMin = Mathf.Clamp (transitionRect.xMin, rect.xMin, rect.xMax);

								__nodeTransitionGUIContent.tooltip = "Transition=" + (startTimePos - startTransitionTimePos);
								GUI.Box (transitionRect, __nodeTransitionGUIContent, "TL LogicBar 0");



								GUI.color = colorSave;

								

						}

						// __timeAreaW.translation.y; is negative when down scroll is clicked
						Rect nodeRect = new Rect (startTimePos, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT + __timeAreaW.translation.y, endTimePos - startTimePos, NODE_RECT_HEIGHT);

						__nodeGUIContent.tooltip = ">" + propertyAnimated;

						if (!__sequence.isPlaying && __nodeSelected == node) {

								nodeRect.height = __keyframeMarker.image.height;
								DoKeyFrames (nodeRect, rect);


								nodeRect.yMax = nodeRect.yMin + NODE_RECT_HEIGHT;
								nodeRect.yMin += __keyframeMarker.image.height;

								//do not draw if it is ouside visible area
								
								nodeRect.yMax = Mathf.Clamp (nodeRect.yMax, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								nodeRect.xMax = Mathf.Clamp (nodeRect.xMax, rect.xMin, rect.xMax);
								nodeRect.yMin = Mathf.Clamp (nodeRect.yMin, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								nodeRect.xMin = Mathf.Clamp (nodeRect.xMin, rect.xMin, rect.xMax);

							
								

								GUI.Box (nodeRect, __nodeGUIContent, "TL LogicBar 0");
								
								
						} else {
								//do not draw if it is ouside visible area
								
								nodeRect.yMax = Mathf.Clamp (nodeRect.yMax, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								nodeRect.xMax = Mathf.Clamp (nodeRect.xMax, rect.xMin, rect.xMax);
								nodeRect.yMin = Mathf.Clamp (nodeRect.yMin, rect.yMin + rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								nodeRect.xMin = Mathf.Clamp (nodeRect.xMin, rect.xMin, rect.xMax);
								GUI.Box (nodeRect, __nodeGUIContent, "TL LogicBar 0");
						}

						

						//add offset for cursor
						nodeRect.xMin += 5;
						nodeRect.xMax -= 5;
						EditorGUIUtility.AddCursorRect (nodeRect, MouseCursor.Pan);

			
						GUIStyle style = new GUIStyle ("Label");
						if (nodeRect.height > Mathf.Max (12, style.fontSize)) {
								style.fontSize = (__nodeSelected == node ? 12 : style.fontSize);
								style.fontStyle = (__nodeSelected == node ? FontStyle.Bold : FontStyle.Normal);
						
								Color color = style.normal.textColor;
								color.a = (__nodeSelected == node ? 1.0f : 0.7f);
								style.normal.textColor = color;

							
								
						
								//calc draw name of the node
								__nodeLabelGUIContent.text = node.name + propertyName;

								Vector3 size = style.CalcSize (__nodeLabelGUIContent);
						
							
								int labelLength = node.name.Length;

								//if labelWidth is greater then node box rect => remove chars
								while (nodeRect.width < size.x && labelLength>0) {
									
										__nodeLabelGUIContent.text = node.name.Substring (0, labelLength);
										size = style.CalcSize (__nodeLabelGUIContent);
										labelLength--;
								}

								if (labelLength > 0) {
										Rect rect1 = new Rect (nodeRect.x + nodeRect.width * 0.5f - size.x * 0.5f, nodeRect.y + nodeRect.height * 0.5f - size.y * 0.5f, size.x, size.y);
										GUI.Label (rect1, __nodeLabelGUIContent, style);
								}
						}


						
			

			
				}



				///////////////////// HANDLERS OF CHANNEL LIST ////////////////////


				/// <summary>
				/// Ons the reorder sequence channel callback.
				/// </summary>
				/// <param name="list">List.</param>
				static void onReorderSequenceChannelCallback (ReorderableList list)
				{
						//	Debug.Log (list.index);

				}

				void onRemoveSequenceChannelCallback (ReorderableList list)
				{
						SequenceChannel sequenceChannel = null;
				
						if (__sequence != null && (sequenceChannel = __sequence.channelSelected) != null) {
								List<SequenceNode> nodes = __sequence.channelSelected.nodes;
								
								for (int i=0; i<1; i++)
										onRemoveNode (nodes [0]);


								__nodeSelected = null;
								
								
								
								Stop ();//cos target can be "paused" in some animation time t
								
								
								__sequence.timeCurrent = 0f;
								
								ResetChannelTarget (sequenceChannel);//if target is in pause somewhere in time reset to Orginal pos/rot
								
								
								if (!AnimationMode.InAnimationMode ())
										AnimationMode.StartAnimationMode ();
								ReOffsetNodesAnimation (sequenceChannel);//Now with node removed
								AnimationMode.StopAnimationMode ();
								
								
								ResetChannelTarget (sequenceChannel);
								
								window.Repaint ();
				
						}
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
						__sequence.channelSelected = (list.list [list.index] as SequenceChannel);

				}
		
				/// /////////////////////////////////////////////////////////////////////


				private void OnAddEvent ()
				{
						EditorUtility.DisplayDialog ("Not implemented!", "Events are not availible in this version.", 
			                            "Close");			
				}


				/// <summary>
				/// Handles the editor to  play mode state change event.
				/// </summary>
				private void OnPlayModeStateChange ()
				{

						if (!__isPlayMode && EditorApplication.isPlaying && !EditorApplication.isPaused) {

								///TODO check this if not blocking playmode
								
								Stop ();

								__sequenceGameObject = null;
								__isPlayMode = true;
				 
						
						} else {
								__isPlayMode = false;

								
								this.Repaint ();
					
						}


						Debug.Log ("Play mode:" + __isPlayMode);
				}

				public static bool __update;

				/// <summary>
				/// Postprocesses the animation recording and property modifications.
				/// </summary>
				/// <returns>The animation recording modifications.</returns>
				/// <param name="modifications">Modifications.</param>
				private static UndoPropertyModification[] PostprocessAnimationRecordingModifications (UndoPropertyModification[] modifications)
				{
//						if (__update) {
//				Debug.Log("update skiped");
//								return modifications;
//						}
						UndoPropertyModification[] nodeChannelTargetModifications;
						SequenceNode node = __sequence.nodeSelected;

						if (node != null && node.channel.target != null && node.channel.type == SequenceChannel.SequenceChannelType.Animation) {


								nodeChannelTargetModifications = AnimationModeUtility.Process (node.channel.target, node.source as AnimationClip, modifications, TimeAreaW.SnapTimeToWholeFPS ((float)(__sequence.timeCurrent - node.timeStart), __sequence.frameRate));
								
								//Debug.Log((modifications[0].propertyModification.target as IAnimatedValues).LHEPositionWeight+" "+float.Parse(modifications[0].propertyModification.value));
								//U5.2
#if UNITY_5_2
				PropertyModification propertyModification=modifications [0].previousValue;

				//__nodeSelectedSourceEditorCurveBinding=AnimationUtility.GetCurveBindings(node.source as AnimationClip).FirstOrDefault(itm=>itm.path==AnimationUtility.CalculateTransformPath((propertyModification.target as Component).transform,node.channel.target.transform)  && itm.propertyName==propertyModification.propertyPath);

				//if(!String.IsNullOrEmpty(__nodeSelectedSourceEditorCurveBinding.propertyName)) EditorWindow.GetWindow<SequenceEditorWindow>().Repaint();

				IAnimatedValues animatedValues = modifications [0].previousValue.target as IAnimatedValues;
				if (animatedValues != null) {
					SampleClipNodesAt (__sequence.timeCurrent, false);
				}
#endif
				
								//U5
#if UNITY_5_0
								
				PropertyModification propertyModification=modifications [0].propertyModification;	

				//__nodeSelectedSourceEditorCurveBinding=AnimationUtility.GetCurveBindings(node.source as AnimationClip).FirstOrDefault(itm=>itm.path==AnimationUtility.CalculateTransformPath((propertyModification.target as Component).transform,node.channel.target.transform)  && itm.propertyName==propertyModification.propertyPath);


				//if(!String.IsNullOrEmpty(__nodeSelectedSourceEditorCurveBinding.propertyName)) EditorWindow.GetWindow<SequenceEditorWindow>().Repaint();

				IAnimatedValues animatedValues = modifications [0].propertyModification.target as IAnimatedValues;
				if (animatedValues != null) {
					SampleClipNodesAt (__sequence.timeCurrent, false);
				}
#endif
				  
								modifications.Concat (nodeChannelTargetModifications);


						}
					

					
						return modifications;// new UndoPropertyModification[0];
				}

//				void LateUpdate ()
//				{
//						if (!EditorApplication.isPlaying && __sequence != null && __sequence.isPlaying)
//								__sequence.LateUpdateSequence ();
//
//				}

				public static void UpdateSequence ()
				{
						double time = __sequence.timeCurrent;
			
						Transform rootBoneTransform;
			
			
						SequenceNode node = null;
						Animator animator;
						double timeCurrent = 0f;
			
		
			
						AnimationMode.BeginSampling ();
			
						//find changels of type Animation and find first node in channel that is in time or nearest node left of time
						foreach (SequenceChannel channel in __sequence.channels) {
				
								timeCurrent = time;
				
				
								if (channel.target != null) {
					
					
										//node = channel.nodes.FirstOrDefault (itm => time - itm.startTime >= 0 && time <= itm.startTime + itm.duration);
					
										node = null;
					
					
					
										//find node in time (node in which time is in range between nodeStartTime and nodeEndTime)
										foreach (SequenceNode n in channel.nodes) {
						
												//find first node which has n.startTime >= then current time
												if (timeCurrent - n.timeStart >= 0) {
														//check if time comply to upper boundary
														if (timeCurrent <= n.timeStart + n.duration) {
																node = n;
																break;
														} else { 
																//if channel is of animation type and there is next node by time is between prev and next => snap time to prev node endTime
																if (channel.type == SequenceChannel.SequenceChannelType.Animation 
																		&& n.index + 1 < channel.nodes.Count && timeCurrent < channel.nodes [n.index + 1].timeStart
								    
								    ) {
									
																		node = n;
																		timeCurrent = n.timeStart + n.duration;
																		break;
									
																}
								
														}
												}
						
										}
					
					
										if (node != null) {
												if (channel.type == SequenceChannel.SequenceChannelType.Animation) {
							
							
							
							
														animator = channel.target.GetComponent<Animator> ();
							
														if (animator == null) {
																animator = channel.target.AddComponent<Animator> ();
														}
							
														animator.runtimeAnimatorController = channel.runtimeAnimatorController;
							
														Quaternion boneRotation = Quaternion.identity;
							
							
							
							
							
														AnimationMode.SampleAnimationClip (channel.target, node.source as AnimationClip, (float)(timeCurrent - node.timeStart));
							
							
							
							
							
														///!!! It happen that Unity change the ID of channel or something happen and channal as key not to be found
							
							
							
														//						Correction is needed for root bones as Animation Mode doesn't respect current GameObject transform position,rotation
														//						=> shifting current boneTransform position as result of clip animation, to offset of orginal position before animation(alerady saved)
														if ((rootBoneTransform = channel.targetBoneRoot) != null) {
																//	Debug.Log ("node:" + node.name + " time:" + time + " pos:" + rootBoneTransform.position);
																rootBoneTransform.position = rootBoneTransform.position + node.clipBinding.boneRootPositionOffset;
								
																rootBoneTransform.rotation = node.clipBinding.boneRootRotationOffset;
																//	Debug.Log ("node:" + node.name + " time:" + time + "After pos:" + rootBoneTransform.position);
														} 
							
							
							
							
							
														var ikAnimatedValues = channel.target.GetComponent<IAnimatedValues> ();
														if (ikAnimatedValues != null) {
								
																ikAnimatedValues.UpdateValues ();
														}		
							
							
							
												} else if (channel.type == SequenceChannel.SequenceChannelType.Audio) {
							
														AudioClip audioClip = node.source as AudioClip;
							
							
														int sampleStart = (int)Math.Ceiling (audioClip.samples * ((__sequence.timeCurrent - node.timeStart) / audioClip.length));
							
														AudioUtilW.SetClipSamplePosition (audioClip, sampleStart);
							
							
												} else if (channel.type == SequenceChannel.SequenceChannelType.Video) {
														///!!!I don't know the way of  Sampling or timeshifting of MovieTexture 
							
							
												} else if (channel.type == SequenceChannel.SequenceChannelType.Particle) {
														ParticleSystem particleSystem = node.source as ParticleSystem;
														particleSystem.Simulate ((float)__sequence.timeCurrent - node.timeStart, true);
												}
										}
								}
						}
			
			
			
						AnimationMode.EndSampling ();

				}

				/// <summary>
				/// Update this instance.
				/// </summary>
				private void Update ()
				{



						if (__sequenceGameObject != null) {
								__sequence = __sequenceGameObject.GetComponent<Sequence> ();
						}

						
		
						if (!EditorApplication.isPlaying && (__sequence != null) && __sequence.isPlaying) {//?TODO ?? enters here even isPlaying is false		
							
								
								//Debug.Log ("Update sequence from editor");
								__sequence.UpdateSequence (EditorApplication.timeSinceStartup);
								


								//Debug.Log ((float)__sequence.timeCurrent + "__" + __sequence.duration + " " + __sequence.isPlaying + " " + __sequence.name);
								//Debug.Log ("Update() ThreadID:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

								if (AnimationMode.InAnimationMode ())//this solves problem of entering inside this block even playing is false
										SampleClipNodesAt (__sequence.timeCurrent);

								
								

								this.Repaint ();

								//this ensure update of MovieTexture (it its bottle neck do some reflection and force render call)
								if (SceneView.currentDrawingSceneView != null)				
										SceneView.currentDrawingSceneView.Repaint ();//TODO test this
								//SceneView.RepaintAll ();

								
						}


//							if (__sequence!=null && __sequence.isRecording) {
//				if (AnimationMode.InAnimationMode ())//this solves problem of entering inside this block even playing is false
//					SampleClipNodesAt1 ();
//					//SampleClipNodesAt(__sequence.timeCurrent);
//				
//			}

				}

				private static void StopAllVideo ()
				{
						double timeCurrent = __sequence.timeCurrent;
						foreach (SequenceChannel ch in __sequence.channels) 
								if (ch.type == SequenceChannel.SequenceChannelType.Video) {
										ch.nodes.ForEach (itm => {

												if (timeCurrent > itm.timeStart && timeCurrent < itm.timeStart + itm.duration) 
														(itm.source as MovieTexture).Pause ();
												else
														(itm.source as MovieTexture).Stop ();
						
										});
								}
				}

				private static void OnRecord ()
				{

						Debug.Log (" OnRecord:" + __sequence.isRecording);
			          
						if (__sequence.isRecording) {//STOP RECORD
								Stop ();
									
								foreach (SequenceChannel channel in __sequence.channels)
										ResetChannelTarget (channel);

						} else {//START RECORD

								StartRecording ();


								AudioUtilW.StopAllClips ();
								
								SampleClipNodesAt (__sequence.timeCurrent);
				
								
						}
				}

				private static void StartRecording ()
				{
						__sequence.Record ();
			
			
			
			
						if (!AnimationMode.InAnimationMode ()) {
				
								AnimationMode.StartAnimationMode ();
								Undo.postprocessModifications -= PostprocessAnimationRecordingModifications;
								Undo.postprocessModifications += PostprocessAnimationRecordingModifications;

			
				
						} 
			
						
			
			
				}
		
				private void OnPlay ()
				{
						//						AnimationMode.StartAnimationMode ();
//						MethodInfo SetIKPositionWeight = typeof(Animator).GetMethod ("SetIKPositionWeightInternal", BindingFlags.NonPublic | BindingFlags.Instance);
//						MethodInfo SetIKPosition = typeof(Animator).GetMethod ("SetIKPositionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
//						//this.SetIKPositionWeightInternal (goal, value);
//
//						GameObject target = GameObject.Find ("Sphere");
//						SequenceChannel channel1 = __sequence.channels [0];
//						Animator animator = channel1.target.GetComponent<Animator> ();
//						SetIKPosition.Invoke (animator, new object[] {
//								AvatarIKGoal.LeftHand,
//								target.transform.position
//						});
//						SetIKPositionWeight.Invoke (animator, new object[]{AvatarIKGoal.LeftHand,1f});
//		
//
//						//channel1.target.GetComponent<Animator>().SetIKPosition(AvatarIKGoal.LeftHand,target.transform.position);
//						//channel1.target.GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.LeftHand,1f);
//						AnimationMode.StopAnimationMode ();
//						return;


			
						if (!__sequence.isPlaying) {//PLAY
								
								
								Undo.postprocessModifications -= PostprocessAnimationRecordingModifications;

								__sequence.SequenceNodeStart -= onSequenceNodeStart;
								__sequence.SequenceNodeStart += onSequenceNodeStart;
//
								__sequence.SequenceNodeStop -= onSequenceNodeStop;
								__sequence.SequenceNodeStop += onSequenceNodeStop;

								__sequence.OnEnd.RemoveListener (onSequenceEnd);
								__sequence.OnEnd.AddListener (onSequenceEnd);


				
				
								if ((float)__sequence.timeCurrent >= __sequence.timeEnd) {//start from begining


										//if(__sequence.isRecording) Stop();
										AnimationMode.StopAnimationMode ();//stop if paused or recording then => reset target

										__sequence.timeCurrent = 0f;

										foreach (SequenceChannel channel in __sequence.channels)
												ResetChannelTarget (channel);
								} else
										__sequence.timeCurrent = Mathf.Min ((float)__sequence.timeCurrent, (float)__sequence.timeEnd);
				
							
				
								if (!AnimationMode.InAnimationMode ()) {

									
										AnimationMode.StartAnimationMode ();

										

										
								} 
				         
				         
				         
								
								__sequence.Play (EditorApplication.timeSinceStartup);

								
				
				
				
				
						} else {//PAUSE 

								Stop (true);//soft stop preserve gameobject transforms after stop animation mode (something like "pause", not reseting GameObjets as stop record do)

								

					
				
								__sequence.SequenceNodeStart -= onSequenceNodeStart;
			
				
								__sequence.SequenceNodeStop -= onSequenceNodeStop;

								foreach (SequenceChannel channel in __sequence.channels) {
										ApplyTransformAtPause (channel);
								}
						}

						

				}
			

				/// <summary>
				/// Stop Sequence
				/// </summary>
				private static void Stop (bool pause=false)
				{
						//Debug.Log ("Stop() ThreadID:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

			
			
						AnimationMode.StopAnimationMode ();
			
						AudioUtilW.StopAllClips ();
			

				
						if (__sequence != null) {
								__sequence.Stop (__sequence.playForward);
				
								__sequence.StopRecording ();
								StopAllVideo ();
						
			
								Undo.postprocessModifications -= PostprocessAnimationRecordingModifications;

								//Normal gameobjects do not preserve transform when animaiton mode is stoped as characters
								//so preserving
								foreach (SequenceChannel channel in __sequence.channels) {
										PreserveTransformAtPause (channel);
								}

								AnimationMode.StopAnimationMode ();

								if (pause)
										foreach (SequenceChannel channel in __sequence.channels) {
												ApplyTransformAtPause (channel);
										}

								//RESET 
								IAnimatedValues fbbikAnimatedValues;
								foreach (SequenceChannel channel in __sequence.channels)

										if (channel.target != null && channel.type == SequenceChannel.SequenceChannelType.Animation && (fbbikAnimatedValues = channel.target.GetComponent<IAnimatedValues> ()) != null)
												fbbikAnimatedValues.ResetValues ();//as this component is binded to FFBIK component need to reset IK values
			
						}



				}
			

			


				/// <summary>
				/// Handles OnGUI event
				/// </summary>
				private void OnGUI ()
				{

						if (EditorApplication.isPlaying)
								return;

					



						if (__eventMarker == null)
								__eventMarker = EditorGUILayoutEx.ANIMATION_STYLES.eventMarker;
						
						if (__keyframeMarker == null)
								__keyframeMarker = new GUIContent (EditorGUILayoutEx.ANIMATION_STYLES.pointIcon);

						Rect rect = this.position;



						rect.x = this.position.width * 0.2f;
						rect.width = this.position.width - rect.x;//80% for timeArea
						

						rect.y = 0;

						if (Event.current.type == EventType.MouseDown && rect.Contains (Event.current.mousePosition))//deselect
						__sequence.channelSelected = null;
				
				
						if (__sequence != null)
								DoTimelineGUI (rect, __sequence.frameRate);
						else
								DoTimelineGUI (rect, 30);

						rect.width = rect.x;//20% for settings && channels
						rect.x = 0;
						rect.y = 0;

				
						DoChannelsGUI (rect);

						//	Debug.Log ("Scale:" + __timeAreaW.scale);

						
				}




				/////////////  HANDLING KEYFRAMES ROTATION/SCALE Events ////////////////
					
				/// <summary>
				/// Ons the mecanim event edit.
				/// </summary>
				/// <param name="args">Arguments.(return normalized value)</param>
				static void onKeyframeEdit (TimeLineArgs<float> args)
				{
						
						if (__sequence != null && __sequence.nodeSelected != null && !__sequence.isPlaying) {
								__sequence.timeCurrent = __sequence.nodeSelected.timeStart + args.selectedValue * __sequence.nodeSelected.duration;
								if (!__sequence.isRecording) {
										StartRecording ();

					
										

										
								}


								SampleClipNodesAt (__sequence.timeCurrent);
								
						}
						
						
				}

				/// <summary>
				/// Ons the keyframe add.
				/// </summary>
				/// <param name="args">Arguments.</param>
				static void onKeyframeAdd (TimeLineArgs<float> args)
				{
						
						if (__sequence != null && __nodeSelected != null && __nodeSelected.channel.target != null) {

								float time = TimeAreaW.SnapTimeToWholeFPS (args.selectedValue * __nodeSelected.duration, __sequence.frameRate);

								if (__nodeSelectedSourceEditorCurveBinding.type == typeof(Transform)) {

										String propertyNameBase = __nodeSelectedSourceEditorCurveBinding.propertyName.Split ('.') [0];

										__nodeSelectedSourceEditorCurveBinding.propertyName = propertyNameBase + ".x";
										AnimationUtilityEx.AddKeyframeTo (__nodeSelectedSourceEditorCurveBinding, __nodeSelected.channel.target, __nodeSelected.source as AnimationClip, time);
										__nodeSelectedSourceEditorCurveBinding.propertyName = propertyNameBase + ".y";
										AnimationUtilityEx.AddKeyframeTo (__nodeSelectedSourceEditorCurveBinding, __nodeSelected.channel.target, __nodeSelected.source as AnimationClip, time);

										__nodeSelectedSourceEditorCurveBinding.propertyName = propertyNameBase + ".z";
										AnimationUtilityEx.AddKeyframeTo (__nodeSelectedSourceEditorCurveBinding, __nodeSelected.channel.target, __nodeSelected.source as AnimationClip, time);

				
								} else {
										if (!String.IsNullOrEmpty (__nodeSelectedSourceEditorCurveBinding.propertyName))

												AnimationUtilityEx.AddKeyframeTo (__nodeSelectedSourceEditorCurveBinding, __nodeSelected.channel.target, __nodeSelected.source as AnimationClip, time);
										else
												Debug.LogWarning ("Create property in AnimationClip you want to add keyframe to");
								}
			
						}
				}
					
				/// <summary>
				/// Ons the keyframe delete.
				/// </summary>
				/// <param name="args">Arguments.</param>
				static void onKeyframeDelete (TimeLineArgs<float> args)
				{
						
						
						if (__sequence != null && __sequence.nodeSelected != null && !String.IsNullOrEmpty(__nodeSelectedSourceEditorCurveBinding.propertyName))
								AnimationUtilityEx.RemoveKeyframeFrom (__sequence.nodeSelected.source as AnimationClip,__nodeSelectedSourceEditorCurveBinding, args.selectedIndex);
				}

				static void onKeyframeDragEnd (TimeLineArgs<float> args)
				{

						//Debug.Log ("onKeyframeDragEnd");


				}


				/// <summary>
				/// Shows the key frames of node selected animated property.
				/// </summary>
				/// <param name="keyframesRect">Keyframes rect.</param>
				private static void DoKeyFrames (Rect keyframesRect, Rect rect)
				{
						/////////////  ROTATION/SCALE KEYFRAMES  /////////////////////////
						
						
			
						//Show rotation or scale keyframes if Rotation or Scale tools are selected
						if (/*Selection.activeGameObject != null &&*/ __sequence != null && __nodeSelected != null
			   					
								&& __nodeSelected.channel.target != null
			    				
								&& __nodeSelected.channel.type == SequenceChannel.SequenceChannelType.Animation
								//&& (Selection.activeGameObject == __sequence.nodeSelected.channel.target || Selection.activeGameObject.transform.IsChildOf (__sequence.nodeSelected.channel.target.transform))


			   			 ) {


				           

								

								AnimationClip clip = __nodeSelected.source as AnimationClip;

							

							
								

								//limit keyframes to visible area
								float timeFromNormalized = TimeAreaW.SnapTimeToWholeFPS (Math.Abs (__timeAreaW.PixelToTime (Mathf.Clamp (keyframesRect.xMin, rect.xMin, rect.xMax), rect) - __nodeSelected.timeStart), __sequence.frameRate) / __nodeSelected.duration;			
								float timeToNormalized = TimeAreaW.SnapTimeToWholeFPS (Math.Abs (__timeAreaW.PixelToTime (Mathf.Clamp (keyframesRect.xMax, rect.xMin, rect.xMax), rect) - __nodeSelected.timeStart), __sequence.frameRate) / __nodeSelected.duration;
								

								keyframesRect.xMax += __keyframeMarker.image.width * 0.5f;
								keyframesRect.xMin -= __keyframeMarker.image.width * 0.5f;

								int indexFrom = 0;
								int indexTo = 0;
								AnimationCurve curve = null;

								if (String.IsNullOrEmpty (__nodeSelectedSourceEditorCurveBinding.propertyName)) 
										keyframeTimeValuesNormalized = new float[0];
								else {

										EditorCurveBinding bindingReMapped = RotationCurveInterpolationW.RemapAnimationBindingForRotationCurves (__nodeSelectedSourceEditorCurveBinding, clip);
					
					
										keyframeTimeValuesNormalized = AnimationUtilityEx.GetTimesNormalized (clip,bindingReMapped , clip.length);
							
									

										int keysNum = keyframeTimeValuesNormalized.Length;

										//filter only visible keyframes normalized values
										keyframeTimeValuesNormalized = keyframeTimeValuesNormalized.Select (itm => itm).Where ((itm) => (itm > timeFromNormalized || Mathf.Approximately (itm, timeFromNormalized)) && (itm < timeToNormalized || Mathf.Approximately (itm, timeToNormalized))).ToArray ();
								

										/////// FIND INDICES OF FISRTS AND LAST SHOWN KEY ///
										

										curve = AnimationUtility.GetEditorCurve (clip, bindingReMapped);
								
										if (keyframeTimeValuesNormalized.Length > 0) {
												float timeFrom = keyframeTimeValuesNormalized [0] * __nodeSelected.duration;
												float timeTo = keyframeTimeValuesNormalized [keyframeTimeValuesNormalized.Length - 1] * __nodeSelected.duration;
									
												for (int i=0; i<keysNum; i++) {
														if (Mathf.Approximately (timeFrom, curve.keys [i].time))
																indexFrom = i;
														if (Mathf.Approximately (timeTo, curve.keys [i].time)) {
																indexTo = i;
																break;
														}
										
										
										
												}
										}
								
					
										//prepare text for tooltip
										keyframesDisplayNames = keyframeTimeValuesNormalized.Select ((itm) => {
												
												return Decimal.Round (Convert.ToDecimal (itm * clip.length), 2).ToString () + ".s"; }).ToArray ();
					
								}
								if (keyframeTimeValuesNormalizedSelected == null || keyframeTimeValuesNormalizedSelected.Length != keyframeTimeValuesNormalized.Length)
										keyframeTimeValuesNormalizedSelected = new bool[keyframeTimeValuesNormalized.Length];
					
								//create custom timeline showing rotation or scale keyframes
								EditorGUILayoutEx.CustomTimeLine (ref keyframesRect, __keyframeMarker, ref keyframeTimeValuesNormalized, ref keyframeTimeValuesNormalizePrev, ref keyframesDisplayNames, ref keyframeTimeValuesNormalizedSelected, -1f, onKeyframeAdd
					                                   , onKeyframeDelete, null, onKeyframeEdit, onKeyframeDragEnd
								);


								if (!__sequence.isPlaying && keyframeTimeValuesNormalized.Length > 0 && Event.current.type == EventType.Used) {
										//UPDATE of DRAGED CHANGED VALUES (dragging of keyframe marker)


										
										if (!String.IsNullOrEmpty (__nodeSelectedSourceEditorCurveBinding.propertyName)) {

												
												
						float time=0f;
										
												//Debug.Log ("Drag inx from:"+indexFrom+" to:"+indexTo);
												//if (indexFrom < indexTo)
												for (int l = indexFrom; l <=indexTo; l++) {
														if (keyframeTimeValuesNormalizedSelected [l - indexFrom]) {
																//Keyframe keyframe = curve.keys [l];
																//Debug.Log ("Drag inx:" + l + " keyframe:" + keyframe.time + " to:" + keyframeTimeValuesNormalized [l-indexFrom] * __nodeSelected.duration+" value of frm 60:"+keyframeTimeValuesNormalized[60]*__nodeSelected.duration);
								//keyframe.time = ;//cos of normalized values
								time=keyframeTimeValuesNormalized [l - indexFrom] * __nodeSelected.duration;
								AnimationUtilityEx.MoveKeyToTime(__nodeSelectedSourceEditorCurveBinding,clip,l,time);
									
									if (__sequence.isRecording)//drag timeScrubber with 
																		__sequence.timeCurrent =time + __nodeSelected.timeStart;
											
														}
												}
									
									
										}
										
					
								}
				
						} 
			
			

				}


				/// <summary>
				/// Sequences the drop channel target event handler.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="channel">Channel.</param>
				static void sequenceDropChannelTargetEventHandler (Rect rect, int channelInx)
				{
						if (__sequence.isPlaying || __sequence.isRecording)
								return;



						Event evt = Event.current;

						


						
						switch (evt.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
								
								
								DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							
								
							
								if (evt.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag ();
										if (EditorApplication.isPlaying) {
												Debug.Log ("Can't add tween object in play mode. Stop the play mode and readd it.");
										} else {
												GameObject target = DragAndDrop.objectReferences [0] as GameObject;

												if (target != null) {
														SequenceChannel channel = null;
														channel = __sequence.channels [channelInx];
														channel.target = target;
														channel.targetPositionOriginal = target.transform.position;
														channel.targetRotationOriginal = target.transform.rotation;
														channel.targetBoneRoot = target.GetRootBone ();

														//AnimationMode

														if (!AnimationMode.InAnimationMode ())
																AnimationMode.StartAnimationMode ();
														ReOffsetNodesAnimation (channel);
														AnimationMode.StopAnimationMode ();
												}
										}




								}
								break;
						}
				}

				private static GUIContent __addEventContent;
				private static GUIContent __nextKeyContent;
				private static GUIContent __prevKeyContent;
				private static GUIContent __recordIcon;
				private static GUIContent __playIcon;
				private static GUIContent __focusContent;
				private static GUIContent __playIconOn;

				/// <summary>
				/// Dos the play record toolbar and channels GUI.
				/// </summary>
				/// <param name="rect">Rect.</param>
				void DoChannelsGUI (Rect rect)
				{
						GUIStyle style = new GUIStyle ("ProgressBarBack");
						style.padding = new RectOffset (0, 0, 0, 0);
						
						GUILayout.BeginArea (rect, GUIContent.none, style);



			

						if (__sequence != null) {
								GUILayout.BeginHorizontal (EditorStyles.toolbar);

							
								///DRAW PLAY,RECORD AND ADD EVENT BUTTON
								Color colorSave = GUI.backgroundColor;


								GUI.backgroundColor = __sequence.isRecording ? Color.red : Color.white;


								if (__recordIcon == null)
										__recordIcon = EditorGUILayoutEx.ANIMATION_STYLES.recordIcon;

								//Record
								if (GUILayout.Button (__recordIcon.image, EditorStyles.toolbarButton)) {
									
										OnRecord ();

								}

								GUI.backgroundColor = colorSave;

								if (__playIcon == null)
										__playIcon = EditorGUILayoutEx.ANIMATION_STYLES.playIcon;

								if (__playIconOn == null)
										__playIconOn = EditorGUILayoutEx.ANIMATION_STYLES.playIconOn;


								//Play
								if (GUILayout.Button (__sequence.isPlaying ? __playIconOn.image : __playIcon.image, EditorStyles.toolbarButton)) {
									

									
										OnPlay ();

								}
							
								///PREV
								if (__prevKeyContent == null) {
										__prevKeyContent = EditorGUILayoutEx.ANIMATION_STYLES.prevKeyContent;
										__prevKeyContent.tooltip = "Back to start";
								}
				
				
				
								GUILayout.FlexibleSpace ();
								if (GUILayout.Button (__prevKeyContent, EditorStyles.toolbarButton)) {
					
										__sequence.timeCurrent = 0f;
										foreach (SequenceChannel ch in __sequence.channels)
												ResetChannelTarget (ch);
					
					
					
					
								}
				
				
								GUILayout.FlexibleSpace ();


								////////NEXT
								if (__nextKeyContent == null) {
										__nextKeyContent = EditorGUILayoutEx.ANIMATION_STYLES.nextKeyContent;
										__nextKeyContent.tooltip = "Go to end";
								}



								GUILayout.FlexibleSpace ();
								if (GUILayout.Button (__nextKeyContent, EditorStyles.toolbarButton)) {
																
										__sequence.timeCurrent = __sequence.timeEnd;
														
								}

			
								GUILayout.FlexibleSpace ();
								if (__addEventContent == null) {
										__addEventContent = EditorGUILayoutEx.ANIMATION_STYLES.addEventContent;
								}
								if (GUILayout.Button (__addEventContent, EditorStyles.toolbarButton)) {
					
										onEventAdd ();
					
								}


									

								if (__focusContent == null)
										__focusContent = new GUIContent ("F", "Focus");
			
								//FOCUS on nodes (spread width and scroll to)
								if (GUILayout.Button (__focusContent, EditorStyles.toolbarButton)) {
										float sequenceTimeStartPositionX = 0f;
										float sequenceTimeEndPositionX = 0f;

										sequenceTimeStartPositionX = __timeAreaW.TimeToPixel ((float)__sequence.timeStart, __timeAreaW.rect);
										sequenceTimeEndPositionX = __timeAreaW.TimeToPixel ((float)__sequence.timeStart + __sequence.duration, __timeAreaW.rect);

										float sequenceWidth = sequenceTimeEndPositionX - sequenceTimeStartPositionX;

										__timeAreaW.Focus (new Rect (sequenceTimeStartPositionX, 0, sequenceWidth, 0));

								}


								GUILayout.EndHorizontal ();
						}
			
						GUILayout.BeginHorizontal ();
						GUILayout.BeginVertical ();




						DoSettingsGUI (rect.width - 1.5f);


						if (__sequence != null) {

								
								
				
								
								//Debug.Log (__timeAreaW.translation + " " + __channelsTranslation);


								//add remove button 20f
								float channelsHeight = __sequenceChannelsReordableList.elementHeight * __sequenceChannelsReordableList.count + 20f;
								float timeAreaChannelsHeight = rect.height - TIME_LABEL_HEIGHT - EVENT_PAD_HEIGHT;
								//Debug.Log("Vmax:="+Mathf.Max(0,channelsHeight-timeAreaChannelsHeight));
								float scrollValueY = -Mathf.Max (0, channelsHeight - timeAreaChannelsHeight);
								//TODO find better solution
								__timeAreaW.translation = new Vector2 (__timeAreaW.translation.x, Mathf.Max (scrollValueY, __timeAreaW.translation.y));



								__channelsTranslation = new Vector2 (0, -__timeAreaW.translation.y);
								__channelsTranslation = GUILayout.BeginScrollView (__channelsTranslation, false, false, GUIStyle.none, GUIStyle.none);                              
								
								//Draw Channels
								if (__sequenceChannelsReordableList.list != __sequence.channels)
										__sequenceChannelsReordableList.list = __sequence.channels;

								__sequenceChannelsReordableList.DoLayoutList ();


								

								GUILayout.EndScrollView ();

								
								//reuse rect so rect become rect of sequenceChannelList 
								rect.yMin += TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT;
								
								rect.yMax -= 16f;//16f for +/- buttons

								if (__sequence.channels.Count > 0 && rect.Contains (Event.current.mousePosition)) {
										sequenceDropChannelTargetEventHandler (rect, (int)((Event.current.mousePosition.y - rect.yMin - __timeAreaW.translation.y) / NODE_RECT_HEIGHT));
								}

							

						}

						GUILayout.EndVertical ();
						GUILayout.Space (1.5f);
						GUILayout.EndHorizontal ();



						

						
						GUILayout.EndArea ();
			
				}



				/// <summary>
				/// Handles drawing settings OnGUI .
				/// </summary>
				/// <param name="width">Width.</param>
				private void DoSettingsGUI (float width)
				{
						GUILayout.BeginHorizontal ();
						if (GUILayout.Button (__sequence != null ? __sequence.name : "[None Selected]", EditorStyles.toolbarDropDown, GUILayout.Height (20), GUILayout.Width (width * 0.3f))) {
								GenericMenu toolsMenu = new GenericMenu ();
				
								List<Sequence> sequences = GameObjectUtilityEx.FindAllContainComponentOfType<Sequence> ();
								foreach (Sequence sequence in sequences) {
										toolsMenu.AddItem (new GUIContent (sequence.name), false, OnGameObjectSelectionChanged, sequence);
								}
								toolsMenu.AddItem (new GUIContent ("[New Sequence]"), false, CreateNewSequence);
								toolsMenu.AddItem (new GUIContent ("[New Animation Clip]"), false, CreateNewAnimationClip);
								toolsMenu.AddItem (new GUIContent ("[New Animation Controller]"), false, CreateRuntimeAnimatorController);
								


								

								toolsMenu.DropDown (GUILayoutUtility.GetLastRect ());
								
								EditorGUIUtility.ExitGUI ();
						}
			
						if (__sequence != null) {
								wrap = __sequence.wrap;			
						}
						wrap = (Sequence.SequenceWrap)EditorGUILayout.EnumPopup (wrap, EditorStyles.toolbarDropDown, GUILayout.Width (width * 0.4f));
						if (__sequence != null) {
								__sequence.wrap = wrap;			
						}

						if (__sequence != null) {
								//Handle FRAMERATE input and changes		
								EditorGUI.BeginChangeCheck ();

								EditorGUILayout.LabelField (__frameRateGUIContent, GUILayout.Width (width * 0.1f));
								__sequence.frameRate = Mathf.Max (EditorGUILayout.IntField (__sequence.frameRate, GUILayout.Width (width * 0.15f)), 1);
			
								if (EditorGUI.EndChangeCheck ()) {

										//TODO check this
										foreach (SequenceChannel channel in __sequence.channels)
												foreach (SequenceNode n in channel.nodes) {
										
														if (n.source is AnimationClip) {
									
												
																(n.source as AnimationClip).frameRate = __sequence.frameRate;
														} 

														//resnap to new framerate
														//n.duration = TimeAreaW.SnapTimeToWholeFPS (n.duration, __sequence.frameRate);

					
												}

										//now tick drawn on new frame rate
										__timeAreaW.hTicks.SetTickModulosForFrameRate (__sequence.frameRate);

								}
						}
			

						GUILayout.EndHorizontal ();






				}


				//////////////////////////////////////////////
				///     	Event timeline Handler        ///
				/// 


				/// <summary>
				/// Ons the event drag end.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onEventDragEnd (TimeLineArgs<float> args)
				{


				}

				/// <summary>
				/// Ons the mecanim event edit.
				/// </summary>
				/// <param name="args">Arguments.(return normalized value)</param>
				static void onEventEdit (object userData)
				{
						//EditorWindow popup
				}
		
				/// <summary>
				/// Ons the keyframe add.
				/// </summary>
				/// <param name="args">Arguments.</param>
				static void onEventAdd ()
				{
						

						if (__sequence != null) {
								SequenceEvent ev = new SequenceEvent ();
								ev.time = TimeAreaW.SnapTimeToWholeFPS ((float)__sequence.timeCurrent, __sequence.frameRate);
								__sequence.events.Add (ev);
						}
				}
		
				/// <summary>
				/// Ons the keyframe delete.
				/// </summary>
			
				static void onEventDelete (object userData)
				{
					

						if (__sequence != null) {
						
						
								__sequence.events.Remove (userData as SequenceEvent);
						}

				}


				/// <summary>
				/// Dos the event GUI.
				/// </summary>
				/// <param name="eventRect">Event rect.</param>
				private void DoEventGUI (Rect eventRect)
				{
						eventRect.y = EVENT_PAD_HEIGHT;
						eventRect.height = EVENT_PAD_HEIGHT;
						

					

						if (Event.current.type == EventType.Repaint) 
								EditorGUILayoutEx.ANIMATION_STYLES.eventBackground.Draw (eventRect, GUIContent.none, 0);

						SequenceEvent evt = null;
						int numEvents = __sequence.events.Count;
						Rect eventMarkerRect = new Rect (0, eventRect.y, __eventMarker.image.width, __eventMarker.image.height);
						float eventMarkerRectHalfW = __eventMarker.image.width * 0.5f;


						for (int i=0; i<numEvents; i++) {
								evt = __sequence.events [i];
								eventMarkerRect.x = __timeAreaW.TimeToPixel ((float)evt.time, eventRect) - eventMarkerRectHalfW;


				 
								GUI.DrawTexture (eventMarkerRect, __eventMarker.image);
								sequenceEventInteractiveHandler (eventRect, eventMarkerRect, evt);
						}
			
			



				}


				/// <summary>
				/// Creates the events menu.
				/// </summary>
				/// <returns>The event menu.</returns>
				/// <param name="evt">Evt.</param>
				private static GenericMenu createEventMenu (SequenceEvent evt)
				{

						GenericMenu menu = new GenericMenu ();
								
						//menu.AddItem (new GUIContent ("Edit"), false, onEventEdit, evt);
						menu.AddItem (new GUIContent ("Delete"), false, onEventDelete, evt);
								
						

						return menu;
				}
				

				/// <summary>
				/// Sequences the event interactive handler.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="eventMarkerRect">Event marker rect.</param>
				/// <param name="evt">Evt.</param>
				private static void sequenceEventInteractiveHandler (Rect rect, Rect eventMarkerRect, SequenceEvent evt)
				{

						Event ev = Event.current;
			
					
						switch (ev.rawType) {
						
						case EventType.MouseDrag:
								if (__isEventDragged && __sequence.eventSelected == evt) {

										float time = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) + __clickDelta;
					
										time = TimeAreaW.SnapTimeToWholeFPS (time, __sequence.frameRate);
					
										if (time >= 0) {
												evt.time = time;
												__sequence.timeCurrent = evt.time;

												SampleClipNodesAt (__sequence.timeCurrent);
										}

								}


								break;
						case EventType.ContextClick:
							//show add,remove,edit
								if (!__isEventDragged && eventMarkerRect.Contains (ev.mousePosition)) 
										createEventMenu (evt).ShowAsContext ();
								break;

						case EventType.MouseDown:

								if (!__isEventDragged && eventMarkerRect.Contains (ev.mousePosition)) {
										if (ev.button == 1) {//show add,remove,edit
												
												createEventMenu (evt).ShowAsContext ();

										} else if (ev.button == 0) {
												__clickDelta = (float)evt.time - __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
												__isEventDragged = true;
												__sequence.eventSelected = evt;
												__sequence.timeCurrent = evt.time;

												if (!__sequence.isRecording)
														StartRecording ();

												
												//EditorUtility.SetDirty(__sequence);//Repaint Inspector
												SampleClipNodesAt (__sequence.timeCurrent);


										}

										ev.Use ();
								}



								break;

						case EventType.MouseUp:
								
								__isEventDragged = false;
								break;

						}
						



				}


				/// <summary>
				/// Ons the timeline click.
				/// </summary>
				/// <param name="rect">Rect.</param>
				private static void onTimelineClick (Rect rect)
				{

						double time = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);

						


						__sequence.timeCurrent = time;

					
						if (!__sequence.isRecording)
								StartRecording ();


 
			
						SampleClipNodesAt (__sequence.timeCurrent);

						

						//TODO stop all movietextures
						
						AudioUtilW.StopAllClips ();

				}

				private static void PreserveTransformAtPause (SequenceChannel channel)
				{
						if (channel.target != null && channel.type == SequenceChannel.SequenceChannelType.Animation && channel.targetBoneRoot == null) {
								channel.targetPositionCurrent = channel.target.transform.position;
								channel.targetRotationCurrent = channel.target.transform.rotation;
						
						}
				}
		
		
				/// <summary>
				/// Resets the channel target position and rotation (as before Animation sampling is applied)
				/// </summary>
				private static void ResetChannelTarget (SequenceChannel channel)
				{
			
			
						PrefabType prefabType = PrefabType.None;
			
			
						if (channel.type == SequenceChannel.SequenceChannelType.Animation && channel.target != null) {
								prefabType = PrefabUtility.GetPrefabType (channel.target);
				
								if (prefabType == PrefabType.ModelPrefabInstance || prefabType == PrefabType.PrefabInstance)
										channel.target.ResetPropertyModification<Transform> ();
								//PrefabUtility.RevertPrefabInstance (channel.target);
				
				
				
				
								//rewind to start position and rotation (before Animation sampling)
								channel.target.transform.position = channel.targetPositionOriginal;
								channel.target.transform.rotation = channel.targetRotationOriginal;
				
						}
			
				}

				private static void ApplyTransformAtPause (SequenceChannel channel)
				{
						if (channel.target != null && channel.type == SequenceChannel.SequenceChannelType.Animation && channel.targetBoneRoot == null) {
								channel.target.transform.position = channel.targetPositionCurrent;
								channel.target.transform.rotation = channel.targetRotationCurrent;

						}
				}

				/// <summary>
				/// Saves the AnimationData from nodes's Animations.
				/// AnimationData of first node (Node1) has target.rootBone.position offset before animation applied and and at animation t=0s
				/// Animationdata od next node has position offset calculated from  
				/// also contains data of channel.target position and rotation
				/// </summary>
				public static void ReOffsetNodesAnimation (SequenceChannel channel)
				{
						Transform rootBoneTransform;

					
						
						

						//	AnimationMode.BeginSampling ();
			
					
						if (channel.target != null && channel.type == SequenceChannel.SequenceChannelType.Animation) {
					
								//save bone position
								Vector3 positionPrev = Vector3.zero;
								Quaternion rotationPrev = Quaternion.identity;

								Animator animator = channel.target.GetComponent<Animator> ();

								if (animator == null)
										animator = channel.target.AddComponent<Animator> ();

								//must have controller or sampling doesn't work(weird???)
								animator.runtimeAnimatorController = channel.runtimeAnimatorController;

								
								rootBoneTransform = channel.targetBoneRoot;
								if (rootBoneTransform == null && (rootBoneTransform = channel.targetBoneRoot = channel.target.GetRootBone ()) == null) {

										
										Debug.LogWarning ("Can't find Root bone Hips");
										return;
								}
					
					
								



								//check first node if exist and if no change was done to channel.target => no need to resave
								// gameobject(target) wasn't move or rotated => no need to save
//										if (!forceSave && channel.positionOriginalRoot == channel.target.transform.position
//												&& channel.rotationOriginalRoot == channel.target.transform.rotation)
//												continue;

								Debug.Log ("SaveAnimationData> resaving...");

								//save target position and rotation so can be restored after stoping animation mode
								//channel.positionOriginalRoot = channel.target.transform.position;
								//channel.rotationOriginalRoot = channel.target.transform.rotation;

								//!!! AnimationClipSettings are incorrect from thoose set inside Inspector
								AnimationClipSettings clipSettings = null;

					
					
								foreach (SequenceNode n in channel.nodes) {

										//check AnimationClip settings from previous node
										clipSettings = AnimationUtility.GetAnimationClipSettings (channel.nodes [n.index].source as AnimationClip);

												
							
										if (n.index > 0) {

												//get sample of animation clip of previous node at the end t=duration[s]
												AnimationMode.SampleAnimationClip (channel.target, channel.nodes [n.index - 1].source as AnimationClip, channel.nodes [n.index - 1].duration);

												
												rotationPrev = rootBoneTransform.rotation;	//take orientations from previous node

												positionPrev = rootBoneTransform.position + channel.nodes [n.index - 1].clipBinding.boneRootPositionOffset;
										
										} else {//first node => there is no previous

												//save postion and rotation before Animation from first node is applied
												
												positionPrev = rootBoneTransform.position;
												
												rotationPrev = rootBoneTransform.rotation;	
										}


										
						

										//Debug.Log ("node:" + n.name + "Before pos:" + rootBoneTransform.position);
						
										//get sample of animation clip of current node at start t=0s.
										AnimationMode.SampleAnimationClip (channel.target, (AnimationClip)n.source, 0f);

										//Debug.Log ("node:" + n.name + "After pos:" + rootBoneTransform.position);

										if (clipSettings.keepOriginalOrientation)
												rotationPrev = rootBoneTransform.rotation;	

										
										Vector3 positionAfter = rootBoneTransform.position;
										
										n.clipBinding.boneRootPositionOffset = positionPrev - positionAfter;
										n.clipBinding.boneRootRotationOffset = rotationPrev;
//										
//
//										if (!clipSettings.keepOriginalPositionXZ)// using CenterOfMass
//										//calculate difference of bone position orginal - bone postion after clip effect at AnimationClip time t=0
//										n.clipBinding.boneRootPositionOffset=positionPrev;
//										else
//											n.clipBinding.boneRootPositionOffset = positionPrev - positionAfter;
//										//else offset is 0
//
//										if(!clipSettings.keepOriginalOrientation)//use BodyOrientation
//											n.clipBinding.boneRootRotationOffset=rotationPrev;
//										else
//											n.clipBinding.boneRootRotationOffset=rootBoneTransform.rotation;
										
											
										
										//	Debug.Log ("node:" + n.name + "offset" + n.clipBinding.boneRootPositionOffset);
						
										


											
						
								}
						}
						



						//	AnimationMode.EndSampling ();
				}



//		public Texture DoRenderPreview (Rect previewRect, GUIStyle background)
//		{
//			this.m_PreviewUtility.BeginPreview (previewRect, background);
//			Vector3 bodyPosition = this.bodyPosition;
//			Quaternion quaternion;
//			Vector3 vector;
//			Quaternion quaternion2;
//			Vector3 pivotPos;
//			if (this.Animator && this.Animator.isHuman)
//			{
//				quaternion = this.Animator.rootRotation;
//				vector = this.Animator.rootPosition;
//				quaternion2 = this.Animator.bodyRotation;
//				pivotPos = this.Animator.pivotPosition;
//			}
//			else
//			{
//				if (this.Animator && this.Animator.hasRootMotion)
//				{
//					quaternion = this.Animator.rootRotation;
//					vector = this.Animator.rootPosition;
//					quaternion2 = Quaternion.identity;
//					pivotPos = Vector3.zero;
//				}
//				else
//				{
//					quaternion = Quaternion.identity;
//					vector = Vector3.zero;
//					quaternion2 = Quaternion.identity;
//					pivotPos = Vector3.zero;
//				}
//			}
//			bool oldFog = this.SetupPreviewLightingAndFx ();
//			Vector3 forward = quaternion2 * Vector3.forward;
//			forward [1] = 0f;
//			Quaternion directionRot = Quaternion.LookRotation (forward);
//			Vector3 directionPos = vector;
//			Quaternion pivotRot = quaternion;



				/// <summary>
				/// Samples the clip nodes at time.
				/// </summary>
				/// <param name="time">Time.</param>
				private static void SampleClipNodesAt (double time, bool registerRecordObjectsAsUndos=true)
				{


						Transform rootBoneTransform;
					
					
						SequenceNode node = null;
						Animator animator;
						double timeCurrent = 0f;
						IAnimatedValues animatedValues = null;

						//Ensure objects recorded using RecordObject or ::ref:RecordObjects are registered as an undoable action. In most cases there is no reason to invoke FlushUndoRecordObjects since 
						//	it's automatically done right after mouse-up and certain other events that conventionally marks the end of an action
						if (registerRecordObjectsAsUndos)
								Undo.FlushUndoRecordObjects ();

						AnimationMode.BeginSampling ();
			
						//find changels of type Animation and find first node in channel that is in time or nearest node left of time
						foreach (SequenceChannel channel in __sequence.channels) {

								timeCurrent = time;


								if (channel.target != null) {


										//node = channel.nodes.FirstOrDefault (itm => time - itm.startTime >= 0 && time <= itm.startTime + itm.duration);

										node = null;

										

										//find node in time (node in which time is in range between nodeStartTime and nodeEndTime)
										foreach (SequenceNode n in channel.nodes) {

												//find first node which has n.startTime >= then current time
												if (timeCurrent - n.timeStart >= 0) {
														//check if time comply to upper boundary
														if (timeCurrent <= n.timeStart + n.duration) {
																node = n;
																break;
														} else { 
																//if channel is of animation type and there is next node by time is between prev and next => snap time to prev node endTime
																if (channel.type == SequenceChannel.SequenceChannelType.Animation 
																		&& n.index + 1 < channel.nodes.Count && timeCurrent < channel.nodes [n.index + 1].timeStart
								    
								    							) {

																		node = n;
																		timeCurrent = n.timeStart + n.duration;
																		break;
																		
																}

														}
												}

										}

										//if node found in which time is between node's timeStart and timeEnd or this node is prev node and time is between this timeEnd and next node timeStart
										if (node != null) {
												if (channel.type == SequenceChannel.SequenceChannelType.Animation) {
											
											
													

														animator = channel.target.GetComponent<Animator> ();

														if (animator == null) {
																animator = channel.target.AddComponent<Animator> ();
														}

														animator.runtimeAnimatorController = channel.runtimeAnimatorController;
									
														Quaternion boneRotation = Quaternion.identity;

														
														
													

														AnimationMode.SampleAnimationClip (channel.target, node.source as AnimationClip, (float)(timeCurrent - node.timeStart));

													


						
														///!!! It happen that Unity change the ID of channel or something happen and channal as key not to be found



														//						Correction is needed for root bones as Animation Mode doesn't respect current GameObject transform position,rotation
														//						=> shifting current boneTransform position as result of clip animation, to offset of orginal position before animation(alerady saved)
														if ((rootBoneTransform = channel.targetBoneRoot) != null) {
																//	Debug.Log ("node:" + node.name + " time:" + time + " pos:" + rootBoneTransform.position);
																rootBoneTransform.position = rootBoneTransform.position + node.clipBinding.boneRootPositionOffset;

																rootBoneTransform.rotation = node.clipBinding.boneRootRotationOffset;
																//	Debug.Log ("node:" + node.name + " time:" + time + "After pos:" + rootBoneTransform.position);
														} 

														
												


														 
														if ((animatedValues = channel.target.GetComponent<IAnimatedValues> ()) != null) {
																//Debug.Log ("Sample effector:" + ikAnimatedValues.ik.solver.leftHandEffector.positionWeight + " LHE " + ikAnimatedValues.LHEPositionWeight);
																animatedValues.UpdateValues ();
																//Debug.Log ("Sample effector After:" + ikAnimatedValues.ik.solver.leftHandEffector.positionWeight + " LHE " + ikAnimatedValues.LHEPositionWeight);
								
														}		

										
								
												} else if (channel.type == SequenceChannel.SequenceChannelType.Audio) {
										
														AudioClip audioClip = node.source as AudioClip;

//														if(!node.isRunning)
//														{
//
//															int sampleStart = (int)Math.Ceiling (audioClip.samples * ((__sequence.timeCurrent - node.timeStart) / audioClip.length));
//													     
//															AudioUtilW.SetClipSamplePosition (audioClip, sampleStart);
//															AudioUtilW.PlayClip(audioClip,sampleStart,node.loop);
//														}
														

														
														
										
												} else if (channel.type == SequenceChannel.SequenceChannelType.Video) {
														///!!!I don't know the way of  Sampling or timeshifting of MovieTexture 
							
							
												} else if (channel.type == SequenceChannel.SequenceChannelType.Particle) {
														ParticleSystem particleSystem = node.source as ParticleSystem;
														particleSystem.Simulate ((float)__sequence.timeCurrent - node.timeStart, true);
												}
										} 
										//else if (channel.type == SequenceChannel.SequenceChannelType.Animation && (ikAnimatedValues = channel.target.GetComponent<IAnimatedValues> ()) != null) {
//												{
//												//reset need cos you might have click in one node with ikAnimatedValues then in another => first node should be reseted
//														ikAnimatedValues.Reset ();
//													
//												}
//						
//										}
								}
						}


						
						AnimationMode.EndSampling ();


		
				
						
				

						__update = false;
					

				}


	

		
		
		
				/// <summary>
				/// Raises the timeline GU event.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="frameRate">Frame rate.</param>
				private void DoTimelineGUI (Rect rect, float frameRate)
				{
						Handles.color = new Color (0.5f, 0.5f, 0.5f, 0.2f);
					
						if (__sequence != null) {

								

								//Draw horizontal lines (rows)
								int rows = __sequence.channels.Count + 1;
								int y = 0;
								float lineY = 0f;
								for (; y<rows; y++) {
								
										lineY = TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + y * NODE_RECT_HEIGHT + __timeAreaW.translation.y;
										Handles.DrawLine (new Vector3 (rect.xMin, lineY, 0), new Vector3 (rect.xMax, lineY, 0));	
								
								
								}

							
								//make all area from end of label and Event pad to botton droppable
								sequenceDropSourceEventHandler (new Rect (rect.x, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.width, rect.height - TIME_LABEL_HEIGHT - EVENT_PAD_HEIGHT), Math.Min ((int)((Event.current.mousePosition.y - rect.y - TIME_LABEL_HEIGHT - EVENT_PAD_HEIGHT) / NODE_RECT_HEIGHT), __sequence.channels.Count));
			
								Handles.color = Color.white;

						}

						//TimeArea
						if (__sequence != null)
								__timeAreaW.DoTimeArea (rect, __sequence.frameRate);
						else
								__timeAreaW.DoTimeArea (rect, 3);
					
						
						if (__timeAreaW.vSlider)
								rect.width -= 16f;//slider width =16f
						if (__timeAreaW.hSlider)
								rect.height -= 16f;//slider width =16f

						if (__sequence != null) {


							
							

								//DRAW EVENTS GUI
								DoEventGUI (rect);


								//DRAW NODES 
								int channelNumber = __sequence.channels.Count;
								SequenceChannel channel = null;
								for (int i=0; i<channelNumber; i++) {
										channel = __sequence.channels [i];
										foreach (SequenceNode node in channel.nodes) {
									
												
												if (rect.Contains (Event.current.mousePosition))
														sequenceNodeClickEventHandler (node, rect, i);

												DoNode (node, rect, i);

										
										}
								}


				
								timeScrubberEventHandler (rect);
						

								//draw time scrubber
								Color colorSaved = GUI.color;

								//if(__sequence.isPlaying)
						
								float timeScrubberX = __timeAreaW.TimeToPixel ((float)__sequence.timeCurrent, rect);
								Handles.color = Color.red;
								Handles.DrawLine (new Vector3 (timeScrubberX, rect.y), new Vector3 (timeScrubberX, rect.height));//horizontal scroller
								Handles.color = colorSaved;
							
								if (rect.Contains (Event.current.mousePosition))
										sequenceNodeDragEventHandler (rect);
						}
				}


				/// <summary>
				/// Times the scrubber event handler.
				/// </summary>
				/// <param name="rect">Rect.</param>
				private void timeScrubberEventHandler (Rect rect)
				{
						Event e = Event.current;
					
						//check timeline click
						if (!__sequence.isPlaying && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && new Rect (rect.x, 0, rect.width, TIME_LABEL_HEIGHT).Contains (Event.current.mousePosition)) {
								e.Use ();	
						
						
								onTimelineClick (rect);
						
						
						}
			
				}



				class EditorCurveBindingComparer:IEqualityComparer<EditorCurveBinding>
				{
					#region IEqualityComparer implementation

						public bool Equals (EditorCurveBinding x, EditorCurveBinding y)
						{
								bool equal = x.type == typeof(Transform) 
										&& y.type == typeof(Transform) 
										&& x.path == y.path 
										&& ((x.propertyName.Contains ("m_LocalRotation") && y.propertyName.Contains ("m_LocalRotation"))
										|| (x.propertyName.Contains ("m_LocalPosition") && y.propertyName.Contains ("m_LocalPosition"))
						   				 || (x.propertyName.Contains ("m_LocalScale") && y.propertyName.Contains ("m_LocalScale"))
						    );


								return equal;
						}

						public int GetHashCode (EditorCurveBinding obj)
						{
								int propertyNameHash = obj.propertyName.Split ('.') [0].GetHashCode ();
								return obj.type.GetHashCode () ^ obj.path.GetHashCode () ^ propertyNameHash;
						}

					#endregion



				}

				private static EditorCurveBindingComparer __editorCurveBindingComparer = new EditorCurveBindingComparer ();

				private static GenericMenu createNodeMenu (SequenceNode node)
				{
						GenericMenu genericMenu = new GenericMenu ();

						if (!__sequence.isPlaying && !__sequence.isRecording)
								genericMenu.AddItem (new GUIContent ("Remove"), false, onRemoveNode, node);

						AnimationClip clip = node.source as AnimationClip;
						String path = String.Empty;
						if (clip != null && node.channel.target != null) {
								EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings (clip);

								if (curveBindings.Length > 0)//Remove multiply m_LocalPostion and m_LocalRotation
										curveBindings = curveBindings.Distinct (__editorCurveBindingComparer).ToArray ();

								//curveBindings.GroupBy(itm=>itm.propertyName).ToArray();

								for (int i=0; i<curveBindings.Length; i++) {

										if (curveBindings [i].type == typeof(Animator))
												continue;

										path = curveBindings [i].path;
										if (!String.IsNullOrEmpty (path))
												path += "/";
										path = "Property/" + path + curveBindings [i].propertyName.Split ('.') [0].Replace ("m_LocalRotation", "Rotation").Replace ("m_LocalPosition", "Position");
										genericMenu.AddItem (new GUIContent (path), __nodeSelectedSourceEditorCurveBinding == curveBindings [i], onPropertyAnimatedSelected, curveBindings [i]);
								}
						}

							
						return genericMenu;
				}

				private static void onPropertyAnimatedSelected (object userData)
				{
						__nodeSelectedSourceEditorCurveBinding = (EditorCurveBinding)userData;
						window.Repaint ();
				}
		
		
				/// <summary>
				/// Sequence's node click event handler.
				/// </summary>
				/// <param name="node">Node.</param>
				/// <param name="rect">Rect.</param>
				/// <param name="channelOrd">Channel ord.</param>
				private void sequenceNodeClickEventHandler (SequenceNode node, Rect rect, int channelOrd)
				{

						Event ev = Event.current;
						Rect clickRect;

						float keyFramesRectHeight = __nodeSelected == node ? __keyframeMarker.image.height : 0f;
			
						
						switch (ev.rawType) {
						case EventType.MouseDown:
				
			
								//resize rect at start of the node rect width=5px
								clickRect = new Rect (__timeAreaW.TimeToPixel (node.timeStart, rect) - 5, TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT + __timeAreaW.translation.y, 10, NODE_RECT_HEIGHT);
					
								if (__nodeSelected == node)//resize mouse interaction only on blue rect area (not key frames area)
										clickRect.yMin += __keyframeMarker.image.height;

								clickRect.xMin = Mathf.Clamp (clickRect.xMin, rect.xMin, rect.xMax);
								clickRect.xMax = Mathf.Clamp (clickRect.xMax, rect.xMin, rect.xMax);
								clickRect.yMax = Mathf.Clamp (clickRect.yMax, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);
								clickRect.yMin = Mathf.Clamp (clickRect.yMin, rect.yMin + TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT, rect.yMax);



								//check start of the node rect width=5
								if (clickRect.Contains (Event.current.mousePosition)) {
										
										__nodeSelected = node;

										

										__wasResizingNodeStart = true;

										ev.Use ();
								}
					
								//resize rect at the end of rect width=5x;
								clickRect.x = __timeAreaW.TimeToPixel (node.timeStart + node.duration, rect) - 5;
					
					
								//check the end of node rect width=5
								if (clickRect.Contains (Event.current.mousePosition)) {
										__nodeSelected = node;
										__wasResizingNodeEnd = true;
										ev.Use ();
								}
					
					
								clickRect.x = __timeAreaW.TimeToPixel (node.timeStart, rect) + 5;
								clickRect.width = __timeAreaW.TimeToPixel (node.timeStart + node.duration, rect) - clickRect.x - 5;
					
								if (clickRect.Contains (Event.current.mousePosition)) {
										
										if (ev.button == 0) {
												
												__clickDelta = node.timeStart - __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
												__isDragged = true;
												__nodeSelected = node;

//												if (__sequence.isRecording)
//														Selection.activeGameObject = node.channel.target;
										} else
										if (ev.button == 1) {
												__nodeSelected = node;
												createNodeMenu (node).ShowAsContext ();
												
										}
										ev.Use ();
								}

								break;
				
						case EventType.ContextClick:

								

								clickRect = new Rect (0, 0, 0, NODE_RECT_HEIGHT - keyFramesRectHeight);
				

					
								clickRect.x = __timeAreaW.TimeToPixel (node.timeStart, rect);
								clickRect.y = TIME_LABEL_HEIGHT + EVENT_PAD_HEIGHT + channelOrd * NODE_RECT_HEIGHT;
								
								clickRect.width = __timeAreaW.TimeToPixel (node.timeStart + node.duration, rect) - clickRect.x;
					
								if (clickRect.Contains (Event.current.mousePosition)) {
										__nodeSelected = node;
										createNodeMenu (node).ShowAsContext ();
										
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
						//Rect clickRect;

						float startTime;
						float leftOffset = 0f;
						float rightOffset = 0f;

						switch (ev.rawType) {

						case EventType.MouseDrag:

								//TODO don't allow resize bigger then sound.length 
								//TODO don't allow any video resize as MovieTexture doesn't support it
								//TODO sound can't be decreased not increased
								


								if (__wasResizingNodeStart) {
										
										//float prevStartTime = __nodeSelected.startTime;
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
										

										if (startTime >= 0) {
												//__nodeSelected.startTime = TimeAreaW.SnapTimeToWholeFPS (startTime, __frameRate);
												//__nodeSelected.duration += TimeAreaW.SnapTimeToWholeFPS (prevStartTime - __nodeSelected.startTime, __sequence.frameRate);
												
												
											

												
												ev.Use ();
										}
										

										
								}

								if (__wasResizingNodeEnd) {
										
										//__nodeSelected.duration = TimeAreaW.SnapTimeToWholeFPS (__timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) - __nodeSelected.startTime, __sequence.frameRate);
										ev.Use ();
								}

								//PAN of the node
								if (__isDragged && !__wasResizingNodeStart && !__wasResizingNodeEnd) {
										
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) + __clickDelta;

										startTime = TimeAreaW.SnapTimeToWholeFPS (startTime, __sequence.frameRate);

										if (startTime >= 0) {

												
												SequenceChannel channel = __nodeSelected.channel;

												SequenceNode nodePrev = null;
												SequenceNode nodeNext = null;

												float nodeEnd = 0f;

												if (channel.nodes.Count > 1) {

														//find if prev node from selected node exist
														if (__nodeSelected.index > 0) {
																nodePrev = channel.nodes [__nodeSelected.index - 1];

																if (__nodeSelected.duration < nodePrev.duration)
																		leftOffset = nodePrev.duration - __nodeSelected.duration;
														}

														//find if next node from selected node exist
														if (__nodeSelected.index + 1 < channel.nodes.Count) { //if next node exist
																nodeNext = channel.nodes [__nodeSelected.index + 1];

																if (__nodeSelected.duration < nodeNext.duration)
																		rightOffset = nodeNext.duration - __nodeSelected.duration;
														}

														if (__nodeSelected.source is AnimationClip) { //nodes animation clips are allowed to overlap(transitions)

																__nodeSelected.transition = 0f;
															
																if (nodePrev != null && nodeNext != null) {//if there is prev and next node of current selected node
	

																		//restrict selected node draging 
																		if (nodePrev.timeStart + leftOffset < startTime && startTime + __nodeSelected.duration < nodeNext.timeStart + nodeNext.duration - rightOffset) {
																				__nodeSelected.timeStart = startTime;

																				//////////////////////////////////////
																				//calc transition with prev node with selected
																				__nodeSelected.transition = 0f;
																				nodeEnd = nodePrev.timeStart + nodePrev.duration;
																				if (startTime < nodeEnd) {
																						__nodeSelected.transition = TimeAreaW.SnapTimeToWholeFPS (nodeEnd - startTime, __sequence.frameRate);
																						//Debug.Log ("Transition Prev:" + __nodeSelected.transition);
																				}

																				/////////////////////////////
																				//calc transition from selected to next node
																				nodeNext.transition = 0f;//reset
																				nodeEnd = startTime + __nodeSelected.duration;
																				if (nodeEnd > nodeNext.timeStart) {
																						nodeNext.transition = TimeAreaW.SnapTimeToWholeFPS (nodeEnd - nodeNext.timeStart, __sequence.frameRate);
																						//Debug.Log ("Transition Next:" + nodeNext.transition);
																				}


																		}
																} else if ((nodePrev != null && nodePrev.timeStart + leftOffset < startTime) || (nodeNext != null && startTime + __nodeSelected.duration < nodeNext.timeStart + nodeNext.duration - rightOffset)) {
																		__nodeSelected.timeStart = startTime;


																		if (nodePrev != null) {
																				//////////////////////////////////////
																				//calc transition with prev node with selected
																				__nodeSelected.transition = 0f;
																				nodeEnd = nodePrev.timeStart + nodePrev.duration;
																				if (startTime < nodeEnd) {
																						__nodeSelected.transition = TimeAreaW.SnapTimeToWholeFPS (nodeEnd - startTime, __sequence.frameRate);
																						//Debug.Log ("Transition Only Prev:" + __nodeSelected.transition);
																				}
																		}
																		
																		if (nodeNext != null) {
																				/////////////////////////////
																				//calc transition from selected to next node
																				nodeNext.transition = 0f;
																				nodeEnd = startTime + __nodeSelected.duration;
																				if (nodeEnd > nodeNext.timeStart) {
																						nodeNext.transition = TimeAreaW.SnapTimeToWholeFPS (nodeEnd - nodeNext.timeStart, __sequence.frameRate);
																						//Debug.Log ("Transition Only Next:" + nodeNext.transition);
																				}
																		}





																	    
																}
									
																Repaint ();

																//ANY OTHER CHANNEL TYPE	
														} else {
																if (nodePrev != null && nodeNext != null) {//if there is prev and next node of current
																		if (nodePrev.timeStart + nodePrev.duration < startTime && startTime + __nodeSelected.duration < nodeNext.timeStart) {
																				__nodeSelected.timeStart = startTime;
																		}

																} else if ((nodePrev != null && nodePrev.timeStart + nodePrev.duration < startTime) || (nodeNext != null && startTime + __nodeSelected.duration < nodeNext.timeStart)) {
																		__nodeSelected.timeStart = startTime;
																}
														}
												} else
														__nodeSelected.timeStart = startTime;


												
							 


										}// startTime>=0
								}// if PAN
										

										
										//change channel 
//										if (Event.current.mousePosition.y > selectedNode.channel * __nodeRectHeight + 25) {
//												selectedNode.channel += 1;
//										}
//										if (Event.current.mousePosition.y < selectedNode.channel * __nodeRectHeight - 5) {
//												selectedNode.channel -= 1;
//										}
//										selectedNode.channel = Mathf.Clamp (selectedNode.channel, 0, int.MaxValue);
								ev.Use ();
								
								break;
						case EventType.MouseUp:
								__isDragged = false;
								__wasResizingNodeStart = false;
								__wasResizingNodeEnd = false;
								break;
						}
				}

				private static void RemoveNode (SequenceNode node, SequenceChannel sequenceChannel)
				{
						if (node.source is AnimationClip) {
								if (node.index + 1 < sequenceChannel.nodes.Count)//if prev node exist reset its transition
										sequenceChannel.nodes [node.index + 1].transition = 0;
				
								//bypass transition
								//								if (node.index - 1 > -1 && node.index + 1 < sequenceChannel.nodes.Count) {
				
								//										UnityEditor.Animations.AnimatorController animatorController = (sequenceChannel.runtimeAnimatorController as UnityEditor.Animations.AnimatorController);
								//										UnityEditor.Animations.AnimatorState statePrev = animatorController.GetStateBy (sequenceChannel.nodes [node.index - 1].stateNameHash);
								//								
								//										UnityEditor.Animations.AnimatorStateTransition transition = statePrev.transitions [0];
								//										transition.destinationState = animatorController.GetStateBy (sequenceChannel.nodes [node.index + 1].stateNameHash);
								//								
								//								}
						}
			
			
			
						//remove node
						sequenceChannel.nodes.Remove (node);
						DestroyImmediate (node);
			
			
						if (sequenceChannel.runtimeAnimatorController != null)//remove AnimatorState
								(sequenceChannel.runtimeAnimatorController as UnityEditor.Animations.AnimatorController).RemoveStateWith (node.stateNameHash, node.layerIndex);
			
						int nodesCount = sequenceChannel.nodes.Count;
			
			
						//reindex nodes after remove
						for (int i=node.index; i<nodesCount; i++)
								sequenceChannel.nodes [i].index--;
			
			
			
			
			
						//if this removed node was last node => remove channel and move channels up(reindex)
						if (sequenceChannel.nodes.Count == 0) {
				
								//remove channel
								__sequence.channels.Remove (sequenceChannel);
				
								if (sequenceChannel == __sequence.channelSelected)
										__sequence.channelSelected = null;
				
								DestroyImmediate (sequenceChannel);
				
						} 

			
				}
		
		
		
				/// <summary>
				/// Removes the node.
				/// </summary>
				/// <param name="data">Data.</param>
				private static void onRemoveNode (object data)
				{
						SequenceNode node = data as SequenceNode;
						
					

						SequenceChannel sequenceChannel = __sequence.channels.Find (itm => itm.nodes.Exists (nd => nd.GetInstanceID () == node.GetInstanceID ()));

			
						
						RemoveNode (node, sequenceChannel);
			
			
						__nodeSelected = null;
			
						

						Stop ();//cos target can be "paused" in some animation time t
						//if not stop animation mode reset resets to Vector3D.zero

						__sequence.timeCurrent = 0f;
			
						ResetChannelTarget (sequenceChannel);//if target is in pause somewhere in time reset to Orginal pos/rot
			
			
						if (!AnimationMode.InAnimationMode ())
								AnimationMode.StartAnimationMode ();
						ReOffsetNodesAnimation (sequenceChannel);//Now with node removed
						AnimationMode.StopAnimationMode ();
			
			
						ResetChannelTarget (sequenceChannel);
			
						window.Repaint ();
				}



				/// <summary>
				/// Handles the game object selection changed event thru menu.
				/// </summary>
				/// <param name="data">Data.</param>
				private void OnGameObjectSelectionChanged (object data)
				{
						Sequence sequence = data as Sequence;
						Selection.activeGameObject = sequence.gameObject;
						__sequenceGameObject = sequence.gameObject;
						__sequence = sequence;

						
				}

				private void OnFocus ()
				{
						//Debug.Log ("Focus");
						selectionChangeEventHandler ();
				}


				/// <summary>
				/// Sequences the drop source event handler.
				/// </summary>
				/// <param name="area">Area.</param>
				/// <param name="channel">Channel.</param>
				private void sequenceDropSourceEventHandler (Rect area, int channel)
				{

						if (__sequence.isPlaying || __sequence.isRecording)
								return;
			
						Event evt = Event.current;
			
						switch (evt.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
								if (!area.Contains (evt.mousePosition))
										return;
								DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

								Type type;
								UnityEngine.Object component;
				
								if (evt.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag ();

										if (EditorApplication.isPlaying) {
												Debug.Log ("Can't add tween object in play mode. Stop the play mode and readd it.");
										} else {
												foreach (UnityEngine.Object droppedObject in DragAndDrop.objectReferences) {

														//Debug.Log ("Dropped object of type:" + dragged_object);
														
														//handle sound, video, animation clip
														type = droppedObject.GetType ();



														if (type == typeof(UnityEngine.AnimationClip) || 
																type == typeof(UnityEngine.AudioClip) ||
																type == typeof(UnityEngine.MovieTexture) ||
																type == typeof(GameObject) && (component = ((GameObject)droppedObject).GetComponent<ParticleSystem> ()) != null) {

																
																Stop ();

																AddDropToNewOrExistingChannel (component != null ? component : droppedObject, component != null ? component.GetType () : type, channel, Event.current.mousePosition);
														}//draggedType check
														else
																Debug.LogWarning ("Unsuppored type. Audio,Video,Animaiton Clip or GameObject with ParticleSystem supported.");
												}//

												EditorUtility.SetDirty (__sequence);
										}
								}
								break;
						}
			
				}

				


				
				/// <summary>
				/// Adds the source(drop object) to new or existing channel.
				/// //allow only dropping multiply items of same type in channel
				/// </summary>
				/// <param name="source">Source.</param>
				/// <param name="type">Type.</param>
				/// <param name="channel">Channel ord</param>
				/// <param name="pos">Position.</param>
				private static void AddDropToNewOrExistingChannel (UnityEngine.Object droppedObject, Type type, int channel, Vector2 pos)
				{
						SequenceChannel sequenceChannel = null;
						if (__sequence.channels.Count > channel)
								sequenceChannel = __sequence.channels [channel];

						AddDropToNewOrExistingChannel (droppedObject, type, sequenceChannel, pos);
				}

				private static void AddDropToNewOrExistingChannel (UnityEngine.Object droppedObject, Type type, SequenceChannel sequenceChannel, Vector2 pos)
				{


						if (sequenceChannel == null) {
								sequenceChannel = (SequenceChannel)ScriptableObject.CreateInstance<SequenceChannel> ();
				
				
								if (type == typeof(AnimationClip)) {
					
										//open Animation controller for the channel
										string path = EditorUtility.OpenFilePanel (
						"Select AnimaitonController",
						"Assets",
						"controller");
					
										if (String.IsNullOrEmpty (path))
												return;
					
										sequenceChannel.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath (AssetDatabaseUtility.AbsoluteUrlToAssets (path), typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
					
										sequenceChannel.name = "Animation";
					
								} else if (type == typeof(AudioClip)) {
										sequenceChannel.name = "Audio";
										sequenceChannel.type = SequenceChannel.SequenceChannelType.Audio;
								} else if (type == typeof(MovieTexture)) {
										sequenceChannel.name = "Video";
										sequenceChannel.type = SequenceChannel.SequenceChannelType.Video;
								} else if (type == typeof(ParticleSystem)) {
										sequenceChannel.name = "Particle";
										sequenceChannel.type = SequenceChannel.SequenceChannelType.Particle;
								}
				
								sequenceChannel.sequence = __sequence;
								__sequence.channels.Add (sequenceChannel);
						} else
			
								if (sequenceChannel.nodes.Count > 0) {
								if (sequenceChannel.nodes [0].source.GetType () != type) {
										Debug.LogWarning ("You can have only same type nodes in same channel");	
										return;
								}
						}

						int layerIndex = 0;
						if (sequenceChannel.type == SequenceChannel.SequenceChannelType.Animation) {
								//SHOW SELECTION LAYER DIALOGS
								UnityEditor.Animations.AnimatorController animatorController = sequenceChannel.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

								
			
								if (animatorController.layers.Length == 1) {

										if (!EditorUtility.DisplayDialog ("Layer Selection", "Select Layer", animatorController.layers [0].name, "New Layer"))
												layerIndex = 1;
								} else if (animatorController.layers.Length == 2) {

										layerIndex = EditorUtility.DisplayDialogComplex ("Layer Selection", "Select Layer", animatorController.layers [0].name, animatorController.layers [1].name, "New Layer");
								} else {
							
										while (layerIndex<animatorController.layers.Length && !EditorUtility.DisplayDialog ("Layer Selection", "Select Layer", animatorController.layers [layerIndex].name, "Next")) {
												layerIndex++;
										}
					
					
								}
						}
						

		

			
			
						//Vector2
						//TODO current logic prevents overlapping on drop (might be ehnaced to allow transition overlap in Animation's node to some meassure)
						//and if they overlapp move start time to fit (there is bug)
						SequenceNode node = CreateNewSequenceNode (pos, droppedObject, sequenceChannel, layerIndex);
			
			
						if (node != null) {
								sequenceChannel.nodes.Insert (node.index, node);
				
								int nodesCount = sequenceChannel.nodes.Count;
				
								//reindex nodes after insert
								for (int i=node.index+1; i<nodesCount; i++)
										sequenceChannel.nodes [i].index++;
				
				
				
								Stop ();
								//AnimationMode.StopAnimationMode ();//stop animation mode cos target can be "paused" in some point in time
								///and Reset channel will fail
								__sequence.timeCurrent = 0f;
				
								//be sure that while droping new node paused target is reseted to Orginal position
								ResetChannelTarget (sequenceChannel);
				
								if (!AnimationMode.InAnimationMode ())
										AnimationMode.StartAnimationMode ();
								ReOffsetNodesAnimation (sequenceChannel);//Now with new node added
								AnimationMode.StopAnimationMode ();
				
								ResetChannelTarget (sequenceChannel);
				
						}
			
			
			
			
			
						__nodeSelected = node;


				}

							

				/// <summary>
				/// Creates the sequence node.
				/// </summary>
				/// <returns>The sequence node.</returns>
				/// <param name="pos">Position.</param>
				/// <param name="source">Source.</param>
				/// <param name="channel">Channel.</param>
				private static SequenceNode CreateNewSequenceNode (Vector2 pos, UnityEngine.Object source, SequenceChannel sequenceChannel, int layerIndex=0)
				{


						float duration = 0f;


						float startTime = 0f;

						
						startTime = TimeAreaW.SnapTimeToWholeFPS (__timeAreaW.PixelToTime (pos.x, __timeAreaW.rect), __sequence.frameRate);
						duration = TimeAreaW.SnapTimeToWholeFPS (duration, __sequence.frameRate);

						

						//prevent intersection on Drop
						if (sequenceChannel.nodes.Exists (itm => (itm.timeStart < startTime && startTime < itm.timeStart + itm.duration) || (itm.timeStart < startTime + duration && startTime + duration < itm.timeStart + itm.duration)))
								return null;

						


						SequenceNode node = ScriptableObject.CreateInstance<SequenceNode> ();			
					
						node.source = source;
			
			
						node.channel = sequenceChannel;

						
					
						node.name = source.name;
						
					

					
					
						node.timeStart = startTime;
						

						int startTimeEarliestIndex = -1;

						if (node.source is AnimationClip) {

								
					
								UnityEditor.Animations.AnimatorController animatorController = node.channel.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;



								if (animatorController.layers.Length - 1 < layerIndex)
										animatorController.AddLayer ("New Layer " + layerIndex);

								//create and add AnimatorState in controller from node.source
								UnityEditor.Animations.AnimatorState stateCurrent = animatorController.AddMotion ((source as Motion), layerIndex);

								//save AnimatorState ID hase
								node.stateNameHash = stateCurrent.nameHash;
								node.layerIndex = layerIndex;

								node.clipBinding = ScriptableObject.CreateInstance<EditorClipBinding> ();
				
								//if stateCurrent is first and default create empty state for default
								if (animatorController.layers [layerIndex].stateMachine.defaultState == stateCurrent) {
										stateCurrent = animatorController.layers [layerIndex].stateMachine.AddState ("DefaultState");
										animatorController.layers [layerIndex].stateMachine.defaultState = stateCurrent;
								}


						}
				

								
				
						//UnityEditor.Animations.AnimatorStateTransition transition = null;
				
						

								
						if (sequenceChannel.nodes.Count > -1) {

								//find index of node with earliest startTime
								startTimeEarliestIndex = sequenceChannel.nodes.FindLastIndex (itm => itm.timeStart < node.timeStart);

						}
						
					
						node.index = startTimeEarliestIndex + 1;
						
			
						return node;
				}

				private static void CreateRuntimeAnimatorController ()
				{
						string path = EditorUtility.SaveFilePanel (
				"Create AnimaitonController",
				"Assets",
				"",
				"controller");
			
						if (!String.IsNullOrEmpty (path)) {
				
				
								//create Controller
								UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath (AssetDatabaseUtility.AbsoluteUrlToAssets (path));
				
				
						}
			
			
			
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

								if (__sequence != null) {
										clip.frameRate = __sequence.frameRate;


										//add to current channel after last node
										Vector2 pos = Vector2.zero;
										if (__sequence.channelSelected != null && __sequence.channelSelected.type == SequenceChannel.SequenceChannelType.Animation) {
												SequenceNode node = __sequence.channelSelected.nodes [__sequence.channelSelected.nodes.Count - 1];
												pos.x = __timeAreaW.TimeToPixel (node.timeStart + node.duration + 1, __timeAreaW.rect);
									
										} else {
												pos.x = __timeAreaW.TimeToPixel (1f, __timeAreaW.rect);
										}
								
										AddDropToNewOrExistingChannel (clip, typeof(AnimationClip), __sequence.channelSelected, pos);
								}

						
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
					
					
						Selection.activeGameObject = __sequenceGameObject;
				}
				




				///////////////// SEQUENCE NODE START/END HANDLES /////////////////


				/// <summary>
				/// Ons the sequence node start.
				/// </summary>
				/// <param name="node">Node.</param>
				public static void onSequenceNodeStart (SequenceNode node)
				{

						//Debug.Log ("onSequenceNodeStart " + node.name);
			          
						if (node.source is AudioClip) {
								AudioClip audioClip = node.source as AudioClip;
								int sampleStart = (int)Math.Ceiling (audioClip.samples * ((__sequence.timeCurrent - node.timeStart) / audioClip.length));
																	
								AudioUtilW.PlayClip (node.source as AudioClip, sampleStart, node.loop);//startSample doesn't work in this function????
								AudioUtilW.SetClipSamplePosition (audioClip, sampleStart);//sample function should follow to play from startSample positionAudioUtilW.SetClipSamplePosition (audioClip, sampleStart);
						} else if (node.source is MovieTexture) {
								MovieTexture movieTexture = node.source as MovieTexture;
								if (node.channel.target.tag == Tags.MAIN_CAMERA) {//==Camera.main

										Debug.Log (node.name + " RenderSettings.skybox isn't supported in EditMode");
										//RenderSettings.skybox.mainTexture = movieTexture;
										
										
								} else {

										Renderer renderer = node.channel.target.GetComponent<Renderer> ();
										if (renderer != null) {
												renderer.material = new Material (Shader.Find ("Unlit/Texture"));	

												renderer.material.mainTexture = movieTexture;
												//renderer.sharedMaterial.mainTexture=movieTexture;//Instantiating material due to calling renderer.material during edit mode. This will leak materials into the scene. You most likely want to use renderer.sharedMaterial instead.
												//renderer.material.shader=Shader.Find("Unlit/Texture");

												//remapp uv so it is not upsidedown


												//renderer.sharedMaterial.mainTexture = movieTexture;

															
												
										}
								}

								//This line would play movieTexture sound but if you have other sound would stack sound samplerr
								//if (movieTexture.audioClip != null)
								//		AudioUtilW.PlayClip (movieTexture.audioClip, 0);//startSample doesn't work in this function????

				
				
				
//								AudioSource audioSource = null;
//										if (movieTexture.audioClip != null) {
//											
//											audioSource = node.channel.target.GetComponent<AudioSource> ();
//											
//											if (audioSource == null)
//												audioSource = (AudioSource)node.channel.target.AddComponent (typeof(AudioSource));
//											
//											audioSource.clip = movieTexture.audioClip;
//					                        
//											audioSource.PlayOneShot (audioSource.clip);
//											//audioSource.Play();
//											
//											
//										}
				
								movieTexture.Play ();

						} 
				}






				/// <summary>
				/// Ons the sequence end.
				/// </summary>
				/// <param name="sequence">Sequence.</param>
				public static void onSequenceEnd (object data)
				{
						
						Stop ();
				}


				/// <summary>
				/// Ons the sequence node stop.
				/// </summary>
				/// <param name="node">Node.</param>
				public static void onSequenceNodeStop (SequenceNode node)
				{
						 
			
				}

			

				/// <summary>
				/// Gets the audio clip texture.
				/// </summary>
				/// <returns>The audio clip texture.</returns>
				/// <param name="clip">Clip.</param>
				/// <param name="width">Width.</param>
				/// <param name="height">Height.</param>
				public static Texture2D GetAudioClipTexture (AudioClip clip, float width, float height)
				{
						if (clip == null) {
								return null;
						}
						AudioImporter audioImporter = (AudioImporter)AssetImporter.GetAtPath (AssetDatabase.GetAssetPath (clip));

						Texture2D[] array = new Texture2D[clip.channels];
						for (int i = 0; i < clip.channels; i++) {
								array [i] = (Texture2D)AudioUtilW.GetWaveForm (
				                                    
					clip,
					audioImporter,
					i,
					width,
					height / (float)clip.channels
								);
						}


						return AudioClipInspectorW.CombineWaveForms (array);
				}


		


				/// <summary>
				/// MonoBehavior binding
				/// Handle the selection(gameobject) change event.
				/// this is EditorWindow function and its autobindend by name
				/// </summary>
				void OnSelectionChange ()
				{
						selectionChangeEventHandler ();
				}

				private static void selectionChangeEventHandler ()
				{

						GameObject activeGameObject = Selection.activeGameObject;

						if (activeGameObject != null && activeGameObject != __sequenceGameObject) {

								
								

								Sequence sequence = activeGameObject.GetComponent<Sequence> ();
								if (sequence != null) {
										__sequenceGameObject = sequence.gameObject;
										__sequence = sequence;

										if (__sequence.isPlaying || __sequence.isRecording)
												Stop ();
					
										if (SceneView.currentDrawingSceneView != null)
												SceneView.currentDrawingSceneView.Repaint ();
									
										if (window != null)
												window.Repaint ();

									

										__timeAreaW.hTicks.SetTickModulosForFrameRate (__sequence.frameRate);


										





								}



								
						}

						if (activeGameObject != null 
								&& __sequence != null
								&& __nodeSelected != null
								&& __nodeSelected.channel.target != null
								&& __nodeSelected.channel.type == SequenceChannel.SequenceChannelType.Animation
							   ) {

								Animator component = __nodeSelected.channel.target.GetComponent<Animator> ();
								if (component != null 
										&& component.isHuman
										&& component.IsBoneTransform (activeGameObject.transform)
										&& (activeGameObject == __nodeSelected.channel.target || activeGameObject.transform.IsChildOf (__nodeSelected.channel.target.transform))) {

										EditorCurveBinding curveBinding = default(EditorCurveBinding);

										String path = AnimationUtility.CalculateTransformPath (activeGameObject.transform, __nodeSelected.channel.target.transform);

										EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings (__nodeSelected.source as AnimationClip);
					
										if (Tools.current == Tool.Move) {
												curveBinding = AnimationUtility.GetCurveBindings (__nodeSelected.source as AnimationClip).FirstOrDefault (itm => itm.type == typeof(Transform) && itm.propertyName.StartsWith ("m_LocalPosition.") && itm.path == path);

										} else if (Tools.current == Tool.Rotate) {
												curveBinding = AnimationUtility.GetCurveBindings (__nodeSelected.source as AnimationClip).FirstOrDefault (itm => itm.type == typeof(Transform) && (itm.propertyName.StartsWith ("m_LocalRotation.") || itm.propertyName.StartsWith ("localEulerAngles.") || itm.propertyName.StartsWith ("localEulerAnglesBaked.")) && itm.path == path);
					
					
										} else if (Tools.current == Tool.Scale) {
												curveBinding = AnimationUtility.GetCurveBindings (__nodeSelected.source as AnimationClip).FirstOrDefault (itm => itm.type == typeof(Transform) && itm.propertyName.Contains ("m_LocalScale.") && itm.path == path);
					
										}



										if (!String.IsNullOrEmpty (curveBinding.propertyName)) {
												__nodeSelectedSourceEditorCurveBinding = curveBinding;
												if (window != null)
														window.Repaint ();
										}

								}
						}

				}
		
		
		}
}