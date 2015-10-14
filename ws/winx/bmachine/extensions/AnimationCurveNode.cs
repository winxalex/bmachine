#if BEHAVIOUR_MACHINE
using UnityEngine;
using System.Collections;
using BehaviourMachine;
using ws.winx.unity;
using ws.winx.unity.attributes;



namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Animation Curve")]
		public class AnimationCurveNode : ActionNode
		{


				
				public bool syncWithAnimation = true;

				[AnimatorStateAttribute("animator","layer")]
				public AnimatorState
						animaStateInfoSelected;
				[VariableInfo (requiredField = true,canBeConstant = false,tooltip = "Object.property or blackboard that will be controlled by Animation curve")]
				public FloatVar
						property;
				public AnimationCurve curve;

				

				public int layer;

				//[HideInInspector]//doesn't work togheter
				[RangeAttributeEx("timeStart","timeEnd","isTimeControlEnabled")]
				public float
						timeControl = 0f;
				[HideInInspector]
				public bool
						isTimeControlEnabled = false;
				[HideInInspector]
				public float
						timeStart;
				[HideInInspector]
				public float
						timeEnd;
				

				public Animator animator;

				float _timeCurrent;

				public override void OnEnable ()
				{
						Debug.Log ("AniCurve Enabled");
				}

				public override void Awake ()
				{

						if (curve != null || curve.keys != null || curve.keys.Length > 1) {
								timeStart = curve.keys [0].time;
					
								timeEnd = curve.keys [curve.length - 1].time;
						}

						
						
				}

				public override void Reset ()
				{
						animator = this.self.GetComponent<Animator> ();
						curve = new AnimationCurve ();
						_timeCurrent = 0f; 
						
				}

				public override void Start ()
				{
						_timeCurrent = timeStart;
				}

				public override Status Update ()
				{

						if (curve == null || curve.keys == null || curve.keys.Length < 2)
								return Status.Error;

						timeStart = curve.keys [0].time;

						timeEnd = curve.keys [curve.length - 1].time;



						if (animator != null) {

								//time should be normalized in 0-1 when animator.normalizedTime is used as time ticker
								if (timeStart > 1f || timeEnd > 1f || timeStart < 0f || timeEnd < 0f)
										return Status.Error;


								
								//take Animator AnimaStateInfo 
								AnimatorStateInfo animatorStateInfoCurrent = animator.GetCurrentAnimatorStateInfo (layer);
				
		

			
								if (animator.IsInTransition (layer)) {
										animatorStateInfoCurrent = animator.GetNextAnimatorStateInfo (layer);
								} else {
										animatorStateInfoCurrent = animator.GetCurrentAnimatorStateInfo (layer);
								}

								//check if selected and current state are equal
								if (animatorStateInfoCurrent.shortNameHash == animaStateInfoSelected.nameHash) {
										_timeCurrent = animatorStateInfoCurrent.normalizedTime;

									

										if (isTimeControlEnabled)
												animator.Update (timeControl - animatorStateInfoCurrent.normalizedTime);

								}
						} else {

								if (isTimeControlEnabled)
										_timeCurrent = timeControl;
								else
										_timeCurrent += base.owner.deltaTime;
								
						}

		

						if (!isTimeControlEnabled) {
								if (_timeCurrent >= timeStart && _timeCurrent <= timeEnd)
										property.Value = curve.Evaluate (_timeCurrent);

								if (_timeCurrent >= timeEnd)
										return Status.Success;
						}
				
			 
						//Debug.Log ("Time current:"+_timeCurrent);

	

						return Status.Running;
				}

		}
}
#endif
