using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;



namespace ws.winx.editor
{
public class AnimationModeUtility
{

	public static object GetCurrentValue (GameObject rootGameObject, EditorCurveBinding curveBinding)
	{
		if (curveBinding.isPPtrCurve) {
			UnityEngine.Object result;
			AnimationUtility.GetObjectReferenceValue (rootGameObject, curveBinding, out result);
			return result;
		}
		float num;
		AnimationUtility.GetFloatValue (rootGameObject, curveBinding, out num);
		return num;
	}
	
	
	
	
	
	
	
	
	
	
	
	private static void AddKey (float time,GameObject rootGameObject, AnimationClip activeAnimationClip, EditorCurveBinding binding,Type type, PropertyModification modification)
	{
		
		object currentValue = GetCurrentValue (rootGameObject, binding);
		
		//Debug.Log (rootGameObject.transform.position + " " + binding.propertyName+"="+currentValue);
		object value = null;
		
		ObjectReferenceKeyframe[] keyframesCurveReferenced;
		bool isCurveReferenced = binding.isPPtrCurve;
		AnimationCurve curve;
		int frameCurrent=(int)(time*activeAnimationClip.frameRate);
		
		if (isCurveReferenced) {
			keyframesCurveReferenced = AnimationUtility.GetObjectReferenceCurve (activeAnimationClip, binding);
			
			
			
			if (keyframesCurveReferenced.Length == 0 && frameCurrent != 0) {
				if (!ValueFromPropertyModification (modification, binding, out value)) {
					value = currentValue;
				}
				
				AddKeyframeToObjectReferenceCurve (keyframesCurveReferenced,activeAnimationClip,binding, value, type, 0);
				
			}
			
			AddKeyframeToObjectReferenceCurve (keyframesCurveReferenced,activeAnimationClip,binding, currentValue, type, time);
			
			
		} else {
			curve = AnimationUtility.GetEditorCurve (activeAnimationClip, binding);
			
			if(curve==null)
				curve=new AnimationCurve();
			
			if (curve.length == 0 && frameCurrent != 0) {
				if (!ValueFromPropertyModification (modification, binding, out value)) {
					value = currentValue;
				}
				
				AddKeyframeToCurve (curve,activeAnimationClip,binding,(float) value, type, 0);
			}
			
			
			AddKeyframeToCurve (curve,activeAnimationClip,binding,(float) currentValue, type, time);
			
		}
		
		
	}
	
	
	
	public static void AddKeyframeToObjectReferenceCurve (ObjectReferenceKeyframe[] keyframes,AnimationClip animatedClip,EditorCurveBinding binding, object value, Type type, float time)
	{
		int keyframeIndex = Array.FindIndex (keyframes, (itm) => (int)itm.time * animatedClip.frameRate == (int)time * animatedClip.frameRate);
		
		//no keyframe found
		if (keyframeIndex <0) {
			List<ObjectReferenceKeyframe> list=  keyframes.ToList();
			
			ObjectReferenceKeyframe objectReferenceKeyframe = default(ObjectReferenceKeyframe);
			objectReferenceKeyframe.time = time;
			objectReferenceKeyframe.value = value as UnityEngine.Object;
			list.Add (objectReferenceKeyframe);
			
			keyframes=list.ToArray();
			
			
		}else{
			//??? maybe I should add new time too
			keyframes[keyframeIndex].value=value as UnityEngine.Object;
			
			
		}
		
		
		//save
		AnimationUtility.SetObjectReferenceCurve (animatedClip, binding,keyframes);
	}
	
	
	public static void AddKeyframeToCurve (AnimationCurve animationCurve,AnimationClip animatedClip,EditorCurveBinding binding, float value, Type type, float time)
	{
		
		int keyframeIndex = Array.FindIndex (animationCurve.keys, (itm) => (int)itm.time * animatedClip.frameRate == (int)time * animatedClip.frameRate);
		Keyframe key = default(Keyframe);
		if (keyframeIndex < 0) {
			if (type == typeof(bool) || type == typeof(float)) {
				
				key= new Keyframe (time, value);
				if (type == typeof(bool)) {
					//CurveUtility.SetKeyTangentMode (ref key, 0, TangentMode.Stepped);
					//CurveUtility.SetKeyTangentMode (ref key, 1, TangentMode.Stepped);
					//CurveUtility.SetKeyBroken (ref key, true);
					key.SetKeyTangentMode (0, TangentMode.Stepped);
					key.SetKeyTangentMode (1, TangentMode.Stepped);
					key.SetKeyBroken (true);
					
					Debug.Log("type bool "+keyframeIndex);
					
				} else {
					int num = animationCurve.AddKey (key);
					if (num != -1) {
						animationCurve.SetKeyModeFromContext (num);
						Debug.Log("type float "+keyframeIndex);
					}
				}
				
			}
			
		} else {
			
			
			
			//??? maybe I should add new time too
			//animationCurve.keys[keyframeIndex].value=value;
			key=animationCurve.keys[keyframeIndex];
			key.value=value;
			animationCurve.MoveKey(keyframeIndex,key);
			
		}
		
		
		//Save changes
		SaveCurve (animationCurve, animatedClip, binding);
		
		
		
		
		
		
		
	}
	
	
	public static void SaveCurve (AnimationCurve animationCurve,AnimationClip animatedClip,EditorCurveBinding binding)
	{
		Undo.RegisterCompleteObjectUndo (animatedClip, "Edit Curve");
		
		QuaternionCurveTangentCalculationW.UpdateTangentsFromMode (animationCurve, animatedClip, binding);
		
		
		AnimationUtility.SetEditorCurve(animatedClip, binding, animationCurve);
	}
	
