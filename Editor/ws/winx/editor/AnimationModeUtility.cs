using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using ws.winx.unity;
using ws.winx.editor.utilities;

namespace ws.winx.editor
{
		public class AnimationModeUtility
		{
				/// <summary>
				/// Gets the current value.
				/// </summary>
				/// <returns>The current value.</returns>
				/// <param name="rootGameObject">Root game object.</param>
				/// <param name="curveBinding">Curve binding.</param>
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
	

				/// <summary>
				/// Adds the key.
				/// </summary>
				/// <param name="time">Time.</param>
				/// <param name="rootGameObject">Root game object.</param>
				/// <param name="activeAnimationClip">Active animation clip.</param>
				/// <param name="binding">Binding.</param>
				/// <param name="type">Type.</param>
				/// <param name="modification">Modification.</param>
				private static void AddKey (float time, GameObject rootGameObject, AnimationClip activeAnimationClip, EditorCurveBinding binding, Type type, PropertyModification modification)
				{
		
						object currentValue = GetCurrentValue (rootGameObject, binding);
		
						//Debug.Log (rootGameObject.transform.position + " " + binding.propertyName+"="+currentValue);
						object value = null;
		
						ObjectReferenceKeyframe[] keyframesCurveReferenced;
						bool isCurveReferenced = binding.isPPtrCurve;
						AnimationCurve curve;
						int frameCurrent = (int)(time * activeAnimationClip.frameRate);
		
						if (isCurveReferenced) {
								keyframesCurveReferenced = AnimationUtility.GetObjectReferenceCurve (activeAnimationClip, binding);
			
			
			
								if (keyframesCurveReferenced.Length == 0 && frameCurrent != 0) {
										if (!ValueFromPropertyModification (modification, binding, out value)) {
												value = currentValue;
										}
				
										AddKeyframeToObjectReferenceCurve (keyframesCurveReferenced, activeAnimationClip, binding, value, type, 0);
				
								}
			
								AddKeyframeToObjectReferenceCurve (keyframesCurveReferenced, activeAnimationClip, binding, currentValue, type, time);
			
			
						} else {
								curve = AnimationUtility.GetEditorCurve (activeAnimationClip, binding);
			
								if (curve == null)
										curve = new AnimationCurve ();
			
								if (curve.length == 0 && frameCurrent != 0) {
										if (!ValueFromPropertyModification (modification, binding, out value)) {
												value = currentValue;
										}
				
										AddKeyframeToCurve (curve, activeAnimationClip, binding, (float)value, type, 0);
								}
			
			
								AddKeyframeToCurve (curve, activeAnimationClip, binding, (float)currentValue, type, time);
			
						}
		
		
				}
	

				/// <summary>
				/// Adds the keyframe to object reference curve.
				/// </summary>
				/// <param name="keyframes">Keyframes.</param>
				/// <param name="animatedClip">Animated clip.</param>
				/// <param name="binding">Binding.</param>
				/// <param name="value">Value.</param>
				/// <param name="type">Type.</param>
				/// <param name="time">Time.</param>
				public static void AddKeyframeToObjectReferenceCurve (ObjectReferenceKeyframe[] keyframes, AnimationClip animatedClip, EditorCurveBinding binding, object value, Type type, float time)
				{
						int keyframeIndex = Array.FindIndex (keyframes, (itm) => (int)itm.time * animatedClip.frameRate == (int)time * animatedClip.frameRate);
		
						//no keyframe found
						if (keyframeIndex < 0) {
								List<ObjectReferenceKeyframe> list = keyframes.ToList ();
			
								ObjectReferenceKeyframe objectReferenceKeyframe = default(ObjectReferenceKeyframe);
								objectReferenceKeyframe.time = time;
								objectReferenceKeyframe.value = value as UnityEngine.Object;
								list.Add (objectReferenceKeyframe);
			
								keyframes = list.ToArray ();
			
			
						} else {
								//??? maybe I should add new time too
								keyframes [keyframeIndex].value = value as UnityEngine.Object;
			
			
						}
		
		
						//save
						AnimationUtility.SetObjectReferenceCurve (animatedClip, binding, keyframes);
				}
	

