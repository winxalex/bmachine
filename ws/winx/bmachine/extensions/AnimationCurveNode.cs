using UnityEngine;
using System.Collections;
using BehaviourMachine;
using ws.winx.unity;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Animation Curve")]
		public class AnimationCurveNode : ActionNode
		{
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
				float _timeNormalizedPrev = -1f;
				float _currentTime;

				public override void Awake ()
				{

						if (this.branch is MecanimNode) {

								_animator = this.self.GetComponent<Animator> ();
						}
				}

				public override void Reset ()
				{
						curve = new AnimationCurve ();
						_currentTime = 0f; 
						//property = new ConcreteFloatVar ("mile", this.blackboard, 24);

				}


				public override void Start ()
				{
					_currentTime = timeStart;
				}

				public override Status Update ()
				{

						if (curve == null || curve.keys == null || curve.keys.Length < 2)
								return Status.Error;

						timeStart = curve.keys [0].time;

						timeEnd = curve.keys [curve.length - 1].time;



						if (_animator != null) {

								if (timeStart > 1f || timeEnd > 1f || timeStart < 0f || timeEnd < 0f)
										return Status.Error;


								//take selected AnimaState in parent MecanimNode
								MecanimStateInfo animaStateInfoSelected = ((MecanimNode)this.branch).animaStateInfoSelected;
			
								//take Animator AnimaStateInfo 
								AnimatorStateInfo animatorStateInfoCurrent = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
				
		

			
								if (_animator.IsInTransition (animaStateInfoSelected.layer)) {
										animatorStateInfoCurrent = _animator.GetNextAnimatorStateInfo (animaStateInfoSelected.layer);
								} else {
										animatorStateInfoCurrent = _animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
								}

								//check if selected and current state are equal
								if (animatorStateInfoCurrent.nameHash == animaStateInfoSelected.hash) {
										_currentTime = animatorStateInfoCurrent.normalizedTime;
						
										if (timeControl > 0)
												_animator.Update (timeControl - animatorStateInfoCurrent.normalizedTime);
								}
						} else {
								if (timeControl > 0)
										_currentTime = timeControl;
								else
										_currentTime += base.owner.deltaTime;
								
						}
			
			
						if (_currentTime >= timeStart && _currentTime <= timeEnd)
								property.Value = curve.Evaluate (_currentTime);

						if (_currentTime > timeEnd)
								return Status.Success;
				
			 


	

						return Status.Running;
				}

		}
}

