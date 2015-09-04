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
using ws.winx.editor.extensions;

namespace ws.winx.editor
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

	#region EditorGUIUtilityW
		public class EditorGUIUtilityW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo _LoadIcon_MethodInfo;
		
				public static Texture2D LoadIcon (string name)
				{

						if (_LoadIcon_MethodInfo == null) 
								_LoadIcon_MethodInfo = GetWrappedType ().GetMethod ("LoadIcon", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);


						return _LoadIcon_MethodInfo.Invoke (null, new object[]{name}) as Texture2D;
				}
		
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.EditorGUIUtility");
						}
			
						return __RealType;
				}
		}

	#endregion

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

						return (HighLevelEvent)HighLevelEventEnumInt;
		
				}

		#endregion
		
		}
	#endregion



	#region QuaternionCurveTangentCalculationW
		public class QuaternionCurveTangentCalculationW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo _UpdateTangentsFromMode_MethodInfo;
		
				public static MethodInfo UpdateTangentsFromMode_MethodInfo {
						get {
								if (_UpdateTangentsFromMode_MethodInfo == null)
										_UpdateTangentsFromMode_MethodInfo = GetWrappedType ().GetMethod ("UpdateTangentsFromMode", new Type[] {
												typeof(AnimationCurve),
												typeof(AnimationClip),
												typeof(EditorCurveBinding)
										});
				
								return _UpdateTangentsFromMode_MethodInfo;
						}
				}
		
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.QuaternionCurveTangentCalculation");
						}
			
						return __RealType;
				}
		
				public static void UpdateTangentsFromMode (AnimationCurve curve, AnimationClip clip, EditorCurveBinding curveBinding)
				{
			
						UpdateTangentsFromMode_MethodInfo.Invoke (null, new object[] {
								curve,
								clip,
								curveBinding
						});
				}		
		
		
		}
	#endregion
	
	#region RotationCurveInterpolationW
		public class RotationCurveInterpolationW
		{
		
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				static MethodInfo _RemapAnimationBindingForAddKey_MethodInfo;
		
				public static MethodInfo RemapAnimationBindingForAddKey_MethodInfo {
						get {
								if (_RemapAnimationBindingForAddKey_MethodInfo == null)
										_RemapAnimationBindingForAddKey_MethodInfo = GetWrappedType ().GetMethod ("RemapAnimationBindingForAddKey");
				
								return _RemapAnimationBindingForAddKey_MethodInfo;
						}
				}
		
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.RotationCurveInterpolation");
						}
			
						return __RealType;
				}
		
				public static EditorCurveBinding[] RemapAnimationBindingForAddKey (EditorCurveBinding binding, AnimationClip clip)
				{
						return (EditorCurveBinding[])RemapAnimationBindingForAddKey_MethodInfo.Invoke (null, new object[] {
								binding,
								clip
						});
				}		
		
		
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
				UnityEditor.Animations.AnimatorController _previewAnimatorController;
				UnityEditor.Animations.AnimatorStateMachine _previewStateMachine;
				UnityEditor.Animations.AnimatorState _previewState;
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
				float _timeStartNormailized = 0f;
				float _timeStopNormalized = 1f;

	
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

						_previewState.motion = motion;
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
								_previewAnimatorController = new UnityEditor.Animations.AnimatorController ();
								_previewAnimatorController.AddLayer ("previewLayer");
								_previewAnimatorController.hideFlags = HideFlags.DontSave;
				
								_previewStateMachine = _previewAnimatorController.layers [0].stateMachine;
								CreateParameters (motion);
				
								_previewState = _previewStateMachine.AddState ("previewState");
				
								_previewState.motion = motion;
								_previewState.iKOnFeet = this.IKOnFeet;
								_previewState.hideFlags = HideFlags.DontSave;
				
				
								UnityEditor.Animations.AnimatorController.SetAnimatorController (this.Animator, _previewAnimatorController);
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
								UnityEditor.Animations.AnimatorController.SetAnimatorController (this.Animator, null);
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
			
						if (motion is UnityEditor.Animations.BlendTree) {
								UnityEditor.Animations.BlendTree blendTree = motion as UnityEditor.Animations.BlendTree;
				
								for (int j = 0; j < blendTree.GetRecursiveBlendParamCount(); j++) {
										_previewAnimatorController.AddParameter (blendTree.GetRecursiveBlendParam (j), UnityEngine.AnimatorControllerParameterType.Float);
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
				
//								this.timeControl.loop = true;
								float timeAnimationLength = 1f;//1s
								float timeNormalized = 0f;
//
//
								if (this.Animator.layerCount > 0) {
					
										AnimatorStateInfo currentAnimatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo (0);
										timeAnimationLength = currentAnimatorStateInfo.length;


										timeNormalized = currentAnimatorStateInfo.normalizedTime;
								}
				
								//this.timeControl.startTime = timeAnimationLength * _timeStartNormailized;
								//this.timeControl.stopTime = timeAnimationLength * _timeStopNormalized;


								//float timeAnimationLength=((AnimationClip)motion).length;

								this.timeControl.startTime = timeAnimationLength * _timeStartNormailized;
								this.timeControl.stopTime = timeAnimationLength * _timeStopNormalized;
								
								this.timeControl.Update ();


			


								//deltaTime is nextCurrentTime-currentTime
								//is set my drag of red Timeline handle or manually thru SetTimeValue

								float timeDelta = this.timeControl.deltaTime;



								

								if (this.timeControl.playing) {
										if (!motion.isLooping) {
												if (timeNormalized >= 1f) {
														timeDelta -= this.timeControl.stopTime - this.timeControl.startTime;//timeAnimationLength;
												} else {
														if (timeNormalized < 0f) {
																timeDelta += this.timeControl.stopTime - this.timeControl.startTime;// timeAnimationLength;
														}
												}
										}
								}
				


								this.Animator.Update (timeDelta);
						}
				}

				public void SetStartTime (float timeNormalized)
				{

						_timeStartNormailized = timeNormalized;

				}

				public void SetStopTime (float timeNormalized)
				{
					
						_timeStopNormalized = timeNormalized;
					
				}
	
				public void SetTimeAt (float timeNormalized)
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
				
				float m_MouseDrag;
				bool m_WrapForwardDrag;
				string[] displayNames;
		
				//
				// Methods
				//
				public void DoTimeControl2 (Rect rect)
				{
						
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
						GUI.Box (rect2, GUIContent.none, EditorGUILayoutEx.ANIMATION_STYLES.timeScrubber);

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
	
		/// <summary>
		/// Curve editor wrapper.
		/// 
		///  Use it like;
		/// 
		// if (curveEditor == null) {
			
		//	CurveWrapperW[] curveWrappers;
		//	
		//	int numCurves = curves.Length;
		//	
		//	curveWrappers = new CurveWrapperW[numCurves];
		//	
		//	CurveWrapperW curveWrapperNew;
		//	
		//	for (i=0; i<numCurves; i++) {
		//		curveWrapperNew = new CurveWrapperW ();
		//		curveWrapperNew.curve = curves [i];
		//		curveWrapperNew.color = curveColors [i];
		//		curveWrappers [i] = curveWrapperNew;
		//	}
		//	
		//	
		//	
		//	curveEditor = new CurveEditorW (curveEditorRect, curveWrappers, false);
		//	
		//	curveEditor.FrameSelected (true, true);
		//	curveEditor.scaleWithWindow = true;
		//	curveEditor.hSlider = false;
		//	curveEditor.hRangeMin = 0f;
		//	curveEditor.hRangeMax = 1f;
		//	curveEditor.hRangeLocked = true;
		//	
		//	curveEditor.onSelect += onCurveSelect;
		//	
		//	
		//	
		//	
		//} else {
		//	
		//	curveEditor.rect = curveEditorRect;
		//	curveEditor.FrameSelected (false, false);
		//	
		//}
		//
		//
		//
		//curveEditor.DoEditor ();
		/// </summary>
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
				private static PropertyInfo __PropertyInfo_rect;

				static PropertyInfo PropertyInfo_rect {
						get {
								if (__PropertyInfo_rect == null)
										__PropertyInfo_rect = __RealType.GetProperty ("rect");
								return __PropertyInfo_rect;
						}
				}

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


		///!!! Started optmizaiton with MethodInfo Getters

		//TickHandler
	#region TickHandlerWrapper
		public class TickHandlerW
		{

				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				private object __instance;
				public static MethodInfo __GetLevelWithMinSeparation_MethodInfo;

				public static MethodInfo GetLevelWithMinSeparation_MethodInfo {
						get {
								if (__GetLevelWithMinSeparation_MethodInfo == null)
										__GetLevelWithMinSeparation_MethodInfo = __RealType.GetMethod ("GetLevelWithMinSeparation");
								return __GetLevelWithMinSeparation_MethodInfo;
						}
				}

				private static MethodInfo __GetTicksAtLevel_MethodInfo;

				public static MethodInfo GetTicksAtLevel_MethodInfo {
						get {
								if (__GetTicksAtLevel_MethodInfo == null)
										__GetTicksAtLevel_MethodInfo = __RealType.GetMethod ("GetTicksAtLevel");
								return __GetTicksAtLevel_MethodInfo;
						}
				}

				private static MethodInfo __GetStrengthOfLevel_MethodInfo;

				public static MethodInfo GetStrengthOfLevel_MethodInfo {
						get {
								if (__GetStrengthOfLevel_MethodInfo == null)
										__GetStrengthOfLevel_MethodInfo = __RealType.GetMethod ("GetStrengthOfLevel");
								return __GetStrengthOfLevel_MethodInfo;
						}
				}

				private static MethodInfo __SetTickModulosForFrameRate_MethodInfo;
				private static MethodInfo __SetTickModulos_MethodInfo;

				static MethodInfo SetTickModulos_MethodInfo {
						get {
								if (__SetTickModulos_MethodInfo == null)
										__SetTickModulos_MethodInfo = __RealType.GetMethod ("SetTickModulos");

								return __SetTickModulos_MethodInfo;
						}
				}

				static MethodInfo SetTickModulosForFrameRate_MethodInfo {
						get {

								if (__SetTickModulosForFrameRate_MethodInfo == null)
										__SetTickModulosForFrameRate_MethodInfo = __RealType.GetMethod ("SetTickModulosForFrameRate");

								return __SetTickModulosForFrameRate_MethodInfo;
						}
				}

				private static MethodInfo __SetTickStrengths_MethodInfo;

				static MethodInfo SetTickStrengths_MethodInfo {
						get {
								if (__SetTickStrengths_MethodInfo == null)
										__SetTickStrengths_MethodInfo = __RealType.GetMethod ("SetTickStrengths");
								return __SetTickStrengths_MethodInfo;
						}
				}

				private static PropertyInfo __tickLevels_PropertyInfo;

				static PropertyInfo tickLevels_PropertyInfo {
						get {
								if (__tickLevels_PropertyInfo == null)
										__tickLevels_PropertyInfo = __RealType.GetProperty ("tickLevels");
								return __tickLevels_PropertyInfo;
						}
				}

				public object wrapped {
						get {
								return __instance;
						}
						set {
								__instance = value;
						}
			
				}

				public int tickLevels {
						get {
								return	(int)tickLevels_PropertyInfo.GetValue (__instance, null);
						}
						
				}
		
				public static void InitType ()
				{
						if (method_ctor == null) {

								method_ctor = GetWrappedType ().GetConstructor (new Type[] {});
			
						}
				}
		
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.TickHandler");
						}
				
						return __RealType;
				
				}

				public int GetLevelWithMinSeparation (float pixelSeparation)
				{
					
						return (int)GetLevelWithMinSeparation_MethodInfo.Invoke (__instance, new object[]{pixelSeparation});
				}

				public float[] GetTicksAtLevel (int level, bool excludeTicksFromHigherlevels)
				{
						return (float[])GetTicksAtLevel_MethodInfo.Invoke (__instance, new object[] {
								level,
								excludeTicksFromHigherlevels
						});
				}

				public float GetStrengthOfLevel (int level)
				{
						return (float)GetStrengthOfLevel_MethodInfo.Invoke (__instance, new object[]{level});
				}

				public void SetTickModulosForFrameRate (float frameRate)
				{
						SetTickModulosForFrameRate_MethodInfo.Invoke (__instance, new object[]{frameRate});
				}

				public void SetTickModulos (float[] tickModulos)
				{
					
						SetTickModulos_MethodInfo.Invoke (__instance, new object[]{tickModulos});
				}

				public void SetTickStrengths (float tickMinSpacing, float tickMaxSpacing, bool sqrt)
				{
						SetTickStrengths_MethodInfo.Invoke (__instance, new object[] {
								tickMinSpacing,
								tickMaxSpacing,
								sqrt
						});
				}
			
				public TickHandlerW ()
				{
						InitType ();

						__instance = method_ctor.Invoke (new object[]{});

				}
		}
	#endregion
	
	
	
	
		/// <summary>
		/// Time arear wrapper.
		/// </summary>
	#region TimeAreaWrapper
		public class TimeAreaW
		{
				private object __instance;
				private TickHandlerW _hTicks;
				private TickHandlerW _vTicks;
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				private	static MethodInfo EndViewGUI_MethodInfo;
				private	static MethodInfo BeginViewGUI_MethodInfo;
				private static MethodInfo __DrawingToViewTransformVector_MethodInfo;

				public static MethodInfo DrawingToViewTransformVector_MethodInfo {
						get {
								if (__DrawingToViewTransformVector_MethodInfo == null)
										__DrawingToViewTransformVector_MethodInfo = __RealType.BaseType.GetMethod ("DrawingToViewTransformVector", new Type[]{typeof(Vector2)});

								return __DrawingToViewTransformVector_MethodInfo;
						}
				}

				private static MethodInfo __Zoom_MethodInfo;

				public static MethodInfo Zoom_MethodInfo {
						get {
								if (__Zoom_MethodInfo == null)
										__Zoom_MethodInfo = __RealType.BaseType.GetMethod ("Zoom", BindingFlags.NonPublic | BindingFlags.Instance);

								return __Zoom_MethodInfo;
						}
				}

				private static MethodInfo __FrameToPixel_MethodInfo;

				public static MethodInfo FrameToPixel_MethodInfo {
						get {
								if (__FrameToPixel_MethodInfo == null)
										__FrameToPixel_MethodInfo = __RealType.GetMethod ("FrameToPixel");
								return __FrameToPixel_MethodInfo;
						}
				}

				private static MethodInfo __FormatFrame_MethodInfo;

				public static MethodInfo FormatFrame_MethodInfo {
						get {
								if (__FormatFrame_MethodInfo == null)
										__FormatFrame_MethodInfo = __RealType.GetMethod ("FormatFrame");
								return __FormatFrame_MethodInfo;
						}
				}

				private static MethodInfo __SetTickMarkerRanges_MethodInfo;

				public static MethodInfo SetTickMarkerRanges_MethodInfo {
						get {
								if (__SetTickMarkerRanges_MethodInfo == null)
										__SetTickMarkerRanges_MethodInfo = __RealType.GetMethod ("SetTickMarkerRanges", BindingFlags.NonPublic | BindingFlags.Instance);
								return __SetTickMarkerRanges_MethodInfo;
						}
				}

				private static MethodInfo __TimeToPixel_MethodInfo;

				public static MethodInfo TimeToPixel_MethodInfo {
						get {
								if (__TimeToPixel_MethodInfo == null)
										__TimeToPixel_MethodInfo = __RealType.GetMethod ("TimeToPixel");

								return __TimeToPixel_MethodInfo;
						}
				}

				private static MethodInfo __PixelToTime_MethodInfo;

				public static MethodInfo PixelToTime_MethodInfo {
						get {
								if (__PixelToTime_MethodInfo == null) 
										__PixelToTime_MethodInfo = __RealType.GetMethod ("PixelToTime");

								return __PixelToTime_MethodInfo;
						}
				}

				private static PropertyInfo __PropertyInfo_scaleWithWindow;

				public static PropertyInfo PropertyInfo_scaleWithWindow {
						get {
								if (__PropertyInfo_scaleWithWindow == null)
										__PropertyInfo_scaleWithWindow = GetWrappedType ().GetProperty ("scaleWithWindow");

								return __PropertyInfo_scaleWithWindow;
						}
				}

				private static PropertyInfo __PropertyInfo_rect;
				
				static PropertyInfo PropertyInfo_rect {
						get {
								if (__PropertyInfo_rect == null)
										__PropertyInfo_rect = __RealType.GetProperty ("rect");
								return __PropertyInfo_rect;
						}
				}

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


				//m_Translation

				private static FieldInfo __translation_FieldInfo;
		
				static FieldInfo translation_FieldInfo {
						get {
								if (__translation_FieldInfo == null)
										__translation_FieldInfo = __RealType.BaseType.GetField ("m_Translation", BindingFlags.Instance | BindingFlags.NonPublic);
				
				
								return __translation_FieldInfo;
						}
				}

				//m_Scale

				private static FieldInfo __scale_FieldInfo;
			
				static FieldInfo scale_FieldInfo {
						get {
								if (__scale_FieldInfo == null)
										__scale_FieldInfo = __RealType.BaseType.GetField ("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
									
									
								return __scale_FieldInfo;
						}
				}



//				static PropertyInfo __PropertyInfo_scale;
//
//				static PropertyInfo PropertyInfo_scale {
//						get {
//								if (__PropertyInfo_scale == null)
//										__PropertyInfo_scale = __RealType.GetProperty ("scale");
//						
//						
//								return __PropertyInfo_scale;
//						}
//				}

				static PropertyInfo __PropertyInfo_hTicks;

				static PropertyInfo PropertyInfo_hTicks {
						get {
								if (__PropertyInfo_hTicks == null)
										__PropertyInfo_hTicks = __RealType.GetProperty ("hTicks");


								return __PropertyInfo_hTicks;
						}
				}

				static PropertyInfo __PropertyInfo_vTicks;

				static PropertyInfo PropertyInfo_vTicks {
						get {
								if (__PropertyInfo_vTicks == null)
										__PropertyInfo_vTicks = __RealType.GetProperty ("vTicks");

								return __PropertyInfo_vTicks;
						}
				}

				static MethodInfo __DrawMajorTicks_MethodInfo;

				static MethodInfo DrawMajorTicks_MethodInfo {
						get {
								if (__DrawMajorTicks_MethodInfo == null)
										__DrawMajorTicks_MethodInfo = TimeAreaW.GetWrappedType ().GetMethod ("DrawMajorTicks");


								return __DrawMajorTicks_MethodInfo;
						}
				}

				public float tickMinSpacing = 3f;
				public float tickMaxSpacing = 80f;
				public float timeRullerLabelHeight = 20f;
			
				public object wrapped {
						get {
								return __instance;
						}
						
				}

				public TickHandlerW hTicks {
						get { 
								if (_hTicks == null) {
										_hTicks = new TickHandlerW ();
										_hTicks.wrapped = PropertyInfo_hTicks.GetValue (__instance, null);
								}
								return _hTicks;
						}
						set{ _hTicks = value;}
				}

				public TickHandlerW vTicks {
						get { 
								if (_vTicks == null) {
										_vTicks = new TickHandlerW ();
										_vTicks.wrapped = PropertyInfo_vTicks.GetValue (__instance, null);
								}
								return _vTicks;
						}
						set { 
								_vTicks = value;
								PropertyInfo_vTicks.SetValue (__instance, _vTicks.wrapped, null);
						}
				}

				public Vector2 translation {
						get{ return (Vector2)translation_FieldInfo.GetValue (__instance);}
						set{ translation_FieldInfo.SetValue (__instance, value);}
				}

				public Vector2 scale {
						get{ return (Vector2)scale_FieldInfo.GetValue (__instance);}
						set{ scale_FieldInfo.SetValue (__instance, value);}
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

				public	bool hRangeLocked {
			
						get{ return (bool)PropertyInfo_hRangeLocked.GetValue (__instance, null);}
						set{ PropertyInfo_hRangeLocked.SetValue (__instance, value, null);}
				}
		
				public	bool vRangeLocked {
						get{ return (bool)PropertyInfo_vRangeLocked.GetValue (__instance, null);}
						set{ PropertyInfo_vRangeLocked.SetValue (__instance, value, null);}
				}
		
				public	bool hSlider {
			
						get{ return (bool)PropertyInfo_hSlider.GetValue (__instance, null);}
						set{ PropertyInfo_hSlider.SetValue (__instance, value, null);}
				}
		
				public	bool vSlider {
			
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



				public delegate string FrameFormatDelegate (int i,float frameRate);

				public TimeAreaW.FrameFormatDelegate onFrameFormatCallback;

				/// <summary>
				/// Inits the type.
				/// </summary>
				public static void InitType ()
				{
						if (method_ctor == null) {
							
							
							
							
								method_ctor = GetWrappedType ().GetConstructor (new Type[] {
								typeof(bool)
									
							});
							
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
								
								
				


								EndViewGUI_MethodInfo = __RealType.GetMethod ("EndViewGUI");
								BeginViewGUI_MethodInfo = __RealType.GetMethod ("BeginViewGUI");
								

							
						}
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="ws.winx.editor.TimeAreaW"/> class.
				/// </summary>
				/// <param name="minimalGUI">If set to <c>true</c> minimal GU.</param>
				public TimeAreaW (bool minimalGUI)
				{
						InitType ();
						
						__instance = method_ctor.Invoke (new object[] {
							
							minimalGUI
						});


						//this.margin = 0f;

				}
					
				/// <summary>
				/// Gets the type of the wrapped.
				/// </summary>
				/// <returns>The wrapped type.</returns>
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.TimeArea");
						}
						
						return __RealType;
						
				}


				/// <summary>
				/// CUSTOM FUNCTIONS
				/// </summary>
		 

				/// <summary>
				/// Focus the specified rect.(currenlty only Xpos is implemented)
				/// </summary>
				/// <param name="rect">Rect.</param>
				public void Focus (Rect rect)
				{

						//find ratio of focus rect inside timeArea rect
						float ratio = this.rect.width / rect.width;

						//find pos offset
						float diff = (rect.x - this.rect.x);

						this.translation = new Vector2 ((this.translation.x - diff) * ratio, this.translation.y);
						this.scale = new Vector2 (this.scale.x * ratio, this.scale.y);

				}

				public static float SnapTimeToWholeFPS (float time, float frameRate)
				{
						if (frameRate == 0f) {
								return time;
						}
						return Mathf.Round (time * frameRate) / frameRate;
				}

				public Vector2 DrawingToViewTransformVector (Vector2 lhs)
				{
						return (Vector2)DrawingToViewTransformVector_MethodInfo.Invoke (__instance, new object[]{lhs});
				}

				public void Zoom (Vector2 zoomAround, bool scrollwhell)
				{
						Zoom_MethodInfo.Invoke (__instance, new object[]{zoomAround,scrollwhell});
				}

				public float TimeToPixel (float time, Rect rect)
				{
						return (float)TimeToPixel_MethodInfo.Invoke (__instance, new object[] {
								time,
								rect
						});

				}

				public float PixelToTime (float pixelX, Rect rect)
				{
						return (float)PixelToTime_MethodInfo.Invoke (__instance, new object[] {
								pixelX,
								rect
						});
				}

				public int PixelToFrame (float pixelX, int frameRate, Rect rect)
				{
						return (int)Mathf.Ceil (PixelToTime (pixelX, rect) * frameRate);
				}
		
				public float FrameToPixel (float frame, float frameRate, Rect rect)
				{
						return (float)FrameToPixel_MethodInfo.Invoke (__instance, new object[] {
								frame,
								frameRate,
								rect
						});
				}

				public virtual string FormatFrame (int frame, float frameRate)
				{
						return (string)FormatFrame_MethodInfo.Invoke (__instance, new object[] {
								frame,
								frameRate
						});
				}

				void SetTickMarkerRanges ()
				{
						SetTickMarkerRanges_MethodInfo.Invoke (__instance, null);
				}

				void DrawFrameTicks (Rect position, float frameRate)
				{
						Color color = Handles.color;
						GUI.BeginGroup (position);
						if (Event.current.type != EventType.Repaint) {
								GUI.EndGroup ();
								return;
						}

						this.SetTickMarkerRanges ();
						this.hTicks.SetTickStrengths (3f, 80f, true);
						Color textColor = EditorGUILayoutEx.TIMEAREA_STYLES.TimelineTick.normal.textColor;
						textColor.a = 0.1f;
						Handles.color = textColor;
						for (int i = 0; i < this.hTicks.tickLevels; i++) {
								float num = this.hTicks.GetStrengthOfLevel (i) * 0.9f;
								if (num > 0.1f) {//if (num > 0.5f)
					
										float[] ticksAtLevel = this.hTicks.GetTicksAtLevel (i, true);
										for (int j = 0; j < ticksAtLevel.Length; j++) {
												if (ticksAtLevel [j] >= 0f) {
														int num2 = Mathf.RoundToInt (ticksAtLevel [j] * frameRate);
														float x = this.FrameToPixel ((float)num2, frameRate, position);
														Handles.DrawLine (new Vector3 (x, 0f, 0f), new Vector3 (x, position.height, 0f));
												}
										}
								}
						}
						GUI.EndGroup ();
						Handles.color = color;
				}
		
				public virtual void TimeRuler (Rect position, float frameRate)
				{
						Color color = GUI.color;
						GUI.BeginGroup (position);
						if (Event.current.type != EventType.Repaint) {
								GUI.EndGroup ();
								return;
						}
						//TimeArea.InitStyles ();
						HandleUtilityW.ApplyWireMaterial ();
						GL.Begin (1);
						Color backgroundColor = GUI.backgroundColor;
						this.SetTickMarkerRanges ();
						this.hTicks.SetTickStrengths (this.tickMinSpacing, this.tickMaxSpacing, true);
						Color textColor = EditorGUILayoutEx.TIMEAREA_STYLES.TimelineTick.normal.textColor;
						textColor.a = 0.75f;
						for (int i = 0; i < this.hTicks.tickLevels; i++) {
								float num = this.hTicks.GetStrengthOfLevel (i) * 0.9f;
								float[] ticksAtLevel = this.hTicks.GetTicksAtLevel (i, true);
								for (int j = 0; j < ticksAtLevel.Length; j++) {
										if (ticksAtLevel [j] >= this.hRangeMin && ticksAtLevel [j] <= this.hRangeMax) {
												int num2 = Mathf.RoundToInt (ticksAtLevel [j] * frameRate);
												float num3 = position.height * Mathf.Min (1f, num) * 0.7f;
												float num4 = this.FrameToPixel ((float)num2, frameRate, position);
												GL.Color (new Color (1f, 1f, 1f, num / 0.5f) * textColor);
												GL.Vertex (new Vector3 (num4, position.height - num3 + 0.5f, 0f));
												GL.Vertex (new Vector3 (num4, position.height - 0.5f, 0f));
												if (num > 0.5f) {
														GL.Color (new Color (1f, 1f, 1f, num / 0.5f - 1f) * textColor);
														GL.Vertex (new Vector3 (num4 + 1f, position.height - num3 + 0.5f, 0f));
														GL.Vertex (new Vector3 (num4 + 1f, position.height - 0.5f, 0f));
												}
										}
								}
						}
						GL.End ();
						int levelWithMinSeparation = this.hTicks.GetLevelWithMinSeparation (40f);
						float[] ticksAtLevel2 = this.hTicks.GetTicksAtLevel (levelWithMinSeparation, false);
						for (int k = 0; k < ticksAtLevel2.Length; k++) {
								if (ticksAtLevel2 [k] >= this.hRangeMin && ticksAtLevel2 [k] <= this.hRangeMax) {
										int num5 = Mathf.RoundToInt (ticksAtLevel2 [k] * frameRate);
										float num6 = Mathf.Floor (this.FrameToPixel ((float)num5, frameRate, this.rect));


										string text = String.Empty;
										if (onFrameFormatCallback != null) {
												text = onFrameFormatCallback (num5, frameRate);
										} else {
												text = this.FormatFrame (num5, frameRate);
										}
										GUI.Label (new Rect (num6 + 3f, -3f, 40f, 20f), text, EditorGUILayoutEx.TIMEAREA_STYLES.TimelineTick);
										
								}
						}
						GUI.EndGroup ();
						GUI.backgroundColor = backgroundColor;
						GUI.color = color;
				}
		
				/// <summary>
				/// Do the "time ruller area" at position and frame rate
				/// If you set framerate=60 then 60 fps in 1s and makers
				/// 0s  0.15(frms)  0.30(frms) 0.45(frms) 1.00s[60frms]
				/// </summary>
				/// <param name="position">Position.</param>
				/// <param name="framerate">Framerate.</param>
				public void DoTimeArea (Rect position, float framerate)
				{


//						if (Event.current.type == EventType.Repaint)
//						{
//							this.rect = position;
//						}

						this.rect = position;

						Rect frameRect = rect;
						frameRect.yMin += timeRullerLabelHeight;

						DrawFrameTicks (frameRect, framerate);
						DrawMajorTicks_MethodInfo.Invoke (__instance, new object[] {
				position,
				framerate
			});
			
						BeginViewGUI_MethodInfo.Invoke (__instance, null);

						// TimeRuller label heigth=20f
//						TimeRuler_MethodInfo.Invoke (__instance, new object[] {
//								new Rect (position.x, position.y, position.width, 20f),
//								framerate
//			});

						TimeRuler (new Rect (position.x, position.y, position.width, timeRullerLabelHeight), framerate);

			
						EndViewGUI_MethodInfo.Invoke (__instance, null);

						
				}


		}
	#endregion



	#region TimeAreaWrapper
		public class HandleUtilityW
		{
				private object __instance;
				private static Type __RealType;
				private static ConstructorInfo method_ctor;
				private static MethodInfo __ApplyWireMaterial_MethodInfo;

				public static MethodInfo ApplyWireMaterial_MethodInfo {
						get {
								if (__ApplyWireMaterial_MethodInfo == null)
										__ApplyWireMaterial_MethodInfo = GetWrappedType ().GetMethod ("ApplyWireMaterial", BindingFlags.NonPublic | BindingFlags.Static);
								return __ApplyWireMaterial_MethodInfo;
						}
				}

				/// <summary>
				/// Gets the type of the wrapped.
				/// </summary>
				/// <returns>The wrapped type.</returns>
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.HandleUtility");
						}
			
						return __RealType;
			
				}

				public static void ApplyWireMaterial ()
				{
						ApplyWireMaterial_MethodInfo.Invoke (null, null);
				}
		}

	#endregion




	#region AudioUtilW
		public class AudioUtilW
		{

				private static Type __RealType;
				private static MethodInfo __PlayClip_MethodInfo;
		
				public static MethodInfo PlayClip_MethodInfo {
						get {
								if (__PlayClip_MethodInfo == null)
										__PlayClip_MethodInfo = GetWrappedType ().GetMethod ("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new Type[] {
												typeof(AudioClip),
												typeof(int),
												typeof(bool)
										}, null);
								return __PlayClip_MethodInfo;
						}
				}

				private static MethodInfo __StopClip__MethodInfo;
		
				public static MethodInfo StopClip__MethodInfo {
						get {
								if (__StopClip__MethodInfo == null)
										__StopClip__MethodInfo = GetWrappedType ().GetMethod ("StopClip", BindingFlags.Static | BindingFlags.Public);
								return __StopClip__MethodInfo;
						}
				}

				private static MethodInfo __PauseClip__MethodInfo;
		
				public static MethodInfo PauseClip__MethodInfo {
						get {
								if (__PauseClip__MethodInfo == null)
										__PauseClip__MethodInfo = GetWrappedType ().GetMethod ("PauseClip", BindingFlags.Static | BindingFlags.Public);
								return __PauseClip__MethodInfo;
						}
				}

				private static MethodInfo __ResumeClip__MethodInfo;
		
				public static MethodInfo ResumeClip__MethodInfo {
						get {
								if (__ResumeClip__MethodInfo == null)
										__ResumeClip__MethodInfo = GetWrappedType ().GetMethod ("ResumeClip", BindingFlags.Static | BindingFlags.Public);
								return __ResumeClip__MethodInfo;
						}
				}

				private static MethodInfo __GetWaveForm_MethodInfo;
		
				public static MethodInfo GetWaveForm__MethodInfo {
						get {
								if (__GetWaveForm_MethodInfo == null)
										__GetWaveForm_MethodInfo = GetWrappedType ().GetMethod ("GetWaveForm", BindingFlags.Static | BindingFlags.Public);
								return __GetWaveForm_MethodInfo;
						}
				}

				private static MethodInfo __SetClipSamplePosition_MethodInfo;
		
				public static MethodInfo SetClipSamplePosition_MethodInfo {
						get {
								if (__SetClipSamplePosition_MethodInfo == null)
										__SetClipSamplePosition_MethodInfo = GetWrappedType ().GetMethod ("SetClipSamplePosition", BindingFlags.Static | BindingFlags.Public);
								return __SetClipSamplePosition_MethodInfo;
						}
				}

				private static MethodInfo __StopAllClips_MethodInfo;
		
				public static MethodInfo StopAllClips_MethodInfo {
						get {
								if (__StopAllClips_MethodInfo == null)
										__StopAllClips_MethodInfo = GetWrappedType ().GetMethod ("StopAllClips", BindingFlags.Static | BindingFlags.Public);
								return __StopAllClips_MethodInfo;
						}
				}

				private static MethodInfo __UpdateAudio_MethodInfo;
		
				public static MethodInfo UpdateAudio_MethodInfo {
						get {
								if (__UpdateAudio_MethodInfo == null)
										__UpdateAudio_MethodInfo = GetWrappedType ().GetMethod ("StopAllClips", BindingFlags.Static | BindingFlags.Public);
								return __UpdateAudio_MethodInfo;
						}
				}

				private static MethodInfo __SetListenerTransform_MethodInfo;
		
				public static MethodInfo SetListenerTransform_MethodInfo {
						get {
								if (__SetListenerTransform_MethodInfo == null)
										__SetListenerTransform_MethodInfo = GetWrappedType ().GetMethod ("SetListenerTransform", BindingFlags.Static | BindingFlags.Public);
								return __SetListenerTransform_MethodInfo;
						}
				}

				private static MethodInfo __ClearWaveForm_MethodInfo;
		
				public static MethodInfo ClearWaveForm_MethodInfo {
						get {
								if (__ClearWaveForm_MethodInfo == null)
										__ClearWaveForm_MethodInfo = GetWrappedType ().GetMethod ("ClearWaveForm", BindingFlags.Static | BindingFlags.Public);
								return __ClearWaveForm_MethodInfo;
						}
				}

				private static PropertyInfo __resetAllAudioClipPlayCountsOnPlay_PropertyInfo;
		
				public static PropertyInfo resetAllAudioClipPlayCountsOnPlay_PropertyInfo {
						get {
								if (__resetAllAudioClipPlayCountsOnPlay_PropertyInfo == null)
										__resetAllAudioClipPlayCountsOnPlay_PropertyInfo = GetWrappedType ().GetProperty ("resetAllAudioClipPlayCountsOnPlay", BindingFlags.Static | BindingFlags.Public);
								return __resetAllAudioClipPlayCountsOnPlay_PropertyInfo;
						}
				}
		
				/// <summary>
				/// Gets the type of the wrapped.
				/// </summary>
				/// <returns>The wrapped type.</returns>
				public static Type GetWrappedType ()
				{
						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.AudioUtil");
						}
			
						return __RealType;
			
				}

				public static  bool resetAllAudioClipPlayCountsOnPlay {

						get {
								return (bool)resetAllAudioClipPlayCountsOnPlay_PropertyInfo.GetValue (null, null);
						}

						set {
								resetAllAudioClipPlayCountsOnPlay_PropertyInfo.SetValue (null, value, null);
						}
				}

				public static void ResumeClip (AudioClip clip)
				{

						ResumeClip__MethodInfo.Invoke (null, new object[]{clip});
				}

				public static void PauseClip (AudioClip clip)
				{
						PauseClip__MethodInfo.Invoke (null, new object[]{clip});
				}

				public static void StopClip (AudioClip clip)
				{
						StopClip__MethodInfo.Invoke (null, new object[]{clip});

				}
		
				public static void PlayClip (AudioClip clip, int startSample=0, bool loop=false)
				{
						PlayClip_MethodInfo.Invoke (null, new object[]{clip,startSample,loop});
				}

				public static void SetClipSamplePosition (AudioClip clip, int iSamplePosition)
				{
						SetClipSamplePosition_MethodInfo.Invoke (null, new object[] {
								clip,
								iSamplePosition
						});
				}

				public static void StopAllClips ()
				{
						StopAllClips_MethodInfo.Invoke (null, null);
				}

				public static void UpdateAudio ()
				{
						UpdateAudio_MethodInfo.Invoke (null, null);
				}

				public static void SetListenerTransform (Transform t)
				{
						SetListenerTransform_MethodInfo.Invoke (null, new object[]{t});
				}

				public static Texture2D GetWaveForm (AudioClip clip, AssetImporter importer, int channel, float width, float height)
				{
						return (Texture2D)GetWaveForm__MethodInfo.Invoke (null, new object[] {
								clip,
								importer,
								channel,
								width,
								height
						});
				}

				public static void ClearWaveForm (AudioClip clip)
				{
						ClearWaveForm_MethodInfo.Invoke (null, new object[]{clip});
				}




		}
	
	#endregion

	#region AudioClipInspectorW
		public class AudioClipInspectorW
		{

				private static Type __RealType;


				/// <summary>
				/// Gets the type of the wrapped.
				/// </summary>
				/// <returns>The wrapped type.</returns>
				public static Type GetWrappedType ()
				{



						if (__RealType == null) {
								Assembly assembly = Assembly.GetAssembly (typeof(Editor));
								__RealType = assembly.GetType ("UnityEditor.AudioClipInspector");
						}
			
						return __RealType;
			
				}

				public static Texture2D CombineWaveForms (Texture2D[] waveForms)
				{
						throw new NotImplementedException ();
				}

		}
	#endregion



}//namespace




	




