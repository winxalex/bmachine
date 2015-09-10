using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Linq.Expressions;
using ws.winx.unity.utilities;
using UnityEditor.Animations;

namespace ws.winx.unity
{


	#region Rect Extensions
		public static class RectExtensions
		{
				public static bool Intersect (this Rect rectA, Rect rectB)
				{
						return (Mathf.Abs (rectA.x - rectB.x) < (Mathf.Abs (rectA.width + rectB.width) / 2)) &&
								(Mathf.Abs (rectA.y - rectB.y) < (Mathf.Abs (rectA.height + rectB.height) / 2));
				}

			
		}
	
	
	#endregion
	
	#region RectTransformExtensions
	
		public static class RectTransformExtensions
		{
				public static void SetDefaultScale (this RectTransform trans)
				{
						trans.localScale = new Vector3 (1, 1, 1);
				}

				public static void SetPivotAndAnchors (this RectTransform trans, Vector2 aVec)
				{
						trans.pivot = aVec;
						trans.anchorMin = aVec;
						trans.anchorMax = aVec;
				}
		
				public static Vector2 GetSize (this RectTransform trans)
				{
						return trans.rect.size;
				}

				public static float GetWidth (this RectTransform trans)
				{
						return trans.rect.width;
				}

				public static float GetHeight (this RectTransform trans)
				{
						return trans.rect.height;
				}
		
				public static void SetPositionOfPivot (this RectTransform trans, Vector2 newPos)
				{
						trans.localPosition = new Vector3 (newPos.x, newPos.y, trans.localPosition.z);
				}
		