	private static bool ValueFromPropertyModification (PropertyModification modification, EditorCurveBinding binding, out object outObject)
	{
		if (modification == null) {
			outObject = null;
			return false;
		}
		if (binding.isPPtrCurve) {
			outObject = modification.objectReference;
			return true;
		}
		float num;
		if (float.TryParse (modification.value, out num)) {
			outObject = num;
			return true;
		}
		outObject = null;
		return false;
	}
	
	private static PropertyModification FindPropertyModification (GameObject root, UndoPropertyModification[] modifications, EditorCurveBinding binding)
	{
		for (int i = 0; i < modifications.Length; i++) {
			EditorCurveBinding lhs;
			AnimationUtility.PropertyModificationToEditorCurveBinding (modifications [i].propertyModification, root, out lhs);
			if (lhs == binding) {
				return modifications [i].propertyModification;
			}
		}
		return null;
	}
	
	
	
	public static UndoPropertyModification[] Process (GameObject rootGameObject, AnimationClip activeAnimationClip, UndoPropertyModification[] modifications,float time)
	{
		
		Animator component = rootGameObject.GetComponent<Animator> ();
		if (!HasAnyRecordableModifications (rootGameObject, modifications)) {
			return modifications;
		}
		List<UndoPropertyModification> list = new List<UndoPropertyModification> ();
		for (int i = 0; i < modifications.Length; i++) {
			EditorCurveBinding binding = default(EditorCurveBinding);
			PropertyModification propertyModification = modifications [i].propertyModification;
			Type type = AnimationUtility.PropertyModificationToEditorCurveBinding (propertyModification, rootGameObject, out binding);
			if (type != null) {
				
				if (component != null && component.isHuman && binding.type == typeof(Transform) && component.IsBoneTransform (propertyModification.target as Transform)) {
					Debug.LogWarning ("Keyframing for humanoid rig is not supported!", propertyModification.target as Transform);
				} else {
					AnimationMode.AddPropertyModification (binding, propertyModification, modifications [i].keepPrefabOverride);
					EditorCurveBinding[] array = RotationCurveInterpolationW.RemapAnimationBindingForAddKey (binding, activeAnimationClip);
					if (array != null) {
						for (int j = 0; j < array.Length; j++) {
							AddKey (time,rootGameObject, activeAnimationClip, array [j], type, FindPropertyModification (rootGameObject, modifications, array [j]));
						}
					} else {
						AddKey (time,rootGameObject, activeAnimationClip, binding, type, propertyModification);
					}
				}
			} else {
				list.Add (modifications [i]);
			}
		}
		return list.ToArray ();
	}
	
	private static bool HasAnyRecordableModifications (GameObject root, UndoPropertyModification[] modifications)
	{
		for (int i = 0; i < modifications.Length; i++) {
			EditorCurveBinding editorCurveBinding;
			if (AnimationUtility.PropertyModificationToEditorCurveBinding (modifications [i].propertyModification, root, out editorCurveBinding) != null) {
				return true;
			}
		}
		return false;
	}
}
}

