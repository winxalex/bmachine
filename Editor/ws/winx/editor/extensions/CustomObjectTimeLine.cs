using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ws.winx.unity;
using ws.winx.bmachine.extensions;

namespace ws.winx.editor.extensions
{
		[Serializable]
		public class CustomObjectTimeLine
		{

				private static int CONTROL_ID = -1;
				private static int SELECTED_INDEX = -1;
				private static IList CHANGED_VALUES = null;


				public class TimeLineEventArgs:EventArgs
				{
						

						int _selectedIndex;
						float[] _values;
						float _selectedValue;
						int _controlID;

						public int controlID{ get { return _controlID; } }

						public float selectedValue{ get { return _selectedValue; } }

						public float[] values{ get { return _values; } }

						public int selectedIndex {
								get{ return _selectedIndex;}
						}

						public TimeLineEventArgs (int selectedIndex, float selectedValue, float[] values, int controlID)
						{
								this._selectedValue = selectedValue;
								this._values = values;
								this._controlID = controlID;
								this._selectedIndex = selectedIndex;
						}
				}

				public event EventHandler<TimeLineEventArgs> EditOpen;
				public event EventHandler<TimeLineEventArgs> EditClose;
				public event EventHandler<TimeLineEventArgs> Delete;
				public event EventHandler<TimeLineEventArgs> Add;
		        




				//
				// Fields
				//
				private int m_HoverEvent = -1;
				private bool m_DirtyTooltip;
				private static Texture image = EditorGUIUtility.IconContent ("Animation.EventMarker").image;
				private string m_InstantTooltipText;
				private bool[] __valuesSelected;
				[NonSerialized]
				private float[]
						timeValuesTime;
				
		


				//
				// Constructors
				//
				public CustomObjectTimeLine ()
				{

				}
		
				//
				// Methods
				//
				
		
				private void CheckRectsOnMouseMove (Rect postionRect, ref float[] values, Rect[] hitRects, int controlID)
				{
						Vector2 mousePosition = Event.current.mousePosition;
						bool flag = false;
						if (values.Length == hitRects.Length) {
								for (int i = hitRects.Length - 1; i >= 0; i--) {
										if (hitRects [i].Contains (mousePosition)) {
												if (Event.current.button == 1 && Event.current.isMouse) {
														Debug.Log ("Right click inside hitRect" + hitRects [i] + " " + mousePosition);
														Event.current.Use ();
														this.m_HoverEvent = -1;
														this.m_InstantTooltipText = string.Empty;
														onContextClickOnTimeValue (new TimeLineEventArgs (i, values [i], values, controlID));
														return;
												}


												flag = true;
												if (this.m_HoverEvent != i) {


														//Debug.Log("Hover over "+i);
														this.m_HoverEvent = i;
													
														this.m_InstantTooltipText = i.ToString ();// events [this.m_HoverEvent].functionName;
														//	this.m_InstantTooltipPoint = new Vector2 (hitRects [this.m_HoverEvent].xMin + (float)((int)(hitRects [this.m_HoverEvent].width / 2f)) + eventLineRect.x, eventLineRect.yMax);
														this.m_DirtyTooltip = true;



												}
										}
								}
						}
						if (!flag) {
								this.m_HoverEvent = -1;
								this.m_InstantTooltipText = string.Empty;
						}
				}
		
				private void DeleteEvents (TimeLineEventArgs args, bool[] deleteIndices)
				{
						bool deletionHappend = false;
						List<float> list = new List<float> (args.values);

						
						for (int i = list.Count  - 1; i >= 0; i--) {
		
								if (deleteIndices [i]) {
										list.RemoveAt (i);
										deletionHappend = true;
								}
						}

						if (deletionHappend) {
								
								if (this.EditClose != null)
										this.EditClose (this, null);

								//TODO CREATE UNDOS
								//Undo.RegisterCompleteObjectUndo (this, "Delete Event");

								CHANGED_VALUES = list.ToArray ();
								CONTROL_ID = args.controlID;
								
								

								this.__valuesSelected = new bool[list.Count];
								this.m_DirtyTooltip = true;

								if (this.Delete != null)
										this.Delete (this, new TimeLineEventArgs (-1, 0f, (float[])CHANGED_VALUES, args.controlID));
						}
				}
		
