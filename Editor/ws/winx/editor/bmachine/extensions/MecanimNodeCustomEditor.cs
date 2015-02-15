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

				CurveProperty curvePropertySelected;

		UnityEngine.Object objectSelected;
		GUIContent propertyPopupLabel = new GUIContent (String.Empty);
		Property propertySelected;
				string[] curvePropertyDisplayOptions;

		MethodInfo GetFloatVar_MethodInfo;

		int curvePropertyIndexSelected;
				Color colorSelected;
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


				void onCurveSelect(int index){

					if(mecanimNode.curveProperties!=null)
						curvePropertySelected = mecanimNode.curveProperties [index];

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


				CurveProperty propertyNew=null;


				//_curvesEditorShow=EditorGUILayout.Foldout(_curvesEditorShow,"Curves");
				//EditorGUILayout.CurveField
				int indentLevel=0;
				Rect curveEditorRect;
				//Rect curveEditorRect=GUILayoutUtility.GetLastRect();




			

				
				if(true){
					//Debug.Log("LAst"+GUILayoutUtility.GetLastRect()+" "+GUILayoutUtility.GetRect(100,200));

					curveEditorRect=GUILayoutUtility.GetRect(Screen.width-46f,200);

//					if(curveEditorRect.height<200) curveEditorRect.height=200;
//					curveEditorRect.width=Screen.width-46;
//					curveEditorRect.y=400;
					//GUILayout.BeginArea(curveEditorRect);
					//curveEditorRect=



						EditorGUILayout.BeginHorizontal();

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

					indentLevel=EditorGUI.indentLevel;
						EditorGUI.indentLevel=0;



					/////// CURVE EDITOR ////////
					curveEditorRect.width=curveEditorRect.width-200;

//					if(curveEditor==null){
//
//						curveEditor=new CurveEditorW(curveEditorRect,new AnimationCurve[]{curve1,curve2},false);
//					
//						curveEditor.FrameSelected (true, true);
//						curveEditor.scaleWithWindow=true;
//
//						curveEditor.onSelect+=onCurveSelect;
//
//					}
//						else {
//
//						curveEditor.rect=curveEditorRect;
//						curveEditor.FrameSelected (false, false);
//					}
//
//
//					EditorGUILayout.BeginVertical(new GUILayoutOption[] {GUILayout.Width(200)});
//					curveEditor.DoEditor();
//					EditorGUILayout.EndVertical();




//					curvePropertiesScroller.x=curveEditorRect.x;
//					curvePropertiesScroller.y=curveEditorRect.y;
//
//					curvePropertiesScroller=EditorGUILayout.BeginScrollView(curvePropertiesScroller);
//							EditorGUILayout.BeginHorizontal();
//					Color color=new Color();
//					color=EditorGUILayout.ColorField(color);
//
//							EditorGUILayout.EndHorizontal();
//						EditorGUILayout.EndScrollView();



//					curvePropertySelected=(CurveProperty)ScriptableObject.CreateInstance<CurveProperty>();
//
//					if(curvePropertySelected!=null) Debug.Log ("why");
//					return;
//
					GUIContent[] displayOptions=null;
					CurveProperty[] values=null;


					//select object which properties would be extracted
					objectSelected=EditorGUILayout.ObjectField(objectSelected,typeof(UnityEngine.Object),true);

			
						
					if(objectSelected!=null){//extract properties
						propertyPopupLabel.text="Select ["+objectSelected.name+"] property";
						ws.winx.unity.Utility.ObjectToDisplayOptionsValues<float,CurveProperty>(objectSelected,out displayOptions,out values);
					}else{//use Global and Local blackboard
						propertyPopupLabel.text="Select blackboard variable";

					FloatVar[] floatVarsGlobal=GlobalBlackboard.Instance.floatVars;

					int floatVarsLengthGlobal=floatVarsGlobal.Length;

					FloatVar[] floatVarLocal=mecanimNode.blackboard.floatVars;

					int floatVarsLengthLocal=floatVarLocal.Length;
					int floatVarsMax=Mathf.Max(floatVarsLengthGlobal,floatVarsLengthLocal);

						displayOptions=new GUIContent[floatVarsLengthLocal+floatVarsLengthGlobal];


						values=new CurveProperty[floatVarsLengthLocal+floatVarsLengthGlobal];


					

					if(GetFloatVar_MethodInfo==null)
					GetFloatVar_MethodInfo = mecanimNode.blackboard.GetType ().GetMethod ("GetFloatVar", new Type[]{typeof(string)});

					int index=i;
					for(;i<floatVarsMax;i++){
						if(i<floatVarsLengthGlobal){

								displayOptions[index]=new GUIContent("Global/"+floatVarsGlobal[i].name);
								propertyNew=(CurveProperty)ScriptableObject.CreateInstance<CurveProperty>();

								values[index]=propertyNew;
								propertyNew.memberInfo=GetFloatVar_MethodInfo;
								propertyNew.reflectedInstance=GlobalBlackboard.Instance;
								values[index].name=floatVarsGlobal[i].name;


							index++;
						}

						if(i<floatVarsLengthLocal){
							propertyNew=(CurveProperty)ScriptableObject.CreateInstance<CurveProperty>();
							displayOptions[index]=new GUIContent("Local/"+floatVarLocal[i].name);
							values[index]=propertyNew;
								propertyNew.memberInfo=GetFloatVar_MethodInfo;
								propertyNew.reflectedInstance=mecanimNode.blackboard;
							values[index].name=floatVarLocal[i].name;
							index++;
						}
						
					}
					}


					//if(displayOptions!=null && values!=null && displayOptions.Length==values.Length){
					//propertySelected=EditorGUILayoutEx.CustomObjectPopup<Property>(propertyPopupLabel,propertySelected,displayOptions,values);


//					curvePropertySelected=EditorGUILayoutEx.CustomObjectPopup<CurveProperty>(propertyPopupLabel,curvePropertySelected,displayOptions,values);

					colorSelected=EditorGUILayout.ColorField(colorSelected);

					//if(mecanimNode.mile!=null)
					//Debug.Log("Mecanode"+mecanimNode.mile);



					if(GUILayout.Button("Add") && propertySelected!=null){

						//mecanimNode.mile=propertySelected;





						CurveProperty newCurveProperty=(CurveProperty)ScriptableObject.CreateInstance<CurveProperty>();
								//mecanimNode.curveProperties.Add(newCurveProperty);


								//can be some preset
								AnimationCurve curve1=new AnimationCurve();
							
														curve1.AddKey(new Keyframe(0, 1));
														curve1.AddKey(new Keyframe(1, 1));

								
								newCurveProperty.id=("Curve"+mecanimNode.curveProperties.Length).GetHashCode();
								newCurveProperty.color=colorSelected;
								
								curvePropertySelected=newCurveProperty;



								Array.Resize(ref mecanimNode.curveProperties,mecanimNode.curveProperties.Length+1);

								mecanimNode.curveProperties[mecanimNode.curveProperties.Length-1]=curvePropertySelected;


						this.serializedNode.ApplyModifiedProperties();

					}








					EditorGUILayout.EndHorizontal();


					//GUILayout.EndArea();

					EditorGUI.indentLevel=indentLevel;
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

										avatarRect.y=curveEditorRect.yMax;

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