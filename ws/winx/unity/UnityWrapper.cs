using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.unity;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace ws.winx.unity
{
		public enum HighLevelEvent
		{
				None,
				Click,
				DoubleClick,
				ContextClick,
				BeginDrag,
				Drag,
				EndDrag,
				Delete,
				SelectionChanged
		}



	#region EditorGUIExtW
		public class EditorGUIExtW
		{
		#region Reflection
		
				private static Type __RealType;
				private static MethodInfo MethodInfo_MultiSelection;

				public static void InitType ()
				{
			
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.EditorGUIExt");
				
								MethodInfo_MultiSelection = __RealType.GetMethod ("MultiSelection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
				
						}
				}
		
		#endregion
		
		#region Wrapper
				public static HighLevelEvent MultiSelection (Rect rect, Rect[] positions, GUIContent content, Rect[] hitPositions, ref bool[] selections, bool[] readOnly, out int clickedIndex, out Vector2 offset, out float startSelect, out float endSelect, GUIStyle style)
				{
						InitType ();
						float startSelectTemp = 0f;
						float endSelectTemp = 0f;
						int clickedIndexTemp = 0;

		
						Vector2 offsetTemp = new Vector2 ();


						object[] arguments = new object[] {
								rect,
								positions,
								content,
								hitPositions,
								selections,
								readOnly,
								clickedIndexTemp,
								offsetTemp,
								startSelectTemp,
								endSelectTemp,
								style
						};
						object result = null;

						result = MethodInfo_MultiSelection.Invoke (null, arguments);
			

						int HighLevelEventEnumInt = (int)result;
		

						clickedIndex = (int)arguments [6];
						offset = (Vector2)arguments [7];
						startSelect = (float)arguments [8];
						endSelect = (float)arguments [9];

						return (ws.winx.unity.HighLevelEvent)HighLevelEventEnumInt;
		
				}

		#endregion
		
		}
	#endregion


	#region AvatarPreviewW

		public class AvatarPreviewW
		{
	#region Reflection
	
				private static Type __RealType;
				private static MethodInfo MethodInfo_DoPreviewSettings;
				private static MethodInfo MethodInfo_OnDestroy;
				private static MethodInfo MethodInfo_DoAvatarPreview;
				private static MethodInfo MethodInfo_DoRenderPreview;
				private static MethodInfo MethodInfo_Init;
				private static MethodInfo MethodInfo_AvatarTimeControlGUI;
				private static FieldInfo FieldInfo_timeControl;
				private static ConstructorInfo method_ctor;
				private static PropertyInfo PropertyInfo_OnAvatarChangeFunc;
				private static PropertyInfo PropertyInfo_IKOnFeet;
				private static PropertyInfo PropertyInfo_Animator;
				Texture image = null;
				Rect lastRect;
				AnimatorController _previewAnimatorController;
				UnityEditorInternal.StateMachine _previewStateMachine;
				State _previewState;
				bool PrevIKOnFeet;
				Motion _previewedMotion;
	
				public static void InitType ()
				{

						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.AvatarPreview");
			
								method_ctor = __RealType.GetConstructor (new Type[] {
										typeof(Animator),
										typeof(Motion)
								});
								PropertyInfo_OnAvatarChangeFunc = __RealType.GetProperty ("OnAvatarChangeFunc");
								PropertyInfo_IKOnFeet = __RealType.GetProperty ("IKOnFeet");
								PropertyInfo_Animator = __RealType.GetProperty ("Animator");
								MethodInfo_DoPreviewSettings = __RealType.GetMethod ("DoPreviewSettings");
								MethodInfo_OnDestroy = __RealType.GetMethod ("OnDestroy");
								MethodInfo_DoAvatarPreview = __RealType.GetMethod ("DoAvatarPreview", new Type[] {
										typeof(Rect),
										typeof(GUIStyle)
								});
								FieldInfo_timeControl = __RealType.GetField ("timeControl");
								MethodInfo_DoRenderPreview = __RealType.GetMethod ("DoRenderPreview", new Type[] {
										typeof(Rect),
										typeof(GUIStyle)
								});
								MethodInfo_Init = __RealType.GetMethod ("Init", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
								MethodInfo_AvatarTimeControlGUI = __RealType.GetMethod ("AvatarTimeControlGUI", new Type[]{typeof(Rect)});
			
						}
				}
	
	#endregion
	
	#region Wrapper
	
	
				private object __instance;
				protected TimeControlW _timeControl;

	
				public delegate void OnAvatarChange ();
	

				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.unity.AvatarPreviewW"/> class.
				/// </summary>
				/// <param name="previewObjectInScene">Preview object in scene.</param>
				/// <param name="objectOnSameAsset">Object on same asset.</param>
				public AvatarPreviewW (Animator previewObjectInScene, Motion objectOnSameAsset)
				{
						InitType ();
			
						__instance = method_ctor.Invoke (new object[] {
								previewObjectInScene,
								objectOnSameAsset
						});




						_previewedMotion = objectOnSameAsset;

						CreatePreviewStateMachine (_previewedMotion);
			
				}

				/// <summary>
				/// Sets the preview motion.
				/// </summary>
				/// <param name="motion">Motion.</param>
				public void SetPreviewMotion (Motion motion)
				{
						//if the same motion then return;
						if (_previewedMotion == motion && motion != null)
								return;
				

						_previewedMotion = motion;

						_previewState.SetMotion (motion);
						CreateParameters (motion);
						this.timeControl.currentTime = 0f;

						this.Animator.Update (0f);

				}


				/// <summary>
				/// Creates the preview state machine.
				/// </summary>
				/// <param name="motion">Motion.</param>
				private void CreatePreviewStateMachine (Motion motion)
				{
						if (this.Animator == null)
								return;
			
						//Debug.Log ("CreateStateMachine");
			
						if (_previewAnimatorController == null) {
								_previewAnimatorController = new AnimatorController ();
								_previewAnimatorController.AddLayer ("previewLayer");
								_previewAnimatorController.hideFlags = HideFlags.DontSave;
				
								_previewStateMachine = _previewAnimatorController.GetLayer (0).stateMachine;
								CreateParameters (motion);
				
								_previewState = _previewStateMachine.AddState ("previewState");
				
								_previewState.SetMotion (motion);
								_previewState.iKOnFeet = this.IKOnFeet;
								_previewState.hideFlags = HideFlags.DontSave;
				
				
								AnimatorController.SetAnimatorController (this.Animator, _previewAnimatorController);
								//Debug.Log ("Setting avatarPreview.Animator " + this.Animator.name + " to temp controller");
						} 
			
			
			
//			if (AnimatorController.GetEffectiveAnimatorController (this.Animator) != this._previewAnimatorController) {
//				AnimatorController.SetAnimatorController (this.Animator, this._previewAnimatorController);
//				
//				Debug.Log ("Getting Effective Animator and set avatarPreview.Animator " + this.Animator.name + " to temp controller");
//			}
				}


				/// <summary>
				/// Clears the state machine.
				/// </summary>
				private void ClearPreviewStateMachine ()
				{
						if (this.Animator != null) {
								AnimatorController.SetAnimatorController (this.Animator, null);
						}
						UnityEngine.Object.DestroyImmediate (this._previewAnimatorController);
						UnityEngine.Object.DestroyImmediate (this._previewStateMachine);
						UnityEngine.Object.DestroyImmediate (this._previewState);
						_previewStateMachine = null;
						_previewAnimatorController = null;
						_previewState = null;
				}
		

		
		
				/// <summary>
				/// Creates the parameters.
				/// </summary>
				/// <param name="motion">Motion.</param>
				private void CreateParameters (Motion motion)
				{
//			int parameterCount = _previewAnimatorController.parameterCount;
//			for (int i = 0; i < parameterCount; i++) {
//				_previewAnimatorController.RemoveParameter (0);
//			}
			
						if (motion is BlendTree) {
								BlendTree blendTree = motion as BlendTree;
				
								for (int j = 0; j < blendTree.GetRecursiveBlendParamCount(); j++) {
										_previewAnimatorController.AddParameter (blendTree.GetRecursiveBlendParam (j), AnimatorControllerParameterType.Float);
								}
						}
			
			
				}
	
				private void UpdateMotion (Motion motion)
				{
						if (Event.current.type != EventType.Repaint) {
								return;
						}
			
						//Debug.Log ("UpdateAvatarState");
			

						if (this.Animator) {
//								if (PrevIKOnFeet != IKOnFeet)
//								{
//									PrevIKOnFeet =IKOnFeet;
//
//									//save root pos and rot
//									Vector3 rootPosition = this.Animator.rootPosition;
//									Quaternion rootRotation = this.Animator.rootRotation;
//				
//									ClearPreviewStateMachine();
//									CreatePreviewStateMachine(motion);
//									
//									this.Animator.Update(this.timeControl.currentTime);
//									this.Animator.Update(0f);
//
//									//restore root pos and rot
//									this.Animator.rootPosition = rootPosition;
//									this.Animator.rootRotation = rootRotation;
//								}
				
								this.timeControl.loop = true;
								float timeAnimationLength = 1f;
								float timeNormalized = 0f;


								if (this.Animator.layerCount > 0) {
					
										AnimatorStateInfo currentAnimatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo (0);
										timeAnimationLength = currentAnimatorStateInfo.length;


										timeNormalized = currentAnimatorStateInfo.normalizedTime;
								}
				
								this.timeControl.startTime = 0f;
								this.timeControl.stopTime = timeAnimationLength;
								this.timeControl.Update ();


			


								//deltaTime is nextCurrentTime-currentTime
								//is set my drag of red Timeline handle or manually thru SetTimeValue
								float timeDelta = this.timeControl.deltaTime;



								if (this.timeControl.playing) {
										if (!motion.isLooping) {
												if (timeNormalized >= 1f) {
														timeDelta -= timeAnimationLength;
												} else {
														if (timeNormalized < 0f) {
																timeDelta += timeAnimationLength;
														}
												}
										}
								}
				


								this.Animator.Update (timeDelta);
						}
				}
	
				public void SetTimeValue (float timeNormalized)
				{

						if (this.timeControl.playing)
								return;

						//currentTime is negative infinity not intialized => set it to 0f
						if (this.timeControl.currentTime < 0f)
								this.timeControl.currentTime = 0f;
			
						//calculate nextCurrentTime based on timeNormalized
						//deltaTime is nextCurrentTime-currentTime
						this.timeControl.nextCurrentTime = this.timeControl.startTime * (1f - timeNormalized) + this.timeControl.stopTime * timeNormalized;	


				}

				public void DoAvatarPreview2 (Rect rect, GUIStyle background)
				{
						this.Init ();
//			Rect position = new Rect (rect.xMax - 16f, rect.yMax - 16f, 16f, 16f);
//			if (EditorGUI.ButtonMouseDown (position, GUIContent.none, FocusType.Passive, GUIStyle.none))
//			{
//				GenericMenu genericMenu = new GenericMenu ();
//				genericMenu.AddItem (new GUIContent ("Auto"), false, new GenericMenu.MenuFunction2 (my.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.Auto);
//				genericMenu.AddItem (new GUIContent ("Unity Model"), false, new GenericMenu.MenuFunction2 (my.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.DefaultModel);
//				genericMenu.AddItem (new GUIContent ("Other..."), false, new GenericMenu.MenuFunction2 (my.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.Other);
//				genericMenu.ShowAsContext ();
//			}
						Rect rect2 = rect;
//			rect2.yMin += 21f;
//			rect2.height = Mathf.Max (rect2.height, 64f);
//			my.m_PreviewDir = PreviewGUI.Drag2D (my.m_PreviewDir, rect2);
//			int controlID = GUIUtility.GetControlID (my.m_PreviewHint, FocusType.Native, rect2);
						int controlID = GUIUtility.GetControlID (FocusType.Passive) + 1;
						Event current = Event.current;

						EventType typeForControl = current.GetTypeForControl (controlID);
						if (typeForControl == EventType.Repaint) {// && my.m_IsValid)

								lastRect = GUILayoutUtility.GetLastRect ();
								Debug.Log ("y:" + lastRect.ToString ());
								image = this.DoRenderPreview (rect2, background);

			
								//GUI.DrawTexture (rect2, image, ScaleMode.StretchToFill, false);

						}

						GUILayout.Box (image);

						//if (image != null) {
						//GUILayout.BeginHorizontal ();
						
						//GUILayout.Button("mile");
						//GUILayout.EndHorizontal ();
						//	}

						lastRect.width = Screen.width; 
						this.AvatarTimeControlGUI (lastRect);
//			GUI.DrawTexture (position, AvatarPreview.s_Styles.avatarIcon.image);
//
//			if (!my.m_IsValid)
//			{
//				Rect position2 = rect2;
//				position2.yMax -= position2.height / 2f - 16f;
//				EditorGUI.DropShadowLabel (position2, "No model is available for preview.\nPlease drag a model into my Preview Area.");
//			}
//			my.DoAvatarPreviewDrag (typeForControl);
//			if (current.type == EventType.ExecuteCommand)
//			{
//				string commandName = current.commandName;
//				if (commandName == "ObjectSelectorUpdated" && ObjectSelector.get.objectSelectorID == this.m_ModelSelectorId)
//				{
//					my.SetPreview (ObjectSelector.GetCurrentObject () as GameObject);
//					current.Use ();
//				}
//			}
				}

				public void AvatarTimeControlGUI (Rect rect)
				{
						MethodInfo_AvatarTimeControlGUI.Invoke (__instance, new object[]{rect});
				}

				public Texture DoRenderPreview (Rect previewRect, GUIStyle background)
				{
		

						return MethodInfo_DoRenderPreview.Invoke (__instance, new object[] {
								previewRect,
								background
						}) as Texture;
				}

				void Init ()
				{
						MethodInfo_Init.Invoke (__instance, null);
				}
	
				public Animator Animator {
						get {
								return PropertyInfo_Animator.GetValue (__instance, null) as Animator;
						}
				}

				public bool IKOnFeet {
						get {
								return (bool)PropertyInfo_IKOnFeet.GetValue (__instance, null);
						}
				}
	
				public OnAvatarChange OnAvatarChangeFunc {
						set {
								PropertyInfo_OnAvatarChangeFunc.SetValue (__instance, Delegate.CreateDelegate (PropertyInfo_OnAvatarChangeFunc.PropertyType, value.Target, value.Method), null);
						}
				}
	
				public void DoPreviewSettings ()
				{
						MethodInfo_DoPreviewSettings.Invoke (__instance, null);
				}
	
				public void OnDestroy ()
				{
						MethodInfo_OnDestroy.Invoke (__instance, null);
				}
	
				public void DoAvatarPreview (Rect rect, GUIStyle background, float normalizedTime=-1f)
				{
						UpdateMotion (_previewedMotion);

						MethodInfo_DoAvatarPreview.Invoke (__instance, new object[] {
								rect,
								background
						});
				}
	
				public TimeControlW timeControl {
						get {
								if (_timeControl == null)
										_timeControl = new TimeControlW (FieldInfo_timeControl.GetValue (__instance));

								return _timeControl;
						}
				}
	#endregion




	
		}
#endregion




#region TimeControlWrapper
		public class TimeControlW
		{
				private static Type realType;
				private object instance;
				private static FieldInfo FieldInfo_playbackSpeed;
				private static FieldInfo FieldInfo_currentTime;
				private static FieldInfo FieldInfo_loop;
				private static FieldInfo FieldInfo_startTime;
				private static FieldInfo FieldInfo_stopTime;
				private static MethodInfo MethodInfo_Update;
				private static PropertyInfo PropertyInfo_deltaTime;
				private static PropertyInfo PropertyInfo_normalizedTime;
				private static PropertyInfo PropertyInfo_playing;
				private static PropertyInfo PropertyInfo_nextCurrentTime;
	
				public static void InitType ()
				{
						if (realType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								realType = assembly.GetType ("UnityEditor.TimeControl");
			
								FieldInfo_currentTime = realType.GetField ("currentTime");
								FieldInfo_loop = realType.GetField ("loop");
								FieldInfo_startTime = realType.GetField ("startTime");
								FieldInfo_stopTime = realType.GetField ("stopTime");
								FieldInfo_playbackSpeed = realType.GetField ("playbackSpeed");
								MethodInfo_Update = realType.GetMethod ("Update");
								PropertyInfo_deltaTime = realType.GetProperty ("deltaTime");
								PropertyInfo_normalizedTime = realType.GetProperty ("normalizedTime");
								PropertyInfo_playing = realType.GetProperty ("playing");
								PropertyInfo_nextCurrentTime = realType.GetProperty ("nextCurrentTime");

						}
				}
	
				public TimeControlW (object realTimeControl)
				{
						InitType ();
						this.instance = realTimeControl;
				}

				//
				// Nested Types
				//
				public class Styles
				{
						public GUIContent playIcon = EditorGUIUtility.IconContent ("PlayButton");
						public GUIContent pauseIcon = EditorGUIUtility.IconContent ("PauseButton");
						public GUIStyle playButton = "TimeScrubberButton";
						public GUIStyle timeScrubber = "TimeScrubber";
				}
		
				static Styles s_Styles;

				public static Styles style {
					get {
						if(s_Styles==null) s_Styles=new TimeControlW.Styles();
						return s_Styles;
					}
				}

				float m_MouseDrag;
				bool m_WrapForwardDrag;
				string[] displayNames;
		
				//
				// Methods
				//
				public void DoTimeControl2 (Rect rect)
				{
						if (TimeControlW.s_Styles == null) {
								TimeControlW.s_Styles = new TimeControlW.Styles ();
						}
						Event current = Event.current;
						//int controlID = GUIUtility.GetControlID (TimeControl.kScrubberIDHash, FocusType.Keyboard);
						int controlID = GUIUtility.GetControlID (FocusType.Passive);
						Rect rect2 = rect;
						rect2.height = 21f;
						Rect rect3 = rect2;
						rect3.xMin += 33f;
						switch (current.GetTypeForControl (controlID)) {
						case EventType.MouseDown:
								if (rect.Contains (current.mousePosition)) {
										GUIUtility.keyboardControl = controlID;
								}
								if (rect3.Contains (current.mousePosition)) {
										EditorGUIUtility.SetWantsMouseJumping (1);
										GUIUtility.hotControl = controlID;
										this.m_MouseDrag = current.mousePosition.x - rect3.xMin;
										//this.nextCurrentTime = this.m_MouseDrag * (this.stopTime - this.startTime) / rect3.width + this.startTime;
										this.m_WrapForwardDrag = false;
										current.Use ();
								}
								break;
						case EventType.MouseUp:
								if (GUIUtility.hotControl == controlID) {
										EditorGUIUtility.SetWantsMouseJumping (0);
										GUIUtility.hotControl = 0;
										current.Use ();
								}
								break;
						case EventType.MouseDrag:
								if (GUIUtility.hotControl == controlID) {
										this.m_MouseDrag += current.delta.x * 1f;//this.playbackSpeed;
										//if (false && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width)) {
										if (this.loop && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width)) {
												this.m_WrapForwardDrag = true;
												this.m_MouseDrag = Mathf.Repeat (this.m_MouseDrag, rect3.width);
										}
										//this.nextCurrentTime = Mathf.Clamp (this.m_MouseDrag, 0f, rect3.width) * (this.stopTime - this.startTime) / rect3.width + this.startTime;
										current.Use ();
								}
								break;
						case EventType.KeyDown:
				//				if (GUIUtility.keyboardControl == controlID)
				//				{
				//					if (current.keyCode == KeyCode.LeftArrow)
				//					{
				//						if (this.currentTime - this.startTime > 0.01f)
				//						{
				//							this.deltaTime = -0.01f;
				//						}
				//						current.Use ();
				//					}
				//					if (current.keyCode == KeyCode.RightArrow)
				//					{
				//						if (this.stopTime - this.currentTime > 0.01f)
				//						{
				//							this.deltaTime = 0.01f;
				//						}
				//						current.Use ();
				//					}
				//				}
								break;
						}
			
						//GUI.Box (rect2, GUIContent.none, TimeControl.s_Styles.timeScrubber);
						GUI.Box (rect2, GUIContent.none, TimeControlW.s_Styles.timeScrubber);

						//this are the buttons
						//thisg = GUI.Toggle (rect2, this.playing, (!this.playing) ? TimeControl.s_Styles.playIcon : TimeControl.s_Styles.pauseIcon, TimeControl.s_Styles.playButton);
			
						float num = Mathf.Lerp (rect3.x, rect3.xMax, this.normalizedTime);

			
						if (GUIUtility.keyboardControl == controlID) {
								Handles.color = new Color (1f, 0f, 0f, 1f);
						} else {
								Handles.color = new Color (1f, 0f, 0f, 0.5f);
						}
						Handles.DrawLine (new Vector2 (num, rect3.yMin), new Vector2 (num, rect3.yMax));
						Handles.DrawLine (new Vector2 (num + 1f, rect3.yMin), new Vector2 (num + 1f, rect3.yMax));
				}
	
				public float currentTime {
						get {
								return (float)FieldInfo_currentTime.GetValue (instance);
						}
						set {
								FieldInfo_currentTime.SetValue (instance, value);
						}
				}
	
				public bool loop {
						get {
								return (bool)FieldInfo_loop.GetValue (instance);
						}
						set {
								FieldInfo_loop.SetValue (instance, value);
						}
				}
	
				public float startTime {
						get {
								return (float)FieldInfo_startTime.GetValue (instance);
						}
						set {
								FieldInfo_startTime.SetValue (instance, value);
						}
				}
	
				public float stopTime {
						get {
								return (float)FieldInfo_stopTime.GetValue (instance);
						}
						set {
								FieldInfo_stopTime.SetValue (instance, value);
						}
				}
	
				public float deltaTime {
						get {
								return (float)PropertyInfo_deltaTime.GetValue (instance, null);
						}
						set {
								PropertyInfo_deltaTime.SetValue (instance, value, null);
						}
				}
	
				public float normalizedTime {
						get {
								return (float)PropertyInfo_normalizedTime.GetValue (instance, null);
						}
						set {
								PropertyInfo_normalizedTime.SetValue (instance, value, null);
						}
				}

				public float playbackSpeed {
						get {
								return (float)FieldInfo_playbackSpeed.GetValue (instance);
						}
						set {
								FieldInfo_playbackSpeed.SetValue (instance, value);
						}
				}
	
				public bool playing {
						get {
								return (bool)PropertyInfo_playing.GetValue (instance, null);
						}
						set {
								PropertyInfo_playing.SetValue (instance, value, null);
						}
				}
	
				public float nextCurrentTime {
						set {
								PropertyInfo_nextCurrentTime.SetValue (instance, value, null);
						}
				}
	
				public void Update ()
				{
						MethodInfo_Update.Invoke (instance, null);
				}
		}
