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

namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/",
                icon = "Animator")]
		public class MecanimNode:ActionNode
		{
				
				[AnimaStateInfoAttribute]
				public AnimaStateInfo
					selectedAnimaStateInfo;

				public bool loop = false;

		
				[MecanimBlendParameterAttribute(axis=MecanimBlendParameterAttribute.Axis.X)]
				public int blendParamXBlackboardBindID;

		        [MecanimBlendParameterAttribute(axis=MecanimBlendParameterAttribute.Axis.Y)]
				public int blendParamYBlackboardBindID;
	
				
				[RangeAttribute(0f,1f)]
				public float transitionDuration = 0.3f;
				
				[RangeAttribute(0f,1f)]
				public float
						timeStart = 0f;
				
				[HideInInspector]
				public float
						normalizedTimeCurrent = -1f;
				public float normalizedTimeStart = 0f;

				public AnimationCurve curve;


				

				int loopIdleCurrent = 0;
				Animator _animator;
				RuntimeAnimatorController _runtimeAnimaController;
				float normalizedTimeLast = 0f;
				int numBlendParamters;


		                         
				public Animator animator {
						get {
								if (_animator == null) {
										_animator = self.GetComponent<Animator> ();
										_runtimeAnimaController = _animator.runtimeAnimatorController;
								}
								return _animator;
						}
				}

				void PlayAnimaState (int hash, int layer, float normalizedTime, float transitionDuration=0f)
				{

						if (selectedAnimaStateInfo != null) {
							
								Debug.Log ("CrossFade to state hash:" + hash + " layer" + layer);
							
								AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (layer);
							
								//if we are already in that AnimaState
								if (stateInfo.nameHash == hash) {
							
										normalizedTimeLast = normalizedTimeStart = stateInfo.normalizedTime;
								
								
								} else {
										normalizedTimeLast = normalizedTimeStart = 0f;
										normalizedTimeCurrent = -1f;
								}
							
								animator.CrossFade (hash, transitionDuration, layer, normalizedTime);
			
								//animator.Play(selectedAnimaStateInfo.hash,selectedAnimaStateInfo.layer,normalizedTime);
						}
				}

				public override void Reset ()
				{
						_animator = null;
						timeStart = 0f;
						transitionDuration = 0f;
						selectedAnimaStateInfo = null;
	
						//mecanimBlendParamter = ScriptableObject.CreateInstance< MecanimBlendTreeParameters >();
//						curve = new AnimationCurve (new Keyframe[] {
//								new Keyframe (0f, 0f),
//								new Keyframe (1f, 1f)
//						});

				}

				public override void Awake ()
				{
          
						Debug.Log ("Awake");
			


				}
		
				public override void Start ()
				{

						Debug.Log ("Start");

						
						//animator.runtimeAnimatorController = _runtimeAnimaController;
						PlayAnimaState (selectedAnimaStateInfo.hash, selectedAnimaStateInfo.layer, timeStart, transitionDuration);

						
						//animator.Play(selectedStateHash,layer,normalizedTime);

				}

				public override void OnTick ()
				{
						base.OnTick ();

				}


//		public override void OnTick ()
//		{
//			Status status = Status.Error;
//			for (int i = 0; i < this.m_Children.Length; i++)
//			{
//				this.m_Children [i].OnTick ();
//				status = this.m_Children [i].status;
//				if (status == Status.Error || status == Status.Running)
//				{
//					break;
//				}
//			}
//			base.status = status;
//		}

//		public override void OnTick ()
//		{
//			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (selectedAnimaStateInfo.layer);
//			Debug.Log (stateInfo.loop);
//			Debug.Log (stateInfo.normalizedTime);
//
//			this.status = Status.Running;
//		}

				public override void Update ()
				{

		
						AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (selectedAnimaStateInfo.layer);
						normalizedTimeCurrent = stateInfo.normalizedTime;


//						if (loop && animator.GetNextAnimatorStateInfo (selectedAnimaStateInfo.layer).nameHash != selectedAnimaStateInfo.hash) {
//						
//								Debug.Log ("Mecanim node : Animator have been assigned to other Anima State");		
//								loop = false;
//								this.status = Status.Success;
//								return;
//			
//						}


						Debug.Log ("stateInfo.Hash " + stateInfo.nameHash + " " + selectedAnimaStateInfo.hash);		


						//int lastLoop = (int)lastNormalizedTime;
						//int currLoop = (int)stateInfo.normalizedTime;

						//				float lastNormalizedTime = lastLayerState[layer].normalizedTime - lastLoop;
						//				float currNormalizedTime = stateInfo.normalizedTime - currLoop;

						if (stateInfo.nameHash == selectedAnimaStateInfo.hash
								&& normalizedTimeStart != normalizedTimeCurrent) {// && !animator.IsInTransition(selectedAnimaStateInfo.layer)){


								Debug.Log ("NormalizedTime: " + (stateInfo.normalizedTime));





							
							
								if (normalizedTimeCurrent > 1f) {
										if (loop) {
												PlayAnimaState (selectedAnimaStateInfo.hash, selectedAnimaStateInfo.layer, timeStart, transitionDuration);
										} else {
												this.status = Status.Success;
												return;
							
										}

								} else {

										//send event if its between previous and current time
										if (normalizedTimeLast < 0.67f && normalizedTimeCurrent >= 0.67f)
												Debug.Log ("Event sent designated at 0.67 sent at:" + normalizedTimeCurrent);
								}

							
								if (selectedAnimaStateInfo.blendParamsIDs != null && (numBlendParamters = selectedAnimaStateInfo.blendParamsIDs.Length) > 0) {

										if (numBlendParamters > 1) {
												animator.SetFloat (selectedAnimaStateInfo.blendParamsIDs [0], this.blackboard.GetFloatVar (this.blendParamXBlackboardBindID).Value);
												animator.SetFloat (selectedAnimaStateInfo.blendParamsIDs [1], this.blackboard.GetFloatVar (this.blendParamYBlackboardBindID).Value);

										} else
												animator.SetFloat (selectedAnimaStateInfo.blendParamsIDs [0], this.blackboard.GetFloatVar (blendParamXBlackboardBindID).Value);


								}


								Debug.Log ("Update at: " + normalizedTimeCurrent);	

								//characterControllerRadius.Value= curve.Evaluate(normalizedTimeCurrent);

								normalizedTimeLast = normalizedTimeCurrent;



			
						} else {

								Debug.LogWarning ("MecanimNode Update on wrong  AnimaState");
						}

						//					this.status = Status.Running;
//			Debug.Log (stateInfo.nameHash);
//
//				if (stateInfo.nameHash == selectedAnimaStateInfo.hash) {
//				    if(stateInfo.normalizedTime < 1f){
//					normalizedTime=stateInfo.normalizedTime;
//					this.status = Status.Running;
//						} else {
//					this.status = Status.Success;
//						}
//
//
//				}
//
//
						this.status = Status.Running;

				}

				public override void End ()
				{

				}


//
//		public static MecanimEvent[] GetEvents(Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> contextLoadedData,
//		                                       Dictionary<int, Dictionary<int, AnimatorStateInfo>> contextLastStates,
//		                                       int animatorControllerId, Animator animator)
//		{
//			List<MecanimEvent> allEvents = new List<MecanimEvent>();
//			
//			int animatorHash = animator.GetHashCode();
//			if (!contextLastStates.ContainsKey(animatorHash))
//				contextLastStates[animatorHash] = new Dictionary<int, AnimatorStateInfo>();
//			
//			int layerCount = animator.layerCount;
//			
//			Dictionary<int, AnimatorStateInfo> lastLayerState = contextLastStates[animatorHash];
//			
//			for (int layer = 0; layer < layerCount; layer++) {
//				if (!lastLayerState.ContainsKey(layer)) {
//					lastLayerState[layer] = new AnimatorStateInfo();
//				}
//				
//				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
//				
//				int lastLoop = (int)lastLayerState[layer].normalizedTime;
//				int currLoop = (int)stateInfo.normalizedTime;
//				float lastNormalizedTime = lastLayerState[layer].normalizedTime - lastLoop;
//				float currNormalizedTime = stateInfo.normalizedTime - currLoop;
//				
//				if (lastLayerState[layer].nameHash == stateInfo.nameHash) {
//					if (stateInfo.loop == true) {
//						if (lastLoop == currLoop) {
//							allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, stateInfo.nameHash, stateInfo.tagHash, lastNormalizedTime, currNormalizedTime));
//						}
//						else {
//							allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, stateInfo.nameHash, stateInfo.tagHash, lastNormalizedTime, 1.00001f));
//							allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, stateInfo.nameHash, stateInfo.tagHash, 0.0f, currNormalizedTime));
//						}
//					}
//					else {
//						float start = Mathf.Clamp01(lastLayerState[layer].normalizedTime);
//						float end = Mathf.Clamp01(stateInfo.normalizedTime);
//						
//						if (lastLoop == 0 && currLoop == 0) {
//							if (start != end)
//								allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, stateInfo.nameHash, stateInfo.tagHash, start, end));
//						}
//						else if (lastLoop == 0 && currLoop > 0) {
//							allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, lastLayerState[layer].nameHash, lastLayerState[layer].tagHash, start, 1.00001f));
//						}
//						else {
//							
//						}
//					}
//				}
//				else {
//					
//					allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, stateInfo.nameHash, stateInfo.tagHash, 0.0f, currNormalizedTime));
//					
//					if (!lastLayerState[layer].loop) {
//						allEvents.AddRange(CollectEvents(contextLoadedData, animator, animatorControllerId, layer, lastLayerState[layer].nameHash, lastLayerState[layer].tagHash, lastNormalizedTime, 1.00001f, true));
//					}
//				}
//				
//				lastLayerState[layer] = stateInfo;
//			}
//			
//			return allEvents.ToArray();
//		}







		}
}