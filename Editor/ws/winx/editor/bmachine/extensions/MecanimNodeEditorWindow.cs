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
using BehaviourMachine;
using BehaviourMachineEditor;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.unity;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ws.winx.editor.extensions;
using System.IO;

namespace ws.winx.editor.bmachine.extensions
{
		public class MecanimNodeEditorWindow:EditorWindow
		{
				private static MecanimNodeEditorWindow window;
				private static MecanimNode __mecanimNode;
				private static SerializedNode __serializedNode;
				private static ReorderableList __gameObjectClipList;
				private static AvatarPreviewW avatarPreview;
				private static  SerializedNodeProperty curvesSerialized;
				private static	SerializedNodeProperty curvesColorsSerialized;
				private static	SerializedNodeProperty variablesBindedToCurvesSerialized;
				private static	SerializedNodeProperty animatorStateSerialized;
				private static	SerializedNodeProperty animatorStateRuntimeControlEnabledSerialized;
				private static	SerializedNodeProperty animatorStateRunTimeControlSerialized;
				private static	SerializedNodeProperty motionOverrideSerialized;
				private static	SerializedNodeProperty clipBindingsSerialized;
				private static float timeNormalized;
				private static AnimationCurve[] curves;
				private		static Color[] curveColors;
				private		static UnityVariable[] variablesBindedToCurves;
				private		static string[] curvePropertyDisplayOptions;
				private		static int _curveIndexSelected;
				private		static Color _colorSelected;
				private		static bool _curvesEditorShow;
				private		static CurveEditorW curveEditor;
				private		static GUIContent propertyPopupLabel = new GUIContent (String.Empty);
				private		static Vector2 curvePropertiesScroller;
				private		static UnityVariable _variableSelected;
				private static bool __isPlaying;
				private static bool __isRecording;
				private static float __timeCurrent;//in [seconds]
				private static EditorClipBinding __nodeClipBinding;
				private static EditorClipBinding[] __clipBindings;

				public static void Show (MecanimNode target, SerializedNode node, Rect? position)
				{
						MecanimNodeEditorWindow.__mecanimNode = target;

						MecanimNodeEditorWindow.__serializedNode = node;


						

						///////   ACCESS SERIALIZED DATA /////////
						NodePropertyIterator iterator = node.GetIterator ();

						__isPlaying = false;
						__isRecording = false;
						_variableSelected = null;
						timeNormalized = 0f;
						AnimationMode.StopAnimationMode ();
						Undo.postprocessModifications -= PostprocessAnimationRecordingModifications;


						if (iterator.Find ("animatorStateSelected"))
								animatorStateSerialized = iterator.current;
			
			
						if (iterator.Find ("motionOverride"))
								motionOverrideSerialized = iterator.current;

						if (iterator.Find ("clipBindings"))
								clipBindingsSerialized = iterator.current;



			
						if (__nodeClipBinding == null)
								__nodeClipBinding = ScriptableObject.CreateInstance<EditorClipBinding> ();
						
						__nodeClipBinding.gameObject = target.self;
						__nodeClipBinding.clip = getNodeClip ();
						__nodeClipBinding.boneTransform = __nodeClipBinding.gameObject.GetRootBone ();

						/////// INIT SERIALIZED NODE PROPERTIES - CURVES, COLORS, VARIABLES //////
						
							
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
						
						AnimationModeUtility.ResetBindingsTransformPropertyModification (clipBindingsSerialized.value as EditorClipBinding[]);
						AnimationModeUtility.ResetBindingTransformPropertyModification (__nodeClipBinding);
						

						__gameObjectClipList = new ReorderableList (clipBindingsSerialized.value as IList, typeof(EditorClipBinding), true, true, true, true);
						__gameObjectClipList.drawElementCallback = onDrawElement;
									
						
									
						__gameObjectClipList.drawHeaderCallback = onDrawHeaderElement;
									
						__gameObjectClipList.onRemoveCallback = onRemoveCallback;
						__gameObjectClipList.onAddCallback = onAddCallback;
						
									
						//__gameObjectClipList.elementHeight = 32f;                                    
			                                                              
			                                                              
						if (MecanimNodeEditorWindow.window != null)//restore last 
								position = window.position;

		
						MecanimNodeEditorWindow.window = (MecanimNodeEditorWindow)EditorWindow.GetWindow (typeof(MecanimNodeEditorWindow));


						 
						if (position.HasValue)
								MecanimNodeEditorWindow.window.position = position.Value;
						MecanimNodeEditorWindow.window.Show ();
				}

