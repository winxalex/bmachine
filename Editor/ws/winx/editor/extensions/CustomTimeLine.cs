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
		public class CustomTimeLine
		{

				private static int CONTROL_ID = -1;
				private static int SELECTED_INDEX = -1;
				private static IList CHANGED_VALUES = null;
				private static Texture image = EditorGUIUtility.IconContent ("Animation.EventMarker").image;


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


				//
				// Events
				//
				public event EventHandler<TimeLineEventArgs<float>> EditOpen;
				public event EventHandler<TimeLineEventArgs<float>> EditClose;
				public event EventHandler<TimeLineEventArgs<float>> Delete;
				public event EventHandler<TimeLineEventArgs<float>> Add;
		        




				//
				// Fields
				//
				
				
				private bool[] __valuesSelected;
				[NonSerialized]
				private float[]
						timeValuesTime;
				
		


				//
				// Constructors
				//
				public CustomTimeLine ()
				{

				}
		
				//
				// Methods
				//
				
				/// <summary>
				/// Gets the index of the mouse hover rect.
				/// </summary>
				/// <returns>The mouse hover rect index.</returns>
				/// <param name="postionRect">Postion rect.</param>
				/// <param name="values">Values.</param>
				/// <param name="hitRects">Hit rects.</param>
				/// <param name="controlID">Control I.</param>
				private int GetMouseHoverRectIndex (Rect postionRect, float[] values, Rect[] hitRects, int controlID)
				{
						Vector2 mousePosition = Event.current.mousePosition;
						bool flag = false;
						if (values.Length == hitRects.Length) {
								for (int i = hitRects.Length - 1; i >= 0; i--) {
										if (hitRects [i].Contains (mousePosition)) {
												if (Event.current.button == 1 && Event.current.isMouse) {
														Debug.Log ("Right click inside hitRect" + hitRects [i] + " " + mousePosition);
														Event.current.Use ();
														
														onContextClickOnTimeValue (new TimeLineEventArgs<float> (i, values [i], values, controlID));
														return -1;
												}


												return i;
										}
								}
						}
						
						return -1;
				}


				/// <summary>
				/// Deletes the events.
				/// </summary>
				/// <param name="args">Arguments.</param>
				/// <param name="deleteIndices">Delete indices.</param>
				private void DeleteEvents (TimeLineEventArgs<float> args, bool[] deleteIndices)
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
								

								if (this.Delete != null)
										this.Delete (this, new TimeLineEventArgs<float> (-1, 0f, (float[])CHANGED_VALUES, args.controlID));
						}
				}

				/// <summary>
				/// Deselects all.
				/// </summary>
				public void DeselectAll ()
				{
						this.__valuesSelected = null;
				}
		
			
				/// <summary>
				/// Ons the add.
				/// </summary>
				/// <param name="obj">Object.</param>
				public virtual void onAdd (System.Object obj)
				{
						//Debug.Log ("onAdd");
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;
					
						//TODO Create UNDO
						//Undo.RegisterCompleteObjectUndo (this, "Add Event");

						int newTimeValueInx = args.values.Count;

						//find first time > then current and insert before it
						for (int i = 0; i < newTimeValueInx; i++) {
								if (args.values [i] > args.selectedValue) {
										newTimeValueInx = i;
										break;
								}
						}

						float[] timeValues = new float[args.values.Count];
						args.values.CopyTo (timeValues, 0);

						

						ArrayUtility.Insert<float> (ref timeValues, newTimeValueInx, args.selectedValue);

						
						
						CONTROL_ID = args.controlID;
						CHANGED_VALUES = timeValues;



						this.Select (newTimeValueInx, timeValues.Length);

						
						//open editor for newely added 
						if (Add != null)
								Add (this, new TimeLineEventArgs<float> (newTimeValueInx, args.selectedValue, timeValues, args.controlID));
				}

				/// <summary>
				/// Ons the delete.
				/// </summary>
				/// <param name="obj">Object.</param>
				public virtual void onDelete (System.Object obj)
				{
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;

						int index = args.selectedIndex;
						if (this.__valuesSelected [index]) {
								this.DeleteEvents (args, this.__valuesSelected);
						} else {
								bool[] timeValuesSelected = new bool[this.__valuesSelected.Length];
								timeValuesSelected [index] = true;
								this.DeleteEvents (args, timeValuesSelected);
						}


				}

				/// <summary>
				/// Ons the edit.
				/// </summary>
				/// <param name="obj">Object.</param>
				public virtual void onEdit (System.Object obj)
				{
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;

						if (EditOpen != null) {
								EditOpen (this, args);
						}

						this.Select (args.selectedIndex, args.values.Count);
				}


				/// <summary>
				/// Ons the context click on time value.
				/// </summary>
				/// <param name="args">Arguments.</param>
				public virtual void onContextClickOnTimeValue (TimeLineEventArgs<float> args)
				{

						GenericMenu genericMenu = new GenericMenu ();
						genericMenu.AddItem (new GUIContent ("Edit"), false, new GenericMenu.MenuFunction2 (this.onEdit), args);
						genericMenu.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
						genericMenu.AddItem (new GUIContent ("Delete"), false, new GenericMenu.MenuFunction2 (this.onDelete), args);
						genericMenu.ShowAsContext ();

				}


				/// <summary>
				/// Ons the context click.
				/// </summary>
				/// <param name="args">Arguments.</param>
				public virtual void onContextClick (TimeLineEventArgs<float> args)
				{
						//Debug.Log ("ContextClick on empty");
						Event.current.Use ();
						GenericMenu genericMenu2 = new GenericMenu ();
						genericMenu2.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
						genericMenu2.ShowAsContext ();
				}
				
		
				/// <summary>
				/// Ons the time line GU.
				/// </summary>
				/// <param name="timeValues">Time values.</param>
				/// <param name="displayNames">Display names.</param>
				public void onTimeLineGUI (ref float[] timeValues, ref string[] displayNames)
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
						int timeValuesNumberOfTheSame = 0;//items that have same time
						int timeValuesTheSameCounterDown = 0;//same time items count down

						//mulitiplier simple changes the y position of the timeValue handle so
						//same timeValues's hanldes are on of top of another

						float[] timeValuesTheSameHightMultiply = new float[timeValuesNumber]; 
						float timeValue;
						int i = 0;
						int fromToEndInx = 0;

						float timeValuePositionX = 0f;

					

						
						for (i = 0; i < timeValuesNumber; i++) {

								
								timeValue = timeValues [i];
								timeValuePositionX = timeValue * rectLocal.width;
								timeValuesNumberOfTheSame = 0;

								//version 1 display one tube no visible separtion od handles
//								if (timeValuesTheSameHightMultiply [i] == 0) {
//										//find other with same value and record multiply (1x,2x,...)
//										for (fromToEndInx=i+1; fromToEndInx < timeValuesNumber; fromToEndInx++) {
//												if (timeValues [fromToEndInx] == timeValue) {
//														timeValuesNumberOfTheSame++;
//														timeValuesTheSameHightMultiply [fromToEndInx] = timeValuesNumberOfTheSame;
//												}
//										
//										}
//								}

									

				//version 2 display has visible separtion od handles
												if (timeValuesTheSameHightMultiply [i] == 0) {
														//find other with same value and record multiply (1x,2x,...)
														for (fromToEndInx=timeValuesNumber-1; fromToEndInx > i; fromToEndInx--) {
																if (timeValues [fromToEndInx] == timeValue) {
																		
																		timeValuesTheSameHightMultiply [fromToEndInx] = timeValuesNumberOfTheSame;
																		timeValuesNumberOfTheSame++;
																}
														
														}

													timeValuesTheSameHightMultiply [i]=timeValuesNumberOfTheSame;
												}
							

								

							
								

								
								Rect rect3 = new Rect (timeValuePositionX, image.height * timeValuesTheSameHightMultiply [i] * 0.66f, (float)image.width, (float)image.height);


								positionsHitRectArray [i] = rect3;
								positionsRectArray [i] = rect3;
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
														EditOpen (this, new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
												
												}
											
										} else {
												//never enters here
												this.onAdd (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, controlID));
												
										}
										break;
								
								case HighLevelEvent.ContextClick:
										{
												SELECTED_INDEX = clickedIndex;
												//Debug.Log ("ContextClick on handle");
												onContextClickOnTimeValue (new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
											
														
												break;
										}
					      
								case HighLevelEvent.BeginDrag:
										
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

												
												//TODO CREATE UNDO
												//Undo.RegisterCompleteObjectUndo (this, "Move Event");
					
					
												
												break;
										}
								case HighLevelEvent.Delete:
										this.DeleteEvents (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, controlID), this.__valuesSelected);
										break;
								case HighLevelEvent.SelectionChanged:
						
										if (clickedIndex != -1) {
												if (EditOpen != null) {
														EditOpen (this, new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
														
												}


										}
										break;
								}
						}


						int hoverInx = -1; 
						hoverInx = this.GetMouseHoverRectIndex (rectGlobal, timeValues, positionsHitRectArray, controlID);					



					
						if (Event.current.type == EventType.ContextClick && rectLocal.Contains (Event.current.mousePosition) 
								|| (Event.current.button == 1 && Event.current.isMouse)) {

								onContextClick (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, controlID));


			

						}
				
						GUI.color = color;
		
						GUI.EndGroup ();

						//show tooltip on hover
						if (hoverInx >= 0 && hoverInx < positionsHitRectArray.Length) {


								Rect positionRect = positionsRectArray [hoverInx];

								//from local to global
								positionRect.y += rectGlobal.y;
								positionRect.x += rectGlobal.x;


				
								EditorGUILayoutEx.CustomTooltip (positionRect, displayNames [hoverInx] + "[" + timeValues [hoverInx] + "]");
				
						}


					
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