				public static void SetLeftBottomPosition (this RectTransform trans, Vector2 newPos)
				{
						trans.localPosition = new Vector3 (newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
				}

				public static void SetLeftTopPosition (this RectTransform trans, Vector2 newPos)
				{
						trans.localPosition = new Vector3 (newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
				}

				public static void SetRightBottomPosition (this RectTransform trans, Vector2 newPos)
				{
						trans.localPosition = new Vector3 (newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
				}

				public static void SetRightTopPosition (this RectTransform trans, Vector2 newPos)
				{
						trans.localPosition = new Vector3 (newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
				}
		
				public static void SetSize (this RectTransform trans, Vector2 newSize)
				{
						Vector2 oldSize = trans.rect.size;
						Vector2 deltaSize = newSize - oldSize;
						trans.offsetMin = trans.offsetMin - new Vector2 (deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
						trans.offsetMax = trans.offsetMax + new Vector2 (deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
				}

				public static void SetWidth (this RectTransform trans, float newSize)
				{
						SetSize (trans, new Vector2 (newSize, trans.rect.size.y));
				}

				public static void SetHeight (this RectTransform trans, float newSize)
				{
						SetSize (trans, new Vector2 (trans.rect.size.x, newSize));
				}
		
		}
	
	
	#endregion





	#region AnimatorExtension
		public static class AnimatorExtension
		{

				public static void Play (this Animator animator, AnimationClip clip, float normalizedTime=0f)
				{
			
			
			
						AnimatorOverrideController animatorOverrideController;
						AnimationClip clipOverride = null;
			
			
						if (animator.runtimeAnimatorController is AnimatorOverrideController) {
				
								//animator.runtimeAnimatorController is already overrided just take reference
								animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
								clipOverride = animatorOverrideController.clips [0].originalClip;
						} else {
								//AS RuntimeAnimatorController can't be created at runtime
								RuntimeAnimatorController dummyController = AnimatorUtilityEx.DUMMY_CONTROLLER;
				
								clipOverride = dummyController.animationClips [0];
				
				
								animatorOverrideController = new AnimatorOverrideController ();
				
								//bind all clips from animator.runtimeAnimatorController to overrider
								animatorOverrideController.runtimeAnimatorController = dummyController;
						}	
			
			
			
			
						animatorOverrideController [clipOverride] = clip;
			
			
						//to avoid nesting 
						if (animator.runtimeAnimatorController is AnimatorOverrideController) {
								animator.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
						}
			
						//rebind back												
						animator.runtimeAnimatorController = animatorOverrideController;
			
						animator.Play ("Override", 0, normalizedTime);
			
			
				}
		
		}
	
	#endregion
	
	#region Transform Extensions
		public static class TransformExtension
		{
		
				/// <summary>
				/// Gets the position offset delta negated.
				/// </summary>
				/// <returns>The position offset delta negated.</returns>
				/// <param name="fromTransform">From transform.</param>
				/// <param name="toTransform">To transform.</param>
				public static Vector3 GetPositionOffsetDeltaNegated (this Transform fromTransform, Transform toTransform)
				{
						return Quaternion.Inverse (fromTransform.rotation) * (toTransform.position - fromTransform.position);
				}

				/// <summary>
				/// Adds the position offset.
				/// </summary>
				/// <returns>The position offset.</returns>
				/// <param name="fromTransform">From transform.</param>
				/// <param name="positionOffset">Position offset.</param>
				public static Vector3 AddPositionOffset (this Transform fromTransform, Vector3 positionOffsetDeltaNegated)
				{
						return fromTransform.position + fromTransform.rotation * positionOffsetDeltaNegated;
				}

				//fromTransform.position + fromTransform.rotation(current) * Quaternion (fromTransform.rotation)(at start) * (toTransform - fromTransform);
				//fromTransform.position + delta Rotation * (toTransform - fromTransform);

				public static void Reset (this Transform trans)
				{
						trans.position = Vector3.zero;
						trans.localRotation = Quaternion.identity;
						trans.localScale = new Vector3 (1, 1, 1);
				}

				public static float GetPositionX (this Transform t)
				{
						return t.position.x;
				}
		
				public static float GetPositionY (this Transform t)
				{
						return t.position.y;
				}
		
				public static float GetPositionZ (this Transform t)
				{

						return t.position.z;
				}

				public static void SetPositionX (this Transform t, float s)
				{
						Vector3 v = t.position;
						v.x = s;
						t.position = v;
				}

				public static void SetPositionY (this Transform t, float s)
				{
						Vector3 v = t.position;
						v.y = s;
						t.position = v;
				}

				public static void SetPositionZ (this Transform t, float s)
				{
						Vector3 v = t.position;
						v.z = s;
						t.position = v;
				}

				public static void SetLocalScaleX (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.x = s;
						t.localScale = v;
				}

				public static void SetLocalScaleY (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.y = s;
						t.localScale = v;
				}

				public static void SetLocalScaleZ (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.z = s;
						t.localScale = v;
				}

				public static void MulLocalScaleX (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.x *= s;
						t.localScale = v;
				}

				public static void MulLocalScaleY (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.y *= s;
						t.localScale = v;
				}

				public static void MulLocalScaleZ (this Transform t, float s)
				{
						Vector3 v = t.localScale;
						v.z *= s;
						t.localScale = v;
				}
		}


	#endregion


	#region Quaternion Extensions
	
		public static class QuaternionExtensions
		{


//		public Vector3 relativeDirection = Vector3.forward;
//		void Update() {
//			Vector3 absoluteDirection = transform.rotation * relativeDirection;
//			transform.position += absoluteDirection * Time.deltaTime;
//		}

				/// <summary>
				/// Rotations the offset. (ex. q1=40,q2=30 => new q=10)
				/// </summary>
				/// <returns>The offset.</returns>
				/// <param name="quaternion1">Quaternion1.</param>
				/// <param name="quaternion2">Quaternion2.</param>
				public static Quaternion RotationOffset (this Quaternion quaternion1, Quaternion quaternion2)
				{

						return Quaternion.Inverse (quaternion1) * quaternion2;

				}

				/// <summary>
				/// Add the specified "offset" to quaternion2 .
				/// 
				/// </summary>
				/// <param name="quaternion2">Quaternion2.</param>
				/// <param name="offset">Offset.</param>
				public static Quaternion Add (this Quaternion quaternion2, Quaternion offset)
				{
			
						return quaternion2 * offset;
			
				}

				public static Quaternion Normalize (this Quaternion q)
				{
						float sum = 0;
						for (int i = 0; i < 4; ++i)
								sum += q [i] * q [i];
						float magnitudeInverse = 1 / Mathf.Sqrt (sum);
						for (int i = 0; i < 4; ++i)
								q [i] *= magnitudeInverse; 

						return q;
				}

				public static Quaternion Pow (this Quaternion input, float power)
				{
						float inputMagnitude = input.Magnitude ();
						Vector3 nHat = new Vector3 (input.x, input.y, input.z).normalized;
						Quaternion vectorBit = new Quaternion (nHat.x, nHat.y, nHat.z, 0)
				.ScalarMultiply (power * Mathf.Acos (input.w / inputMagnitude))
					.Exp ();
						return vectorBit.ScalarMultiply (Mathf.Pow (inputMagnitude, power));
				}
		
				public static Quaternion Exp (this Quaternion input)
				{
						float inputA = input.w;
						Vector3 inputV = new Vector3 (input.x, input.y, input.z);
						float outputA = Mathf.Exp (inputA) * Mathf.Cos (inputV.magnitude);
						Vector3 outputV = Mathf.Exp (inputA) * (inputV.normalized * Mathf.Sin (inputV.magnitude));
						return new Quaternion (outputV.x, outputV.y, outputV.z, outputA);
				}
		
				public static float Magnitude (this Quaternion input)
				{
						return Mathf.Sqrt (input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
				}
		
				public static Quaternion ScalarMultiply (this Quaternion input, float scalar)
				{
						return new Quaternion (input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
				}



				/// <summary>
				/// From to around axis creates a FromToRotation, but makes sure it's axis remains fixed near to th Quaternion singularity point.
				/// </summary>
				/// <returns>
				/// The from to rotation around an axis.
				/// </returns>
				/// <param name='fromDirection'>
				/// From direction.
				/// </param>
				/// <param name='toDirection'>
				/// To direction.
				/// </param>
				/// <param name='axis'>
				/// Axis. Should be normalized before passing into this method.
				/// </param>
				public static Quaternion FromToAroundAxis (Vector3 fromDirection, Vector3 toDirection, Vector3 axis)
				{
						Quaternion fromTo = Quaternion.FromToRotation (fromDirection, toDirection);
			
						float angle = 0;
						Vector3 freeAxis = Vector3.zero;
			
						fromTo.ToAngleAxis (out angle, out freeAxis);
			
						float dot = Vector3.Dot (freeAxis, axis);
						if (dot < 0)
								angle = -angle;
			
						return Quaternion.AngleAxis (angle, axis);
				}
		
				/// <summary>
				/// Gets the rotation that can be used to convert a rotation from one axis space to another.
				/// </summary>
				public static Quaternion RotationToLocalSpace (Quaternion space, Quaternion rotation)
				{
						return Quaternion.Inverse (Quaternion.Inverse (space) * rotation);
				}
		
				/// <summary>
				/// Gets the Quaternion from rotation "from" to rotation "to".
				/// </summary>
				public static Quaternion FromToRotation (Quaternion from, Quaternion to)
				{
						if (to == from)
								return Quaternion.identity;
			
						return to * Quaternion.Inverse (from);
				}



				// To get the local right vector from just a rotation.
				public static Vector3 GetRight (this Quaternion rotation)
				{
						return new Vector3 (1 - 2 * (rotation.y * rotation.y + rotation.z * rotation.z),
			                   2 * (rotation.x * rotation.y + rotation.w * rotation.z),
			                   2 * (rotation.x * rotation.z - rotation.w * rotation.y));
				}
		
		
				// To get the local up vector from just a rotation.
				public static Vector3 GetUp (this Quaternion rotation)
				{
						return new Vector3 (2 * (rotation.x * rotation.y - rotation.w * rotation.z),
			                   1 - 2 * (rotation.x * rotation.x + rotation.z * rotation.z),
			                   2 * (rotation.y * rotation.z + rotation.w * rotation.x));
				}
		
		
				// To get the local forward vector from just a rotation.
				public static Vector3 GetForward (this Quaternion rotation)
				{
						return new Vector3 (2f * (rotation.x * rotation.z + rotation.w * rotation.y),
			                   2f * (rotation.y * rotation.x - rotation.w * rotation.x),
			                   1f - 2f * (rotation.x * rotation.x + rotation.y * rotation.y));
				}
		}
	#endregion


	#region Vector Extensions
		public static class VectorExtensions
		{





				public static Vector3 ToGuiCoordinateSystem (this Vector3 a)
				{
						var copy = a;
						copy.y = Screen.height - copy.y;
						return copy;
				}
		
				public static Vector3 Mask (this Vector3 a, Vector3 mask)
				{
						return new Vector3 (a.x * mask.x, a.y * mask.y, a.z * mask.z);
				}
		
				public static Vector3 Abs (this Vector3 a)
				{
						return new Vector3 (Mathf.Abs (a.x), Mathf.Abs (a.y), Mathf.Abs (a.z));
				}
		
				public static Vector3 Inverse (this Vector3 a)
				{
						return new Vector3 (1 / a.x, 1 / a.y, 1 / a.z);
				}
		}
	#endregion



	#region GameObjectExtensions
		public static class GameObjectExtensions
		{

				public static bool isRoot (this GameObject go)
				{
						return go.transform == go.transform.root;
				}

				public static Transform GetRootBone (this GameObject go)
				{
						Animator animator = go.GetComponent<Animator> ();
						Transform boneTransform = null;

						if (animator != null)
			
								boneTransform = animator.GetBoneTransform (HumanBodyBones.Hips);
						//op1
						if (boneTransform != null)
								return boneTransform;

						//op2
						SkinnedMeshRenderer[] skinmeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer> ();
						int numRenderers = skinmeshRenderers.Length;
						
						if (numRenderers == 1)
								boneTransform = skinmeshRenderers [0].rootBone;
						else
								for (int i=0; i<numRenderers; i++) {

										boneTransform = skinmeshRenderers [i].rootBone;

										if (boneTransform != null && (boneTransform.name.ToLower ().Contains ("hip") || boneTransform.name.ToLower ().Contains ("pelvis"))) {
												return boneTransform;
										}
											
								}
						


						//op3
						Transform[] bones = go.transform.GetComponentsInChildren<Transform> ();
							
						boneTransform = bones.FirstOrDefault (itm => itm.name.ToLower ().Contains ("pelvis") || itm.name.ToLower ().Contains ("hip"));

										
						
							
									
			
									

						return boneTransform;




				}

				public static string GetPath (this GameObject obj)
				{
						string path = "/" + obj.name;
						while (obj.transform.parent != null) {
								obj = obj.transform.parent.gameObject;
								path = "/" + obj.name + path;
						}
						return path;
				}

				/// <summary>
				/// </summary>
				public static GameObject InstantiateChild (this GameObject parent, GameObject prototype, bool preserveScale = false)
				{
						var child = UnityEngine.Object.Instantiate (prototype) as GameObject;
						var rotCache = child.transform.rotation;
						var scaleCache = child.transform.localScale;
						child.transform.position = parent.transform.position;
						child.transform.parent = parent.transform;
						if (!preserveScale)
								child.transform.localScale = scaleCache;
						child.transform.localRotation = rotCache;
						return child;
				}

				public static void ForEach (this GameObject o, System.Action<GameObject> f)
				{
						f (o);
				
						int numChildren = o.transform.childCount;
				
						for (int i = 0; i < numChildren; ++i) {
								o.transform.GetChild (i).gameObject.ForEach (f);
						}
				}



		}
	#endregion

	#region AnimatorControllerExtension
		public static class AnimatorControllerExtension
		{
				public static  UnityEditor.Animations.AnimatorState GetStateBy (this UnityEditor.Animations.AnimatorController controller, int nameHash, int layerIndex=0)
				{
			
						ChildAnimatorState child = controller.layers [layerIndex].stateMachine.states.FirstOrDefault (itm => itm.state.nameHash == nameHash);

						if (child.state != null)
								return child.state;

						

						return null;
				}

				public static  UnityEditor.Animations.AnimatorState RemoveStateWith (this UnityEditor.Animations.AnimatorController controller, int nameHash, int layerIndex=0)
				{
				
						UnityEditor.Animations.AnimatorState state = controller.GetStateBy (nameHash,layerIndex);

						if (state != null) {
								controller.layers [layerIndex].stateMachine.RemoveState (state);
									
								return state;
						}
				
				
				
						return null;
				}
		}






	#endregion

	
	
}
