//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
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
using UnityEditorInternal;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.csharp.extensions;
using ws.winx.unity;
using ws.winx.editor.extensions;
using Motion = UnityEngine.Motion;


using StateMachine = UnityEditorInternal.StateMachine;
using ws.winx.bmachine;

namespace ws.winx.editor.bmachine.extensions
{
	
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(MecanimNode), true)]
		public class MecanimNodeCustomEditor : NodeEditor
		{

				CurveVariable curvePropertySelected;
				UnityEngine.Object _objectSelected;
				GUIContent propertyPopupLabel = new GUIContent (String.Empty);
				string[] curvePropertyDisplayOptions;
				MethodInfo GetFloatVar_MethodInfo;
				int curvePropertyIndexSelected;
				Color _colorSelected;
				bool _curvesEditorShow;
				CurveEditorW curveEditor;
				MecanimNode mecanimNode;
				GUIContent[] displayOptions;
				List<MecanimStateInfo> animaInfoValues;
				MecanimStateInfo selectedAnimaStateInfo;
				float[] eventTimeValues;
				float[] eventTimeValuesPrev;
				bool eventTimeLineInitalized;
				Rect eventTimeLineValuePopUpRect;
				bool[] eventTimeValuesSelected;
				EventComparer eventTimeComparer = new EventComparer ();
				string[] eventDisplayNames;
				float timeNormalizedStartPrev = -1;
				GUIStyle playButtonStyle;
				Vector2 playButtonSize;
				AvatarPreviewW avatarPreview;
				Vector2 curvePropertiesScroller;
				Type _typeSelected;
				UnityVariable _variableSelected;
		        

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



			

			






			


				///  TIMELINE EVENTHANDLERS
		 
		 
				/// <summary>
				/// Ons the mecanim event edit.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventEdit (TimeLineArgs<float> args)
				{

						SendEventNormalized child = mecanimNode.children [args.selectedIndex] as SendEventNormalized;
					
		
						child.timeNormalized.Value = args.selectedValue;
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

				}




			



				/// <summary>
				/// On the mecanim event close.
				/// </summary>
				/// <param name="args">Arguments.</param>
				void onMecanimEventClose (TimeLineArgs<float> args)
				{
						SendEventNormalizedEditor.Hide ();
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
						child.timeNormalized.Value = args.selectedValue;

						mecanimNode.tree.SaveNodes ();

						eventTimeValues = (float[])args.values;

						//recreate (not to optimal but doesn't have
						eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

					

						
						
						//show popup
						SendEventNormalizedEditor.Show (child, eventTimeLineValuePopUpRect);

						Undo.RecordObject (target.self, "Add Node");
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
								this.eventTimeValues [m] = ev.timeNormalized; 
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
						eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();

	
						

						StateUtility.SetDirty (mecanimNode.tree);

						Undo.RecordObject (target.self, "Delete Node");
						SendEventNormalizedEditor.Hide ();


				}

				void onCurveSelect (int index)
				{

						//	if (mecanimNode.curveProperties != null)
						//			curvePropertySelected = mecanimNode.curveProperties [index];

				}

	
			
          

				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						int i = 0;



						DrawDefaultInspector ();

						mecanimNode = target as MecanimNode;

						if (mecanimNode != null) {

						
								Motion motion;
								if (mecanimNode.motionOverride == null)
										motion = mecanimNode.animaStateInfoSelected.motion;
								else
										motion = mecanimNode.motionOverride;
	
		

								if (mecanimNode.motionOverride != null && mecanimNode.animaStateInfoSelected.motion == null) {
										Debug.LogError ("Can't override state that doesn't contain motion");
								}
								
								

								if (Event.current.type == EventType.Layout) {
										this.serializedNode.Update ();

								}


								
								//_curvesEditorShow=EditorGUILayout.Foldout(_curvesEditorShow,"Curves");
								//EditorGUILayout.CurveField
								int indentLevel = 0;
								Rect curveEditorRect;
								//Rect curveEditorRect=GUILayoutUtility.GetLastRect();




			

				
								if (true) {
										//Debug.Log("LAst"+GUILayoutUtility.GetLastRect()+" "+GUILayoutUtility.GetRect(100,200));


										//This makes layout to work (Reserving space)
										curveEditorRect = GUILayoutUtility.GetRect (Screen.width - 46f, 200);

//					if(curveEditorRect.height<200) curveEditorRect.height=200;
//					curveEditorRect.width=Screen.width-46;
//					curveEditorRect.y=400;
										//GUILayout.BeginArea(curveEditorRect);
										//curveEditorRect=



										EditorGUILayout.BeginHorizontal ();

										//	curveEditorRect.y=curveEditorRect.height+16f;
										//	curveEditorRect.height=200;
										//	curveEditorRect.width=Screen.width-46f;
				
										//	Debug.Log(curveEditorRect+" "+Event.current.type);


					
			
					
					
//					AnimationCurve curve1=new AnimationCurve();
//
//
//
//						curve1.AddKey(new Keyframe(0, 1));
//						curve1.AddKey(new Keyframe(1, 1));
//
//				
//
//					AnimationCurve curve2=new AnimationCurve();
//
//
//					float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);
//					
//				
//					curve2.AddKey(new Keyframe(0, 0, tan45, tan45));
//					curve2.AddKey(new Keyframe(1, 1, tan45, tan45));

										indentLevel = EditorGUI.indentLevel;
										EditorGUI.indentLevel = 0;



										/////// CURVE EDITOR ////////
										curveEditorRect.width = curveEditorRect.width - 200;

										//mecanimNode.curveProperties.Select ((item)=>item.curve);

										if (curveEditor == null) {

												curveEditor = new CurveEditorW (curveEditorRect, new AnimationCurve (), false);
					
												curveEditor.FrameSelected (true, true);
												curveEditor.scaleWithWindow = true;

												curveEditor.onSelect += onCurveSelect;

										} else {

												curveEditor.rect = curveEditorRect;
												curveEditor.FrameSelected (false, false);
												//curveEditor.animationCurves
										}


										EditorGUILayout.BeginVertical (new GUILayoutOption[] {GUILayout.Width (200)});
										curveEditor.DoEditor ();
										EditorGUILayout.EndVertical ();










										GUIContent[] displayOptions = null;
										UnityVariable[] values = null;


										//select object which properties would be extracted
										_objectSelected = EditorGUILayout.ObjectField (_objectSelected, typeof(UnityEngine.Object), true);

										
										_typeSelected = EditorGUILayoutEx.CustomObjectPopup<Type> (null, _typeSelected, EditorGUILayoutEx.unityTypesDisplayOptions, EditorGUILayoutEx.unityTypes);

										
						
										if (_objectSelected != null) {//extract properties
												propertyPopupLabel.text = "Select [" + _objectSelected.name + "] property";
												
									
												//get properties from object by object type
												Utility.ObjectToDisplayOptionsValues<UnityVariable> (_objectSelected, _typeSelected, out displayOptions, out values);


										} else {//use Global and Local blackboard


												propertyPopupLabel.text = "Select blackboard variable";


												List<UnityVariable> localBlackBoardList = mecanimNode.blackboard.GetPropertyBy (_typeSelected);



												displayOptions = localBlackBoardList.Select ((item) => new GUIContent (item.name)).ToArray ();


												values = localBlackBoardList.ToArray ();

										}

					
										_variableSelected = EditorGUILayoutEx.CustomObjectPopup<UnityVariable> (null, _variableSelected, displayOptions, values);

				
										_colorSelected = EditorGUILayout.ColorField (_colorSelected);

									
									



						

										if (GUILayout.Button ("Add") && _variableSelected != null) {


												//how to access elements in LIST !!!! ????
												NodePropertyIterator iterator = this.serializedNode.GetIterator ();
												if (iterator.Find ("variablesBindedToCurves")) {

												SerializedNodeProperty variablesBindedToCurvesSerialized = iterator.current;
													

												}

												List<UnityVariabl> mecanimNode.variablesBindedToCurves.ToList().Add(_variableSelected).
												mecanimNode.variablesBindedToCurves=
												mecanimNode.curvesColors.Add (_colorSelected);
												mecanimNode.curves.Add (new AnimationCurve (new Keyframe[] {
														new Keyframe (0f, 0f),
														new Keyframe (1f, 1f)
												}));




												this.serializedNode.ApplyModifiedProperties ();

											

										}








										EditorGUILayout.EndHorizontal ();


										//GUILayout.EndArea();

										EditorGUI.indentLevel = indentLevel;
								}

//				
								EditorGUILayout.LabelField ("mile2");

								EditorGUILayout.LabelField ("mile2");

								EditorGUILayout.LabelField ("mile2");
								EditorGUILayout.LabelField ("mile5");
//				
				
						
								return;

								/////////// Avatar Preview GUI ////////////
								
				
								if (!Application.isPlaying && motion != null) {
						
										EditorGUILayout.LabelField ("Events Timeline");
										

										

									
										if (avatarPreview == null)
												avatarPreview = new AvatarPreviewW (null, motion);
										else
												avatarPreview.SetPreviewMotion (motion);
									
										
										Rect avatarRect = EditorGUILayout.BeginHorizontal ();

										avatarRect.y = curveEditorRect.yMax;

										avatarRect.y += 30f;
										avatarRect.height = Screen.height - avatarRect.y - 20f;

										if (eventTimeValues != null && Event.current.type == EventType.Repaint) {

												//find first selected if exist
												int eventTimeValueSelectedIndex = Array.IndexOf (eventTimeValuesSelected, true);


												
					
												if (eventTimeValueSelectedIndex > -1) {
														avatarPreview.SetTimeValue (eventTimeValues [eventTimeValueSelectedIndex]);
												} else {
														if (mecanimNode.timeNormalizedStart != timeNormalizedStartPrev) {
																timeNormalizedStartPrev = mecanimNode.timeNormalizedStart;
																avatarPreview.SetTimeValue (timeNormalizedStartPrev);
																
														}

												}
	

										}				


										

										avatarPreview.timeControl.playbackSpeed = mecanimNode.speed;





										avatarPreview.DoAvatarPreview (avatarRect, GUIStyle.none);
										//Debug.Log(avatarPreview.timeControl.currentTime+" "+);
										EditorGUILayout.EndHorizontal ();		
										
										
									
										


								
										//////////////////////////////////////////////////////////////


										////////// Events Timeline GUI //////////

										if (!eventTimeLineInitalized) {
					
					
					
					
												//TODO calculate PopupRect
					
												eventTimeLineValuePopUpRect = new Rect ((Screen.width - 250) * 0.5f, (Screen.height - 150) * 0.5f, 250, 150);
												//select the time values from nodes
												eventTimeValues = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).timeNormalized.Value).ToArray ();
												eventDisplayNames = mecanimNode.children.Select ((val) => ((SendEventNormalized)val).name).ToArray ();
												eventTimeValuesSelected = new bool[eventTimeValues.Length];
					
												playButtonStyle = "TimeScrubberButton";
					
												if (playButtonStyle != null)
														playButtonSize = playButtonStyle.CalcSize (new GUIContent ());
					
												eventTimeLineInitalized = true;
										}
				
										Rect timeLineRect = GUILayoutUtility.GetLastRect ();
				
										timeLineRect.xMin += playButtonSize.x - EditorGUILayoutEx.eventMarkerTexture.width * 0.5f;
										timeLineRect.height = EditorGUILayoutEx.eventMarkerTexture.height * 3 * 0.66f + playButtonSize.y;
				
										EditorGUILayoutEx.CustomTimeLine (ref timeLineRect, ref eventTimeValues, ref eventTimeValuesPrev, ref eventDisplayNames, ref eventTimeValuesSelected, avatarPreview.timeControl.normalizedTime,
				                                  onMecanimEventAdd, onMecanimEventDelete, onMecanimEventClose, onMecanimEventEdit, onMecanimEventDragEnd
										);
				
				
				
										SendEventNormalized ev;
				
				
				
										//update time values 
										int eventTimeValuesNumber = mecanimNode.children.Length;
										for (i=0; i<eventTimeValuesNumber; i++) {
												ev = ((SendEventNormalized)mecanimNode.children [i]);	
												ev.timeNormalized = eventTimeValues [i];
					
					
												//if changes have been made in pop editor or SendEventNormailized inspector
												if (ev.name != eventDisplayNames [i])
														eventDisplayNames [i] = ((SendEventNormalized)mecanimNode.children [i]).name;
					
					
					
					
										}

								
										// Restore the indent level
										EditorGUI.indentLevel = indentLevel;
								
										// Apply modified properties
										this.serializedNode.ApplyModifiedProperties ();
								
								}



						} 
			
					
								



						//NOTES!!! I"ve gone with edit popup but I might draw Nodes here but think would move whole avatar and timeline preview down/up
						//if I draw them all or maybe just selected one(but what if many are selected ???) maybe I would draw it here as popup sucks

			

								

				}
		}
}