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
	
	//private object instance;
		private object instance;
//		private AvatarPreview my;
	
	public delegate void OnAvatarChange();
	
	public AvatarPreviewW(Animator previewObjectInScene, Motion objectOnSameAsset) {
		InitType();
		
		instance = method_ctor.Invoke( new object[] { previewObjectInScene, objectOnSameAsset } );
		
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
			MethodInfo_AvatarTimeControlGUI.Invoke (instance,new object[]{rect});
		}

     	public Texture DoRenderPreview (Rect previewRect, GUIStyle background){
			return MethodInfo_DoRenderPreview.Invoke (instance, new object[]{previewRect,background}) as Texture;
		}

		void Init ()
		{
			MethodInfo_Init.Invoke (instance, null);
		}
	
	public Animator Animator
	{
		get
		{
			return PropertyInfo_Animator.GetValue(instance, null) as Animator;
		}
	}
	public bool IKOnFeet {
		get {
			return (bool)PropertyInfo_IKOnFeet.GetValue(instance, null);
		}
	}
	
	public OnAvatarChange OnAvatarChangeFunc {
		set {
			PropertyInfo_OnAvatarChangeFunc.SetValue(instance, Delegate.CreateDelegate(PropertyInfo_OnAvatarChangeFunc.PropertyType, value.Target, value.Method), null);
		}
	}
	
	public void DoPreviewSettings() {
		MethodInfo_DoPreviewSettings.Invoke(instance, null);
	}
	
	public void OnDestroy() {
		MethodInfo_OnDestroy.Invoke(instance, null);
	}
	
	public void DoAvatarPreview(Rect rect, GUIStyle background) {
		MethodInfo_DoAvatarPreview.Invoke(instance, new object[] { rect, background });
	}
	
	public TimeControlW timeControl {
		get {
			return new TimeControlW(FieldInfo_timeControl.GetValue(instance));
		}
	}
	#endregion
	
}

public class TimeControlW {
	private static Type realType;
	private object instance;
	
	private static FieldInfo field_currentTime;
	private static FieldInfo field_loop;
	private static FieldInfo field_startTime;
	private static FieldInfo field_stopTime;
	private static MethodInfo method_Update;
	private static PropertyInfo property_deltaTime;
	private static PropertyInfo property_normalizedTime;
	private static PropertyInfo property_playing;
	private static PropertyInfo property_nextCurrentTime;
	
	public static void InitType() {
		if (realType == null) {
			Assembly assembly = Assembly.GetAssembly(typeof(Editor));
			realType = assembly.GetType("UnityEditor.TimeControl");
			
			field_currentTime = realType.GetField("currentTime");
			field_loop = realType.GetField("loop");
			field_startTime = realType.GetField("startTime");
			field_stopTime = realType.GetField("stopTime");
			method_Update = realType.GetMethod("Update");
			property_deltaTime = realType.GetProperty("deltaTime");
			property_normalizedTime = realType.GetProperty("normalizedTime");
			property_playing = realType.GetProperty("playing");
			property_nextCurrentTime = realType.GetProperty("nextCurrentTime");
		}
	}
	
	public TimeControlW(object realTimeControl) {
		InitType();
		this.instance = realTimeControl;
	}
	
	public float currentTime {
		get {
			return (float)field_currentTime.GetValue(instance);
		}
		set {
			field_currentTime.SetValue(instance, value);
		}
	}
	
	public bool loop {
		get {
			return (bool)field_loop.GetValue(instance);
		}
		set {
			field_loop.SetValue(instance, value);
		}
	}
	
	public float startTime {
		get {
			return (float)field_startTime.GetValue(instance);
		}
		set {
			field_startTime.SetValue(instance, value);
		}
	}
	
	public float stopTime {
		get {
			return (float)field_stopTime.GetValue(instance);
		}
		set {
			field_stopTime.SetValue(instance, value);
		}
	}
	
	public float deltaTime {
		get {
			return (float)property_deltaTime.GetValue(instance, null);
		}
		set {
			property_deltaTime.SetValue(instance, value, null);
		}
	}
	
	public float normalizedTime {
		get {
			return (float)property_normalizedTime.GetValue(instance, null);
		}
		set {
			property_normalizedTime.SetValue(instance, value, null);
		}
	}
	
	public bool playing {
		get {
			return (bool)property_playing.GetValue(instance, null);
		}
		set {
			property_playing.SetValue(instance, value, null);
		}
	}
	
	public float nextCurrentTime {
		set {
			property_nextCurrentTime.SetValue(instance, value, null);
		}
	}
	
	public void Update() {
		method_Update.Invoke(instance, null);
	}
}



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

public static class StateEx {
	
	public static void SetMotion(this State state, Motion motion) {
		state.GetType().GetMethod("SetMotionInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[] {typeof(Motion)}, null).Invoke(state, new object[] {motion});
	}
	
}



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




