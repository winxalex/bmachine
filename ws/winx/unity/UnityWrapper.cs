using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;
using System.Reflection;
using ws.winx.unity;

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


	public class EditorGUIExtW {
		#region Reflection
		
		private static Type realType;
		
		
		private static MethodInfo MethodInfo_MultiSelection;

		public static void InitType() {
			
			if (realType == null) {
				Assembly assembly = Assembly.GetAssembly(typeof(Editor));
				realType = assembly.GetType("UnityEditor.EditorGUIExt");
				
				MethodInfo_MultiSelection	= realType.GetMethod("MultiSelection",BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static |  BindingFlags.Public);
				
			}
		}
		
		#endregion
		
		#region Wrapper
		public static HighLevelEvent MultiSelection (Rect rect, Rect[] positions, GUIContent content, Rect[] hitPositions, ref bool[] selections, bool[] readOnly, out int clickedIndex, out Vector2 offset, out float startSelect, out float endSelect, GUIStyle style)
		{
			InitType ();
			float startSelectTemp=0f;
			float endSelectTemp=0f;
			int clickedIndexTemp=0;

		
			Vector2 offsetTemp = new Vector2 ();


			object[] arguments=new object[]{rect,positions,content,hitPositions,selections,readOnly,clickedIndexTemp,offsetTemp,startSelectTemp,endSelectTemp,style};
			object result=null;

			result=MethodInfo_MultiSelection.Invoke(null,arguments);
			

			int HighLevelEventEnumInt = (int)result;
		

				clickedIndex=(int)arguments[6];
				offset=(Vector2)arguments[7];
				startSelect=(float)arguments[8];
				endSelect=(float)arguments[9];

			return (ws.winx.unity.HighLevelEvent)HighLevelEventEnumInt;
		
		}

		#endregion
		
	}



	#region AvatarPreviewW

public class AvatarPreviewW {
	#region Reflection
	
	private static Type realType;
	
	
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
	
