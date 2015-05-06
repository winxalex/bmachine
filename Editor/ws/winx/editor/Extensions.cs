using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;


namespace ws.winx.editor
{








	#region KeyframeExtension
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


	public static class KeyframeExtension
	{
		
		public static Keyframe GetNew (float time, float value, TangentMode leftAndRight)
		{
			return GetNew (time, value, leftAndRight, leftAndRight);
		}
		
		public static Keyframe GetNew (float time, float value, TangentMode left, TangentMode right)
		{
			Keyframe keyframe = new Keyframe (time, value); // cant use struct in reflection			
			
			keyframe.SetKeyBroken (true);
			SetKeyTangentMode (keyframe, 0, left);
			SetKeyTangentMode (keyframe, 1, right);
			
			
			if (left == TangentMode.Stepped)
				keyframe.inTangent = float.PositiveInfinity;
			if (right == TangentMode.Stepped)
				keyframe.outTangent = float.PositiveInfinity;
			
			return keyframe;
		}
		
		
		
		
		
		
		public static TangentMode GetKeyTangentMode (this Keyframe key, int leftRight)
		{
			if (leftRight == 0)
			{
				return (TangentMode)((key.tangentMode & 6) >> 1);
			}
			return (TangentMode)((key.tangentMode & 24) >> 3);
		}
		
		
		
		public static void SetKeyTangentMode (this Keyframe key, int leftRight, TangentMode mode)
		{
			if (leftRight == 0)
			{
				key.tangentMode &= -7;
				key.tangentMode |= (int)((int)mode << 1);
			}
			else
			{
				key.tangentMode &= -25;
				key.tangentMode |= (int)((int)mode << 3);
			}
			if (key.GetKeyTangentMode (leftRight) != mode)
			{
				Debug.Log ("bug");
			}
		}
		
		
		public static bool GetKeyBroken (this Keyframe key)
		{
			return (key.tangentMode & 1) != 0;
		}
		
		
		
		public static void SetKeyBroken (this Keyframe key, bool broken)
		{
			if (broken)
			{
				key.tangentMode |= 1;
			}
			else
			{
				key.tangentMode &= -2;
			}
		}
		
	}
	#endregion
	
	
	
	
	#region Curve Extension
	public static class CurveExtension
	{
		
