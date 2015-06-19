using UnityEngine;
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

namespace VisualTween
{
		public class SequenceEditor : EditorWindow
		{
				private Timeline timeline;
				private static Sequence sequence;
				private Sequence.SequenceWrap wrap = Sequence.SequenceWrap.ClampForever;
				private GameObject sequenceGameObject;
				private Vector2 settingsScroll;
				private bool dragNode;
				private float timeClickOffset;
				private bool resizeNodeStart;
				private bool resizeNodeEnd;
				private List<Record> records;
				private bool lastRecordState;
				private bool lastPlayState;
				private float playStartTime;
				private bool stop;
				private bool playForward;
				private float time;
				private static TimeAreaW __timeAreaW;
				private static SequenceNode __testNode;
				private static ReorderableList __gameObjectClipList;
				private static bool __isPlaying;
				private static bool __isRecording;
				private static int __frameRate = 30;
				private const float NODE_RECT_HEIGHT = 20f;
				private const float TIME_LABEL_HEIGHT = 40f;//20f for timer ruller + 20f for Events Pad
		
				private static SequenceNode selectedNode {
						get {
								if (sequence != null) {
										return sequence.selectedNode;
								}
								return null;
						}
						set {
								sequence.selectedNode = value;
						}
				}

				[MenuItem("Window/Visual Tween/Sequence", false)]
				public static void ShowWindow ()
				{
						SequenceEditor window = EditorWindow.GetWindow<SequenceEditor> (false, "Sequence");
						window.wantsMouseMove = true;
						UnityEngine.Object.DontDestroyOnLoad (window);


	
				}

				private void OnEnable ()
				{


//			base.hSlider = true;
//			base.vSlider = false;
//			base.vRangeLocked = true;
//			base.hRangeMin = 0f;
//			base.margin = 40f;
//			base.scaleWithWindow = true;
//			base.ignoreScrollWheelUntilClicked = false;
//			base.hTicks.SetTickModulosForFrameRate (state.frameRate);

						if (__timeAreaW == null) {
								__timeAreaW = new TimeAreaW (false);

								//__timeAreaW.rect = new Rect (0, 0, Screen.width, Screen.height);
								__timeAreaW.hSlider = true;
								__timeAreaW.vSlider = false;
								__timeAreaW.vRangeLocked = true;
								__timeAreaW.hRangeMin = 0f;
								__timeAreaW.margin = 0f;
//				__timeAreaW.margin = 40f;
								__timeAreaW.scaleWithWindow = true;
								//__timeAreaW.ignoreScrollWheelUntilClicked = false;
								__timeAreaW.hTicks.SetTickModulosForFrameRate (30f);


								
			
						}




						if (timeline == null) {
								timeline = new Timeline ();


//				__gameObjectClipList = new ReorderableList (sequence.nodes, typeof(SequenceNode), true, true, true, true);
//				__gameObjectClipList.drawElementCallback = onDrawElement;
//				
//				
//				
//				__gameObjectClipList.drawHeaderCallback = onDrawHeaderElement;
//				
//				__gameObjectClipList.onRemoveCallback = onRemoveCallback;
//				__gameObjectClipList.onAddCallback = onAddCallback;
//				__gameObjectClipList.onSelectCallback = onSelectCallback;
						}

						timeline.onSettingsGUI = OnSettingsGUI;	
						timeline.onEventGUI = OnEventGUI;
						timeline.onAddEvent = OnAddEvent;
						//timeline.onTimelineGUI = OnTimelineGUI;
						timeline.onRecord = OnRecord;
						timeline.onTimelineClick = OnTimelineClick;
						timeline.onPlay = OnPlay;
						EditorApplication.playmodeStateChanged += OnPlayModeStateChange;
				}

