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
			
				private static IList CHANGED_VALUES = null;

		private static Texture __eventMarkerTexture = EditorGUIUtility.IconContent ("Animation.EventMarker").image;


				


			


				//
				// Fields
				//
				
				
			
				
		


				//
				// Constructors
				//

				
		
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
				private static int GetMouseHoverRectIndex (Rect postionRect, float[] values, Rect[] hitRects)
				{
						Vector2 mousePosition = Event.current.mousePosition;
						bool flag = false;
						if (values.Length == hitRects.Length) {
								for (int i = hitRects.Length - 1; i >= 0; i--) {
										if (hitRects [i].Contains (mousePosition)) {
											


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
				private static void DeleteEvents (TimeLineEventArgs<float> args, bool[] deleteIndices)
				{
						
						List<float> list = new List<float> (args.values);

						
						for (int i = list.Count  - 1; i >= 0; i--) {
		
								if (deleteIndices [i]) {
										list.RemoveAt (i);
										
								}
						}

						if (list.Count<args.values.Count) {
								
								if (args.EditClose != null)
										args.EditClose (args);

								
								CHANGED_VALUES = list.ToArray ();
								CONTROL_ID = args.controlID;
								
								

								

								if (args.Delete != null)
										args.Delete ( new TimeLineEventArgs<float> (-1, 0f, (float[])CHANGED_VALUES, null, args.controlID));
						}
				}

			
		
			
				/// <summary>
				/// Ons the add.
				/// </summary>
				/// <param name="obj">Object.</param>
				public static void onAdd (System.Object obj)
				{
						//Debug.Log ("onAdd");
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;
					
					
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


						
						//open editor for newely added 
						if (args.Add != null)
								args.Add (new TimeLineEventArgs<float> (newTimeValueInx, args.selectedValue, timeValues, null, args.controlID));
				}

				/// <summary>
				/// Ons the delete.
				/// </summary>
				/// <param name="obj">Object.</param>
				public static void onDelete (System.Object obj)
				{
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;

						int index = args.selectedIndex;
						if (args.selected [index]) {
								DeleteEvents (args, args.selected);
						} else {
								bool[] timeValuesSelected = new bool[args.selected.Length];
								timeValuesSelected [index] = true;
								DeleteEvents (args, timeValuesSelected);
						}


				}

				/// <summary>
				/// Ons the edit.
				/// </summary>
				/// <param name="obj">Object.</param>
				public static void onEdit (System.Object obj)
				{
						TimeLineEventArgs<float> args = (TimeLineEventArgs<float>)obj;

						if (args.EditOpen != null) {
								args.EditOpen (args);
						}

						
				}


				/// <summary>
				/// Ons the context click on time value.
				/// </summary>
				/// <param name="args">Arguments.</param>
				public static void onContextClickOnTimeValue (TimeLineEventArgs<float> args)
				{

						GenericMenu genericMenu = new GenericMenu ();
						genericMenu.AddItem (new GUIContent ("Edit"), false, new GenericMenu.MenuFunction2 (onEdit), args);
						genericMenu.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (onAdd), args);
						genericMenu.AddItem (new GUIContent ("Delete"), false, new GenericMenu.MenuFunction2 (onDelete), args);
						genericMenu.ShowAsContext ();

				}


				/// <summary>
				/// Ons the context click.
				/// </summary>
				/// <param name="args">Arguments.</param>
				public static void onContextClick (TimeLineEventArgs<float> args)
				{
						//Debug.Log ("ContextClick on empty");
						Event.current.Use ();
						GenericMenu genericMenu2 = new GenericMenu ();
						genericMenu2.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (onAdd), args);
						genericMenu2.ShowAsContext ();
				}
				
		
				/// <summary>
				/// Ons the time line GU.
				/// </summary>
				/// <param name="timeValues">Time values.</param>
				/// <param name="displayNames">Display names.</param>
			public static void onTimeLineGUI (ref float[] timeValues,ref float[] timeValuesTime, ref string[] displayNames, ref bool[] selected,
		                                  Action<TimeLineEventArgs<float>> Add=null,Action<TimeLineEventArgs<float>> Delete=null,  Action<TimeLineEventArgs<float>> EditClose=null,Action<TimeLineEventArgs<float>> EditOpen=null,Action<TimeLineEventArgs<float>> DragEnd=null
		                           )
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
						rectLocal.width -= __eventMarkerTexture.width;

		
					
							
						float time = 0f;
						if (rectLocal.Contains (Event.current.mousePosition))
								time = (float)Math.Round (Event.current.mousePosition.x / rectLocal.width, 4);
						


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

									

								//version 2 display has visible separation of handles when they have same time
								if (timeValuesTheSameHightMultiply [i] == 0) {
										//find other with same value and record multiply (1x,2x,...)
										for (fromToEndInx=timeValuesNumber-1; fromToEndInx > i; fromToEndInx--) {
												if (timeValues [fromToEndInx] == timeValue) {
																		
														timeValuesTheSameHightMultiply [fromToEndInx] = timeValuesNumberOfTheSame;
														timeValuesNumberOfTheSame++;
												}
														
										}

										timeValuesTheSameHightMultiply [i] = timeValuesNumberOfTheSame;
								}
							

								

							
								

								
								Rect rect3 = new Rect (timeValuePositionX, __eventMarkerTexture.height * timeValuesTheSameHightMultiply [i] * 0.66f, (float)__eventMarkerTexture.width, (float)__eventMarkerTexture.height);


								positionsHitRectArray [i] = rect3;
								positionsRectArray [i] = rect3;
						}


					


							

						if (selected == null || selected.Length != timeValuesNumber) {
								selected = new bool[timeValuesNumber];

								if (EditClose != null)
										EditClose (null);

								
								
						}


						

						Vector2 offset = Vector2.zero; 
						int clickedIndex;
						float startSelect;
						float endSelect;
						
						HighLevelEvent highLevelEvent = EditorGUIExtW.MultiSelection (rectGlobal, positionsRectArray, new GUIContent (__eventMarkerTexture), positionsHitRectArray, ref selected, null, out clickedIndex, out offset, out startSelect, out endSelect, GUIStyle.none);

						if (highLevelEvent != HighLevelEvent.None) {
								switch (highLevelEvent) {
								case HighLevelEvent.DoubleClick:
										if (clickedIndex != -1) {
												if (EditOpen != null) {
														EditOpen (new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, selected, controlID));
												
												}
											
										} else {
												//never enters here
												onAdd (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, selected, controlID,Add));
												
										}
										break;
								
								case HighLevelEvent.ContextClick:
										{
												
												//Debug.Log ("ContextClick on handle");
												selected = new bool[timeValuesNumber];
												selected [clickedIndex] = true;
												onContextClickOnTimeValue (new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, selected, controlID,Add,Delete,EditClose,EditOpen));
											
														
												break;
										}
					      
								case HighLevelEvent.BeginDrag:
										
										timeValuesTime =(float[]) timeValues.Clone();