		public static void UpdateAllLinearTangents (this AnimationCurve curve)
		{
			for (int i = 0; i < curve.keys.Length; i++) {
				UpdateTangentsFromMode (curve, i);
			}
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static void UpdateTangentsFromMode (AnimationCurve curve, int index)
		{
			if (index < 0 || index >= curve.length)
				return;
			Keyframe key = curve [index];
			if (key.GetKeyTangentMode (0) == TangentMode.Linear && index >= 1) {
				key.inTangent = CalculateLinearTangent (curve, index, index - 1);
				curve.MoveKey (index, key);
			}
			if (key.GetKeyTangentMode (1) == TangentMode.Linear && index + 1 < curve.length) {
				key.outTangent = CalculateLinearTangent (curve, index, index + 1);
				curve.MoveKey (index, key);
			}
			if (key.GetKeyTangentMode (0) != TangentMode.Smooth && key.GetKeyTangentMode (1) != TangentMode.Smooth)
				return;
			curve.SmoothTangents (index, 0.0f);
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		private static float CalculateLinearTangent (AnimationCurve curve, int index, int toIndex)
		{
			return (float)(((double)curve [index].value - (double)curve [toIndex].value) / ((double)curve [index].time - (double)curve [toIndex].time));
		}
		
		
		public static void SetKeyModeFromContext (this AnimationCurve curve, int keyIndex)
		{
			Keyframe key = curve [keyIndex];
			bool flag = false;
			if (keyIndex > 0 && curve [keyIndex - 1].GetKeyBroken ())
			{
				flag = true;
			}
			if (keyIndex < curve.length - 1 && curve [keyIndex + 1].GetKeyBroken ())
			{
				flag = true;
			}
			key.SetKeyBroken (flag);
			if (flag)
			{
				if (keyIndex > 0)
				{
					key.SetKeyTangentMode (0, curve [keyIndex - 1].GetKeyTangentMode (1));
				}
				if (keyIndex < curve.length - 1)
				{
					key.SetKeyTangentMode (1, curve [keyIndex + 1].GetKeyTangentMode (0));
				}
			}
			else
			{
				TangentMode mode = TangentMode.Smooth;
				if (keyIndex > 0 && curve [keyIndex - 1].GetKeyTangentMode (1) != TangentMode.Smooth)
				{
					mode = TangentMode.Editable;
				}
				if (keyIndex < curve.length - 1 && curve [keyIndex + 1].GetKeyTangentMode (0) != TangentMode.Smooth)
				{
					mode = TangentMode.Editable;
				}
				key.SetKeyTangentMode ( 0, mode);
				key.SetKeyTangentMode (1, mode);
			}
			curve.MoveKey (keyIndex, key);
		}
		
	}
	
	#endregion
	
	#region AnimatorExtension
	public static class AnimatorExtension
	{
		static Type _RealType;
		
		static MethodInfo _IsBoneTransform_MethodInfo;
		
		
		static MethodInfo IsBoneTransform_MethodInfo {
			get {
				if(_IsBoneTransform_MethodInfo==null)
					_IsBoneTransform_MethodInfo=RealType.GetMethod("IsBoneTransform",new Type[]{typeof(Transform)});
				
				return _IsBoneTransform_MethodInfo;
			}
		}
		
		static Type RealType {
			get {
				if (_RealType == null)
					_RealType = typeof(UnityEngine.Animator);
				return _RealType;
			}
		}
		
		
		
		public static bool IsBoneTransform (this Animator animator,Transform transform)
		{
			
			
			return (bool)IsBoneTransform_MethodInfo.Invoke (animator, new object[]{transform});
			
		}
	}
	#endregion
	
	
	
	
	
	#region BlendTreeExtension
	
	
	
	public static class BlendTreeExtension
	{
		static Type _RealType;
		
		static Type RealType {
			get {
				if (_RealType == null)
					_RealType = typeof(UnityEditor.Animations.BlendTree);
				return _RealType;
			}
		}
		
		static MethodInfo _GetRecursiveBlendParameter_MethodInfo;
		static PropertyInfo _GetRecursiveBlendParameterCount_PropertyInfo;
		static MethodInfo _GetRecursiveBlendParameterMin_MethodInfo;
		static MethodInfo _GetRecursiveBlendParameterMax_MethodInfo;
		
		public static MethodInfo GetRecursiveBlendParameter_MethodInfo {
			get {
				if (_GetRecursiveBlendParameter_MethodInfo == null)
					_GetRecursiveBlendParameter_MethodInfo = RealType.GetMethod ("GetRecursiveBlendParameter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				return _GetRecursiveBlendParameter_MethodInfo;
			}
		}
		
		
		public static MethodInfo GetRecursiveBlendParameterMin_MethodInfo {
			get {
				if (_GetRecursiveBlendParameterMin_MethodInfo == null)
					_GetRecursiveBlendParameterMin_MethodInfo = RealType.GetMethod ("GetRecursiveBlendParameterMin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				return _GetRecursiveBlendParameterMin_MethodInfo;
			}
		}
		
		
		public static MethodInfo GetRecursiveBlendParameterMax_MethodInfo {
			get {
				if (_GetRecursiveBlendParameterMax_MethodInfo == null)
					_GetRecursiveBlendParameterMax_MethodInfo = RealType.GetMethod ("GetRecursiveBlendParameterMax", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				return _GetRecursiveBlendParameterMax_MethodInfo;
			}
		}
		
		
		
		public static PropertyInfo GetRecursiveBlendParameterCount_PropertyInfo {
			get {
				if (_GetRecursiveBlendParameterCount_PropertyInfo == null)
					_GetRecursiveBlendParameterCount_PropertyInfo = RealType.GetProperty ("recursiveBlendParameterCount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				return _GetRecursiveBlendParameterCount_PropertyInfo;
			}
		}
		
		public static int GetRecursiveBlendParamCount (this UnityEditor.Animations.BlendTree bt)
		{
			object val = GetRecursiveBlendParameterCount_PropertyInfo.GetValue (bt, new object[]{});
			return (int)val;
		}
		
		public static string GetRecursiveBlendParam (this UnityEditor.Animations.BlendTree bt, int index)
		{
			object val = GetRecursiveBlendParameter_MethodInfo.Invoke (bt, new object[]{index});
			return (string)val;
		}
		
		public static float GetRecursiveBlendParamMax (this UnityEditor.Animations.BlendTree bt, int index)
		{
			object val = GetRecursiveBlendParameterMax_MethodInfo.Invoke (bt, new object[]{index});
			return (float)val;
		}
		
		public static float GetRecursiveBlendParamMin (this UnityEditor.Animations.BlendTree bt, int index)
		{
			object val =GetRecursiveBlendParameterMin_MethodInfo.Invoke (bt, new object[]{index});
			return (float)val;
		}
		
	}
	#endregion

}