				private static void DoNode (SequenceNode node, Rect rect)
				{
						//Rect boxRect=new Rect(timeline.SecondsToGUI(node.startTime),node.channel*20,timeline.SecondsToGUI(node.duration),20);

						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.startTime, rect) - 5, TIME_LABEL_HEIGHT + node.channel * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT), MouseCursor.ResizeHorizontal);			
						EditorGUIUtility.AddCursorRect (new Rect (__timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - 5, TIME_LABEL_HEIGHT + node.channel * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT), MouseCursor.ResizeHorizontal);

			
			
						float x = __timeAreaW.TimeToPixel (node.startTime, rect);
						Rect boxRect = new Rect (x, TIME_LABEL_HEIGHT + node.channel * NODE_RECT_HEIGHT, __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - x, NODE_RECT_HEIGHT);

						GUI.Box (boxRect, "", "TL LogicBar 0");

						boxRect.xMin += 5;
						boxRect.xMax -= 5;
						EditorGUIUtility.AddCursorRect (boxRect, MouseCursor.Pan);

						

						
			
						GUIStyle style = new GUIStyle ("Label");
						style.fontSize = (selectedNode == node ? 12 : style.fontSize);
						style.fontStyle = (selectedNode == node ? FontStyle.Bold : FontStyle.Normal);
						Color color = style.normal.textColor;
						color.a = (selectedNode == node ? 1.0f : 0.7f);
						style.normal.textColor = color;
						Vector3 size = style.CalcSize (new GUIContent (node.target.name));
						Rect rect1 = new Rect (boxRect.x + boxRect.width * 0.5f - size.x * 0.5f, boxRect.y + boxRect.height * 0.5f - size.y * 0.5f, size.x, size.y);
						GUI.Label (rect1, node.target.name, style);
			
			
			
				}
		
				private static void onRemoveCallback (ReorderableList list)
				{
						if (UnityEditor.EditorUtility.DisplayDialog ("Warning!", 
			                                             "Are you sure you want to delete the Unity Variable?", "Yes", "No")) {
//				List<EditorClipBinding> bindingList = ((EditorClipBinding[])clipBindingsSerialized.value).ToList ();
//				bindingList.RemoveAt (list.index);
//				
//				clipBindingsSerialized.value = bindingList.ToArray ();
//				//		clipBindingsSerialized.ValueChanged();
//				//		clipBindingsSerialized.ApplyModifiedValue ();
//				__serializedNode.ApplyModifiedProperties ();
//				
//				list.list = clipBindingsSerialized.value as IList;
				
				
				
						}
				}
		
		
				/// <summary>
				/// Ons the add callback.
				/// </summary>
				/// <param name="list">List.</param>
				private static void onAddCallback (ReorderableList list)
				{
//			List<EditorClipBinding> bindingList = ((EditorClipBinding[])clipBindingsSerialized.value).ToList ();
//			
//			
//			bindingList.Add (ScriptableObject.CreateInstance<EditorClipBinding> ());
//			
//			clipBindingsSerialized.value = bindingList.ToArray ();
//			//clipBindingsSerialized.ValueChanged();
//			//clipBindingsSerialized.ApplyModifiedValue ();
//			__serializedNode.ApplyModifiedProperties ();
//			
//			
//			
//			list.list = clipBindingsSerialized.value as IList;
			
				}
		
		
				/// <summary>
				/// Handles draw list element.
				/// </summary>
				/// <param name="rect">Rect.</param>
				/// <param name="index">Index.</param>
				/// <param name="isActive">If set to <c>true</c> is active.</param>
				/// <param name="isFocused">If set to <c>true</c> is focused.</param>
				private static void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
				{
			

			
				}
		
		
				/// <summary>
				/// Handles draw header element event.
				/// </summary>
				/// <param name="rect">Rect.</param>
				static void onDrawHeaderElement (Rect rect)
				{
						EditorGUI.LabelField (rect, "GameObject - AnimationClips:");
				}
		
				/// <summary>
				/// Handles the select list item event.
				/// </summary>
				/// <param name="list">List.</param>
				static void onSelectCallback (ReorderableList list)
				{
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
								timeline.isRecording = true;
								timeline.isPlaying = true;
								StartRecord ();
						} else {
								StopRecord ();
								OnPlay (false);
								timeline.isRecording = false;
								timeline.isPlaying = false;
						}
				}

				private void Update ()
				{
						if (sequenceGameObject != null) {
								sequence = sequenceGameObject.GetComponent<Sequence> ();
						}

						if (sequence != null) {
								sequence.nodes.RemoveAll (x => x.target == null);	
								if (!sequence.nodes.Contains (selectedNode)) {
										selectedNode = null;
								}
								if (!EditorApplication.isPlaying) {
										if (lastRecordState) {
												EditorUpdate (timeline.CurrentTime, false);
										}
								}
								if (lastPlayState && !stop) {
										if ((float)EditorApplication.timeSinceStartup > time) {
												switch (wrap) {
												case Sequence.SequenceWrap.PingPong:
														playForward = !playForward;
														time = (float)EditorApplication.timeSinceStartup + GetSequenceEnd ();
														if (playForward) {
																timeline.CurrentTime = 0;
																playStartTime = (float)EditorApplication.timeSinceStartup;
														}
														break;
												case Sequence.SequenceWrap.Once:
														sequence.Stop (false);
														playStartTime = (float)EditorApplication.timeSinceStartup;
														timeline.CurrentTime = 0;
														stop = true;
														break;
												case Sequence.SequenceWrap.ClampForever:
														sequence.Stop (true);
														stop = true;
														break;
												case Sequence.SequenceWrap.Loop:
														sequence.Stop (false);
														playStartTime = (float)EditorApplication.timeSinceStartup;
														timeline.CurrentTime = 0;
														stop = false;
														time = (float)EditorApplication.timeSinceStartup + GetSequenceEnd ();
														break;
												}		
										}

										timeline.CurrentTime = (playForward ? ((float)EditorApplication.timeSinceStartup - playStartTime) : time - (float)EditorApplication.timeSinceStartup);

										EditorUpdate (timeline.CurrentTime, false);
										Repaint ();
								}

								if (EditorApplication.isPlaying) {
										timeline.CurrentTime = sequence.passedTime;
										Repaint ();
								}
						} else {
								sequence = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<Sequence> () : null;			
						}
				}

				public float GetSequenceEnd ()
				{
						if (sequence == null) {
								return Mathf.Infinity;			
						}
						float sequenceEnd = 0;
						foreach (SequenceNode node in sequence.nodes) {
								if (sequenceEnd < (node.startTime + node.duration)) {
										sequenceEnd = node.startTime + node.duration;
								}
						}
						return sequenceEnd;
				}

				public void EditorUpdate (float time, bool forceUpdate)
				{
						sequence.nodes = sequence.nodes.OrderBy (x => x.startTime).ToList ();
			
						foreach (var kvp in GetGroupTargets()) {
								SequenceNode mNode = kvp.Value.OrderBy (x => x.startTime).First ();
								mNode.RecordAction ();
						}
			
						foreach (SequenceNode node in sequence.nodes) {
								if (((time - node.startTime) / node.duration) > 0.0f && ((time - node.startTime) / node.duration) < 1.0f || forceUpdate && ((time - node.startTime) / node.duration) > 0.0f) {
										node.StartTween ();
								}
				
								node.UpdateTween (time);
				
								if (((time - node.startTime) / node.duration) < 0.0f || ((time - node.startTime) / node.duration) > 1.0f || forceUpdate) {
										node.CompleteTween ();
								}
						}
			
						foreach (var kvp in GetGroupTargets()) {
								SequenceNode mNode = kvp.Value.OrderBy (x => x.startTime).ToList ().Find (y => ((time - y.startTime) / y.duration) < 0.0f);
								if (mNode != null)
										mNode.UndoAction ();
						}
			
				}

				private void OnPlay (bool isPlaying)
				{
						if (EditorApplication.isPlaying) {
								Debug.Log ("You can't stop preview playing in play mode.");
								timeline.isPlaying = true;
								return;			
						}
						lastPlayState = isPlaying;	
						playStartTime = (float)EditorApplication.timeSinceStartup;
						time = (float)EditorApplication.timeSinceStartup + GetSequenceEnd ();
						timeline.CurrentTime = 0;
						if (isPlaying) {
								stop = false;
								playForward = true;
								StartRecord ();	
						} else {
								StopRecord ();			
						}
				}

				private void OnTimelineClick (float time)
				{
						if (EditorApplication.isPlaying) {
								Debug.Log ("You can't change time in play mode.");
								timeline.isRecording = true;
								return;			
						}

						if (lastPlayState) {
								timeline.isPlaying = false;
								OnPlay (false);
								timeline.CurrentTime = time;
						}

						if (!lastRecordState) {
								StartRecord ();
						}
		

						timeline.isRecording = true;
						if (sequence != null) {
								EditorUpdate (time, true);
						}
				}

				private void OnRecord ()
				{
						if (EditorApplication.isPlaying) {
								Debug.Log ("You can't stop recording in play mode.");
								timeline.isRecording = true;
								return;			
						}
						if (lastRecordState) {
								StopRecord ();		
						} else {
								StartRecord ();			
						}
				}

				private void StartRecord ()
				{
						if (lastRecordState) {
								StopRecord ();			
						}
			
						lastRecordState = true;
						if (sequence != null) {
								records = new List<Record> ();
				
								foreach (var kvp in GetGroupTargets()) {
										Record rec = new Record ();
										rec.target = kvp.Key;
										rec.nodes = kvp.Value;
										rec.instantiatedTarget = (GameObject)Instantiate (kvp.Key);
										rec.instantiatedTarget.name = rec.instantiatedTarget.name.Replace ("(Clone)", "");
										if (Selection.activeGameObject == kvp.Key) {
												Selection.activeGameObject = rec.instantiatedTarget;
										}
										rec.target.SetActive (false);
										rec.target.hideFlags = HideFlags.HideInHierarchy;
										foreach (SequenceNode node in rec.nodes) {
												node.target = rec.instantiatedTarget;
										}
										records.Add (rec);
								}
						}
				}

				private void StopRecord ()
				{
						timeline.CurrentTime = 0;
						if (lastRecordState) {
								lastRecordState = false;	
								if (sequence != null) {
										foreach (Record rec in records) {
												foreach (SequenceNode node in rec.nodes) {
														node.target = rec.target;
												}
												if (Selection.activeGameObject == rec.instantiatedTarget) {
														Selection.activeGameObject = rec.target;
												}
												DestroyImmediate (rec.instantiatedTarget);
					
												rec.target.hideFlags = HideFlags.None;
												rec.target.SetActive (true);
										}	
										records.Clear ();
								}
						}
						Repaint ();
				}

				private void OnGUI ()
				{
						//timeline.DoTimeline (new Rect(0,0,this.position.width,this.position.height));

						Rect rect = this.position;



						rect.x = this.position.width * 0.2f;
						rect.width = this.position.width * 0.8f;//80% for timeArea

						rect.y = 0;

						OnTimelineGUI (rect, __frameRate);

						rect.width = rect.x;//20% for settings
						rect.x = 0;
						rect.y = 0;
			
						DoToolbarGUI (rect);


						//DoNode (__testNode,new Rect(0,0,this.position.width,this.position.height));
				
				}

				private void DoToolbarGUI (Rect rect)
				{
						GUIStyle style = new GUIStyle ("ProgressBarBack");
						style.padding = new RectOffset (0, 0, 0, 0);
						//GUILayout.BeginArea (new Rect (position.x,position.y, timelineOffset, position.height),GUIContent.none,style);
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
			
						if (GUILayout.Button (__isPlaying ? EditorGUIUtility.FindTexture ("d_PlayButton On") : EditorGUIUtility.FindTexture ("d_PlayButton"), EditorStyles.toolbarButton)) {
								__isPlaying = !__isPlaying;
//				if(onPlay != null){
//					onPlay(isPlaying);
//				}
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
//			if (onSettingsGUI != null) {
//				onSettingsGUI(timelineOffset-1.5f);			
//			}
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
						if (GUILayout.Button (sequence != null ? sequence.name : "[None Selected]", EditorStyles.toolbarDropDown, GUILayout.Width (width * 0.5f))) {
								GenericMenu toolsMenu = new GenericMenu ();
				
								List<Sequence> sequences = FindAll<Sequence> ();
								foreach (Sequence mSequence in sequences) {
										toolsMenu.AddItem (new GUIContent (mSequence.name), false, OnGameObjectSelectionChanged, mSequence);
								}
								toolsMenu.AddItem (new GUIContent ("[New Sequence]"), false, CreateNewSequence);
				
								toolsMenu.DropDown (new Rect (3, 37, 0, 0));
								EditorGUIUtility.ExitGUI ();
						}
			
						if (sequence != null) {
								wrap = sequence.wrap;			
						}
						wrap = (Sequence.SequenceWrap)EditorGUILayout.EnumPopup (wrap, EditorStyles.toolbarDropDown, GUILayout.Width (width * 0.5f));
						if (sequence != null) {
								sequence.wrap = wrap;			
						}
						GUILayout.EndHorizontal ();
						settingsScroll = GUILayout.BeginScrollView (settingsScroll);

						// TODO CHANGE THIS TO DRAWING OF ALL NODES not just selected
//			if (selectedNode != null) {
//				DoNodeGUI (selectedNode);
//			}

						GUILayout.EndScrollView ();
						GUILayout.FlexibleSpace ();


//			if (selectedNode != null){
//				if (GUILayout.Button ("Add Tween")) {
//					GenericMenu genericMenu = new GenericMenu ();
//					IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (assembly => assembly.GetTypes ()) .Where (type => type.IsSubclassOf (typeof(TweenAction)));
//					foreach (Type type in types) {
//						if (HasRequiredComponents (selectedNode, type)) {
//							genericMenu.AddItem (new GUIContent (GetCategory (type) + "/" + type.ToString ().Split ('.').Last ()), false, this.AddAction, type);
//						} else {
//							genericMenu.AddDisabledItem (new GUIContent (GetCategory (type) + "/" + type.ToString ().Split ('.').Last ()));
//						}
//					}
//					genericMenu.ShowAsContext ();
//				}
//			}
				}
	
				private void DoListGUI (SequenceNode node)
				{


						//Draw Reordable list (no +/-)

						///////////// GAMEOBJECT - CLIP BINDINGS //////////
			
			
						__gameObjectClipList.DoLayoutList ();
			
			
						//////////////////////////////////////////////


						GUILayout.BeginVertical ("box");
						EditorGUIUtility.labelWidth = 60;

						SerializedObject serializedObject = new SerializedObject (node);
						serializedObject.Update ();
						/*SerializedProperty channelProperty = serializedObject.FindProperty ("channel");
			EditorGUILayout.PropertyField(channelProperty,new GUIContent("Channel","Channel for better organization."));
			channelProperty.intValue=Mathf.Clamp (channelProperty.intValue, 0, int.MaxValue);*/
						SerializedProperty startTimeProperty = serializedObject.FindProperty ("startTime");
						EditorGUILayout.PropertyField (startTimeProperty, new GUIContent ("Delay", "Delay the tween in seconds."));
						startTimeProperty.floatValue = Mathf.Clamp (startTimeProperty.floatValue, 0, Mathf.Infinity);
						SerializedProperty durationProperty = serializedObject.FindProperty ("duration");
						EditorGUILayout.PropertyField (durationProperty, new GUIContent ("Duration", "Duration of the tween in seconds."));
						durationProperty.floatValue = Mathf.Clamp (durationProperty.floatValue, 0, Mathf.Infinity);
						serializedObject.ApplyModifiedProperties ();
						GUILayout.EndVertical ();
						GUI.changed = false;
						int deleteActionIndex = -1;
						if (selectedNode.actions == null) {
								selectedNode.actions = new List<BaseAction> ();		
						}
						for (int i=0; i< selectedNode.actions.Count; i++) {
								BaseAction action = selectedNode.actions [i];
								FieldInfo[] fields = action.GetType ().GetFields (BindingFlags.Instance | BindingFlags.Public);
				
								bool foldout = EditorPrefs.GetBool (action.GetHashCode ().ToString (), true);
								GUIContent title = new GUIContent (action.name.Replace ("/", "."), "");
								GUILayout.BeginVertical ("box");
								GUILayout.BeginHorizontal ();
								if (fields.Length > 0) {
										foldout = EditorGUILayout.Foldout (foldout, title);		
								} 
								GUILayout.FlexibleSpace ();
								if (GUILayout.Button (EditorGUIUtility.FindTexture ("Toolbar Minus"), "Label", GUILayout.Width (20))) {
										deleteActionIndex = i;
								}
								GUILayout.EndHorizontal ();
								EditorPrefs.SetBool (action.GetHashCode ().ToString (), foldout);
				
								if (foldout) {
										if (action is TweenProperty) {
												DrawTweenProperty (action as TweenProperty);
										} else {
												SerializedObject serializedActionObject = new SerializedObject (action);
												serializedActionObject.Update ();
												for (int k=0; k< fields.Length; k++) {
														FieldInfo info = fields [k];
														if (!HasCustomAttribute (info, typeof(HideInInspector))) {
																SerializedProperty property = serializedActionObject.FindProperty (info.Name);
																if (property != null) {
																		EditorGUILayout.PropertyField (property, true);
																}
														}
												}
												serializedActionObject.ApplyModifiedProperties ();
										}
								}
				
								GUILayout.EndVertical ();
						}
			
						if (deleteActionIndex != -1) {
								selectedNode.actions.RemoveAt (deleteActionIndex);		
						}
						if (GUI.changed) {
								EditorUtility.SetDirty (sequence);	
						}
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
						int channel = 0;

						int rows = sequence.nodes.Count + 1;
						int y = 0;
						float lineY = 0f;
						for (; y<rows; y++) {
								//Handles.DrawLine (new Vector3 (0, y, 0), new Vector3 (rect.width, y, 0));	
								lineY = TIME_LABEL_HEIGHT + y * NODE_RECT_HEIGHT;
								Handles.DrawLine (new Vector3 (rect.xMin, lineY, 0), new Vector3 (rect.xMax, lineY, 0));	
								
								
						}

						//make all area from last row to the bottom droppable
						DropAreaGUI (new Rect (0, lineY, rect.width, rect.height - lineY), sequence.nodes.Count);
			
						Handles.color = Color.white;

						//TimeArea
						__timeAreaW.DoTimeArea (rect, 30);

						if (sequence == null) {
								return;		
						}
						if (sequence.nodes == null) {
								sequence.nodes = new List<SequenceNode> ();		
						}


						//Draw cursors
//			foreach (SequenceNode node in sequence.nodes) {
//				EditorGUIUtility.AddCursorRect (new Rect (timeline.SecondsToGUI (node.startTime) - 5, node.channel * 20 , 10, 20), MouseCursor.ResizeHorizontal);			
//				EditorGUIUtility.AddCursorRect (new Rect (timeline.SecondsToGUI (node.startTime+node.duration)-5, node.channel * 20 , 10, 20), MouseCursor.ResizeHorizontal);
//				EditorGUIUtility.AddCursorRect (new Rect(timeline.SecondsToGUI(node.startTime),node.channel*20,timeline.SecondsToGUI(node.duration),20), MouseCursor.Pan);
//			}


						//DRAW NODES RECT
						foreach (SequenceNode node in sequence.nodes) {
								if (!node.eventNode) {

										DoNode (node, rect);
//					Rect boxRect=new Rect(timeline.SecondsToGUI(node.startTime),node.channel*20,timeline.SecondsToGUI(node.duration),20);
//					GUI.Box (boxRect,"","TL LogicBar 0");
//
//					GUIStyle style = new GUIStyle("Label");
//					style.fontSize= (selectedNode==node?12:style.fontSize);
//					style.fontStyle= (selectedNode==node?FontStyle.Bold:FontStyle.Normal);
//					Color color=style.normal.textColor;
//					color.a=(selectedNode==node?1.0f:0.7f);
//					style.normal.textColor=color;
//					Vector3 size=style.CalcSize(new GUIContent(node.target.name));
//					Rect rect1=new Rect(boxRect.x+boxRect.width*0.5f-size.x*0.5f,boxRect.y+boxRect.height*0.5f-size.y*0.5f,size.x,size.y);
//					GUI.Label(rect1,node.target.name,style);
								}
						}

						HandleEvents (rect);
				}


				/// <summary>
				/// Handles the events, resize, pan.
				/// </summary>
				/// <param name="rect">Rect.</param>
				private void HandleEvents (Rect rect)
				{
						Event ev = Event.current;
						Rect clickRect;

						float startTime;
						switch (ev.rawType) {
						case EventType.MouseDown:
								foreach (SequenceNode node in sequence.nodes) {

										clickRect = new Rect (__timeAreaW.TimeToPixel (node.startTime, rect) - 5, TIME_LABEL_HEIGHT + node.channel * NODE_RECT_HEIGHT, 10, NODE_RECT_HEIGHT);

										//check start of the node rect width=5
										if (clickRect.Contains (Event.current.mousePosition)) {
												//if (new Rect (timeline.SecondsToGUI (node.startTime) - 5, node.channel * 20 , 10, 20).Contains (Event.current.mousePosition)) {
												selectedNode = node;
												resizeNodeStart = true;
												ev.Use ();
										}

										clickRect.x = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - 5;

										//check the end of node rect width=5
										//if (new Rect (timeline.SecondsToGUI (node.startTime+node.duration)-5, node.channel * 20 , 10, 20).Contains (Event.current.mousePosition)) {
										if (clickRect.Contains (Event.current.mousePosition)) {
												selectedNode = node;
												resizeNodeEnd = true;
												ev.Use ();
										}


										clickRect.x = __timeAreaW.TimeToPixel (node.startTime, rect) + 5;
										clickRect.width = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - clickRect.x - 5;

										if (clickRect.Contains (Event.current.mousePosition)) {
												//if (new Rect(timeline.SecondsToGUI(node.startTime),node.channel*20,timeline.SecondsToGUI(node.duration),20).Contains (Event.current.mousePosition)) {
												if (ev.button == 0) {
														//timeClickOffset = node.startTime - timeline.GUIToSeconds (Event.current.mousePosition.x);
														timeClickOffset = node.startTime - __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
														dragNode = true;
														selectedNode = node;
												} 
												if (ev.button == 1) {
														GenericMenu genericMenu = new GenericMenu ();
														genericMenu.AddItem (new GUIContent ("Remove"), false, this.RemoveNode, node);
														genericMenu.ShowAsContext ();
												}
												ev.Use ();
										}
								}
								break;

						case EventType.ContextClick:

								clickRect = new Rect (0, 0, 0, NODE_RECT_HEIGHT);

								foreach (SequenceNode node in sequence.nodes) {

										clickRect.x = __timeAreaW.TimeToPixel (node.startTime, rect);
										clickRect.y = TIME_LABEL_HEIGHT + node.channel * NODE_RECT_HEIGHT;
										clickRect.width = __timeAreaW.TimeToPixel (node.startTime + node.duration, rect) - clickRect.x;
					
										if (clickRect.Contains (Event.current.mousePosition)) {
												GenericMenu genericMenu = new GenericMenu ();
												genericMenu.AddItem (new GUIContent ("Remove"), false, this.RemoveNode, node);
												genericMenu.ShowAsContext ();
										}
								}
								break;
						case EventType.MouseDrag:
								if (resizeNodeStart) {
										//selectedNode.startTime = timeline.GUIToSeconds (Event.current.mousePosition.x);
										float prevStartTime = selectedNode.startTime;
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect);
										

										if (startTime >= 0) {
												selectedNode.startTime = startTime;
												selectedNode.duration += prevStartTime - selectedNode.startTime;
												
												ev.Use ();
										}
										//selectedNode.duration += __timeAreaW.PixelToTime (Event.current.mousePosition.x+ev.delta.x, rect)-selectedNode.startTime;
										//selectedNode.duration -= timeline.GUIToSeconds (ev.delta.x);

										
								}

								if (resizeNodeEnd) {
										//selectedNode.duration = (timeline.GUIToSeconds (Event.current.mousePosition.x) - selectedNode.startTime);
										selectedNode.duration = (__timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) - selectedNode.startTime);
										ev.Use ();
								}

								if (dragNode && !resizeNodeStart && !resizeNodeEnd) {
										//selectedNode.startTime = timeline.GUIToSeconds (Event.current.mousePosition.x) + timeClickOffset;
										startTime = __timeAreaW.PixelToTime (Event.current.mousePosition.x, rect) + timeClickOffset;
										if (startTime >= 0)
												selectedNode.startTime = startTime;
										;

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

//				private void AddAction (object data)
//				{
//			
//						if (lastRecordState) {
//								StopRecord ();
//						}
//						Type type = (Type)data;
//			
//						if (type.IsSubclassOf (typeof(SequenceEvent))) {
//								SequenceNode node = CreateSequenceNode (selectedNode.target, 0, true);
//								sequence.nodes.Add (node);
//								selectedNode = node;
//						}
//						BaseAction action = ScriptableObject.CreateInstance (type) as BaseAction;
//						action.name = type.ToString ().Split ('.').Last ();
//						if (selectedNode.actions == null) {
//								selectedNode.actions = new List<BaseAction> ();			
//						}
//						selectedNode.actions.Add (action);
//			
//						EditorUtility.SetDirty (sequence);
//			
//						StartRecord ();
//				}

				private Dictionary<GameObject,List<SequenceNode>> GetGroupTargets ()
				{
						Dictionary<GameObject,List<SequenceNode>> targets = new Dictionary<GameObject, List<SequenceNode>> ();
						foreach (SequenceNode node in sequence.nodes) {
								if (!targets.ContainsKey (node.target)) {
										targets.Add (node.target, new List<SequenceNode> (){node});
								} else {
										targets [node.target].Add (node);
								}
						}
						return targets;
				}

				private void RemoveNode (object data)
				{
						SequenceNode node = (SequenceNode)data;
						sequence.nodes.Remove (node);
				}

				private void OnGameObjectSelectionChanged (object data)
				{
						Sequence mSequence = data as Sequence;
						Selection.activeGameObject = mSequence.gameObject;
						sequenceGameObject = mSequence.gameObject;
				}


				/// <summary>
				/// Drops the area GUI.
				/// </summary>
				/// <param name="area">Area.</param>
				/// <param name="channel">Channel.</param>
				public void DropAreaGUI (Rect area, int channel)
				{
			
						Event evt = Event.current;
			
						switch (evt.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
								if (!area.Contains (evt.mousePosition))
										return;
								DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
								if (evt.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag ();
										if (EditorApplication.isPlaying) {
												Debug.Log ("Can't add tween object in play mode. Stop the play mode and readd it.");
										} else {
												foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences) {

														Debug.Log ("Dropped object of type:" + dragged_object);

														//TODO handle sound, video, animation clip

														GameObject go = dragged_object as GameObject;
														if (lastRecordState) {
																foreach (Record rec in records) {
																		if (go == rec.instantiatedTarget) {
																				go = rec.target;
																		}
									
																}
														}
														SequenceNode node = CreateSequenceNode (Event.current.mousePosition, go, channel, false);
														if (sequence == null) {
																CreateNewSequence ();
														}
														sequence.nodes.Add (node);
														selectedNode = node;
												}
												EditorUtility.SetDirty (sequence);
										}
								}
								break;
						}
			
				}
				
				private SequenceNode CreateSequenceNode (Vector2 pos, GameObject target, int channel, bool eventNode)
				{
						SequenceNode node = ScriptableObject.CreateInstance<SequenceNode> ();
						node.target = target;
						node.channel = channel;
						node.duration *= timeline.TimeFactor;
						node.eventNode = eventNode;
						if (!eventNode) {
								//node.startTime = timeline.GUIToSeconds (Event.current.mousePosition.x) - node.duration * 0.5f;
								node.startTime = __timeAreaW.PixelToTime (pos.x, __timeAreaW.rect) - node.duration * 0.5f;
						} else {
								//node.startTime=timeline.GUIToSeconds(timePosition);
								//Debug.Log(node.startTime);
						}
						return node;
				}


//		AudioUtil
//
//		[WrapperlessIcall]
//		[MethodImpl (4096)]
//		public static extern void PlayClip (AudioClip clip, [DefaultValue ("0")] int startSample, [DefaultValue ("false")] bool loop);

//		public MovieTexture movTexture;
//		void Start() {
//			GetComponent<Renderer>().material.mainTexture = movTexture;
//			movTexture.Play();
//		}


				////   CREATE NEW SEQUENCE GAME OBJECT WITH SEQUENCE BEHAVIOUR ////
				/// <summary>
				/// Creates the new sequence.
				/// </summary>
				private void CreateNewSequence ()
				{
						List<Sequence> sequences = FindAll<Sequence> ();
						int count = 0;

						while (sequences.Find(x=>x.name == "Sequence "+count.ToString())!=null) {
								count++;
						}

						sequenceGameObject = new GameObject ("Sequence " + count.ToString ());
						sequence = sequenceGameObject.AddComponent<Sequence> ();

						//add Animator component
						sequenceGameObject.AddComponent<Animator> ();


						if (sequence.nodes == null) {
								sequence.nodes = new List<SequenceNode> ();			
						}
						Selection.activeGameObject = sequenceGameObject;
				}
		
				private void OnSelectionChange ()
				{
						if (Selection.activeGameObject != null && Selection.activeGameObject != sequenceGameObject) {
								Sequence mSequence = Selection.activeGameObject.GetComponent<Sequence> ();
								if (mSequence != null) {
										sequenceGameObject = mSequence.gameObject;
								}
						}
				}

				private void DrawTweenProperty (TweenProperty tween)
				{
						Component[] components = selectedNode.target.GetComponents (typeof(Component));
						components = components.ToList ().FindAll (x => HasSupportedFields (x.GetType ()) == true).ToArray ();
						List<string> componentTypes = components.Select (x => x.GetType ().ToString ().Split ('.').Last ()).ToList ();
						EditorGUIUtility.labelWidth = 70;
			
						tween.componentTypeString = StringPopup ("Component", tween.componentTypeString, componentTypes.ToArray ());
						Type selectedType = TweenProperty.GetType (tween.componentTypeString);
						if (selectedType == null) {
								selectedType = TweenProperty.GetType ("UnityEngine." + tween.componentTypeString);			
						}
			
						if (selectedType != null) {
								FieldInfo[] fields = selectedType
					.GetFields (BindingFlags.Public | BindingFlags.Instance)
						.Where (x => x.FieldType == typeof(float) || x.FieldType == typeof(Color) || x.FieldType == typeof(Vector4) || x.FieldType == typeof(Vector3) || x.FieldType == typeof(Vector2) || x.FieldType == typeof(Quaternion))
						.ToArray ();
								PropertyInfo[] properties = selectedType
					.GetProperties (BindingFlags.Public | BindingFlags.Instance)
						.Where (x => x.CanWrite && (x.PropertyType == typeof(float) || x.PropertyType == typeof(Color) || x.PropertyType == typeof(Vector4) || x.PropertyType == typeof(Vector3) || x.PropertyType == typeof(Vector2) || x.PropertyType == typeof(Quaternion)))
						.ToArray ();	
								if (fields.Length > 0 || properties.Length > 0) {
										List<string> names = fields.Select (x => x.Name).ToList ();
										names.AddRange (properties.Select (x => x.Name).ToList ());
					
										tween.propertyName = StringPopup ("Property", tween.propertyName, names.ToArray ());
										FieldInfo field = fields.ToList ().Find (x => x.Name == tween.propertyName);
										Type type = null;
										if (field != null) {
												type = fields.ToList ().Find (x => x.Name == tween.propertyName).FieldType;
										} else {
												type = properties.ToList ().Find (x => x.Name == tween.propertyName).PropertyType;
										}
										if (type != null) {
												SerializedObject serializedObject = new SerializedObject (tween);
												serializedObject.Update ();
												if (type == typeof(float)) {
														EditorGUILayout.PropertyField (serializedObject.FindProperty ("fromFloat"), new GUIContent ("From"));
														EditorGUILayout.PropertyField (serializedObject.FindProperty ("toFloat"), new GUIContent ("To"));
												} else if (type == typeof(UnityEngine.Color)) {
														EditorGUILayout.PropertyField (serializedObject.FindProperty ("fromColor"), new GUIContent ("From"));
														EditorGUILayout.PropertyField (serializedObject.FindProperty ("toColor"), new GUIContent ("To"));
												} else if (type == typeof(UnityEngine.Vector3) || type == typeof(Quaternion)) {
														tween.fromVector3 = EditorGUILayout.Vector3Field ("From", tween.fromVector3);
														tween.toVector3 = EditorGUILayout.Vector3Field ("To", tween.toVector3);
														/*Crash when using PropertyField
							EditorGUILayout.PropertyField(prop,new GUIContent("From"),true);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("toVector3"),new GUIContent("To"));*/
												} else if (type == typeof(UnityEngine.Vector2)) {
														tween.fromVector2 = EditorGUILayout.Vector3Field ("From", tween.fromVector2);
														tween.toVector2 = EditorGUILayout.Vector3Field ("To", tween.toVector2);
														/*Crash when using PropertyField
							EditorGUILayout.PropertyField(serializedObject.FindProperty("fromVector2"),new GUIContent("From"),true);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("toVector2"),new GUIContent("To"),true);*/
												} else if (type == typeof(UnityEngine.Vector4)) {
														tween.fromVector4 = EditorGUILayout.Vector4Field ("From", tween.fromVector4);
														tween.toVector4 = EditorGUILayout.Vector4Field ("To", tween.toVector4);
														/*Crash when using PropertyField
							EditorGUILayout.PropertyField(serializedObject.FindProperty("fromVector2"),new GUIContent("From"),true);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("toVector2"),new GUIContent("To"),true);*/
												}
												EditorGUILayout.PropertyField (serializedObject.FindProperty ("easeType"), new GUIContent ("EaseType"));
												serializedObject.ApplyModifiedProperties ();
										}
								}
						}
				}

				public static List<T> FindAll<T> () where T : Component
				{
						T[] comps = Resources.FindObjectsOfTypeAll (typeof(T)) as T[];
						List<T> list = new List<T> ();
						foreach (T comp in comps) {
								if (comp.gameObject.hideFlags == 0) {
										string path = AssetDatabase.GetAssetPath (comp.gameObject);
										if (string.IsNullOrEmpty (path))
												list.Add (comp);
								}
						}
						return list;
				}

				private bool HasCustomAttribute (FieldInfo info, Type type)
				{
						object[] attributes = info.GetCustomAttributes (true);
						foreach (object attribute in attributes) {
								if (attribute.GetType () == type) {
										return true;
								}
						}
						return false;	
				}

				private bool HasRequiredComponents (SequenceNode node, Type actionType)
				{
						object[] attributes = actionType.GetCustomAttributes (true);
						bool result = false;
						foreach (object attribute in attributes) {
								if (attribute is RequireComponent) {
										RequireComponent attr = attribute as RequireComponent;
										if (attr.m_Type0 != null) {
												result = node.target.GetComponent (attr.m_Type0) != null;
												if (!result) {
														return false;
												}
										}
								}
						}
						return true;	
				}
		
				private bool HasSupportedFields (Type type)
				{
						FieldInfo[] fields = type
				.GetFields (BindingFlags.Public | BindingFlags.Instance)
					.Where (x => x.FieldType == typeof(float) || x.FieldType == typeof(Color) || x.FieldType == typeof(Vector4) || x.FieldType == typeof(Vector3) || x.FieldType == typeof(Vector2))
					.ToArray ();
						PropertyInfo[] properties = type
				.GetProperties (BindingFlags.Public | BindingFlags.Instance)
					.Where (x => x.PropertyType == typeof(float) || x.PropertyType == typeof(Color) || x.PropertyType == typeof(Vector4) || x.PropertyType == typeof(Vector3) || x.PropertyType == typeof(Vector2))
					.ToArray ();	
						if (fields.Length > 0 || properties.Length > 0) {
								return true;
						}
						return false;
				}

				private string GetCategory (Type type)
				{
						object[] attributes = type.GetCustomAttributes (true);
						foreach (object attribute in attributes) {
								if (attribute is CategoryAttribute) {
										return (attribute as CategoryAttribute).category;
								}
						}
						return string.Empty;
				}

				private string StringPopup (string label, string value, string[] list, params GUILayoutOption[] options)
				{
						int index = 0;
						if (list != null && list.Length > 0) {
								for (int cnt=0; cnt<list.Length; cnt++) {
										if (value == list [cnt]) {
												index = cnt;
										}
								}
								index = string.IsNullOrEmpty (label) ? EditorGUILayout.Popup (index, list, options) : EditorGUILayout.Popup (label, index, list, options);
								return list [index];
						}
						return string.Empty;
				}

				[System.Serializable]
				public class Record
				{
						public GameObject target;
						public GameObject instantiatedTarget;
						public List<SequenceNode> nodes;
				}
		}
}