	public static void InitType() {

		if (realType == null) {
			Assembly assembly = Assembly.GetAssembly(typeof(Editor));
			realType = assembly.GetType("UnityEditor.AvatarPreview");
			
			method_ctor 					= realType.GetConstructor(new Type[] { typeof(Animator), typeof(Motion)});
			PropertyInfo_OnAvatarChangeFunc 	= realType.GetProperty("OnAvatarChangeFunc");
			PropertyInfo_IKOnFeet				= realType.GetProperty("IKOnFeet");
			PropertyInfo_Animator				= realType.GetProperty("Animator");
			MethodInfo_DoPreviewSettings		= realType.GetMethod("DoPreviewSettings");
			MethodInfo_OnDestroy				= realType.GetMethod("OnDestroy");
			MethodInfo_DoAvatarPreview			= realType.GetMethod("DoAvatarPreview", new Type[] {typeof(Rect), typeof(GUIStyle)});
			FieldInfo_timeControl				= realType.GetField("timeControl");
			MethodInfo_DoRenderPreview 		= realType.GetMethod("DoRenderPreview",new Type[] {typeof(Rect), typeof(GUIStyle)});
			MethodInfo_Init 				= realType.GetMethod("Init",BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo_AvatarTimeControlGUI	= realType.GetMethod("AvatarTimeControlGUI",new Type[]{typeof(Rect)});
			
		}
	}
	
	#endregion
	
	#region Wrapper
	
	
	private object __instance;

	protected TimeControlW _timeControl;

	
	public delegate void OnAvatarChange();
	

		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.unity.AvatarPreviewW"/> class.
		/// </summary>
		/// <param name="previewObjectInScene">Preview object in scene.</param>
		/// <param name="objectOnSameAsset">Object on same asset.</param>
		public AvatarPreviewW(Animator previewObjectInScene, Motion objectOnSameAsset) {
			InitType();
			
			__instance = method_ctor.Invoke( new object[] { previewObjectInScene, objectOnSameAsset } );




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
			if ( this.Animator != null) {
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
				float timeNormalized=0f;


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



				if(this.timeControl.playing){
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


	
		public void SetTimeValue(float timeNormalized){

			if (this.timeControl.playing)
								return;

			//currentTime is negative infinity not intialized => set it to 0f
			if(this.timeControl.currentTime<0f) this.timeControl.currentTime=0f;
			
			//calculate nextCurrentTime based on timeNormalized
			//deltaTime is nextCurrentTime-currentTime
			this.timeControl.nextCurrentTime=this.timeControl.startTime*(1f - timeNormalized) + this.timeControl.stopTime * timeNormalized;	


		}



		public void DoAvatarPreview2(Rect rect, GUIStyle background)
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
			if (typeForControl == EventType.Repaint)// && my.m_IsValid)
			{

				lastRect=GUILayoutUtility.GetLastRect();
				Debug.Log("y:"+lastRect.ToString());
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

		public void AvatarTimeControlGUI (Rect rect){
			MethodInfo_AvatarTimeControlGUI.Invoke (__instance,new object[]{rect});
		}

     	public Texture DoRenderPreview (Rect previewRect, GUIStyle background){
		

			return MethodInfo_DoRenderPreview.Invoke (__instance, new object[]{previewRect,background}) as Texture;
		}

		void Init ()
		{
			MethodInfo_Init.Invoke (__instance, null);
		}
	
	public Animator Animator
	{
		get
		{
			return PropertyInfo_Animator.GetValue(__instance, null) as Animator;
		}
	}
	public bool IKOnFeet {
		get {
			return (bool)PropertyInfo_IKOnFeet.GetValue(__instance, null);
		}
	}
	
	public OnAvatarChange OnAvatarChangeFunc {
		set {
			PropertyInfo_OnAvatarChangeFunc.SetValue(__instance, Delegate.CreateDelegate(PropertyInfo_OnAvatarChangeFunc.PropertyType, value.Target, value.Method), null);
		}
	}
	
	public void DoPreviewSettings() {
		MethodInfo_DoPreviewSettings.Invoke(__instance, null);
	}
	
	public void OnDestroy() {
		MethodInfo_OnDestroy.Invoke(__instance, null);
	}
	
	public void DoAvatarPreview(Rect rect, GUIStyle background, float normalizedTime=-1f) {
		UpdateMotion (_previewedMotion);

		MethodInfo_DoAvatarPreview.Invoke(__instance, new object[] { rect, background });
	}
	
	public TimeControlW timeControl {
		get {
			if(_timeControl==null) _timeControl=new TimeControlW(FieldInfo_timeControl.GetValue(__instance));

			return _timeControl;
		}
	}
	#endregion




	
}
#endregion




#region TimeControlWrapper
public class TimeControlW {
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
	
	public static void InitType() {
		if (realType == null) {
			Assembly assembly = Assembly.GetAssembly(typeof(Editor));
			realType = assembly.GetType("UnityEditor.TimeControl");
			
			FieldInfo_currentTime = realType.GetField("currentTime");
			FieldInfo_loop = realType.GetField("loop");
			FieldInfo_startTime = realType.GetField("startTime");
			FieldInfo_stopTime = realType.GetField("stopTime");
			FieldInfo_playbackSpeed = realType.GetField("playbackSpeed");
			MethodInfo_Update = realType.GetMethod("Update");
			PropertyInfo_deltaTime = realType.GetProperty("deltaTime");
			PropertyInfo_normalizedTime = realType.GetProperty("normalizedTime");
			PropertyInfo_playing = realType.GetProperty("playing");
			PropertyInfo_nextCurrentTime = realType.GetProperty("nextCurrentTime");

		}
	}
	
	public TimeControlW(object realTimeControl) {
		InitType();
		this.instance = realTimeControl;
	}

		//
		// Nested Types
		//
		private class Styles
		{
			public GUIContent playIcon = EditorGUIUtility.IconContent ("PlayButton");
			public GUIContent pauseIcon = EditorGUIUtility.IconContent ("PauseButton");
			public GUIStyle playButton = "TimeScrubberButton";
			public GUIStyle timeScrubber = "TimeScrubber";
		}
		
		private static Styles s_Styles;
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
					if (this.loop && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width)){
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
			return (float)FieldInfo_currentTime.GetValue(instance);
		}
		set {
			FieldInfo_currentTime.SetValue(instance, value);
		}
	}
	
	public bool loop {
		get {
			return (bool)FieldInfo_loop.GetValue(instance);
		}
		set {
			FieldInfo_loop.SetValue(instance, value);
		}
	}
	
	public float startTime {
		get {
			return (float)FieldInfo_startTime.GetValue(instance);
		}
		set {
			FieldInfo_startTime.SetValue(instance, value);
		}
	}
	
	public float stopTime {
		get {
			return (float)FieldInfo_stopTime.GetValue(instance);
		}
		set {
			FieldInfo_stopTime.SetValue(instance, value);
		}
	}
	
	public float deltaTime {
		get {
			return (float)PropertyInfo_deltaTime.GetValue(instance, null);
		}
		set {
			PropertyInfo_deltaTime.SetValue(instance, value, null);
		}
	}
	
	public float normalizedTime {
		get {
			return (float)PropertyInfo_normalizedTime.GetValue(instance, null);
		}
		set {
			PropertyInfo_normalizedTime.SetValue(instance, value, null);
		}
	}

		public float playbackSpeed {
			get {
				return (float)FieldInfo_playbackSpeed.GetValue(instance);
			}
			set {
				FieldInfo_playbackSpeed.SetValue(instance, value);
			}
		}
	
	public bool playing {
		get {
			return (bool)PropertyInfo_playing.GetValue(instance, null);
		}
		set {
			PropertyInfo_playing.SetValue(instance, value, null);
		}
	}
	
	public float nextCurrentTime {
		set {
			PropertyInfo_nextCurrentTime.SetValue(instance, value, null);
		}
	}
	
	public void Update() {
		MethodInfo_Update.Invoke(instance, null);
	}
}
#endregion


	public enum TangentMode
	{
		Editable = 0,
		Smooth = 1,
		Linear = 2,
		Stepped = Linear | Smooth,
	}
	
	public enum TangentDirection
	{
		Left,
		Right
	}
	
	
	public class KeyframeUtil {
		
		public static Keyframe GetNew( float time, float value, TangentMode leftAndRight){
			return GetNew(time,value, leftAndRight,leftAndRight);
		}
		
		public static Keyframe GetNew(float time, float value, TangentMode left, TangentMode right){
			object boxed = new Keyframe(time,value); // cant use struct in reflection			
			
			SetKeyBroken(boxed, true);
			SetKeyTangentMode(boxed, 0, left);
			SetKeyTangentMode(boxed, 1, right);
			
			Keyframe keyframe = (Keyframe)boxed;
			if (left == TangentMode.Stepped )
				keyframe.inTangent = float.PositiveInfinity;
			if (right == TangentMode.Stepped )
				keyframe.outTangent = float.PositiveInfinity;
			
			return keyframe;
		}
		
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static void SetKeyTangentMode(object keyframe, int leftRight, TangentMode mode)
		{
			
			Type t = typeof( UnityEngine.Keyframe );
			FieldInfo field = t.GetField( "m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
			int tangentMode =  (int)field.GetValue(keyframe);
			
			if (leftRight == 0)
			{
				tangentMode &= -7;
				tangentMode |= (int) mode << 1;
			}
			else
			{
				tangentMode &= -25;
				tangentMode |= (int) mode << 3;
			}
			
			field.SetValue(keyframe, tangentMode);
			if (GetKeyTangentMode(tangentMode, leftRight) == mode)
				return;
			Debug.Log("bug"); 
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static TangentMode GetKeyTangentMode(int tangentMode, int leftRight)
		{
			if (leftRight == 0)
				return (TangentMode) ((tangentMode & 6) >> 1);
			else
				return (TangentMode) ((tangentMode & 24) >> 3);
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static TangentMode GetKeyTangentMode(Keyframe keyframe, int leftRight)
		{
			Type t = typeof( UnityEngine.Keyframe );
			FieldInfo field = t.GetField( "m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
			int tangentMode =  (int)field.GetValue(keyframe);
			if (leftRight == 0)
				return (TangentMode) ((tangentMode & 6) >> 1);
			else
				return (TangentMode) ((tangentMode & 24) >> 3);
		}
		
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static void SetKeyBroken(object keyframe, bool broken)
		{
			Type t = typeof( UnityEngine.Keyframe );
			FieldInfo field = t.GetField( "m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
			int tangentMode =  (int)field.GetValue(keyframe);
			
			if (broken)
				tangentMode |= 1;
			else
				tangentMode &= -2;
			field.SetValue(keyframe, tangentMode);
		}
		
	}
}


#region BlendTreeExtension
public static class BlendTreeEx {
	
	public static int GetRecursiveBlendParamCount(this BlendTree bt) {
		object val = bt.GetType().GetProperty("recursiveBlendParameterCount", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public).GetValue(bt, new object[]{});
		return (int)val;
	}
	public static string GetRecursiveBlendParam(this BlendTree bt, int index) {
		object val = bt.GetType().GetMethod("GetRecursiveBlendParameter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(bt, new object[]{index});
		return (string)val;
	}
	public static float GetRecursiveBlendParamMax(this BlendTree bt, int index) {
		object val = bt.GetType().GetMethod("GetRecursiveBlendParameterMax", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(bt, new object[]{index});
		return (float)val;
	}
	public static float GetRecursiveBlendParamMin(this BlendTree bt, int index) {
		object val = bt.GetType().GetMethod("GetRecursiveBlendParameterMin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(bt, new object[]{index});
		return (float)val;
	}
	
}

#endregion

#region State Extension
public static class StateEx {
	
	public static void SetMotion(this State state, Motion motion) {
		state.GetType().GetMethod("SetMotionInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[] {typeof(Motion)}, null).Invoke(state, new object[] {motion});
	}
	
}
#endregion


/*****************
 * Linear curve
	AnimationCurve linearCurve = new AnimationCurve();
// Add 2 keyframe on 0.5 and 1.0 seconds with 1 and 2 values
linearCurve.AddKey(KeyframeUtil.GetNew(0.5f, 1.0f, TangentMode.Linear));
linearCurve.AddKey(KeyframeUtil.GetNew(1.0f, 2.0f, TangentMode.Linear));
// If you have at leas one keyframe with TangentMode.Linear you should recalculate tangents after you assign all values
linearCurve.UpdateAllLinearTangents();
// assign this curve to clip
animationClip.SetCurve(gameObject, typeof(Transform),"localPosition.x", linearCurve);


* Constant curve
	This type of curve is very useful for m_IsActive properties (to enable and disable gameobjects)
		AnimationCurve constantCurve = new AnimationCurve();

constantCurve.AddKey(KeyframeUtil.GetNew(0.5f, 0.0f, TangentMode.Linear)); //false on 0.5 second
constantCurve.AddKey(KeyframeUtil.GetNew(1.0f, 1.0f, TangentMode.Linear)); // true on 1.0 second

animationClip.SetCurve(gameObject, typeof(GameObject),"m_IsActive", constantCurve);
*/

#region CurveExtension
public static class CurveExtension {
	
	public static void UpdateAllLinearTangents(this AnimationCurve curve){
		for (int i = 0; i < curve.keys.Length; i++) {
			UpdateTangentsFromMode(curve, i);
		}
	}
	
	// UnityEditor.CurveUtility.cs (c) Unity Technologies
	public static void UpdateTangentsFromMode(AnimationCurve curve, int index)
	{
		if (index < 0 || index >= curve.length)
			return;
		Keyframe key = curve[index];
		if (KeyframeUtil.GetKeyTangentMode(key, 0) == TangentMode.Linear && index >= 1)
		{
			key.inTangent = CalculateLinearTangent(curve, index, index - 1);
			curve.MoveKey(index, key);
		}
		if (KeyframeUtil.GetKeyTangentMode(key, 1) == TangentMode.Linear && index + 1 < curve.length)
		{
			key.outTangent = CalculateLinearTangent(curve, index, index + 1);
			curve.MoveKey(index, key);
		}
		if (KeyframeUtil.GetKeyTangentMode(key, 0) != TangentMode.Smooth && KeyframeUtil.GetKeyTangentMode(key, 1) != TangentMode.Smooth)
			return;
		curve.SmoothTangents(index, 0.0f);
	}
	
	// UnityEditor.CurveUtility.cs (c) Unity Technologies
	private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
	{
		return (float) (((double) curve[index].value - (double) curve[toIndex].value) / ((double) curve[index].time - (double) curve[toIndex].time));
	}
	
}

#endregion


