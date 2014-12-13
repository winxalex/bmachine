//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BehaviourMachine;
using Motion=UnityEngine.Motion;
using ws.winx.unity;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Mecanima inside BTree")]
		public class MecanimNode:CompositeNode,IEventStatusNode
		{

				
				[MecanimStateInfoAttribute]
				public MecanimStateInfo
						animaStateInfoSelected;
				public Motion motionOverride;
				public bool loop = false;
				[MecanimNodeBlendParameterAttribute(axis=MecanimNodeBlendParameterAttribute.Axis.X)]
				public int
						blendParamXBlackboardBindID;
				[MecanimNodeBlendParameterAttribute(axis=MecanimNodeBlendParameterAttribute.Axis.Y)]
				public int
						blendParamYBlackboardBindID;
				[RangeAttribute(0f,1f)]
				public float
						transitionDuration = 0.3f;
				[RangeAttribute(0f,1f)]
				//[HideInInspector]
				public float
						normalizedTimeStart = 0.5f;
				[HideInInspector]
				//	[RangeAttribute(0f,1f)]
				public float
						normalizedTimeCurrent = 0f;
				//	[HideInInspector]


				


				#region IEventStatusNode implementation
				StatusUpdateHandler _statusHandler;
				StatusEventArgs _statusArgs = new StatusEventArgs (Status.Error);
					
				public event StatusUpdateHandler OnChildCompleteStatus {
						
						add {
								_statusHandler += value;
							
								//v1
								//this.tree.StartCoroutine
							
								//v2
								this.tree.update += OnTick;
						}
						
						remove {
								_statusHandler -= value;
							
								//v1
								//this.tree.StopCoroutine()
							
								//v2
								this.tree.update -= OnTick;
						}
						
				}
				#endregion
				
				

				//!!!Curves serialization is buggy
				//public AnimationCurve curve;

		         


				

				Animator _animator;
				float normalizedTimeLast = 0f;
				int numBlendParamters;
				AnimatorStateInfo animatorStateInfoCurrent;
				bool isCurrentEqualToSelectedAnimaInfo = false;
				List<int> _treeInx;
				int _LastTickedChildren = -1;
		                         
				public Animator animator {
						get {
								if (_animator == null) {
										_animator = self.GetComponent<Animator> ();
									
								}
								return _animator;
						}
				}




				


				/// <summary>
				/// Plaies the state of the mecanima.
				/// </summary>
				void PlayAnimaState ()
				{
					    
						//if we are already in that AnimaState
						//cos normalied time doesn't stop but continue to increase
						if (isCurrentEqualToSelectedAnimaInfo) {
							
								normalizedTimeLast = animatorStateInfoCurrent.normalizedTime;
							
							
						} else {
							
							
							
								normalizedTimeLast = 0f;
							
							
						}


			Debug.Log (animaStateInfoSelected.label.text + ">Crosfade() "); 
			Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
			Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
			Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
			


						//		animator.Play (selectedAnimaStateInfo.hash, selectedAnimaStateInfo.layer, normalizedTimeStart);
						animator.CrossFade (animaStateInfoSelected.hash, transitionDuration, animaStateInfoSelected.layer, normalizedTimeStart);

				}

				public override void Remove (ActionNode child)
				{
						base.Remove (child);
				}

				public override bool Add (ActionNode child)
				{
						bool result = false;

						if (!(child is SendEventNormalized)) {
								Debug.LogWarning ("You can add only SendEventNormailized type of ActionCode");
								return false;
						}

						result = base.Add (child);

						

						return result;
				}

//		public override void ResetStatus ()
//		{
//			Debug.Log (selectedAnimaStateInfo.label.text + ">ResetStatus");
//			base.ResetStatus ();
//		}
		
				public override void Reset ()
				{
						//Debug.Log (selectedAnimaStateInfo.label.text+">Reset");
						_animator = null;
						
						transitionDuration = 0f;
				}

				public override void Awake ()
				{
          
						//Debug.Log (selectedAnimaStateInfo.label.text + ">Awake");


		
			
			
			
		}
		//
		//				public override void OnEnable ()
		//				{
		//			
		//						Debug.Log (selectedAnimaStateInfo.label.text + ">Enable");
		//
		//				}
		//		
		//				public override void OnDisable ()
//				{
//			
//						Debug.Log (selectedAnimaStateInfo.label.text + ">Disable");
//			
//			
//			
//				}
		
				public override void Start ()
				{

//						Debug.Log (selectedAnimaStateInfo.label.text + ">Start MecanimNode");

						_LastTickedChildren = -1;

						PlayAnimaState ();

				}

				public override void OnTick ()
				{

						//Debug.Log (selectedAnimaStateInfo.label.text + ">OnTick");

						animatorStateInfoCurrent = animator.GetCurrentAnimatorStateInfo (animaStateInfoSelected.layer);
						isCurrentEqualToSelectedAnimaInfo = animatorStateInfoCurrent.nameHash == animaStateInfoSelected.hash;

			Debug.Log (animaStateInfoSelected.label.text + ">Tick() "); 
			Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
			Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
			if (animator.IsInTransition (animaStateInfoSelected.layer)) {

				Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetNextAnimatorStateInfo(animaStateInfoSelected.layer).nameHash+" Transition time:"+animator.GetNextAnimatorStateInfo(animaStateInfoSelected.layer).normalizedTime); 

						}
			
			




						//The second check ins't nessery if I could reset Status when this node is switched
						if (this.status != Status.Running) {

				if (motionOverride != null) {
					
					
					
					//referrence
					AnimatorOverrideController overrideController;
					
					
					
					if (animator.runtimeAnimatorController is AnimatorOverrideController) {
						
						overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
						
						Debug.Log ("controller Already overrided just change overrrideController");
					} else {
						Debug.Log ("new overrrideController");
						overrideController = new AnimatorOverrideController ();
						overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
					}
					
					if (overrideController [(AnimationClip)animaStateInfoSelected.motion] != motionOverride) {
						
						Debug.Log ("Selected state Motion " + animaStateInfoSelected.motion + "to be overrided to " + motionOverride);
						overrideController [(AnimationClip)animaStateInfoSelected.motion] = (AnimationClip)motionOverride;
						
						
						var a = overrideController [(AnimationClip)animaStateInfoSelected.motion];
						//	animaStateInfoSelected.motion = motionOverRide;
						
						
						
						Debug.Log ("Override result:" + a);
						
						//rereference 
						if (animator.runtimeAnimatorController is AnimatorOverrideController) {
							animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
						}
						
						animator.runtimeAnimatorController = overrideController;
						

						
					}
					
					
					
				}





								
			
								this.Start ();	
				 
								this.status = Status.Running;
								return;
						}

						//	Debug.Log (selectedAnimaStateInfo.label.text + ">" + Status.Running);

			
						if (isCurrentEqualToSelectedAnimaInfo)
								this.Update ();


						if (this.status != Status.Running) {
								this.End ();
						}
				}

				public override void Update ()
				{

						if (loop)
								normalizedTimeCurrent = animatorStateInfoCurrent.normalizedTime - (int)animatorStateInfoCurrent.normalizedTime;
						else
								normalizedTimeCurrent = animatorStateInfoCurrent.normalizedTime;




//						Debug.Log (animaStateInfoSelected.label.text + ">Update() "); 
//						Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
//						Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
//						Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
//		

						

		
					
						if (isCurrentEqualToSelectedAnimaInfo
								&& normalizedTimeLast != normalizedTimeCurrent) {

								//Debug.Log ("NormalizedTime: " + (animatorStateInfoCurrent.normalizedTime));


							
							
								if (normalizedTimeCurrent > 1f) {
										if (!loop) {
												_statusArgs.status = this.status = Status.Success;

												if (_statusHandler != null)
														_statusHandler.Invoke (this, _statusArgs);
												return;
										}

								} else {

										//send event if its between previous and current time
										//Test only event sending is done try SentEventNormalized
										//if (normalizedTimeLast < 0.67f && normalizedTimeCurrent >= 0.67f)
										//Debug.Log ("Event sent designated at 0.67 sent at:" + normalizedTimeCurrent);


										int len = children.Length;

										if (len > 0) {
												_LastTickedChildren++;

												//reset
												if (_LastTickedChildren >= len)
														_LastTickedChildren = 0;

												for (int i=_LastTickedChildren; i<len; i++) {
														children [i].OnTick ();
												}
										}

								}

							
								if (animaStateInfoSelected.blendParamsIDs != null && (numBlendParamters = animaStateInfoSelected.blendParamsIDs.Length) > 0) {

										if (numBlendParamters > 1) {
												animator.SetFloat (animaStateInfoSelected.blendParamsIDs [0], this.blackboard.GetFloatVar (this.blendParamXBlackboardBindID).Value);
												animator.SetFloat (animaStateInfoSelected.blendParamsIDs [1], this.blackboard.GetFloatVar (this.blendParamYBlackboardBindID).Value);

										} else
												animator.SetFloat (animaStateInfoSelected.blendParamsIDs [0], this.blackboard.GetFloatVar (blendParamXBlackboardBindID).Value);


								}


								//Debug.Log ("Update at: " + normalizedTimeCurrent);	

								//characterControllerRadius.Value= curve.Evaluate(normalizedTimeCurrent);

								normalizedTimeLast = normalizedTimeCurrent;



			
						} else {

								Debug.LogWarning ("MecanimNode Update on wrong  AnimaState");
								//this.status = Status.Error;
								//return;
								
						}


						
						
						_statusArgs.status = this.status = Status.Running;

						if (_statusHandler != null)
								_statusHandler.Invoke (this, _statusArgs);

				}





		}
}