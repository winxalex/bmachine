using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ws.winx.unity;
using ws.winx.bmachine.extensions;

namespace ws.winx.editor.extensions
{
		[ExecuteInEditMode]
		[RequireComponent(typeof(Animator))]
		public class AnimatorRuntimeController:MonoBehaviour
		{
				[MecanimStateInfoAttribute]
				public MecanimStateInfo
						animaStateInfoSelected;
				[RangeAttribute(0f,1f)]
				public float
						timeNormalized;
				float _timeNormalizedPrev = -1f;
				float startTime;
				float stopTime;
				float currentTime = float.NegativeInfinity;
				float nextCurrentTime = 0f;
				Animator _animator;
				[RangeAttribute(0f,1f)]
				//[HideInInspector]
			public float
						timeNormalizedStart = 0f;

				void onEnable ()
				{

						Debug.Log ("Enable");

				}

				void Reset ()
				{

						//animator = this.GetComponent<Animator> ();
						//animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

				}

				void Awake ()
				{

						_animator = this.GetComponent<Animator> ();
						//_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;



				}

				void controlAnimation ()
				{
						AnimatorStateInfo animatorStateInfo;
						if (_animator == null)
								return;
			
						if (_animator.IsInTransition (animaStateInfoSelected.layer)) {
								animatorStateInfo = _animator.GetNextAnimatorStateInfo (animaStateInfoSelected.layer);
						} else {
								animatorStateInfo = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
						}
			
			
						if (animatorStateInfo.nameHash == animaStateInfoSelected.hash) {
				
				
								float timeDelta = 0f;
				
							
				

								timeDelta = timeNormalized - animatorStateInfo.normalizedTime;
								_animator.Update (timeNormalized - animatorStateInfo.normalizedTime);
	
				
								_timeNormalizedPrev = timeNormalized;
				
						
				
						} else {
				
								_animator.CrossFade (animaStateInfoSelected.hash, 0f);
						}
				}

				void onGUI ()
				{



				}

				void FixedUpdate ()
				{
						if (_animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
								controlAnimation ();
				}

				void Update ()
				{

						if (_animator.updateMode == AnimatorUpdateMode.Normal)
								controlAnimation ();
					

				}
				
		}
}