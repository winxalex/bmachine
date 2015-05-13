//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//
//     MecanimNodeEditor(Alex Winx)
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BehaviourMachine;
using BehaviourMachineEditor;
using UnityEditor;

using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.csharp.extensions;
using ws.winx.unity;
using ws.winx.editor.extensions;
using Motion = UnityEngine.Motion;


using StateMachine = UnityEditor.Animations.AnimatorStateMachine;
using ws.winx.bmachine;
using UnityEditor.Animations;

namespace ws.winx.editor.bmachine.extensions
{
	
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(MecanimNode), true)]
		[CanEditMultipleObjects()]
		public class MecanimNodeCustomEditor : NodeEditor
		{

				
				
				GUIContent propertyPopupLabel = new GUIContent (String.Empty);
				string[] curvePropertyDisplayOptions;
				MethodInfo GetFloatVar_MethodInfo;
				int _curveIndexSelected;
				Color _colorSelected;
				bool _curvesEditorShow;
				CurveEditorW curveEditor;
				MecanimNode mecanimNode;
				ws.winx.unity.AnimatorState selectedAnimaStateInfo;
				float[] eventTimeValues;
				float[] eventTimeValuesPrev;
				bool eventTimeLineInitalized;
				Rect eventTimeLineValuePopUpRect;
				bool[] eventTimeValuesSelected;
				EventComparer eventTimeComparer = new EventComparer ();
				string[] eventDisplayNames;
				float timeNormalizedStartPrev = -1f;
				float timeNormalizedEndPrev = 1.1f;
				GUIStyle playButtonStyle;
				Vector2 playButtonSize;
				AvatarPreviewW avatarPreview;
				Vector2 curvePropertiesScroller;
				UnityVariable _variableSelected;
				float timeNormalized = 0f;
				SerializedNodeProperty curvesSerialized;
				SerializedNodeProperty curvesColorsSerialized;
				SerializedNodeProperty variablesBindedToCurvesSerialized;
				SerializedNodeProperty animatorStateSerialized;
				SerializedNodeProperty animatorStateRuntimeControlEnabledSerialized;
				SerializedNodeProperty animatorStateRunTimeControlSerialized;
				SerializedNodeProperty motionOverrideSerialized;
				AnimationCurve[] curves;
				Color[] curveColors;
				UnityVariable[] variablesBindedToCurves;
				bool _changesPreserve;
				bool hasRigidbody;			
				
		        

				//
				// Nested Types
				//
				public class EventComparer : IComparer
				{
						int IComparer.Compare (object objX, object objY)
						{
								SendEventNormalized animationEvent = (SendEventNormalized)objX;
								SendEventNormalized animationEvent2 = (SendEventNormalized)objY;
								//float time = (float)animationEvent.timeNormalized.Value;
								//float time2 = (float)animationEvent2.timeNormalized.Value;

								float time = (float)animationEvent.timeNormalized.serializedProperty.floatValue;
								float time2 = (float)animationEvent2.timeNormalized.serializedProperty.floatValue;
								if (time != time2) {
										return (int)Mathf.Sign (time - time2);
								}
								int hashCode = animationEvent.GetHashCode ();
								int hashCode2 = animationEvent2.GetHashCode ();
								return hashCode - hashCode2;
						}
				}



			

				public new void DrawDefaultInspector ()
				{



						NodePropertyIterator iterator = this.serializedNode.GetIterator ();
						

						
						int indentLevel = EditorGUI.indentLevel;
						while (iterator.Next (iterator.current == null || (iterator.current.propertyType != NodePropertyType.Variable && !iterator.current.hideInInspector))) {
								SerializedNodeProperty current = iterator.current;
							
							
								if (!current.hideInInspector) {
								
										if (current.path == "motionOverride" && mecanimNode != null && mecanimNode.animatorStateSelected != null && 
												mecanimNode.animatorStateSelected.motion != null &&
												mecanimNode.animatorStateSelected.motion is BlendTree)
												continue;
										
								
										if (current.path == "blendX" && 
												mecanimNode != null && mecanimNode.animatorStateSelected != null && 
												(mecanimNode.animatorStateSelected.motion == null ||
												!(mecanimNode.animatorStateSelected.motion is BlendTree && !String.IsNullOrEmpty (((BlendTree)mecanimNode.animatorStateSelected.motion).blendParameter))
					 ))
											

												continue;
										if (current.path == "blendY" 
												&& mecanimNode != null && mecanimNode.animatorStateSelected != null && 
												(mecanimNode.animatorStateSelected.motion == null ||
												!(mecanimNode.animatorStateSelected.motion is BlendTree && !String.IsNullOrEmpty (((BlendTree)mecanimNode.animatorStateSelected.motion).blendParameterY))
					 ))
												continue;

										EditorGUI.indentLevel = indentLevel + iterator.depth;


										GUILayoutHelper.DrawNodeProperty (new GUIContent (current.label, current.tooltip), current, mecanimNode, null, true);
										
										
								}
						}

						EditorGUI.indentLevel = indentLevel;


						this.serializedNode.ApplyModifiedProperties ();

				}






			


				///  TIMELINE EVENTHANDLERS
		 
		 
				/// <summary>
				/// Ons the mecanim event edit.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventEdit (TimeLineArgs<float> args)
				{

						SendEventNormalized child = mecanimNode.children [args.selectedIndex] as SendEventNormalized;
					
		
						//child.timeNormalized.Value = args.selectedValue;
						child.timeNormalized.serializedProperty.floatValue = args.selectedValue;
						//child.timeNormalized.ApplyModifiedProperties ();
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

				}




			



				/// <summary>
				/// On the mecanim event close.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventClose (TimeLineArgs<float> args)
				{
						//SendEventNormalizedEditor.Hide ();
				}


				/// <summary>
				/// Ons the mecanim event add.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventAdd (TimeLineArgs<float> args)
				{
				

						//create and add node to internal bhv tree
						SendEventNormalized child = mecanimNode.tree.AddNode (typeof(SendEventNormalized)) as SendEventNormalized;
				
						//add node to its parent list
						mecanimNode.Insert (args.selectedIndex, child);
						//child.timeNormalized.Value = args.selectedValue;
						child.timeNormalized.serializedProperty.floatValue = args.selectedValue;

						mecanimNode.tree.SaveNodes ();

						eventTimeValues = (float[])args.values;

						//recreate (not to optimal but doesn't have
						eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

					

						
						
						//show popup
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

						

						Undo.RecordObject (target.self, "Add Node");

						EditorApplication.SaveScene ();

						
				}


				/// <summary>
				/// Ons the mecanim event drag end.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventDragEnd (TimeLineArgs<float> args)
				{
						int[] indexArray = new int[mecanimNode.children.Length];
						for (int l = 0; l < indexArray.Length; l++) {
								indexArray [l] = l;
						}
			
						Array.Sort (mecanimNode.children, indexArray, eventTimeComparer);
			
						bool[] cloneOfSelected = (bool[])eventTimeValuesSelected.Clone ();
						int inx = -1;
						SendEventNormalized ev;
						for (int m = 0; m < indexArray.Length; m++) {
				
								inx = indexArray [m];
								ev = ((SendEventNormalized)mecanimNode.children [m]);	
								this.eventTimeValuesSelected [m] = cloneOfSelected [inx];
								//this.eventTimeValues [m] = (float)ev.timeNormalized.Value; 
								this.eventTimeValues [m] = (float)ev.timeNormalized.serializedProperty.floatValue; 
								this.eventDisplayNames [m] = ev.name;
				
						}
			
			
			
			
			
						mecanimNode.tree.HierarchyChanged ();
						StateUtility.SetDirty (mecanimNode.tree);
				}

				/// <summary>
				/// Ons the mecanim event delete.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventDelete (TimeLineArgs<float> args)
				{
						float[] timeValues = (float[])args.values;
						int timeValuesNumber = timeValues.Length;



						SendEventNormalized child;
						for (int i=0; i<mecanimNode.children.Length;) {
								child = mecanimNode.children [i] as SendEventNormalized;

								if (i < timeValuesNumber) {

										//child.timeNormalized.Value = timeValues [i];
										child.timeNormalized.serializedProperty.floatValue = timeValues [i];
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
						eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

	
						

						StateUtility.SetDirty (mecanimNode.tree);

						Undo.RecordObject (target.self, "Delete Node");
						//SendEventNormalizedEditor.Hide ();


				}


				/// <summary>
				/// Ons the curve select.
				/// </summary>
				/// <param name="index">Index.</param>
				void onCurveSelect (int index)
				{
						Debug.Log ("Curve " + index + " selected");

						_curveIndexSelected = index;

							

				}

	
			
          

				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						int i = 0;


						//////  RESTORE SAVED  //////
						//restore and delete clipboard
						

						mecanimNode = target as MecanimNode;

						if (mecanimNode != null) {

								
						
								Motion motion = null;



								//////////////////////////////////////////////////////////////
								/// 						PRESEVE RESTORE					//
								if (EditorApplication.isPlaying || EditorApplication.isPaused) {
					
					
										if (GUILayout.Button ("Preserve")) {
						
						
												serializedNode.ApplyModifiedProperties ();
						
												mecanimNode.blendX.OnBeforeSerialize ();
												mecanimNode.blendY.OnBeforeSerialize ();
												mecanimNode.motionOverride.OnBeforeSerialize ();

												if (variablesBindedToCurves != null) {
														UnityVariable[] varArray = variablesBindedToCurves;

														int varNumber = varArray.Length;
														for (int varCurrent=0; varCurrent<varNumber; varCurrent++) {
																varArray [varCurrent].OnBeforeSerialize ();
								
														}
												}
						
						
												EditorUtilityEx.Clipboard.preserve (mecanimNode.instanceID, mecanimNode, mecanimNode.GetType ().GetFields ());
						
										}
								} else {
										if (EditorUtilityEx.Clipboard.HasBeenPreseved (mecanimNode.instanceID) && GUILayout.Button ("Apply Playmode Changes")) {
												EditorUtilityEx.Clipboard.restore (mecanimNode.instanceID, mecanimNode);
												curvesSerialized = null;
												animatorStateSerialized = null;
												NodePropertyIterator iterator = serializedNode.GetIterator ();
						
						
						
												while (iterator.Next (true)) {
														if (iterator.current.value is UnityVariable) {
																//Debug.Log("OnBeforeDeserialize:"+((UnityVariable)iterator.current.value).Value);
																((UnityVariable)iterator.current.value).OnAfterDeserialize ();
								
																//Debug.Log("OnAfterDeserialize:"+((UnityVariable)iterator.current.value).Value);
														} else if (iterator.current.value is UnityVariable[]) {
								
																UnityVariable[] varArray = (UnityVariable[])iterator.current.value;
																int varNumber = varArray.Length;
																for (int varCurrent=0; varCurrent<varNumber; varCurrent++) {
																		varArray [varCurrent].OnAfterDeserialize ();
									
																}
								
								
														}
							
							
														iterator.current.ValueChanged ();
							
														this.serializedNode.Update ();
							
														iterator.current.ApplyModifiedValue ();
							
							
							
							
												}
						
						
						
						
						
						
						
										}
					
					
					
								}
								//////////////////////

							
								if (Event.current.type == EventType.Layout) {
										this.serializedNode.Update ();

								}


								
								_curvesEditorShow = EditorGUILayout.Foldout (_curvesEditorShow, "Curves");
								
								//int indentLevel = 0;

								Rect curveEditorRect = new Rect (0, 0, 0, 0);



					
			
				
								if (_curvesEditorShow) {
										

										

										/////// INIT SERIALIZED NODE PROPERTIES - CURVES, COLORS, VARIABLES //////
										if (curvesSerialized == null) {
												NodePropertyIterator iterator = this.serializedNode.GetIterator ();
												if (iterator.Find ("curves"))
														curvesSerialized = iterator.current;
												else
														Debug.LogError ("MecananimNode should have public field 'curves'");
										
												if (iterator.Find ("curvesColors"))
														curvesColorsSerialized = iterator.current;
												else
														Debug.LogError ("MecananimNode should have public field 'curvesColors'");
										
												if (iterator.Find ("variablesBindedToCurves")) 
														variablesBindedToCurvesSerialized = iterator.current;
												else
														Debug.LogError ("MecananimNode should have public field 'variablesBindedToCurves'");



												curves = (AnimationCurve[])curvesSerialized.value;
												curveColors = (Color[])curvesColorsSerialized.value;
												variablesBindedToCurves = (UnityVariable[])variablesBindedToCurvesSerialized.value;
										}

										
										

										//This makes layout to work (Reserving space)
										curveEditorRect = GUILayoutUtility.GetRect (Screen.width - 16f, 200);

										/////// CURVE EDITOR ////////
										curveEditorRect.width = curveEditorRect.width - 16f;

										
										if (curveEditor == null) {

												CurveWrapperW[] curveWrappers;
												
												int numCurves = curves.Length;

												curveWrappers = new CurveWrapperW[numCurves];

												CurveWrapperW curveWrapperNew;

												for (i=0; i<numCurves; i++) {
														curveWrapperNew = new CurveWrapperW ();
														curveWrapperNew.curve = curves [i];
														curveWrapperNew.color = curveColors [i];
														curveWrappers [i] = curveWrapperNew;
												}



												curveEditor = new CurveEditorW (curveEditorRect, curveWrappers, false);
					
												curveEditor.FrameSelected (true, true);
												curveEditor.scaleWithWindow = true;
												curveEditor.hSlider = false;
												curveEditor.hRangeMin = 0f;
												curveEditor.hRangeMax = 1f;
												curveEditor.hRangeLocked = true;

												curveEditor.onSelect += onCurveSelect;


					

										} else {
												
												curveEditor.rect = curveEditorRect;
												curveEditor.FrameSelected (false, false);
												
										}


										
										curveEditor.DoEditor ();

										
				
										///////////////////////////////////////////////////////////////////////////////
					 
				
										


										/////////////   ADD/REMOVE CURVE BINDED TO OBJECT PROP OR GLOBAL VARIABLE /////////////
										EditorGUILayout.BeginHorizontal ();

										


										//if curve is selected display curve properties
										if (_curveIndexSelected > -1 && _curveIndexSelected < variablesBindedToCurves.Length) {
												
												UnityVariable variableSelected = variablesBindedToCurves [_curveIndexSelected];
											

												EditorGUILayout.LabelField (variableSelected.name, new GUILayoutOption[]{});

												EditorGUI.BeginChangeCheck ();
												Color colorNew = EditorGUILayout.ColorField (curveColors [_curveIndexSelected]);
										
					
												if (EditorGUI.EndChangeCheck ()) {
														curveEditor.animationCurves [_curveIndexSelected].color = colorNew;
														curveColors [_curveIndexSelected] = colorNew;
														curvesColorsSerialized.ValueChanged ();
														curvesColorsSerialized.ApplyModifiedValue ();
														
														
												}
					
										} else {
											


												propertyPopupLabel.text = "Select blackboard var";


												List<UnityVariable> blackboardLocalList = mecanimNode.blackboard.GetVariableBy (typeof(float));

												List<GUIContent> displayOptionsList = blackboardLocalList.Select ((item) => new GUIContent ("Local/" + item.name)).ToList ();
														

												

												_variableSelected = EditorGUILayoutEx.UnityVariablePopup (new GUIContent ("Var:"), _variableSelected, typeof(float), displayOptionsList, blackboardLocalList);



					
										
				
												_colorSelected = EditorGUILayout.ColorField (_colorSelected);

										}
									

									

						
										/////////////// ADD CURVE(+) /////////
										if (GUILayout.Button ("Add") && _variableSelected != null) {


												
												
												List<UnityVariable> vList = variablesBindedToCurves.ToList ();
												vList.Add (_variableSelected);
												variablesBindedToCurvesSerialized.value = variablesBindedToCurves = vList.ToArray ();
												variablesBindedToCurvesSerialized.ValueChanged ();
												//variablesBindedToCurvesSerialized.ApplyModifiedValue ();

												

												

														
												List<Color> cList = curveColors.ToList ();
												_colorSelected.a = 1;
												cList.Add (_colorSelected);
												curvesColorsSerialized.value = curveColors = cList.ToArray ();
												curvesColorsSerialized.ValueChanged ();	
												//curvesColorsSerialized.ApplyModifiedValue ();		
												

												

											
			
												AnimationCurve curveAnimationNew;
						
						
												
												List<AnimationCurve> crList = curves.ToList ();

												curveAnimationNew = new AnimationCurve (new Keyframe[] {
							new Keyframe (0f, (float)_variableSelected.Value),
							new Keyframe (1f, 1f)
						});

												//TODO add from preset
												crList.Add (curveAnimationNew);
							
												curvesSerialized.value = curves = crList.ToArray ();
												curvesSerialized.ValueChanged ();
												//curvesColorsSerialized.ApplyModifiedValue ();



												///add curve wrapped to CurveEditor
												CurveWrapperW curveWrapperW = new CurveWrapperW ();
															
												curveWrapperW.color = _colorSelected;
															
												curveWrapperW.curve = curveAnimationNew;
															
												curveEditor.AddCurve (curveWrapperW);
															
												curveEditor.FrameSelected (true, true);
							
												
						
												this.serializedNode.Update ();

												this.serializedNode.ApplyModifiedProperties ();


												_variableSelected = null;

										}

										if (GUILayout.Button ("Del") || Event.current.keyCode == KeyCode.Delete) {

												
												curveEditor.RemoveCurveAt (_curveIndexSelected);

												
												
												List<UnityVariable> vList = variablesBindedToCurves.ToList ();
												vList.RemoveAt (_curveIndexSelected);
												variablesBindedToCurvesSerialized.value = variablesBindedToCurves = vList.ToArray ();
												variablesBindedToCurvesSerialized.ValueChanged ();

												


												
												List<Color> cList = curveColors.ToList ();

												cList.RemoveAt (_curveIndexSelected);
												curvesColorsSerialized.value = curveColors = cList.ToArray ();
												curvesColorsSerialized.ValueChanged ();
												


												
														
												List<AnimationCurve> crList = curves.ToList ();
				
												crList.RemoveAt (_curveIndexSelected);
						
												curvesSerialized.value = curves = crList.ToArray ();
												curvesSerialized.ValueChanged ();
												



												_curveIndexSelected = -1;
												_variableSelected = null;

												this.serializedNode.ApplyModifiedProperties ();
				

										}



										


				


										

										

										EditorGUILayout.EndHorizontal ();


										

									
								} else {//not _curvesEditorShow => Default draw
										
										DrawDefaultInspector ();

										
								}


								

								


								/////////////////////////////// ANIMATOR STATE /////////////////////////////////

								if (animatorStateSerialized == null) {
										NodePropertyIterator iterator = this.serializedNode.GetIterator ();
										if (iterator.Find ("animatorStateSelected"))
												animatorStateSerialized = iterator.current;

										
										if (iterator.Find ("motionOverride"))
												motionOverrideSerialized = iterator.current;
								}



								//////////  MOTION OVERRIDE HANDLING  //////////
								if (animatorStateSerialized.value != null) {
									
										UnityVariable motionOverridVariable = (UnityVariable)motionOverrideSerialized.value;
									
										//if there are no override use motion of selected AnimationState
										//Debug.Log(((UnityEngine.Object)mecanimNode.motionOverride.Value).);
										if (motionOverridVariable == null || motionOverridVariable.Value == null || motionOverridVariable.ValueType != typeof(AnimationClip))
												motion = ((ws.winx.unity.AnimatorState)animatorStateSerialized.value).motion;
										else //
												motion = (Motion)motionOverridVariable.Value;
									
									
									
										if (motionOverridVariable != null && motionOverridVariable.Value != null && ((ws.winx.unity.AnimatorState)animatorStateSerialized.value).motion == null) {
												Debug.LogError ("Can't override state that doesn't contain motion");
										}
									
									
								}
								///////////////////////////////////////////////////

								/////////////   TIME CONTROL OF ANIMATION (SLIDER) /////////
								if (Application.isPlaying) {

										if (animatorStateRuntimeControlEnabledSerialized == null) {
												NodePropertyIterator iterator = this.serializedNode.GetIterator ();

												if (iterator.Find ("animationRunTimeControlEnabled")) {
														animatorStateRuntimeControlEnabledSerialized = iterator.current;
												}

												if (iterator.Find ("animatorStateRunTimeControl")) {
														animatorStateRunTimeControlSerialized = iterator.current;
												}
										}
					
										
										//mecanimNode.animationRunTimeControlEnabled = EditorGUILayout.Toggle ("Enable TimeControl", mecanimNode.animationRunTimeControlEnabled);
										List<int> nAnimatorStateRuntimeControlHaveEnabled = MecanimNode.nAnimatorStateRuntimeControlHaveEnabled;
										if (animatorStateRuntimeControlEnabledSerialized != null && animatorStateRunTimeControlSerialized != null && (bool)animatorStateRuntimeControlEnabledSerialized.value) {
												Rect timeControlRect = GUILayoutUtility.GetRect (Screen.width - 16f, 26f);
												timeControlRect.xMin += 38f;
												timeControlRect.xMax -= 70f;
												animatorStateRunTimeControlSerialized.value = timeNormalized = EditorGUILayoutEx.CustomHSlider (timeControlRect, (float)animatorStateRunTimeControlSerialized.value, 0f, 1f, TimeControlW.style.timeScrubber);
												

										}	
								
										

								}
								///////////////////////////////////////////////////////////////


								/////////// AVATAR Preview GUI ////////////
								
				
								if (!Application.isPlaying && motion != null) {
						
									
										

										
										//This makes layout to work (Reserving space)
										Rect avatarRect = GUILayoutUtility.GetRect (Screen.width - 16f, 200);
										avatarRect.width -= 70f;
										avatarRect.xMin += 6f;
										
									
										if (avatarPreview == null)
												avatarPreview = new AvatarPreviewW (null, motion);
										else
												avatarPreview.SetPreviewMotion (motion);
									
										
										
										EditorGUILayout.BeginHorizontal ();

										

										if (eventTimeValues != null && Event.current.type == EventType.Repaint) {

												//find first selected if exist
												int eventTimeValueSelectedIndex = Array.IndexOf (eventTimeValuesSelected, true);


							
					
												if (eventTimeValueSelectedIndex > -1) {
														avatarPreview.SetTimeAt (eventTimeValues [eventTimeValueSelectedIndex]);
												} else {
														
													
														//!!! changing 
														//avatarPreview.timeControl.startTime
														// start/stop makes AvatarPreview to play from start to stop
														// but its not offer good usability of resized animation

													
														if (avatarPreview.timeControl.playing) {

																//restrict animation into this range
																if (avatarPreview.timeControl.normalizedTime < mecanimNode.range.rangeStart || avatarPreview.timeControl.normalizedTime > mecanimNode.range.rangeEnd) {

								
																		
																		avatarPreview.timeControl.nextCurrentTime = avatarPreview.timeControl.startTime * (1f - mecanimNode.range.rangeStart) + avatarPreview.timeControl.stopTime * mecanimNode.range.rangeStart;	

																}
														} else {

																//set AvatarPreview animation time range depending of drag of range control handles
																if (Math.Abs (mecanimNode.range.rangeStart - timeNormalizedStartPrev) > 0.01f) {
																		timeNormalizedStartPrev = mecanimNode.range.rangeStart;
																		
																		avatarPreview.SetTimeAt (timeNormalizedStartPrev);
									
																} else
																if (Math.Abs (mecanimNode.range.rangeEnd - timeNormalizedEndPrev) > 0.01f) {
																		timeNormalizedEndPrev = mecanimNode.range.rangeEnd;
																		avatarPreview.SetTimeAt (timeNormalizedEndPrev);
									
																}
								
								
														}
							
							
							
							
							
							
												}
						
						

										}				


										

										avatarPreview.timeControl.playbackSpeed = mecanimNode.speed;

										
										


										avatarPreview.DoAvatarPreview (avatarRect, GUIStyle.none);


										timeNormalized = avatarPreview.timeControl.normalizedTime;

										//Debug.Log(avatarPreview.timeControl.currentTime+" "+);
										EditorGUILayout.EndHorizontal ();		
										
										
									
										


								
									


										////////// Events Timeline GUI //////////

										if (!eventTimeLineInitalized) {
					
					
					
					
												//TODO calculate PopupRect
					
												eventTimeLineValuePopUpRect = new Rect ((Screen.width - 250) * 0.5f, (Screen.height - 150) * 0.5f, 250, 150);


												//select the time values from nodes
												//eventTimeValues = mecanimNode.children.Select ((val) => (float)((SendEventNormalized)val).timeNormalized.Value).ToArray ();

							
												eventTimeValues = mecanimNode.children.Select ((val) => (float)((SendEventNormalized)val).timeNormalized.serializedProperty.floatValue).ToArray ();


												eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();
												eventTimeValuesSelected = new bool[eventTimeValues.Length];
					
												playButtonStyle = "TimeScrubberButton";
					
												if (playButtonStyle != null)
														playButtonSize = playButtonStyle.CalcSize (new GUIContent ());
					
												eventTimeLineInitalized = true;
										}

										
				
										Rect timeLineRect = GUILayoutUtility.GetRect (Screen.width - 16f, 50f);
										//Rect timeLineRect = GUILayoutUtility.GetLastRect ();

										timeLineRect.xMin += playButtonSize.x - EditorGUILayoutEx.eventMarkerTexture.width * 0.5f;
										timeLineRect.xMax -= EditorGUILayoutEx.eventMarkerTexture.width * 0.5f;
										//timeLineRect.height = EditorGUILayoutEx.eventMarkerTexture.height * 3 * 0.66f + playButtonSize.y;
										timeLineRect.width -= 66f;
										EditorGUILayoutEx.CustomTimeLine (ref timeLineRect, ref eventTimeValues, ref eventTimeValuesPrev, ref eventDisplayNames, ref eventTimeValuesSelected, avatarPreview.timeControl.normalizedTime,
				                                  onMecanimEventAdd, onMecanimEventDelete, onMecanimEventClose, onMecanimEventEdit, onMecanimEventDragEnd
										);
				
										EditorGUILayout.LabelField ("Events Timeline");
				
										SendEventNormalized ev;
				
				
				
										//update time values 
										int eventTimeValuesNumber = mecanimNode.children.Length;
										for (i=0; i<eventTimeValuesNumber; i++) {
												ev = ((SendEventNormalized)mecanimNode.children [i]);	
												//ev.timeNormalized.Value = eventTimeValues [i];
												ev.timeNormalized.serializedProperty.floatValue = eventTimeValues [i];
					
												//if changes have been made in pop editor or SendEventNormailized inspector
												if (ev.name != eventDisplayNames [i])
														eventDisplayNames [i] = ((SendEventNormalized)mecanimNode.children [i]).name;
					
					
												ev.timeNormalized.ApplyModifiedProperties ();
							
										}

										
						
						
										// Restore the indent level
										//EditorGUI.indentLevel = indentLevel;
								
										// Apply modified properties
										this.serializedNode.ApplyModifiedProperties ();
								
								}





								//////////////////////////////////////////////////////////////
								/// 
								///			 Draw RED TIME on top of curveEditor
								///
								//////////////////////////////////////////////////////////////
				
								if (_curvesEditorShow) {
										Handles.color = Color.red;
					
										if (avatarPreview != null) {
												timeNormalized = avatarPreview.timeControl.normalizedTime;
										}
					
										float leftrightMargin = 40f;
										float effectiveWidth = curveEditorRect.width - 2 * leftrightMargin - curveEditorRect.xMin;
										float timeLineX = curveEditorRect.xMin + leftrightMargin + effectiveWidth * timeNormalized;
					
										Handles.DrawLine (new Vector2 (timeLineX, curveEditorRect.y), new Vector2 (timeLineX, curveEditorRect.y + curveEditorRect.height));





										////////// EVALUTE //////////
										int variablesNum = variablesBindedToCurves.Length;
										for (int varriableCurrentinx=0; varriableCurrentinx<variablesNum; varriableCurrentinx++) {


												variablesBindedToCurves [varriableCurrentinx].Value = curves [varriableCurrentinx].Evaluate (timeNormalized);
										}
										 
		
					
								}




						} 
			
					
								



						//NOTES!!! I"ve gone with edit popup but I might draw Nodes here but think would move whole avatar and timeline preview down/up
						//if I draw them all or maybe just selected one(but what if many are selected ???) maybe I would draw it here as popup sucks

			

								

				}
		}
}