				public void DeselectAll ()
				{
						this.__valuesSelected = null;
				}
		
				public void DrawInstantTooltip (Rect position)
				{


				}
		
				public virtual void onAdd (System.Object obj)
				{
						//Debug.Log ("onAdd");
						TimeLineEventArgs args = (TimeLineEventArgs)obj;
					
						//TODO Create UNDO
						//Undo.RegisterCompleteObjectUndo (this, "Add Event");

						int newTimeValueInx = args.values.Length;

						//find first time > then current and insert before it
						for (int i = 0; i < newTimeValueInx; i++) {
								if (args.values [i] > args.selectedValue) {
										newTimeValueInx = i;
										break;
								}
						}

						float[] timeValues = args.values;

						ArrayUtility.Insert<float> (ref timeValues, newTimeValueInx, args.selectedValue);

						
						
						CONTROL_ID = args.controlID;
						CHANGED_VALUES = timeValues;



						this.Select (newTimeValueInx, timeValues.Length);

						
						//open editor for newely added 
						if (Add != null)
								Add (this, new TimeLineEventArgs (newTimeValueInx, args.selectedValue, timeValues, args.controlID));
				}
		
				public virtual void onDelete (System.Object obj)
				{
						TimeLineEventArgs args = (TimeLineEventArgs)obj;

						int index = args.selectedIndex;
						if (this.__valuesSelected [index]) {
								this.DeleteEvents (args, this.__valuesSelected);
						} else {
								bool[] timeValuesSelected = new bool[this.__valuesSelected.Length];
								timeValuesSelected [index] = true;
								this.DeleteEvents (args, timeValuesSelected);
						}


				}
		
				public virtual void onEdit (System.Object obj)
				{
						TimeLineEventArgs args = (TimeLineEventArgs)obj;

						if (EditOpen != null)
								EditOpen (this, args);

						this.Select (args.selectedIndex, args.values.Length);
				}

				public virtual void onContextClickOnTimeValue (TimeLineEventArgs args)
				{

						GenericMenu genericMenu = new GenericMenu ();
						genericMenu.AddItem (new GUIContent ("Edit"), false, new GenericMenu.MenuFunction2 (this.onEdit), args);
						genericMenu.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
						genericMenu.AddItem (new GUIContent ("Delete"), false, new GenericMenu.MenuFunction2 (this.onDelete), args);
						genericMenu.ShowAsContext ();

				}