#endregion







		/// ///////////////  CURVE EDITOR  //////////////// 
	 
	#region CurveMenuManagerW
		public class CurveMenuManagerW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo MethodInfo_AddTangentMenuItems;
				private object __instance;
		
				public object wrapped {
						get{ return __instance;}
				}
		
				public static void InitType ()
				{
						if (method_ctor == null) {
			
				
				
				
								method_ctor = GetWrappedType ().GetConstructor (new Type[] {
					CurveEditorW.GetWrappedType ()
						
				});
				
				
								MethodInfo_AddTangentMenuItems = __RealType.GetMethod ("AddTangentMenuItems", new Type[] {
					typeof(GenericMenu),
					typeof(List<>).MakeGenericType (KeyIdentifierW.GetWrappedType ())
				});
				
						}
			
			
				}

				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.CurveMenuManager");
						}
			
						return __RealType;
				}
		
				public CurveMenuManagerW (CurveEditorW editor)
				{
			
						InitType ();
			
						__instance = method_ctor.Invoke (new object[]{editor.wrapped});
			
				}
		
				public void AddTangentMenuItems (GenericMenu menu, object keyList)
				{


						MethodInfo_AddTangentMenuItems.Invoke (__instance, new object[] {
								menu,
								keyList
						});
				}
		
		}
	#endregion

	#region KeyIdentifierW
		public class KeyIdentifierW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo MethodInfo_SetWrap;
				private object __instance;
		
				public object wrapped {
						get{ return __instance;}
				}
		
				public static void InitType ()
				{
						if (method_ctor == null) {
								
				
				
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								method_ctor = GetWrappedType ().GetConstructor (new Type[] {
								assembly.GetType ("UnityEditor.CurveRenderer"),typeof(int),typeof(int)
						
				});
				
				

						}
			
			
				}

				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.KeyIdentifier");
						}

						return __RealType;
				}
		
				public KeyIdentifierW (object renderer, int curveId, int keyIndex)
				{
			
						InitType ();
			
						__instance = method_ctor.Invoke (new object[] {
								renderer,
								curveId,
								keyIndex
						});
			
				}
		

		
		}

	#endregion


	#region CurveSelectionW
		public class CurveSelectionW
		{
		
				private static Type __RealType;
				private object __instance;
				static FieldInfo FieldInfo_m_Host;
				static PropertyInfo PropertyInfo_key;
				static PropertyInfo PropertyInfo_curveID ;
				static PropertyInfo PropertyInfo_curveWrapper;
				CurveWrapperW _curveWrapperW;

				public object wrapped {
						get{ return __instance;}
						set{ __instance = value;}
				}
		
				public static void InitType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.CurveSelection");
				
								FieldInfo_m_Host = __RealType.GetField ("m_Host", BindingFlags.Instance | BindingFlags.NonPublic);
								PropertyInfo_key = __RealType.GetProperty ("key");
								PropertyInfo_curveID = __RealType.GetProperty ("curveID");
								PropertyInfo_curveWrapper = __RealType.GetProperty ("curveWrapper", BindingFlags.Instance | BindingFlags.NonPublic);

				
						}
			
			
				}

				public CurveWrapperW curveWrapper {
						get {
								if (_curveWrapperW == null)
										_curveWrapperW = new CurveWrapperW ();

								_curveWrapperW.wrapped = PropertyInfo_curveWrapper.GetValue (__instance, null);
								return _curveWrapperW;
						}
				}

				public CurveEditorW host {
					
						set{ FieldInfo_m_Host.SetValue (__instance, value.wrapped);}
				}

				public int key {
						get {
								return (int)PropertyInfo_key.GetValue (__instance, null);
						}
						set {
								PropertyInfo_key.SetValue (__instance, value, null);
						}
				}

				public int curveID {
						get {
								return (int)PropertyInfo_curveID.GetValue (__instance, null);
						}
						set {
								PropertyInfo_curveID.SetValue (__instance, value, null);
						}
				}

				public CurveSelectionW ()
				{
						InitType ();
			
						__instance = FormatterServices.GetUninitializedObject (__RealType);
				}
		
				public CurveSelectionW (int curveID, CurveEditorW host, int keyIndex):this()
				{
			
						


						this.host = host;
						this.key = keyIndex;
						this.curveID = curveID;
						
						//__instance = method_ctor.Invoke (new object[]{curveID,host.wrapped,keyIndex});
			
				}
		

		
		}
	#endregion





	#region NormalCurveRenderer
		public class NormalCurveRendererW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo MethodInfo_SetWrap;
				private object __instance;

				public object wrapped {
						get{ return __instance;}
				}

				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.NormalCurveRenderer");
						}
						
						return __RealType;
						
				}

				public static void InitType ()
				{
						if (method_ctor == null) {
							
								method_ctor = GetWrappedType ().GetConstructor (new Type[] {
					typeof(AnimationCurve)
						
				});


								MethodInfo_SetWrap = __RealType.GetMethod ("SetWrap", new Type[] {
										typeof(WrapMode),
										typeof(WrapMode)
								});
				
						}
			
			
				}
		
				public NormalCurveRendererW (AnimationCurve curve)
				{
			
						InitType ();
			
						__instance = method_ctor.Invoke (new object[]{curve});
			
				}
		
				public void SetWrap (WrapMode preWrap, WrapMode postWrap)
				{
						MethodInfo_SetWrap.Invoke (__instance, new object[]{preWrap,postWrap});
				}
		
		}
	#endregion
	
	
	
	#region CurveWrapper
		[Serializable]
		public class CurveWrapperW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static FieldInfo FieldInfo_id;
				static FieldInfo FieldInfo_groupId;
				static FieldInfo FieldInfo_hidden;
				static FieldInfo FieldInfo_readOnly;
				static FieldInfo FieldInfo_color;
				static PropertyInfo PropertyInfo_renderer;
				static PropertyInfo PropertyInfo_curve;
				private object __instance;

				public object wrapped {
						get {
								return __instance;
						}
						set {

								__instance = value;
						}
				}

				public AnimationCurve curve {
						get{ return (AnimationCurve)PropertyInfo_curve.GetValue (__instance, null);}
						set {
								NormalCurveRendererW renderer;
								

								this.hidden = false;
								this.readOnly = false;
								
								renderer = new NormalCurveRendererW (value);
								renderer.SetWrap (value.preWrapMode, value.postWrapMode);
								
								this.renderer = renderer.wrapped;
						}
				}

				public bool hidden {
						get{ return (bool)FieldInfo_hidden.GetValue (__instance);}
						set{ FieldInfo_hidden.SetValue (__instance, value);}
				}

				public bool readOnly {
						get{ return (bool)FieldInfo_readOnly.GetValue (__instance);}
						set{ FieldInfo_readOnly.SetValue (__instance, value);}
				}

				public Color color {
						get{ return (Color)FieldInfo_color.GetValue (__instance);}
						set{ FieldInfo_color.SetValue (__instance, value);}
				}

				public int groupId {
						get{ return (int)FieldInfo_groupId.GetValue (__instance);}
						set{ FieldInfo_groupId.SetValue (__instance, value); }
				}

				public object renderer {
						get{ return PropertyInfo_renderer.GetValue (__instance, null);}
						set{ PropertyInfo_renderer.SetValue (__instance, value, null);}
				}

				public int id {
						get{ return (int)FieldInfo_id.GetValue (__instance);}
						set{ FieldInfo_id.SetValue (__instance, value); }
				}
		
				private static void InitType ()
				{
						if (method_ctor == null) {

								

								method_ctor = GetWrappedType ().GetConstructor (new Type[] {});

								

								PropertyInfo_renderer = __RealType.GetProperty ("renderer");
								PropertyInfo_curve = __RealType.GetProperty ("curve");

								FieldInfo_groupId = __RealType.GetField ("groupId");
								FieldInfo_color = __RealType.GetField ("color");
								FieldInfo_hidden = __RealType.GetField ("hidden");
								FieldInfo_readOnly = __RealType.GetField ("readOnly");
								FieldInfo_id = __RealType.GetField ("id");	
						}

			
				}

				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.CurveWrapper");
						}
						
						return __RealType;

				}
		
				public CurveWrapperW ()
				{
						
						InitType ();
			
						__instance = method_ctor.Invoke (new object[]{});

						this.groupId = -1;
						this.color = new Color (UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
			
				}
		
				
		
		
		}
	#endregion
	
	
	#region CurveEditorW
		public class CurveEditorW
		{
				
				private static Type __RealType;
				private object instance;
				private static MethodInfo MethodInfo_OnGUI;
				static MethodInfo MethodInfo_FrameSelected;
				static MethodInfo MethodInfo_GetCurveAtPosition;
				static MethodInfo MethodInfo_CreateKeyFromClick;
				static MethodInfo MethodInfo_getCurveWrapperById;
				static MethodInfo MethodInfo_GetGUIPoint;
				static MethodInfo MethodInfo_DeleteKeys;
				private static PropertyInfo PropertyInfo_rect;
				private static PropertyInfo PropertyInfo_scaleWithWindow;
				private static PropertyInfo PropertyInfo_drawRect;
				static PropertyInfo PropertyInfo_margin;
				static PropertyInfo PropertyInfo_topmargin;
				static PropertyInfo PropertyInfo_leftmargin;
				static PropertyInfo PropertyInfo_rightmargin;
				static PropertyInfo PropertyInfo_bottommargin;
				static PropertyInfo PropertyInfo_vSlider;
				static PropertyInfo PropertyInfo_hSlider;
				static PropertyInfo PropertyInfo_hRangeMax;
				static PropertyInfo PropertyInfo_hRangeMin;
				static PropertyInfo PropertyInfo_vRangeMax;
				static PropertyInfo PropertyInfo_vRangeMin;
				static PropertyInfo PropertyInfo_vRangeLocked;
				static PropertyInfo PropertyInfo_hRangeLocked;
				static PropertyInfo PropertyInfo_mousePositionInDrawing;
				static PropertyInfo PropertyInfo_animationCurves;
				static FieldInfo FieldInfo_m_Selection;
				static FieldInfo FieldInfo_m_Translation;
				static FieldInfo FieldInfo_m_Scale;
				static FieldInfo FieldInfo_m_DrawOrder;
				static ConstructorInfo method_ctor;
				AnimationCurve m_Curve;
				Color m_Color;
				static Styles ms_Styles;
				private object __instance;
				CurveMenuManagerW m_MenuManager;
				CurveWrapperW[] _curveWrappersW;
				private int _indexSelected = -1;

				public delegate void SelectHandler (int index);
	
				//Defining event based on the above delegate
				public event SelectHandler onSelect;

				public object wrapped {
						get {
								return __instance;
						}

				}

				public Rect rect {
						get{ return (Rect)PropertyInfo_rect.GetValue (__instance, null);}
						set{ PropertyInfo_rect.SetValue (__instance, value, null);}
				}

				public bool scaleWithWindow {
						get{ return (bool)PropertyInfo_scaleWithWindow.GetValue (__instance, null);}
						set{ PropertyInfo_scaleWithWindow.SetValue (__instance, value, null);}
				}

				public float margin {

						set{ PropertyInfo_margin.SetValue (__instance, value, null);}
				}

				public float topmargin {
			
						set{ PropertyInfo_topmargin.SetValue (__instance, value, null);}
				}

				public float leftmargin {
					
						set{ PropertyInfo_leftmargin.SetValue (__instance, value, null);}
				}

				public float rightmargin {
						
						set{ PropertyInfo_rightmargin.SetValue (__instance, value, null);}
				}

				public float bottommargin {
					
						set{ PropertyInfo_bottommargin.SetValue (__instance, value, null);}
				}

				public Vector2 mousePositionInDrawing {
						get{ return (Vector2)PropertyInfo_mousePositionInDrawing.GetValue (__instance, null);}

				}

				public Vector2 Scale {
						get{ return (Vector2)FieldInfo_m_Scale.GetValue (__instance);}
			
				}

				public Vector2 Translation {
						get{ return (Vector2)FieldInfo_m_Translation.GetValue (__instance);}
			
				}

				List<int> DrawOrder {
						get{ return (List<int>)FieldInfo_m_DrawOrder.GetValue (__instance);}
					
				}

				IList m_Selection {
						get{ return FieldInfo_m_Selection.GetValue (__instance) as IList;}
				}

				public		bool hRangeLocked {

						get{ return (bool)PropertyInfo_hRangeLocked.GetValue (__instance, null);}
						set{ PropertyInfo_hRangeLocked.SetValue (__instance, value, null);}
				}

				public		bool vRangeLocked {
						get{ return (bool)PropertyInfo_vRangeLocked.GetValue (__instance, null);}
						set{ PropertyInfo_vRangeLocked.SetValue (__instance, value, null);}
				}

				public	bool hSlider {
					
						get{ return (bool)PropertyInfo_hSlider.GetValue (__instance, null);}
						set{ PropertyInfo_hSlider.SetValue (__instance, value, null);}
				}

				public		bool vSlider {
					
						get{ return (bool)PropertyInfo_hSlider.GetValue (__instance, null);}
						set{ PropertyInfo_vSlider.SetValue (__instance, value, null);}
				}

				public float hRangeMin {
					
						get{ return (float)PropertyInfo_hRangeMin.GetValue (__instance, null);}
						set{ PropertyInfo_hRangeMin.SetValue (__instance, value, null);}
				}

				public float hRangeMax {
					
						get{ return (float)PropertyInfo_hRangeMax.GetValue (__instance, null);}
						set{ PropertyInfo_hRangeMax.SetValue (__instance, value, null);}
				}

				public float vRangeMin {
			
						get{ return (float)PropertyInfo_vRangeMin.GetValue (__instance, null);}
						set{ PropertyInfo_vRangeMin.SetValue (__instance, value, null);}
				}
		
				public float vRangeMax {
			
						get{ return (float)PropertyInfo_vRangeMax.GetValue (__instance, null);}
						set{ PropertyInfo_vRangeMax.SetValue (__instance, value, null);}
				}

				public CurveWrapperW[] animationCurves {
						set {
						
								value.Select ((item) => item.curve);
								_curveWrappersW = value;

								PropertyInfo_animationCurves.SetValue (__instance, CurveWrappersWToCurveWrappers (value), null);
						}
						get {
								return _curveWrappersW;
						}
				}

				void DeleteKeys (object userData)
				{
						MethodInfo_DeleteKeys.Invoke (__instance, new object[]{userData});
				}

				public Rect drawRect {
						get{ return (Rect)PropertyInfo_drawRect.GetValue (__instance, null);}
				}
		
				public static void InitType ()
				{
						if (method_ctor == null) {
								


				
								method_ctor = GetWrappedType ().GetConstructor (new Type[] {
						typeof(Rect),CurveWrapperW.GetWrappedType ().MakeArrayType (),typeof(bool)
						
				});
								
								PropertyInfo_rect = __RealType.GetProperty ("rect");
								PropertyInfo_scaleWithWindow = __RealType.GetProperty ("scaleWithWindow");
								PropertyInfo_margin = __RealType.GetProperty ("margin");
								PropertyInfo_topmargin = __RealType.GetProperty ("topmargin");
								PropertyInfo_leftmargin = __RealType.GetProperty ("leftmargin");
								PropertyInfo_rightmargin = __RealType.GetProperty ("rightmargin");
								PropertyInfo_bottommargin = __RealType.GetProperty ("rightmargin");
								PropertyInfo_vSlider = __RealType.GetProperty ("vSlider");
								PropertyInfo_hSlider = __RealType.GetProperty ("hSlider");
								PropertyInfo_hRangeMax = __RealType.GetProperty ("hRangeMax");
								PropertyInfo_hRangeMin = __RealType.GetProperty ("hRangeMin");
								PropertyInfo_vRangeMax = __RealType.GetProperty ("vRangeMax");
								PropertyInfo_vRangeMin = __RealType.GetProperty ("vRangeMin");
								PropertyInfo_vRangeLocked = __RealType.GetProperty ("vRangeLocked");
								PropertyInfo_hRangeLocked = __RealType.GetProperty ("hRangeLocked");

								PropertyInfo_drawRect = __RealType.BaseType.GetProperty ("drawRect");
								PropertyInfo_mousePositionInDrawing = __RealType.BaseType.GetProperty ("mousePositionInDrawing");
								PropertyInfo_animationCurves = __RealType.GetProperty ("animationCurves");
				
				
								MethodInfo_OnGUI = __RealType.GetMethod ("OnGUI");
								MethodInfo_GetGUIPoint = __RealType.GetMethod ("GetGUIPoint", BindingFlags.NonPublic | BindingFlags.Instance);
								MethodInfo_getCurveWrapperById = __RealType.GetMethod ("getCurveWrapperById");
								MethodInfo_FrameSelected = __RealType.GetMethod ("FrameSelected");
								MethodInfo_GetCurveAtPosition = __RealType.GetMethod ("GetCurveAtPosition", BindingFlags.NonPublic | BindingFlags.Instance);
								MethodInfo_CreateKeyFromClick = __RealType.GetMethod ("CreateKeyFromClick", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]{typeof(object)}, null);
								MethodInfo_DeleteKeys = __RealType.GetMethod ("DeleteKeys", BindingFlags.NonPublic | BindingFlags.Instance);
								


								FieldInfo_m_Translation = __RealType.BaseType.GetField ("m_Translation", BindingFlags.Instance | BindingFlags.NonPublic);
								FieldInfo_m_Scale = __RealType.BaseType.GetField ("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
								FieldInfo_m_DrawOrder = __RealType.GetField ("m_DrawOrder", BindingFlags.Instance | BindingFlags.NonPublic);
								FieldInfo_m_Selection = __RealType.GetField ("m_Selection", BindingFlags.Instance | BindingFlags.NonPublic);


				
						}
				}

				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.CurveEditor");
						}
			
						return __RealType;
			
				}
		
				public CurveEditorW (Rect rect, CurveWrapperW[] curveWrappers, bool minimalGUI)
				{
						
						InitType ();
			
			
						if (CurveEditorW.ms_Styles == null) {
								CurveEditorW.ms_Styles = new CurveEditorW.Styles ();
						}
			
			
			
						__instance = method_ctor.Invoke (new object[] {
				rect,
				CurveWrappersWToCurveWrappers (curveWrappers),
				minimalGUI
			});
				}
		
				public CurveEditorW (Rect rect, AnimationCurve curve, bool minimalGUI):this(rect,new AnimationCurve[]{curve},minimalGUI)
				{

				}
		
				public CurveEditorW (Rect rect, AnimationCurve[] curves, bool minimalGUI)
				{
						InitType ();


						if (CurveEditorW.ms_Styles == null) {
								CurveEditorW.ms_Styles = new CurveEditorW.Styles ();
						}

						

						__instance = method_ctor.Invoke (new object[] {
								rect,
								AnimationCurvesToCurveWrappers (curves),
								minimalGUI
						});

		
			
				}

				public void RemoveCurveAt (int index)
				{
						if (index > -1 && index < _curveWrappersW.Length) {
								List<CurveWrapperW> list = _curveWrappersW.ToList ();
								list.RemoveAt (index);
								_curveWrappersW = list.ToArray ();
						
								PropertyInfo_animationCurves.SetValue (__instance, CurveWrappersWToCurveWrappers (_curveWrappersW), null);

						}

						_indexSelected = -1;
				}

				public void AddCurve (CurveWrapperW curveWrapperW)
				{
						int len = _curveWrappersW.Length;

						curveWrapperW.id = ("Curve" + len).GetHashCode ();

						Array.Resize (ref _curveWrappersW, len + 1);
						_curveWrappersW [len] = curveWrapperW;

						PropertyInfo_animationCurves.SetValue (__instance, CurveWrappersWToCurveWrappers (_curveWrappersW), null);
						
				}

				Array CurveWrappersWToCurveWrappers (CurveWrapperW[] curveWrappers)
				{

						Array curveWrapperArray = Array.CreateInstance (CurveWrapperW.GetWrappedType (), curveWrappers.Length);
			
			
						CurveWrapperW curveWrapperW;
						for (int i=0; i<curveWrapperArray.Length; i++) {
								curveWrapperW = curveWrappers [i];
								curveWrapperW.id = ("Curve" + i).GetHashCode ();
								curveWrapperArray.SetValue (curveWrapperW.wrapped, i);
						}


						_curveWrappersW = curveWrappers;

						return curveWrapperArray;
			
				}
		
				Array AnimationCurvesToCurveWrappers (AnimationCurve[] curves)
				{
						CurveWrapperW curveWrapperW;
						Array curveWrapperArray = Array.CreateInstance (CurveWrapperW.GetWrappedType (), curves.Length);
						_curveWrappersW = new CurveWrapperW[curves.Length];
				
						AnimationCurve curve;
						for (int i=0; i<curves.Length; i++) {
								curve = curves [i];
								_curveWrappersW [i] = curveWrapperW = new CurveWrapperW ();
								curveWrapperW.id = ("Curve" + i).GetHashCode ();
								curveWrapperW.curve = curve;
						
						
								curveWrapperArray.SetValue (curveWrapperW.wrapped, i);
						
						}

						return curveWrapperArray;
				}

				int GetCurveAtPosition (Vector2 mousePosition, out Vector2 closestPointOnCurve)
				{
						Vector2 vectorTemp = new Vector2 ();

						object[] arguments = new object[] {
							mousePosition,
							vectorTemp
			};

						int result = (int)MethodInfo_GetCurveAtPosition.Invoke (__instance, arguments);

						closestPointOnCurve = (Vector2)arguments [1];

						return result;
				}

				public void FrameSelected (bool horizontally, bool vertically)
				{
						MethodInfo_FrameSelected.Invoke (__instance, new object[] {
								horizontally,
								vertically
						});
				}

				CurveWrapperW getCurveWrapperById (int i)
				{
						CurveWrapperW crv = new CurveWrapperW ();
						crv.wrapped = MethodInfo_getCurveWrapperById.Invoke (__instance, new object[]{i});
						return crv;
				}

				Vector2 GetGUIPoint (Vector3 point)
				{
						return (Vector2)MethodInfo_GetGUIPoint.Invoke (__instance, new object[]{point});
				}

				CurveSelectionW FindNearest (Vector2 mousePosition)
				{
						//return MethodInfo_FindNearest.Invoke (__instance, null);



						mousePosition.x = mousePosition.x - this.drawRect.x;
						mousePosition.y = mousePosition.y - this.drawRect.y;

						int num = -1;
						int keyIndex = -1;
						float num2 = 64f;
						for (int i = this.DrawOrder.Count - 1; i >= 0; i--) {



								CurveWrapperW curveWrapperById = this.getCurveWrapperById (this.DrawOrder [i]);
								if (!curveWrapperById.readOnly && !curveWrapperById.hidden) {
										for (int j = 0; j < curveWrapperById.curve.keys.Length; j++) {
												Keyframe keyframe = curveWrapperById.curve.keys [j];
												float sqrMagnitude = (this.GetGUIPoint (new Vector2 (keyframe.time, keyframe.value)) - mousePosition).sqrMagnitude;
												if (sqrMagnitude <= 16f) {
														return new CurveSelectionW (curveWrapperById.id, this, j);
												}
												if (sqrMagnitude < num2) {
														num = curveWrapperById.id;
														keyIndex = j;
														num2 = sqrMagnitude;
												}
										}
										if (i == this.DrawOrder.Count - 1 && num >= 0) {
												num2 = 16f;
										}
								}
						}
						if (num >= 0) {
								return new CurveSelectionW (num, this, keyIndex);
						}
						return null;
				}

				Vector2 ViewToDrawingTransformPoint (Vector2 lhs)
				{
						return new Vector2 ((lhs.x - this.drawRect.x - this.Translation.x) / this.Scale.x, (lhs.y - this.drawRect.y - this.Translation.y) / this.Scale.y);
				}

				void CreateKeyFromClick (object userData)
				{
						MethodInfo_CreateKeyFromClick.Invoke (__instance, new object[]{userData});
				}

				public void DoEditor ()
				{
						

						GUI.Label (this.drawRect, GUIContent.none, CurveEditorW.ms_Styles.curveEditorBackground);
						

						
						//	int controlID =GUIUtility.GetControlID ("SelectKeys".GetHashCode (), FocusType.Passive);


						//int controlID = GUIUtility.GetControlID (897560, FocusType.Passive);
						Event current = Event.current;
						
						//EventType typeForControl = current.GetTypeForControl (controlID);

						//EventType.ContextClick never fired???
						if (current.type == EventType.MouseDown && this.drawRect.Contains (current.mousePosition)) {
								if (current.button == 1) {


								

										//check if clicked happen over key points
										CurveSelectionW curveSelection = this.FindNearest (current.mousePosition);
										if (curveSelection != null) {
		

					
												IList list = (IList)Activator.CreateInstance (typeof(List<>).MakeGenericType (KeyIdentifierW.GetWrappedType ()));
					

										
												bool flag2 = false;
												KeyIdentifierW keyIdW;

												CurveSelectionW current2 = new CurveSelectionW ();

												foreach (var obj in this.m_Selection) {
										
														current2.wrapped = obj;

														keyIdW = new KeyIdentifierW (current2.curveWrapper.renderer, current2.curveID, current2.key);

														list.Add (keyIdW.wrapped);
														if (current2.curveID == curveSelection.curveID && current2.key == curveSelection.key) {
																flag2 = true;
														}
												}

												if (!flag2) {
														list.Clear ();
														keyIdW = new KeyIdentifierW (curveSelection.curveWrapper.renderer, curveSelection.curveID, curveSelection.key);
														list.Add (keyIdW.wrapped);
														this.m_Selection.Clear ();
														this.m_Selection.Add (curveSelection.wrapped);
												}

												this.m_MenuManager = new CurveMenuManagerW (this);
												GenericMenu genericMenu = new GenericMenu ();
												string text;
												if (list.Count > 1) {
														text = "Delete Keys";
												} else {
														text = "Delete Key";
												}
												genericMenu.AddItem (new GUIContent (text), false, new GenericMenu.MenuFunction2 (this.DeleteKeys), list);
												genericMenu.AddSeparator (string.Empty);
												this.m_MenuManager.AddTangentMenuItems (genericMenu, list);
												genericMenu.ShowAsContext ();
												Event.current.Use ();
										} else {//check if click happened on curve

												Vector2 vector;
				
												int curveAtPosition = this.GetCurveAtPosition (this.ViewToDrawingTransformPoint (Event.current.mousePosition), out vector);//this.GetCurveAtPosition (offset, out vector);
			
												if (curveAtPosition >= 0) {
					
																
														GenericMenu genericMenu = new GenericMenu ();
														genericMenu.AddItem (new GUIContent ("Add Key"), false, new GenericMenu.MenuFunction2 (this.CreateKeyFromClick), this.ViewToDrawingTransformPoint (Event.current.mousePosition));
														genericMenu.ShowAsContext ();
														Event.current.Use ();
												}
										}
								} else {
										Vector2 vector;
					
										int curveAtPosition = this.GetCurveAtPosition (this.ViewToDrawingTransformPoint (Event.current.mousePosition), out vector);//this.GetCurveAtPosition (offset, out vector);
					
										if (curveAtPosition > -1) {
												Color clr = this.animationCurves [curveAtPosition].color;
												this.animationCurves [curveAtPosition].color = new Color (clr.r, clr.g, clr.b, 0.5f);
													
												if (_indexSelected > -1) {
														clr = this.animationCurves [_indexSelected].color;
														this.animationCurves [_indexSelected].color = new Color (clr.r, clr.g, clr.b, 1f);
												}
												
												_indexSelected = curveAtPosition;

												
										} else {
												if (_indexSelected > -1) {
														Color clr = this.animationCurves [_indexSelected].color;
														this.animationCurves [_indexSelected].color = new Color (clr.r, clr.g, clr.b, 1f);
												}

												_indexSelected = -1;
										}


											onSelect (_indexSelected);
								}

						}

					



						MethodInfo_OnGUI .Invoke (__instance, null);




				}



				//
				// Nested Types
				//
				internal class Styles
				{
						public GUIStyle curveEditorBackground = "PopupCurveEditorBackground";

				}
		
	
		
		}
	
	#endregion




}//namespace




	