//										//copy values when begin to drag	
//										for (int j = 0; j < timeValues.Length; j++) {
//												timeValuesTime [j] = timeValues [j];
//										}

										

			
										break;
								case HighLevelEvent.Drag:
										{
				
												for (int k = timeValues.Length - 1; k >= 0; k--) {
														if (selected [k]) {
																	
																timeValues [k] = (float)Math.Round (timeValuesTime [k] + offset.x / rectLocal.width, 4);
																	
																if (timeValues [k] > 1f)
																		timeValues [k] = 1f;
																else if (timeValues [k] < 0f)
																		timeValues [k] = 0f;
																//Debug.Log ("Dragged time" + timeValues [k]);
														}
												}

												
												
					
												
												break;
										}

								case HighLevelEvent.EndDrag:
					//Debug.Log("EndDrag");
										if (DragEnd != null) {
												DragEnd (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, selected, controlID));
										}

										break;
								case HighLevelEvent.Delete:
										DeleteEvents (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, selected, controlID,null,Delete), selected);
										break;
								case HighLevelEvent.SelectionChanged:
						
										if (clickedIndex != -1) {

												//	Debug.Log("SelectionChanged");

												if (EditOpen != null) {
														EditOpen (new TimeLineEventArgs<float> (clickedIndex, timeValues [clickedIndex], timeValues, selected, controlID));
														
												}


										}
										break;
								}
						}


						int hoverInx = -1; 
						hoverInx = GetMouseHoverRectIndex (rectGlobal, timeValues, positionsHitRectArray);


							if (hoverInx>0 && Event.current.button == 1 && Event.current.isMouse) {
								
								Event.current.Use ();
								selected = new bool[timeValuesNumber];
								selected [hoverInx] = true;
								onContextClickOnTimeValue (new TimeLineEventArgs<float> (hoverInx, timeValues [hoverInx], timeValues, selected, controlID,Add,Delete,EditClose,EditOpen));
								
							}



						//HighLevelEvent.ContextClick doens't raize when mouse right click so =>
						if (Event.current.type == EventType.ContextClick && rectLocal.Contains (Event.current.mousePosition) 
								|| (Event.current.button == 1 && Event.current.isMouse)) {

								
								onContextClick (new TimeLineEventArgs<float> (clickedIndex, time, timeValues, selected, controlID));


			

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
		

			
		
				


			
		
				
		}
}