				public virtual void onContextClick (TimeLineEventArgs args)
				{
						//Debug.Log ("ContextClick on empty");
						Event.current.Use ();
						GenericMenu genericMenu2 = new GenericMenu ();
						genericMenu2.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
						genericMenu2.ShowAsContext ();
				}
				
		
				//public void EventLineGUI (Rect rect, AnimationSelection selection, AnimationWindowState state, CurveEditor )
				public float[] onTimeLineGUI (float[] timeValues, bool sorted=false)
				{
						//main contorol position
						Rect rectGlobal = GUILayoutUtility.GetLastRect ();
						rectGlobal.y += 100;
						rectGlobal.xMax -= 3f;
						rectGlobal.xMin = 33f;
						rectGlobal.height = 100f;


						int controlID = GUIUtility.GetControlID (FocusType.Passive) + 1;
						//Debug.Log ("Cid:" + controlID + " " + GUIUtility.hotControl);

						if (controlID == CONTROL_ID) {
								CONTROL_ID = -1;
								timeValues = (float[])CHANGED_VALUES;
								CHANGED_VALUES = null;
						}
						
						GUI.BeginGroup (rectGlobal);
						Color color = GUI.color;

						Rect rectLocal = new Rect (0f, 0f, rectGlobal.width, rectGlobal.height);

						//background
						GUI.Box (rectLocal, GUIContent.none);
						rectLocal.width -= image.width;

		
					
							
						float time = 0f;
						if (rectLocal.Contains (Event.current.mousePosition))
								time = (float)Math.Round (Event.current.mousePosition.x / rectLocal.width, 2);
						


						int timeValuesNumber = timeValues.Length;
						Rect[] positionsHitRectArray = new Rect[timeValuesNumber];
						Rect[] positionsRectArray = new Rect[timeValuesNumber];
						int timeValuesNumberOfTheSame = 1;//items that have same time
						int timeValuesTheSameCounterDown = 0;//same time items count down

						for (int i = 0; i < timeValuesNumber; i++) {
								float timeValue = timeValues [i];

								if (timeValuesTheSameCounterDown == 0) {
										timeValuesNumberOfTheSame = 1;

										while (i + timeValuesNumberOfTheSame < timeValuesNumber && timeValues [i + timeValuesNumberOfTheSame] == timeValue) {
												timeValuesNumberOfTheSame++;
										}
						
										//init counter to number of items with same time value
										timeValuesTheSameCounterDown = timeValuesNumberOfTheSame;
								}


							
								float timeValuePosition = timeValue * rectLocal.width;


								timeValuesTheSameCounterDown--;
								Rect rect3 = new Rect (timeValuePosition , image.height * timeValuesTheSameCounterDown * 0.66f, (float)image.width, (float)image.height);


								positionsHitRectArray [i] = rect3;
								positionsRectArray [i] = rect3;
						}


						if (this.m_DirtyTooltip) {

								if (this.m_HoverEvent >= 0 && this.m_HoverEvent < positionsHitRectArray.Length) {
										//Debug.Log ("AnimationEventPopup.FormatEvent... tooltip text");
										this.m_InstantTooltipText = "Mile kitic";

										//DrawInstantTooltip(rectLocal);



					
								}
								this.m_DirtyTooltip = false;
						}


							

						if (this.__valuesSelected == null || this.__valuesSelected.Length != timeValuesNumber) {
								this.__valuesSelected = new bool[timeValuesNumber];

								if (EditClose != null)
										EditClose (this, null);

								
								
						}


						

						Vector2 offset = Vector2.zero; 
						int clickedIndex;
						float startSelect;
						float endSelect;
						
						HighLevelEvent highLevelEvent = EditorGUIExtW.MultiSelection (rectGlobal, positionsRectArray, new GUIContent (image), positionsHitRectArray, ref this.__valuesSelected, null, out clickedIndex, out offset, out startSelect, out endSelect, GUIStyle.none);

						if (highLevelEvent != HighLevelEvent.None) {
								switch (highLevelEvent) {
								case HighLevelEvent.DoubleClick:
										if (clickedIndex != -1) {
												if (EditOpen != null) {
														EditOpen (this, new TimeLineEventArgs (clickedIndex, timeValues [clickedIndex], timeValues, controlID));

												}
											
										} else {
												//never enters here
												this.onAdd (new TimeLineEventArgs (clickedIndex, time, timeValues, controlID));
												
										}
										break;
								case HighLevelEvent.Click:
										SELECTED_INDEX = clickedIndex;
										Debug.Log ("Clikc on " + clickedIndex);
										break;
								case HighLevelEvent.ContextClick:
										{
												SELECTED_INDEX = clickedIndex;
												//Debug.Log ("ContextClick on handle");
												onContextClickOnTimeValue (new TimeLineEventArgs (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
												this.m_InstantTooltipText = null;
												this.m_DirtyTooltip = true;
														
												break;
										}
					      
								case HighLevelEvent.BeginDrag:
										//this.timeValuesAtMouseDown = timeValues;
										this.timeValuesTime = new float[timeValues.Length];

												
										for (int j = 0; j < timeValues.Length; j++) {
												this.timeValuesTime [j] = timeValues [j];
										}

										

			
										break;
								case HighLevelEvent.Drag:
										{
				
												for (int k = timeValues.Length - 1; k >= 0; k--) {
														if (this.__valuesSelected [k]) {
																	
																timeValues [k] = (float)Math.Round (this.timeValuesTime [k] + offset.x / rectLocal.width, 2);
																	
																if (timeValues [k] > 1f)
																		timeValues [k] = 1f;
																else if (timeValues [k] < 0f)
																		timeValues [k] = 0f;
																//Debug.Log ("Dragged time" + timeValues [k]);
														}
												}

												if (sorted) {
														int[] indexArray = new int[this.__valuesSelected.Length];
														for (int l = 0; l < indexArray.Length; l++) {
																indexArray [l] = l;
														}
								
														Array.Sort (timeValues, indexArray);

														bool[] cloneOfSelected = (bool[])this.__valuesSelected.Clone ();
														
														float[] cloneOfTimes = (float[])this.timeValuesTime.Clone ();
														for (int m = 0; m < indexArray.Length; m++) {
																this.__valuesSelected [m] = cloneOfSelected [indexArray [m]];
																this.timeValuesTime [m] = cloneOfTimes [indexArray [m]];
														}

											
												
												}
												//TODO CREATE UNDO
												//Undo.RegisterCompleteObjectUndo (this, "Move Event");
					
					
												this.m_DirtyTooltip = true;
												break;
										}
								case HighLevelEvent.Delete:
										this.DeleteEvents (new TimeLineEventArgs (clickedIndex, time, timeValues, controlID), this.__valuesSelected);
										break;
								case HighLevelEvent.SelectionChanged:
						
										if (clickedIndex != -1) {
												if (EditOpen != null) {
														EditOpen (this, new TimeLineEventArgs (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
													
												}


										}
										break;
								}
						}


						this.CheckRectsOnMouseMove (rectGlobal, ref timeValues, positionsHitRectArray, controlID);

		

//			if (Event.current.type == EventType.MouseUp && Event.current.button==0 && CustomObjectTimeLine.SELECTED_INDEX > -1) {
//				if  (EditOpen != null ) {
//					clickedIndex=SELECTED_INDEX;
//					EditOpen (this, new TimeLineEventArgs (clickedIndex,timeValues[clickedIndex],timeValues,controlID));
//					SELECTED_INDEX=-1;
//				}
//						}
//			
//						//this.m_InstantTooltipText = "mu text";
//						if (this.m_InstantTooltipText != null && this.m_InstantTooltipText != String.Empty) {
//								if (this.m_HoverEvent >= 0 && this.m_HoverEvent < positionsHitRectArray.Length) {
//
//
//
//										GUIStyle gUIStyle;
//										Rect hitRect = positionsHitRectArray [this.m_HoverEvent];
//
//										gUIStyle = "AnimationEventTooltipArrow";
//										Vector2 arrowSize = gUIStyle.CalcSize (new GUIContent ());
//
//
//
//										//GUI.skin.label.alignment=
//										GUI.Label (new Rect (hitRect.x - arrowSize.x * 0.5f - 5f, hitRect.y + hitRect.height, arrowSize.x, arrowSize.y), string.Empty, gUIStyle);
//										//	GUI.skin.label.alignment=TextAnchor.MiddleCenter;
//
//
//
//										gUIStyle = "AnimationEventTooltip";
//										Vector2 textSize = gUIStyle.CalcSize (new GUIContent (this.m_InstantTooltipText));
//										gUIStyle.alignment = TextAnchor.MiddleCenter;
//
//
//
//										//GUI.skin.label.alignment=TextAnchor.MiddleCenter;
//										GUI.Label (new Rect (hitRect.x, hitRect.y + hitRect.height + arrowSize.y, Mathf.Max (arrowSize.x, textSize.x), textSize.y), this.m_InstantTooltipText, gUIStyle);
//
//										
//								}
//						}



						//if (Event.current.type == EventType.ContextClick && rect2.Contains (Event.current.mousePosition) && selection.EnsureClipPresence ())
						if (Event.current.type == EventType.ContextClick && rectLocal.Contains (Event.current.mousePosition) 
								|| (Event.current.button == 1 && Event.current.isMouse)
			    ) {

								onContextClick (new TimeLineEventArgs (clickedIndex, time, timeValues, controlID));


			

						}
						GUI.color = color;

		

						GUI.EndGroup ();


						return timeValues; 
				}
		

				/// <summary>
				/// Set selection flag to true to timeValue with index
				/// into maximum selectable values
				/// </summary>
				/// <param name="index">Index.</param>
				/// <param name="maxItemsSelectable">Max items selectable.</param>
				private void Select (int index, int timeValuesSelectableMax)
				{
						this.__valuesSelected = new bool[timeValuesSelectableMax];
						this.__valuesSelected [index] = true;
				}
		
				


			
		
				
		}
}