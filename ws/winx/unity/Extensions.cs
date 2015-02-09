using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace ws.winx.unity
{


	
	
	
	
	
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
	
	public class KeyframeUtil
	{
		
		public static Keyframe GetNew (float time, float value, TangentMode leftAndRight)
		{
			return GetNew (time, value, leftAndRight, leftAndRight);
		}
		
		public static Keyframe GetNew (float time, float value, TangentMode left, TangentMode right)
		{
			object boxed = new Keyframe (time, value); // cant use struct in reflection			
			
			SetKeyBroken (boxed, true);
			SetKeyTangentMode (boxed, 0, left);
			SetKeyTangentMode (boxed, 1, right);
			
			Keyframe keyframe = (Keyframe)boxed;
			if (left == TangentMode.Stepped)
				keyframe.inTangent = float.PositiveInfinity;
			if (right == TangentMode.Stepped)
				keyframe.outTangent = float.PositiveInfinity;
			
			return keyframe;
		}
		
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static void SetKeyTangentMode (object keyframe, int leftRight, TangentMode mode)
		{
			
			Type t = typeof(UnityEngine.Keyframe);
			FieldInfo field = t.GetField ("m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			int tangentMode = (int)field.GetValue (keyframe);
			
			if (leftRight == 0) {
				tangentMode &= -7;
				tangentMode |= (int)mode << 1;
			} else {
				tangentMode &= -25;
				tangentMode |= (int)mode << 3;
			}
			
			field.SetValue (keyframe, tangentMode);
			if (GetKeyTangentMode (tangentMode, leftRight) == mode)
				return;
			Debug.Log ("bug"); 
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static TangentMode GetKeyTangentMode (int tangentMode, int leftRight)
		{
			if (leftRight == 0)
				return (TangentMode)((tangentMode & 6) >> 1);
			else
				return (TangentMode)((tangentMode & 24) >> 3);
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static TangentMode GetKeyTangentMode (Keyframe keyframe, int leftRight)
		{
			Type t = typeof(UnityEngine.Keyframe);
			FieldInfo field = t.GetField ("m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			int tangentMode = (int)field.GetValue (keyframe);
			if (leftRight == 0)
				return (TangentMode)((tangentMode & 6) >> 1);
			else
				return (TangentMode)((tangentMode & 24) >> 3);
		}
		
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		public static void SetKeyBroken (object keyframe, bool broken)
		{
			Type t = typeof(UnityEngine.Keyframe);
			FieldInfo field = t.GetField ("m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			int tangentMode = (int)field.GetValue (keyframe);
			
			if (broken)
				tangentMode |= 1;
			else
				tangentMode &= -2;
			field.SetValue (keyframe, tangentMode);
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
			if (KeyframeUtil.GetKeyTangentMode (key, 0) == TangentMode.Linear && index >= 1) {
				key.inTangent = CalculateLinearTangent (curve, index, index - 1);
				curve.MoveKey (index, key);
			}
			if (KeyframeUtil.GetKeyTangentMode (key, 1) == TangentMode.Linear && index + 1 < curve.length) {
				key.outTangent = CalculateLinearTangent (curve, index, index + 1);
				curve.MoveKey (index, key);
			}
			if (KeyframeUtil.GetKeyTangentMode (key, 0) != TangentMode.Smooth && KeyframeUtil.GetKeyTangentMode (key, 1) != TangentMode.Smooth)
				return;
			curve.SmoothTangents (index, 0.0f);
		}
		
		// UnityEditor.CurveUtility.cs (c) Unity Technologies
		private static float CalculateLinearTangent (AnimationCurve curve, int index, int toIndex)
		{
			return (float)(((double)curve [index].value - (double)curve [toIndex].value) / ((double)curve [index].time - (double)curve [toIndex].time));
		}
		
	}
	
	#endregion

	#region State Extension
		public static class StateEx
		{
		
				public static void SetMotion (this State state, Motion motion)
				{
						state.GetType ().GetMethod ("SetMotionInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[] {typeof(Motion)}, null).Invoke (state, new object[] {motion});
				}
		
		}
	#endregion

	#region BlendTreeExtension
		public static class BlendTreeExtension
		{
	
				public static int GetRecursiveBlendParamCount (this BlendTree bt)
				{
						object val = bt.GetType ().GetProperty ("recursiveBlendParameterCount", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public).GetValue (bt, new object[]{});
						return (int)val;
				}

				public static string GetRecursiveBlendParam (this BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (string)val;
				}

				public static float GetRecursiveBlendParamMax (this BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameterMax", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (float)val;
				}

				public static float GetRecursiveBlendParamMin (this BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameterMin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (float)val;
				}
	
		}
	#endregion

}
