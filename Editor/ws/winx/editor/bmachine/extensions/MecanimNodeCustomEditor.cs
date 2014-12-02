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

namespace ws.winx.editor.bmachine.extensions
{
	
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(MecanimNode), true)]
		public class MecanimNodeCustomEditor : NodeEditor
		{

				

			
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

				//avatarPreviewTimeControl button
				GUIStyle playButtonStyle;
				Vector2 playButtonSize;
				AvatarPreviewW avatarPreview;

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



				void OnDisable ()
				{

						UnityEngine.Debug.Log ("Disable");
						//OnPreviewDisable();
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

	
          

				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						DrawDefaultInspector ();
			
						mecanimNode = target as MecanimNode;

						Motion motion = mecanimNode.selectedAnimaStateInfo.motion;
	
		
			
						if (mecanimNode != null) {
								// Get children nodes
								//ActionNode[] children = randomChild.children;
				
								// Update serialized node data
								if (Event.current.type == EventType.Layout) {
										this.serializedNode.Update ();
								}
				
								// Get an iterator
								//var iterator = serializedNode.GetIterator ();
				
								// Cache the indent level
								int indentLevel = EditorGUI.indentLevel;



								EditorGUILayout.LabelField("Events Timeline");
				
								if (!Application.isPlaying) {
									
										if (avatarPreview == null)
												avatarPreview = new AvatarPreviewW (null, motion);
										else
												avatarPreview.SetPreviewMotion (motion);
									
										
										
										Rect avatarRect = EditorGUILayout.BeginHorizontal ();

										avatarRect.y += 30f;
										avatarRect.height = Screen.height - avatarRect.y - 20f;

					                    
										avatarPreview.DoAvatarPreview (avatarRect, GUIStyle.none);

										EditorGUILayout.EndHorizontal ();		
										
									
								} 
								
								// Restore the indent level
								EditorGUI.indentLevel = indentLevel;
								
								// Apply modified properties
								this.serializedNode.ApplyModifiedProperties ();
								
						}




			
						if (!Application.isPlaying) {
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
								for (int i=0; i<eventTimeValuesNumber; i++) {
										ev = ((SendEventNormalized)mecanimNode.children [i]);	
										ev.timeNormalized = eventTimeValues [i];
					
										//if changes have been made in pop editor or SendEventNormailized inspector
										if (ev.name != eventDisplayNames [i])
												eventDisplayNames [i] = ((SendEventNormalized)mecanimNode.children [i]).name;
					
										int eventTimeValueSwitchInx = -1;
					
					
								}
						}
								



						//NOTES!!! I"ve gone with edit popup but I might draw Nodes here but think would move whole avatar and timeline preview down/up
						//if I draw them all or maybe just selected one(but what if many are selected ???) maybe I would draw it here as popup sucks

			

								

				}
		}
}