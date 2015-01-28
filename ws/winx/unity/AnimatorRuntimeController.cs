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

		float _timeNormalizedPrev=-1f;
				public AnimatorUpdateMode updateMode;
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
						_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;



				}


		void controlAnimation(){
			AnimatorStateInfo animatorStateInfo;
			if (_animator == null)
				return;
			
			if (_animator.IsInTransition (animaStateInfoSelected.layer)) {
				animatorStateInfo = _animator.GetNextAnimatorStateInfo (animaStateInfoSelected.layer);
			} else {
				animatorStateInfo = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
			}
			
			
			if (animatorStateInfo.nameHash == animaStateInfoSelected.hash) {
				
				
				float timeDelta=0f;
				
				//currentTime is negative infinity not intialized => set it to 0f
				if (currentTime < 0f)
					this.currentTime = 0f;
				
				
				stopTime = animatorStateInfo.length;
				startTime = timeNormalizedStart * stopTime;
				
				
				//calculate nextCurrentTime based on timeNormalized
				//deltaTime is nextCurrentTime-currentTime
				this.nextCurrentTime = this.startTime * (1f - timeNormalized) + this.stopTime * timeNormalized;	
				
				//timeDelta = nextCurrentTime - currentTime;
				
				Debug.Log("before time:"+animatorStateInfo.normalizedTime);
				

					timeDelta=timeNormalized-animatorStateInfo.normalizedTime;
					_animator.Update(timeNormalized-animatorStateInfo.normalizedTime);
	
				
				_timeNormalizedPrev=timeNormalized;
				
				//_animator.enabled=true;
				//_animator.Update (timeDelta);
				
				
				currentTime=nextCurrentTime;
				
				
				Debug.Log("timeDelta:"+timeDelta);
				//Debug.Log("timeDelta:"+timeDelta+"nextCurrentTime:"+nextCurrentTime+"currentTime:"+currentTime);
				
				Debug.Log("after time:"+animatorStateInfo.normalizedTime);
				
			} else {
				
				_animator.CrossFade(animaStateInfoSelected.hash,0f);
			}
				}

		void FixedUpdate()
		{
			controlAnimation ();
		}

				void Update ()
				{

					

				}
				
		}
}