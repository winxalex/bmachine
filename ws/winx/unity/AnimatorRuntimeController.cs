using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ws.winx.unity;
using ws.winx.bmachine.extensions;
using ws.winx.unity.attributes;
using UnityEditor.Animations;

namespace ws.winx.editor.extensions
{
		[ExecuteInEditMode]
		[RequireComponent(typeof(Animator))]
		public class AnimatorRuntimeController:MonoBehaviour
		{
				[AnimatorStateAttribute("animator","layer")]
				public AnimatorState
						animaStateInfoSelected;

				public int layer;

				[RangeAttribute(0f,1f)]
				public float
						timeNormalized;
				float _timeNormalizedPrev = -1f;
				float startTime;
				float stopTime;
				float currentTime = float.NegativeInfinity;
				float nextCurrentTime = 0f;

				public Animator animator;
				[RangeAttribute(0f,1f)]
				//[HideInInspector]
			public float
						timeNormalizedStart = 0f;

				void onEnable ()
				{
						animator = this.GetComponent<Animator> ();
						Debug.Log ("Enable");

				}

				void Reset ()
				{

						//animator = this.GetComponent<Animator> ();
						//animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

				}

				void Awake ()
				{

						animator = this.GetComponent<Animator> ();
						//animator.updateMode = AnimatorUpdateMode.AnimatePhysics;



				}

				void controlAnimation ()
				{
						AnimatorStateInfo animatorStateInfo;
						if (animator == null)
								return;
			
						if (animator.IsInTransition (layer)) {
								animatorStateInfo = animator.GetNextAnimatorStateInfo (layer);
						} else {
								animatorStateInfo = animator.GetCurrentAnimatorStateInfo (layer);
						}
			
			
						if (animatorStateInfo.shortNameHash == animaStateInfoSelected.nameHash) {
				
				
								float timeDelta = 0f;
				
							
				

								timeDelta = timeNormalized - animatorStateInfo.normalizedTime;
								animator.Update (timeNormalized - animatorStateInfo.normalizedTime);
	
				
								_timeNormalizedPrev = timeNormalized;
				
						
				
						} else {
				
								animator.CrossFade (animaStateInfoSelected.nameHash, 0f);
						}
				}

				void onGUI ()
				{



				}

				void FixedUpdate ()
				{
						if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
								controlAnimation ();
				}

				void Update ()
				{

						if (animator.updateMode == AnimatorUpdateMode.Normal)
								controlAnimation ();
					

				}
				
		}
}