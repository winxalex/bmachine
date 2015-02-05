using UnityEngine;
using System.Collections;
using BehaviourMachine;
using ws.winx.unity;


namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Animation Curve")]
		public class AnimationCurveNode : ActionNode
		{
				
				public bool syncWithAnimation = true;

		[MecanimStateInfoAttribute("Ani")]
				public MecanimStateInfo
					animaStateInfoSelected;


				[VariableInfo (requiredField = true,canBeConstant = false,tooltip = "Object.property or blackboard that will be controlled by Animation curve")]
				public FloatVar
						property;
				public AnimationCurve curve;

				//[HideInInspector]//doesn't work togheter
				[RangeAttributeEx("timeStart","timeEnd")]
				public float
						timeControl = 0f;

				[HideInInspector]
				public float
						timeStart;
				[HideInInspector]
				public float
						timeEnd;

				

				Animator _animator;

				public Animator Ani {
						get {
							if(_animator==null) _animator = this.self.GetComponent<Animator> ();
							return _animator;
						}
					}

				float _timeCurrent;

				public override void Awake ()
				{

					if (curve != null || curve.keys != null || curve.keys.Length >1) {
								timeStart = curve.keys [0].time;
					
								timeEnd = curve.keys [curve.length - 1].time;
						}
						
				}

				public override void Reset ()
				{
						curve = new AnimationCurve ();
						_timeCurrent = 0f; 
						//property = new ConcreteFloatVar ("mile", this.blackboard, 24);

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



						if (_animator != null) {

								//time should be normalized in 0-1 when animator.normalizedTime is used as time ticker
								if (timeStart > 1f || timeEnd > 1f || timeStart < 0f || timeEnd < 0f)
										return Status.Error;


								
								//take Animator AnimaStateInfo 
								AnimatorStateInfo animatorStateInfoCurrent = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
				
		

			
								if (_animator.IsInTransition (animaStateInfoSelected.layer)) {
										animatorStateInfoCurrent = _animator.GetNextAnimatorStateInfo (animaStateInfoSelected.layer);
								} else {
										animatorStateInfoCurrent = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
								}

								//check if selected and current state are equal
								if (animatorStateInfoCurrent.nameHash == animaStateInfoSelected.hash) {
										_timeCurrent = animatorStateInfoCurrent.normalizedTime;
						
										if (timeControl > 0)
												_animator.Update (timeControl - animatorStateInfoCurrent.normalizedTime);
								}
						} else {
								if (timeControl > 0)
										_timeCurrent = timeControl;
								else
										_timeCurrent += base.owner.deltaTime;
								
						}
			
			
						if (_timeCurrent >= timeStart && _timeCurrent <= timeEnd)
								property.Value = curve.Evaluate (_timeCurrent);

						if (_timeCurrent >= timeEnd)
								return Status.Success;
				
			 
			Debug.Log ("Time current:"+_timeCurrent);

	

						return Status.Running;
				}

		}
}

