using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;
using System.Reflection;

namespace ws.winx.unity
{
public class AvatarPreviewW {
	#region Reflection
	
	private static Type realType;
	
	private static ConstructorInfo method_ctor;
	private static PropertyInfo property_OnAvatarChangeFunc;
	private static PropertyInfo property_IKOnFeet;
	private static PropertyInfo property_Animator;
	private static MethodInfo method_DoPreviewSettings;
	private static MethodInfo method_OnDestroy;
	private static MethodInfo method_DoAvatarPreview;
	private static MethodInfo MethodInfo_DoRenderPreview;
	private static MethodInfo MethodInfo_Init;
	private static MethodInfo MethodInfo_AvatarTimeControlGUI;
	private static FieldInfo field_timeControl;

		Texture image = null;

		Rect lastRect;	
	
	public static void InitType() {

		if (realType == null) {
			Assembly assembly = Assembly.GetAssembly(typeof(Editor));
			realType = assembly.GetType("UnityEditor.AvatarPreview");
			
			method_ctor 					= realType.GetConstructor(new Type[] { typeof(Animator), typeof(Motion)});
			property_OnAvatarChangeFunc 	= realType.GetProperty("OnAvatarChangeFunc");
			property_IKOnFeet				= realType.GetProperty("IKOnFeet");
			property_Animator				= realType.GetProperty("Animator");
			method_DoPreviewSettings		= realType.GetMethod("DoPreviewSettings");
			method_OnDestroy				= realType.GetMethod("OnDestroy");
			method_DoAvatarPreview			= realType.GetMethod("DoAvatarPreview", new Type[] {typeof(Rect), typeof(GUIStyle)});
			field_timeControl				= realType.GetField("timeControl");
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
			//my = (AvatarPreview)instance;
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
			return property_Animator.GetValue(instance, null) as Animator;
		}
	}
	public bool IKOnFeet {
		get {
			return (bool)property_IKOnFeet.GetValue(instance, null);
		}
	}
	
	public OnAvatarChange OnAvatarChangeFunc {
		set {
			property_OnAvatarChangeFunc.SetValue(instance, Delegate.CreateDelegate(property_OnAvatarChangeFunc.PropertyType, value.Target, value.Method), null);
		}
	}
	
	public void DoPreviewSettings() {
		method_DoPreviewSettings.Invoke(instance, null);
	}
	
	public void OnDestroy() {
		method_OnDestroy.Invoke(instance, null);
	}
	
	public void DoAvatarPreview(Rect rect, GUIStyle background) {
		method_DoAvatarPreview.Invoke(instance, new object[] { rect, background });
	}
	
	public TimeControlW timeControl {
		get {
			return new TimeControlW(field_timeControl.GetValue(instance));
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
}