using UnityEngine;
using UnityEditor;
using ws.winx.editor.utilities;

namespace ws.winx.editor.windows
{
		public class CurveEditorWindow : EditorWindow
		{

				private static CurveEditorWindow __window;

				public static CurveEditorWindow window {
					get {
						return __window;
					}
				}

				public static void ShowWindow (Rect rect, AnimationClip clip, EditorCurveBinding  binding)
				{

						
						if (__window != null) {
								__window.Close ();
								//Debug.Log ("Hide Prev");
						}

						__window = ScriptableObject.CreateInstance<CurveEditorWindow> ();//  EditorWindow.GetWindow<CurveEditorWindow> ();
						__window.__binding = binding;
						__window.__clip = clip;
						//rect = new Rect (0, 0, 100, 100);
						__window.position = rect;
						__window.ShowPopup ();

						__window.Focus ();

				}

				private  CurveEditorW  __curveEditor;
				private  EditorCurveBinding __binding;
				private  AnimationClip __clip;
				private  Rect __curveEditorRect;
				private AnimationCurve[] __curves;
				private EditorCurveBinding[] __bindings;

				void OnGUI ()
				{

						if (__binding != default(EditorCurveBinding)) {
								if (__curveEditor == null) {
				
										CurveWrapperW[] curveWrappers;

										__bindings = RotationCurveInterpolationW.RemapAnimationBindingForAddKey (__binding, __clip);
										
									
										CurveWrapperW curveWrapperNew;
										Color curveColor = Color.red;
										int curveBindingsNum = 0;
										EditorCurveBinding curveBindingCurrent;
										if (__bindings != null) {
												curveBindingsNum = __bindings.Length;
												__curves = new AnimationCurve[curveBindingsNum];
												curveWrappers = new CurveWrapperW[curveBindingsNum];
												for (int i=0; i<curveBindingsNum; i++) {
											
														curveBindingCurrent = __bindings [i];
														__curves [i] = AnimationUtility.GetEditorCurve (__clip, curveBindingCurrent);

														curveWrapperNew = new CurveWrapperW ();
														curveWrapperNew.curve = __curves [i];
														if (i == 1)
																curveColor = Color.green;
														else if (i == 2)
																curveColor = Color.blue;
														
														curveWrapperNew.color = curveColor;
														curveWrappers [i] = curveWrapperNew;
							
							
												}
										} else {
												__bindings = new EditorCurveBinding[]{__binding};
												__curves = new AnimationCurve[1];
												__curves [0] = AnimationUtility.GetEditorCurve (__clip, __binding);
												curveWrappers = new CurveWrapperW[1];

												curveWrapperNew = new CurveWrapperW ();
												curveWrapperNew.curve = __curves [0];

						
												curveWrapperNew.color = curveColor;
												curveWrappers [0] = curveWrapperNew;
						
						
										}
					
					
					
										//This makes layout to work (Reserving space)
										__curveEditorRect = new Rect (0, 0, Screen.width, Screen.height);
										//GUILayoutUtility.GetRect (Screen.width - 16f, 200);
				
										__curveEditor = new CurveEditorW (__curveEditorRect, curveWrappers, false);
				
										__curveEditor.FrameSelected (false, true);//scale y auto
										__curveEditor.scaleWithWindow = true;
										__curveEditor.hSlider = false;
										__curveEditor.vSlider = false;

										__curveEditor.hRangeMin = __curves [0].keys [0].time;
										__curveEditor.hRangeMax = __clip.length;// __curves [0].keys [__curves [0].length - 1].time;
										__curveEditor.hRangeLocked = true;
										__curveEditor.Scale = new Vector2 (__window.position.width / __clip.length, __curveEditor.Scale.y);//scale x manually
										__curveEditor.leftmargin = 0f;

										__curveEditor.rightmargin = 0;// __window.position.width - __curveEditor.hRangeMax * __clip.length / __window.position.width;
						
							
										//__curveEditor.onSelect += onCurveSelect;
				
				
				
				
								} else {
										__curveEditorRect = new Rect (0, 0, Screen.width, Screen.height);
										__curveEditor.rect = __curveEditorRect;
										//__curveEditor.Scale=new Vector2(237,__curveEditor.Scale.y);
										//__curveEditor.FrameSelected (false, false);
				
								}


								//if(Event.current.type==EventType.MouseMove || Event.current.type==EventType.ScrollWheel && !__window.position.Contains(Event.current.mousePosition))
								//this.Close();
			
								//EditorGUI.BeginChangeCheck ();

//								if (Event.current.type == EventType.MouseMove && !__window.position.Contains (Event.current.mousePosition)) {
//										this.Close ();
//								}

								if (Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
									
										//if change happen change curve
										for (int k=0; k<__bindings.Length; k++) {
												AnimationModeUtility.SaveCurve (__curves [k], __clip, __bindings [k]);
												//Debug.Log ("Save");
										}
								}
			
								__curveEditor.DoEditor ();


							//	Debug.Log ("CurveEditorWindow:"+__curveEditor.Scale + " " + __window.position.width + " " + Event.current.type);			

								//if (EditorGUI.EndChangeCheck ()) {

										
								//}

						}
				}

				void OnLostFocus ()
				{
						this.Close ();
						__window = null;

						//Debug.Log ("Hide");

				}
		}
}