				/// <summary>
				/// Adds the keyframe to curve.
				/// </summary>
				/// <param name="animationCurve">Animation curve.</param>
				/// <param name="animatedClip">Animated clip.</param>
				/// <param name="binding">Binding.</param>
				/// <param name="value">Value.</param>
				/// <param name="type">Type.</param>
				/// <param name="time">Time.</param>
				public static void AddKeyframeToCurve (AnimationCurve animationCurve, AnimationClip animatedClip, EditorCurveBinding binding, float value, Type type, float time)
				{
						//frame comparing (frame=(int)(time*animatedClip.frameRate)
						int keyframeIndex = Array.FindIndex (animationCurve.keys, (itm) => (int)(itm.time * animatedClip.frameRate) == (int)(time * animatedClip.frameRate));
						Keyframe key = default(Keyframe);
						if (keyframeIndex < 0) {
								if (type == typeof(bool) || type == typeof(float)) {
				
										key = new Keyframe (time, value);
										if (type == typeof(bool)) {
												//CurveUtility.SetKeyTangentMode (ref key, 0, TangentMode.Stepped);
												//CurveUtility.SetKeyTangentMode (ref key, 1, TangentMode.Stepped);
												//CurveUtility.SetKeyBroken (ref key, true);
												key.SetKeyTangentMode (0, TangentMode.Stepped);
												key.SetKeyTangentMode (1, TangentMode.Stepped);
												key.SetKeyBroken (true);
					
					
					
										} else {
												int num = animationCurve.AddKey (key);
												if (num != -1) {
														animationCurve.SetKeyModeFromContext (num);
						
												}
										}
				
								}
			
						} else {
			
			
			
								//??? maybe I should add new time too
								//animationCurve.keys[keyframeIndex].value=value;
								key = animationCurve.keys [keyframeIndex];
								key.value = value;
								animationCurve.MoveKey (keyframeIndex, key);
			
						}
		
		
						//Save changes
						SaveCurve (animationCurve, animatedClip, binding);
		
		
		
		
		
		
		
				}
	
				/// <summary>
				/// Saves the curve.
				/// </summary>
				/// <param name="animationCurve">Animation curve.</param>
				/// <param name="animatedClip">Animated clip.</param>
				/// <param name="binding">Binding.</param>
				public static void SaveCurve (AnimationCurve animationCurve, AnimationClip animatedClip, EditorCurveBinding binding)
				{
						Undo.RegisterCompleteObjectUndo (animatedClip, "Edit Curve");
		
						QuaternionCurveTangentCalculationW.UpdateTangentsFromMode (animationCurve, animatedClip, binding);
		
		
						AnimationUtility.SetEditorCurve (animatedClip, binding, animationCurve);
				}
	

				/// <summary>
				/// Values from property modification.
				/// </summary>
				/// <returns><c>true</c>, if from property modification was valued, <c>false</c> otherwise.</returns>
				/// <param name="modification">Modification.</param>
				/// <param name="binding">Binding.</param>
				/// <param name="outObject">Out object.</param>
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
	

				/// <summary>
				/// Finds the property modification.
				/// </summary>
				/// <returns>The property modification.</returns>
				/// <param name="root">Root.</param>
				/// <param name="modifications">Modifications.</param>
				/// <param name="binding">Binding.</param>
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
	

