using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ws.winx.unity;
using ws.winx.bmachine.extensions;
using ws.winx.csharp.extensions;

namespace ws.winx.editor.extensions
{
		public class TimeLineEventArgs<T>:EventArgs
		{
				
				
				int _selectedIndex;
				IList<T> _values;
				T _selectedValue;
				int _controlID;
				
				public int controlID{ get { return _controlID; } }
				
				public T selectedValue{ get { return _selectedValue; } }
				
				public IList<T> values{ get { return _values; } }
				
				public int selectedIndex {
						get{ return _selectedIndex;}
				}
		
				public TimeLineEventArgs (int selectedIndex, T selectedValue, IList<T> values, int controlID)
				{
						this._selectedValue = selectedValue;
						this._values = values;
						this._controlID = controlID;
						this._selectedIndex = selectedIndex;
				}
		}

//		[Serializable]
//		public class CustomObjectTimeLine<T> where T:IComparable,ICloneable
//		{
//
//				private static int CONTROL_ID = -1;
//				private static int SELECTED_INDEX = -1;
//				private static IList<IEditorItem<T>> CHANGED_VALUES = null;

//
//
//
//
//				//
//				// Events
//				//
//				public event EventHandler<TimeLineEventArgs<IEditorItem<T>>> EditOpen;
//				public event EventHandler<TimeLineEventArgs<IEditorItem<T>>> EditClose;
//				public event EventHandler<TimeLineEventArgs<IEditorItem<T>>> Delete;
//				public event EventHandler<TimeLineEventArgs<IEditorItem<T>>> Add;
//		        
//
//
//
//
//				//
//				// Fields
//				//
//				
//				private static Texture image = EditorGUIUtility.IconContent ("Animation.EventMarker").image;
//				private bool[] __valuesSelected;
//				[NonSerialized]
//				private IList<IEditorItem<T>>
//						timeValuesTime;
//				
//		
//
//
//				//
//				// Constructors
//				//
//				public CustomObjectTimeLine ()
//				{
//
//				}
//		
//				//
//				// Methods
//				//
//				
//				/// <summary>
//				/// Gets the index of the mouse hover rect.
//				/// </summary>
//				/// <returns>The mouse hover rect index.</returns>
//				/// <param name="postionRect">Postion rect.</param>
//				/// <param name="values">Values.</param>
//				/// <param name="hitRects">Hit rects.</param>
//				/// <param name="controlID">Control I.</param>
//				private int GetMouseHoverRectIndex (Rect postionRect, ref IList<IEditorItem<T>> values, Rect[] hitRects, int controlID)
//				{
//						Vector2 mousePosition = Event.current.mousePosition;
//						bool flag = false;
//						if (values.Count == hitRects.Length) {
//								for (int i = hitRects.Length - 1; i >= 0; i--) {
//										if (hitRects [i].Contains (mousePosition)) {
//												if (Event.current.button == 1 && Event.current.isMouse) {
//														Debug.Log ("Right click inside hitRect" + hitRects [i] + " " + mousePosition);
//														Event.current.Use ();
//														
//														onContextClickOnTimeValue (new TimeLineEventArgs<IEditorItem<T>> (i, values [i], values, controlID));
//														return -1;
//												}
//
//
//												return i;
//										}
//								}
//						}
//						
//						return -1;
//				}
//
//
//				/// <summary>
//				/// Deletes the events.
//				/// </summary>
//				/// <param name="args">Arguments.</param>
//				/// <param name="deleteIndices">Delete indices.</param>
//				private void DeleteEvents (TimeLineEventArgs<IEditorItem<T>> args, bool[] deleteIndices)
//				{
//						bool deletionHappend = false;
//						List<IEditorItem<T>> list = new List<IEditorItem<T>> (args.values);
//
//						
//						for (int i = list.Count  - 1; i >= 0; i--) {
//		
//								if (deleteIndices [i]) {
//										list.RemoveAt (i);
//										deletionHappend = true;
//								}
//						}
//
//						if (deletionHappend) {
//								
//								if (this.EditClose != null)
//										this.EditClose (this, null);
//
//								//TODO CREATE UNDOS
//								//Undo.RegisterCompleteObjectUndo (this, "Delete Event");
//
//								CHANGED_VALUES = list.ToArray ();
//								CONTROL_ID = args.controlID;
//								
//								
//
//								this.__valuesSelected = new bool[list.Count];
//								
//
//								if (this.Delete != null)
//										this.Delete (this, new TimeLineEventArgs<IEditorItem<T>> (-1, null, CHANGED_VALUES, args.controlID));
//						}
//				}
//
//				/// <summary>
//				/// Deselects all.
//				/// </summary>
//				public void DeselectAll ()
//				{
//						this.__valuesSelected = null;
//				}
//		
//			
//				/// <summary>
//				/// Ons the add.
//				/// </summary>
//				/// <param name="obj">Object.</param>
//				public virtual void onAdd (System.Object obj)
//				{
//						//Debug.Log ("onAdd");
//						TimeLineEventArgs<IEditorItem<T>> args = (TimeLineEventArgs<IEditorItem<T>>)obj;
//					
//						//TODO Create UNDO
//						//Undo.RegisterCompleteObjectUndo (this, "Add Event");
//
//						int newTimeValueInx = args.values.Count;
//
//
//
//						//find first time > then current and insert before it
//						for (int i = 0; i < newTimeValueInx; i++) {
//								//if (args.values [i].EditorItemValue > args.selectedValue.EditorItemValue) {
//								if (args.values [i].EditorItemValue.CompareTo (args.selectedValue.EditorItemValue) > 0) {
//										newTimeValueInx = i;
//										break;
//								}
//						}
//
//					IEditorItem<T>[] timeValues=new IEditorItem<T>[args.values.Count];
//
//			args.values.CopyTo (timeValues,0);
//
//
//
//						ArrayUtility.Insert<IEditorItem<T>> (ref timeValues, newTimeValueInx, args.selectedValue);
//
//						
//						
//						CONTROL_ID = args.controlID;
//						CHANGED_VALUES = timeValues;
//
//
//
//						this.Select (newTimeValueInx, timeValues.Length);
//
//						
//						//open editor for newely added 
//						if (Add != null)
//								Add (this, new TimeLineEventArgs<IEditorItem<T>> (newTimeValueInx, args.selectedValue, timeValues, args.controlID));
//				}
//
//				/// <summary>
//				/// Ons the delete.
//				/// </summary>
//				/// <param name="obj">Object.</param>
//				public virtual void onDelete (System.Object obj)
//				{
//						TimeLineEventArgs<IEditorItem<T>> args = (TimeLineEventArgs<IEditorItem<T>>)obj;
//
//						int index = args.selectedIndex;
//						if (this.__valuesSelected [index]) {
//								this.DeleteEvents (args, this.__valuesSelected);
//						} else {
//								bool[] timeValuesSelected = new bool[this.__valuesSelected.Length];
//								timeValuesSelected [index] = true;
//								this.DeleteEvents (args, timeValuesSelected);
//						}
//
//
//				}
//
//				/// <summary>
//				/// Ons the edit.
//				/// </summary>
//				/// <param name="obj">Object.</param>
//				public virtual void onEdit (System.Object obj)
//				{
//						TimeLineEventArgs<IEditorItem<T>> args = (TimeLineEventArgs<IEditorItem<T>>)obj;
//
//						if (EditOpen != null)
//								EditOpen (this, args);
//
//						this.Select (args.selectedIndex, args.values.Count);
//				}
//
//
//				/// <summary>
//				/// Ons the context click on time value.
//				/// </summary>
//				/// <param name="args">Arguments.</param>
//				public virtual void onContextClickOnTimeValue (TimeLineEventArgs<IEditorItem<T>> args)
//				{
//
//						GenericMenu genericMenu = new GenericMenu ();
//						genericMenu.AddItem (new GUIContent ("Edit"), false, new GenericMenu.MenuFunction2 (this.onEdit), args);
//						genericMenu.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
//						genericMenu.AddItem (new GUIContent ("Delete"), false, new GenericMenu.MenuFunction2 (this.onDelete), args);
//						genericMenu.ShowAsContext ();
//
//				}
//
//
//				/// <summary>
//				/// Ons the context click.
//				/// </summary>
//				/// <param name="args">Arguments.</param>
//				public virtual void onContextClick (TimeLineEventArgs<IEditorItem<T>> args)
//				{
//						//Debug.Log ("ContextClick on empty");
//						Event.current.Use ();
//						GenericMenu genericMenu2 = new GenericMenu ();
//						genericMenu2.AddItem (new GUIContent ("Add"), false, new GenericMenu.MenuFunction2 (this.onAdd), args);
//						genericMenu2.ShowAsContext ();
//				}
//				
//				public void onTimeLineGUI (IList<IEditorItem<T>> timeValues)
//				{
//						//main contorol position
//						Rect rectGlobal = GUILayoutUtility.GetLastRect ();
//						rectGlobal.y += 100;
//						rectGlobal.xMax -= 3f;
//						rectGlobal.xMin = 33f;
//						rectGlobal.height = 100f;
//
//
//						int controlID = GUIUtility.GetControlID (FocusType.Passive) + 1;
//						//Debug.Log ("Cid:" + controlID + " " + GUIUtility.hotControl);
//
//						if (controlID == CONTROL_ID) {
//								CONTROL_ID = -1;
//								timeValues = CHANGED_VALUES;
//								CHANGED_VALUES = null;
//						}
//						
//						GUI.BeginGroup (rectGlobal);
//						Color color = GUI.color;
//
//						Rect rectLocal = new Rect (0f, 0f, rectGlobal.width, rectGlobal.height);
//
//						//background
//						GUI.Box (rectLocal, GUIContent.none);
//						rectLocal.width -= image.width;
//
//		
//					
//						IEditorItem<T> timeEditorItem; 	
//						float time = 0f;
//						if (rectLocal.Contains (Event.current.mousePosition))
//								time = (float)Math.Round (Event.current.mousePosition.x / rectLocal.width, 2);
//						
//
//
//						int timeValuesNumber = timeValues.Count;
//						Rect[] positionsHitRectArray = new Rect[timeValuesNumber];
//						Rect[] positionsRectArray = new Rect[timeValuesNumber];
//						int timeValuesNumberOfTheSame = 1;//items that have same time
//						int timeValuesTheSameCounterDown = 0;//same time items count down
//
//						for (int i = 0; i < timeValuesNumber; i++) {
//								T timeValue = timeValues [i].EditorItemValue;
//
//								if (timeValuesTheSameCounterDown == 0) {
//										timeValuesNumberOfTheSame = 1;
//
//										while (i + timeValuesNumberOfTheSame < timeValuesNumber && timeValues [i + timeValuesNumberOfTheSame].EditorItemValue.CompareTo(timeValue)==0) {
//												timeValuesNumberOfTheSame++;
//										}
//						
//										//init counter to number of items with same time value
//										timeValuesTheSameCounterDown = timeValuesNumberOfTheSame;
//								}
//
//
//							
//								float timeValuePosition = Convert.ToSingle (timeValue) * rectLocal.width;
//
//
//								timeValuesTheSameCounterDown--;
//								Rect rect3 = new Rect (timeValuePosition, image.height * timeValuesTheSameCounterDown * 0.66f, (float)image.width, (float)image.height);
//
//
//								positionsHitRectArray [i] = rect3;
//								positionsRectArray [i] = rect3;
//						}
//
//
//					
//
//
//							
//
//						if (this.__valuesSelected == null || this.__valuesSelected.Length != timeValuesNumber) {
//								this.__valuesSelected = new bool[timeValuesNumber];
//
//								if (EditClose != null)
//										EditClose (this, null);
//
//								
//								
//						}
//
//
//						
//
//						Vector2 offset = Vector2.zero; 
//						int clickedIndex;
//						float startSelect;
//						float endSelect;
//						
//						HighLevelEvent highLevelEvent = EditorGUIExtW.MultiSelection (rectGlobal, positionsRectArray, new GUIContent (image), positionsHitRectArray, ref this.__valuesSelected, null, out clickedIndex, out offset, out startSelect, out endSelect, GUIStyle.none);
//
//						if (highLevelEvent != HighLevelEvent.None) {
//								switch (highLevelEvent) {
//								case HighLevelEvent.DoubleClick:
//										if (clickedIndex != -1) {
//												if (EditOpen != null) {
//														EditOpen (this, new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
//
//												}
//											
//										} else {
//												//never enters here
//
//						timeEditorItem = Activator.CreateInstance(typeof(IEditorItem<T>)) as IEditorItem<T>;
//						timeEditorItem.EditorItemValue = (T)Convert.ChangeType(time,typeof(T));
//												this.onAdd (new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, timeEditorItem, timeValues, controlID));
//												
//										}
//										break;
//								
//								case HighLevelEvent.ContextClick:
//										{
//												SELECTED_INDEX = clickedIndex;
//												//Debug.Log ("ContextClick on handle");
//												onContextClickOnTimeValue (new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
//											
//														
//												break;
//										}
//					      
//								case HighLevelEvent.BeginDrag:
//										
//										this.timeValuesTime = timeValues.Clone();
//
//												
////										for (int j = 0; j < timeValuesNumber; j++) {
////												this.timeValuesTime [j].EditorItemValue = timeValues [j].EditorItemValue;
////										}
//
//										
//
//			
//										break;
//								case HighLevelEvent.Drag:
//										{
//				
//												for (int k = timeValuesNumber - 1; k >= 0; k--) {
//														if (this.__valuesSelected [k]) {
//																	
//																timeValues [k].EditorItemValue = (float)Math.Round (this.timeValuesTime [k].EditorItemValue + offset.x / rectLocal.width, 2);
//																	
//																if (timeValues [k] > 1f)
//																		timeValues [k] = 1f;
//																else if (timeValues [k] < 0f)
//																		timeValues [k] = 0f;
//																//Debug.Log ("Dragged time" + timeValues [k]);
//														}
//												}
//
//												
//												int[] indexArray = new int[this.__valuesSelected.Length];
//												for (int l = 0; l < indexArray.Length; l++) {
//														indexArray [l] = l;
//												}
//								
//												Array.Sort (timeValues, indexArray);
//
//												bool[] cloneOfSelected = (bool[])this.__valuesSelected.Clone ();
//														
//												IList<IEditorItem<T>> cloneOfTimes = this.timeValuesTime.Clone ();
//														
//														
//
//												int inx = -1;
//												for (int m = 0; m < indexArray.Length; m++) {
//														inx = indexArray [m];
//														this.__valuesSelected [m] = cloneOfSelected [inx];
//														this.timeValuesTime [m] = cloneOfTimes [inx];
//																
//												}
//
//											
//												
//												
//												//TODO CREATE UNDO
//												//Undo.RegisterCompleteObjectUndo (this, "Move Event");
//					
//					
//												
//												break;
//										}
//								case HighLevelEvent.Delete:
//										this.DeleteEvents (new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, time, timeValues, controlID), this.__valuesSelected);
//										break;
//								case HighLevelEvent.SelectionChanged:
//						
//										if (clickedIndex != -1) {
//												if (EditOpen != null) {
//														EditOpen (this, new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, timeValues [clickedIndex], timeValues, controlID));
//													
//												}
//
//
//										}
//										break;
//								}
//						}
//
//
//						int hoverInx = this.GetMouseHoverRectIndex (rectGlobal, ref timeValues, positionsHitRectArray, controlID);					
//
//
//
//						//if (Event.current.type == EventType.ContextClick && rect2.Contains (Event.current.mousePosition) && selection.EnsureClipPresence ())
//						if (Event.current.type == EventType.ContextClick && rectLocal.Contains (Event.current.mousePosition) 
//								|| (Event.current.button == 1 && Event.current.isMouse)) {
//
//								onContextClick (new TimeLineEventArgs<IEditorItem<T>> (clickedIndex, time, timeValues, controlID));
//
//
//			
//
//						}
//				
//						GUI.color = color;
//		
//						GUI.EndGroup ();
//
//						//show tooltip on hover
//						if (hoverInx >= 0 && hoverInx < positionsHitRectArray.Length) {
//
//
//								Rect positionRect = positionsRectArray [hoverInx];
//
//								//from local to global
//								positionRect.y += rectGlobal.y;
//								positionRect.x += rectGlobal.x;
//
//
//				
//								EditorGUILayoutEx.CustomTooltip (positionRect, timeValues [hoverInx].EditorItemLabel + "[" + timeValues [hoverInx] + "]");
//				
//						}
//
//
//						return timeValues; 
//				}
//		
//
//				/// <summary>
//				/// Set selection flag to true to timeValue with index
//				/// into maximum selectable values
//				/// </summary>
//				/// <param name="index">Index.</param>
//				/// <param name="maxItemsSelectable">Max items selectable.</param>
//				private void Select (int index, int timeValuesSelectableMax)
//				{
//						this.__valuesSelected = new bool[timeValuesSelectableMax];
//						this.__valuesSelected [index] = true;
//				}
//		
//				
//
//
//			
//		
//				
//		}
}