				public static void Hide ()
				{
						if (window != null)
								window.Close ();
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




				///////// ANIMATION MODE ////////////////
	
//		private static void SetAutoRecordMode (bool record)
//		{
//			if ( != record) {
//				if (record) {
//					//Undo.postprocessModifications+=this.PostprocessAnimationRecordingModifications;
//					
//					Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine (Undo.postprocessModifications, new Undo.PostprocessModifications (PostprocessAnimationRecordingModifications));
//				} else {
//					Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove (Undo.postprocessModifications, new Undo.PostprocessModifications (PostprocessAnimationRecordingModifications));
//				}
//				this.m_AutoRecord = record;
//				//			if (this.m_AutoRecord)
//				//			{
//				//				this.EnsureAnimationMode ();
//				//			}
//			}
//		}
		
				private static UndoPropertyModification[] PostprocessAnimationRecordingModifications (UndoPropertyModification[] modifications)
				{
						List<UndoPropertyModification> propertyModificationList = new List<UndoPropertyModification> ();
						EditorClipBinding[] clipBindings = clipBindingsSerialized.value as EditorClipBinding[];

						Array.ForEach (clipBindings, (itm) => {

								propertyModificationList.Concat (AnimationModeUtility.Process (itm.gameObject, itm.clip, modifications, __timeCurrent));


						});


						return propertyModificationList.ToArray ();
				}


			
				////////////////// GAMEOBJECT - CLIP LIST EVENTS //////////////////
				
		
				private static void onRemoveCallback (ReorderableList list)
				{
						if (UnityEditor.EditorUtility.DisplayDialog ("Warning!", 
			                                             "Are you sure you want to delete the Unity Variable?", "Yes", "No")) {
								List<EditorClipBinding> bindingList = ((EditorClipBinding[])clipBindingsSerialized.value).ToList ();
								bindingList.RemoveAt (list.index);

								clipBindingsSerialized.value = bindingList.ToArray ();
								clipBindingsSerialized.ApplyModifiedValue ();
				
								list.list = clipBindingsSerialized.value as IList;
				

				
						}
				}

				private static void onAddCallback (ReorderableList list)
				{
						List<EditorClipBinding> bindingList = ((EditorClipBinding[])clipBindingsSerialized.value).ToList ();


						bindingList.Add (ScriptableObject.CreateInstance<EditorClipBinding> ());

						clipBindingsSerialized.value = bindingList.ToArray ();
						clipBindingsSerialized.ApplyModifiedValue ();

						list.list = clipBindingsSerialized.value as IList;
						
				}
		
				private static void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
				{
			



						EditorClipBinding clipBindingCurrent = __gameObjectClipList.list [index] as EditorClipBinding;
			
						if (clipBindingCurrent == null) {
								return;
						}

						

						float width = rect.xMax;
						
						rect.xMax = 200f;

						EditorGUI.BeginChangeCheck ();

						clipBindingCurrent.gameObject = EditorGUI.ObjectField (rect, clipBindingCurrent.gameObject, typeof(GameObject), true) as GameObject;

						if (EditorGUI.EndChangeCheck () && clipBindingCurrent.gameObject != null) {	
								clipBindingCurrent.boneTransform = clipBindingCurrent.gameObject.GetRootBone ();
					
						}



						rect.xMin = rect.xMax + 2;
						rect.xMax = width - 60f;


						

						clipBindingCurrent.clip = EditorGUI.ObjectField (rect, clipBindingCurrent.clip, typeof(AnimationClip), true) as AnimationClip;

						if (clipBindingCurrent.clip == null) {

								rect.xMin = rect.xMax + 2;
								rect.xMax = width;

								if (GUI.Button (rect, "New Clip")) {

										string path = EditorUtility.SaveFilePanel (
										"Create New Clip",
										"Assets",
										"",
										"anim");
										
										if (!String.IsNullOrEmpty (path)) {
											
												AnimationClip clip = new AnimationClip();//UnityEditor.Animations.AnimatorController.AllocateAnimatorClip ();
						clip.name=Path.GetFileNameWithoutExtension (path);		
						AssetDatabase.CreateAsset(clip,"Assets/"+Path.GetFileName (path));
												AssetDatabase.SaveAssets();
												clipBindingCurrent.clip=clip;

										}
								}
						}
			
				}
		
				static void onDrawHeaderElement (Rect rect)
				{
						EditorGUI.LabelField (rect, "GameObject - AnimationClips:");
				}



				/// /////////////////////////////////////////////////////////////////////


	
				static AnimationClip getNodeClip ()
				{
						//////////  MOTION OVERRIDE HANDLING  //////////
			
						UnityEngine.Motion motion = null;
			
						UnityVariable motionOverridVariable = (UnityVariable)motionOverrideSerialized.value;
			
						//if there are no override use motion of selected AnimationState
						//Debug.Log(((UnityEngine.Object)mecanimNode.motionOverride.Value).);
						if (motionOverridVariable == null || motionOverridVariable.Value == null || motionOverridVariable.ValueType != typeof(AnimationClip))
								motion = ((ws.winx.unity.AnimatorState)animatorStateSerialized.value).motion;
						else //
								motion = (UnityEngine.Motion)motionOverridVariable.Value;
			
			
			
						if (motionOverridVariable != null && motionOverridVariable.Value != null && ((ws.winx.unity.AnimatorState)animatorStateSerialized.value).motion == null) {
								Debug.LogError ("Can't override state that doesn't contain motion");
						}

						return motion as AnimationClip;

				}

				
				/// <summary>
				/// Raises the GU event.
				/// </summary>
				void OnGUI ()
				{


			
						if (!Application.isPlaying && __mecanimNode != null && animatorStateSerialized.value != null) {



								_curvesEditorShow = EditorGUILayout.Foldout (_curvesEditorShow, "Curves");
				
								//int indentLevel = 0;
				
								Rect curveEditorRect = new Rect (0, 0, 0, 0);
								int i = 0;

				
								if (_curvesEditorShow) {
					

					
										//This makes layout to work (Reserving space)
										curveEditorRect = GUILayoutUtility.GetRect (Screen.width - 16f, 200);
					
										/////// CURVE EDITOR ////////
										curveEditorRect.width = curveEditorRect.width - 32f;
										curveEditorRect.x = 16f;
					
					
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
						
						
												List<UnityVariable> blackboardLocalList = __mecanimNode.blackboard.GetVariableBy (typeof(float));
						
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
						
						
						
												__serializedNode.Update ();
						
												__serializedNode.ApplyModifiedProperties ();
						
						
												_variableSelected = null;
						
										}



										/// DELETE CURVE ///
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
						
												__serializedNode.ApplyModifiedProperties ();
						
						
										}
					
					
					
				
					
					
										EditorGUILayout.EndHorizontal ();



				


								} else {//NOT CURVE EDITOR



										///////////// GAMEOBJECT - CLIP BINDINGS //////////
								
										__gameObjectClipList.DoLayoutList ();


										//////////////////////////////////////////////

								}



								EditorGUILayout.Space ();



								__nodeClipBinding.clip = getNodeClip ();


								/////////////   TIME CONTROL OF ANIMATION (SLIDER) /////////
					
								Rect timeControlRect = GUILayoutUtility.GetRect (Screen.width, 26f);

								
							

								timeControlRect.xMax = 32f;

							


								__isPlaying = GUI.Toggle (timeControlRect, __isPlaying, !__isPlaying ? TimeControlW.style.playIcon : TimeControlW.style.pauseIcon, TimeControlW.style.playButton);


								if (__isPlaying) {
					
					
					
								} else {
					
					
					
								}		


								timeControlRect.xMin = timeControlRect.xMax + 1f;
								timeControlRect.xMax = timeControlRect.xMin + 21f;
								timeControlRect.yMin += 2f;


								EditorGUI.BeginChangeCheck ();

								Color color = GUI.color;


								if (AnimationMode.InAnimationMode ())
										GUI.color = AnimationMode.animatedPropertyColor;

								__isRecording = GUI.Toggle (timeControlRect, !__isRecording, TimeControlW.style.recordIcon, EditorStyles.toolbarButton);

								GUI.color = color;

								if (EditorGUI.EndChangeCheck ())
								if (__isRecording) {
						




										if (!AnimationMode.InAnimationMode ()) {


												List<EditorClipBinding> list = (clipBindingsSerialized.value as EditorClipBinding[]).ToList ();
												list.Add (__nodeClipBinding);

												__clipBindings = list.ToArray ();
												
												AnimationMode.StartAnimationMode ();
												Undo.postprocessModifications += PostprocessAnimationRecordingModifications;

												//calculate offset of boonRoot position before animation from boonRoot position at time=0s.
												AnimationModeUtility.SetBindingsOffset (clipBindingsSerialized.value as EditorClipBinding[]);

												AnimationModeUtility.SetBindingOffset (__nodeClipBinding);
												
												//calculate time in seconds from the current postion of time scrubber
												__timeCurrent = timeNormalized * getNodeClip ().length;

												//apply clip animaiton at __timeCurrent
												AnimationModeUtility.SampleClipBindingAt (__clipBindings
						                                        , __timeCurrent);

												

												SceneView.RepaintAll ();
										}
									
										
						
								} else {
										//Remove Undo property modificaiton handlers
										Undo.postprocessModifications -= PostprocessAnimationRecordingModifications;

										AnimationMode.StopAnimationMode ();

										//reset gameobject with bones to state before animation
										AnimationModeUtility.ResetBindingsTransformPropertyModification (clipBindingsSerialized.value as EditorClipBinding[]);

										//reset Node.self gameObject
										AnimationModeUtility.ResetBindingTransformPropertyModification (__nodeClipBinding);
												
										
						
								}

								timeControlRect.xMin = 40f + 16f;
								timeControlRect.xMax = Screen.width - 68f;
								timeControlRect.yMin -= 2f;



								EditorGUI.BeginChangeCheck ();

								timeNormalized = EditorGUILayoutEx.CustomHSlider (timeControlRect, timeNormalized, 0f, 1f, TimeControlW.style.timeScrubber);

								

								
								if (EditorGUI.EndChangeCheck ()) {
						
										__timeCurrent = timeNormalized * getNodeClip ().length;
									
										
										if (!AnimationMode.InAnimationMode ()) {
												AnimationMode.StartAnimationMode ();

												//calculate offset of boonRoot position before animation from boonRoot position at time=0s.
												AnimationModeUtility.SetBindingsOffset (clipBindingsSerialized.value as EditorClipBinding[]);

												AnimationModeUtility.SetBindingOffset (__nodeClipBinding);

										}

										if (!__isRecording) {
												__isRecording = true;	

												//add recording Undo events handlers
												Undo.postprocessModifications += PostprocessAnimationRecordingModifications;

												List<EditorClipBinding> list = (clipBindingsSerialized.value as EditorClipBinding[]).ToList ();
												list.Add (__nodeClipBinding);
						
												__clipBindings = list.ToArray ();

												
										}
									

								
										
										AnimationModeUtility.SampleClipBindingAt (__clipBindings, __timeCurrent);

							


									


								}









								///////////////////////////////////////////////////////////////



								//////////////////////////////////////////////////////////////
								/// 
								///			 Draw red time scrubber line on top of Curve Editor 
								///
								//////////////////////////////////////////////////////////////
				
								if (_curvesEditorShow) {
										Handles.color = Color.red;
				
				
				
										float leftrightMargin = 39f;// 40f;
										float effectiveWidth = curveEditorRect.width - 2 * leftrightMargin - curveEditorRect.xMin;
										float timeLineX = curveEditorRect.xMin + leftrightMargin + effectiveWidth * timeNormalized;
				
										Handles.DrawLine (new Vector2 (timeLineX, curveEditorRect.y), new Vector2 (timeLineX, curveEditorRect.y + curveEditorRect.height));
								}

					
					
					
								////////// EVALUTE CURVES //////////
								int variablesNum = variablesBindedToCurves.Length;
								for (int varriableCurrentinx=0; varriableCurrentinx<variablesNum; varriableCurrentinx++) {
						
						
										variablesBindedToCurves [varriableCurrentinx].Value = curves [varriableCurrentinx].Evaluate (timeNormalized);
								}
					
					

					
					
					
						}				
				
				
				
				
				

						

				
				}



				

		}
}