				/// <summary>
				/// Process the specified rootGameObject, activeAnimationClip, modifications and time.
				/// </summary>
				/// <param name="rootGameObject">Root game object.</param>
				/// <param name="activeAnimationClip">Active animation clip.</param>
				/// <param name="modifications">Modifications.</param>
				/// <param name="time">Time.</param>
				public static UndoPropertyModification[] Process (GameObject rootGameObject, AnimationClip activeAnimationClip, UndoPropertyModification[] modifications, float time)
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
				
//										if (component != null && component.isHuman && binding.type == typeof(Transform) && component.IsBoneTransform (propertyModification.target as Transform)) {
//												Debug.LogWarning ("Keyframing for humanoid rig is not supported!", propertyModification.target as Transform);
//										} else 
//										{
												AnimationMode.AddPropertyModification (binding, propertyModification, modifications [i].keepPrefabOverride);
												EditorCurveBinding[] array = RotationCurveInterpolationW.RemapAnimationBindingForAddKey (binding, activeAnimationClip);
												if (array != null) {
														for (int j = 0; j < array.Length; j++) {
																AddKey (time, rootGameObject, activeAnimationClip, array [j], type, FindPropertyModification (rootGameObject, modifications, array [j]));
														}
												} else {
														AddKey (time, rootGameObject, activeAnimationClip, binding, type, propertyModification);
												}
										//}
								} else {
										list.Add (modifications [i]);
								}
						}
						return list.ToArray ();
				}
	

				/// <summary>
				/// Determines if has any recordable modifications the specified root modifications.
				/// </summary>
				/// <returns><c>true</c> if has any recordable modifications the specified root modifications; otherwise, <c>false</c>.</returns>
				/// <param name="root">Root.</param>
				/// <param name="modifications">Modifications.</param>
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



					



				/// <summary>
				/// Resamples the animation.
				/// </summary>
				/// <param name="clipBindings">Clip bindings.</param>
				/// <param name="time">Time.</param>
				public static void SampleClipBindingAt (EditorClipBinding[] clipBindings, float time)
				{
						
						
						
						
						Undo.FlushUndoRecordObjects ();
						
						
						AnimationMode.BeginSampling ();
						
						int len = clipBindings.Length;
						
						EditorClipBinding clipBindingCurrent;
						
						
						for (int i=0; i<len; i++) {
								clipBindingCurrent = clipBindings [i];

								if (clipBindingCurrent.clip != null) {
										AnimationMode.SampleAnimationClip (clipBindingCurrent.gameObject, clipBindingCurrent.clip, time);

										//Correction is needed for root bones as animation doesn't respect current GameObject transform position,rotation
										//=> shifting current boneTransform position as result of clip animation, to offset of orginal position before animation
										if (clipBindingCurrent.boneTransform != null) {
									
									
												clipBindingCurrent.boneTransform.transform.position = clipBindingCurrent.boneRootPositionOffset + clipBindingCurrent.boneTransform.transform.position;
									
									
									
										}
								}

							
								
						}
						
						
						
						
						
						AnimationMode.EndSampling ();
						SceneView.RepaintAll ();
						
						
				}

			


				/// <summary>
				/// Resamples the animation. !!! Doesn't apply position,rotation correction with offset calculate by SetBindingsOffset
				/// </summary>
				/// <param name="animatedObjects">Animated objects.</param>
				/// <param name="animationClips">Animation clips.</param>
				/// <param name="time">Time.</param>
				public static void SampleClipBindingAt (GameObject[] animatedObjects, AnimationClip[] animationClips, ref float time)
				{

			
						Undo.FlushUndoRecordObjects ();
			
			
						AnimationMode.BeginSampling ();
			
						int len = animationClips.Length;
			
						if (len != animatedObjects.Length) {
								len = 0;
								Debug.LogError ("AnimaitonModeUtility.ResampleAnimation> Number of Animate Object should be same with Animation Clips");
						}
			
						for (int i=0; i<len; i++) {
								AnimationMode.SampleAnimationClip (animatedObjects [i], animationClips [i], time);

								
						}
			

			
						AnimationMode.EndSampling ();
						SceneView.RepaintAll ();
			
			
				}


				public static void SampleClipBindingAt (IList<GameObject> animatedObjects, IList<AnimationClip> animationClips, IList<float> times)
				{
					
					
					Undo.FlushUndoRecordObjects ();


					
					
					
					AnimationMode.BeginSampling ();
					
					int len = animationClips.Count;
					
					if (len != animatedObjects.Count) {
						len = 0;
						Debug.LogError ("AnimaitonModeUtility.ResampleAnimation> Number of Animate Object should be same with Animation Clips");
					}
					
					for (int i=0; i<len; i++) {

						
						AnimationMode.SampleAnimationClip (animatedObjects [i], animationClips [i], times[i]);
						
						
					}
					
					
					
					AnimationMode.EndSampling ();
					
					SceneView.RepaintAll ();
					
				}



				/// <summary>
				/// Saves the binding boonRoot offset - position offset of boonRoot and gameObject holder  at animation time=0s
				/// Saves current root position/rotation
				/// </summary>
				/// <param name="clipBindings">Clip bindings.</param>
				public static void SaveBindingsOffset (EditorClipBinding[] clipBindings)
				{

						int len = clipBindings.Length;
							
						EditorClipBinding clipBindingCurrent;
							
							
						for (int i=0; i<len; i++) {
								clipBindingCurrent = clipBindings [i];


					
								SaveBindingOffset (clipBindingCurrent);
								
						
						}
				}



				/// <summary>
				/// Saves the binding boonRoot offset - position before animation from boonRoot position at time=0s
				/// Saves current root position/rotation
				/// </summary>
				/// <param name="clipBindingCurrent">Clip binding current.</param>
				public static void SaveBindingOffset (EditorClipBinding clipBindingCurrent)
				{
						if (clipBindingCurrent.gameObject != null) {
							
								if(clipBindingCurrent.boneTransform!=null){
								//save bone position
								Vector3 positionPrev = clipBindingCurrent.boneTransform.position;
							
								//make sample at 0f (sample would probably change bone position according to ani clip)
								AnimationMode.SampleAnimationClip (clipBindingCurrent.gameObject, clipBindingCurrent.clip, 0f);
							
							
								//calculate difference of bone position orginal - bone postion after clip effect
								clipBindingCurrent.boneRootPositionOffset = positionPrev - clipBindingCurrent.boneTransform.position;
								}


								clipBindingCurrent.positionOriginalRoot = clipBindingCurrent.gameObject.transform.root.position;
								clipBindingCurrent.rotationOriginalRoot = clipBindingCurrent.gameObject.transform.root.rotation;
							
							
						}
				}


				/// <summary>
				/// Resets the only transform property modifications and restore gameObject transform state as before animation.
				/// </summary>
				/// <param name="clipBindings">Clip bindings.</param>
				public static void ResetBindingsTransformPropertyModification (EditorClipBinding[] clipBindings)
				{
						
						int len = clipBindings.Length;
						
						EditorClipBinding clipBindingCurrent;
						
						
						for (int i=0; i<len; i++) {
								clipBindingCurrent = clipBindings [i];
							

							
								ResetBindingTransformPropertyModification (clipBindingCurrent);
							
							
						}
				}



				/// <summary>
				/// Resets the binding Transform property modification.
				/// </summary>
				/// <param name="clipBindingCurrent">Clip binding current.</param>
				public static void ResetBindingTransformPropertyModification (EditorClipBinding clipBindingCurrent)
				{
					
						PrefabType prefabType = PrefabType.None;

						
						if (clipBindingCurrent.gameObject != null) {
								prefabType = PrefabUtility.GetPrefabType (clipBindingCurrent.gameObject);
				
								if (prefabType == PrefabType.ModelPrefabInstance || prefabType == PrefabType.PrefabInstance)
										clipBindingCurrent.gameObject.ResetPropertyModification<Transform> ();


								//rewind to start position
								clipBindingCurrent.ResetRoot ();
						}
				
					
				}


				








		}
}

