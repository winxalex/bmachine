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
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator")]
	public class MecanimNode:ActionNode
	{

				
				[MecanimStateInfoAttribute]
				public MecanimStateInfo
						selectedAnimaStateInfo;
				public bool loop = false;
				[MecanimBlendParameterAttribute(axis=MecanimBlendParameterAttribute.Axis.X)]
				public int
						blendParamXBlackboardBindID;
				[MecanimBlendParameterAttribute(axis=MecanimBlendParameterAttribute.Axis.Y)]
				public int
						blendParamYBlackboardBindID;
				[RangeAttribute(0f,1f)]
				public float
						transitionDuration = 0.3f;
				[RangeAttribute(0f,1f)]
				public float
						normalizedTimeStart = 0.5f;
				//[HideInInspector]
				[RangeAttribute(0f,1f)]
				public float
						normalizedTimeCurrent = 0f;
				

				//!!!Curves serialization is buggy
				//public AnimationCurve curve;

		   


			
				Animator _animator;
				float normalizedTimeLast = 0f;
				int numBlendParamters;
				AnimatorStateInfo currentAnimatorStateInfo;
				bool isCurrentEqualToSelectedAnimaInfo = false;
		                         
				public Animator animator {
						get {
								if (_animator == null) {
										_animator = self.GetComponent<Animator> ();
									
								}
								return _animator;
						}
				}

				void PlayAnimaState ()
				{
					    
						//if we are already in that AnimaState
						//cos normalied time doesn't stop but continue to increase
						if (isCurrentEqualToSelectedAnimaInfo) {
							
								normalizedTimeLast = currentAnimatorStateInfo.normalizedTime;
							
							
						} else {
							
							
							
								normalizedTimeLast = 0f;
							
							
						}
						
						animator.CrossFade (selectedAnimaStateInfo.hash, transitionDuration, selectedAnimaStateInfo.layer, normalizedTimeStart);

				}
		
				public override void Reset ()
				{
						//Debug.Log (selectedAnimaStateInfo.label.text+">Reset");
						_animator = null;
						
						transitionDuration = 0f;
				}

				public override void Awake ()
				{
          
						Debug.Log (selectedAnimaStateInfo.label.text + ">Awake");
			


				}

				public override void OnEnable ()
				{
			
						Debug.Log (selectedAnimaStateInfo.label.text + ">Enable");
			
			



				}

//				public override void EditorOnTick ()
//				{
//						base.EditorOnTick ();
//
//						Debug.Log (selectedAnimaStateInfo.label.text + ">EditorTick");
//
//				}
		
				public override void OnDisable ()
				{
			
						Debug.Log (selectedAnimaStateInfo.label.text + ">Disable");
			
			
			
				}
		
				public override void Start ()
				{

						Debug.Log (selectedAnimaStateInfo.label.text + ">Start MecanimNode");

						PlayAnimaState ();
			
//			if (!isCurrentEqualToSelectedAnimaInfo || !loop)
//								PlayAnimaState ();
//						else {
//							Debug.Log("No play needed");
//						}
				}

				public override void OnTick ()
				{

						Debug.Log (selectedAnimaStateInfo.label.text + ">OnTick");

						currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo (selectedAnimaStateInfo.layer);
						isCurrentEqualToSelectedAnimaInfo = currentAnimatorStateInfo.nameHash == selectedAnimaStateInfo.hash;

						Debug.Log (selectedAnimaStateInfo.label.text + ">Before");


					//The second check ins't nessery if I could reset Status when this node is switched
					if (this.status != Status.Running || (!isCurrentEqualToSelectedAnimaInfo && loop && this.status == Status.Running)) {
			
								this.Start ();	
				 
								this.status = Status.Running;
								return;
						}

						Debug.Log (selectedAnimaStateInfo.label.text + ">" + Status.Running);

			
						if (isCurrentEqualToSelectedAnimaInfo)
								this.Update ();
//						else if(loop)//I need to this check
//								this.Start ();

						if (this.status != Status.Running) {
								this.End ();
						}
				}

				public override void Update ()
				{

		
						//-(int)currentAnimatorStateInfo.normalizedTime;

						if (loop)
								normalizedTimeCurrent = currentAnimatorStateInfo.normalizedTime - (int)currentAnimatorStateInfo.normalizedTime;
						else
								normalizedTimeCurrent = currentAnimatorStateInfo.normalizedTime;




						Debug.Log (selectedAnimaStateInfo.label.text + ">Update() "); 
						Debug.Log (selectedAnimaStateInfo.label.text + ">current state: " + currentAnimatorStateInfo.nameHash + " requested state " + selectedAnimaStateInfo.hash);
						Debug.Log (selectedAnimaStateInfo.label.text + ">current state time= " + currentAnimatorStateInfo.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
						Debug.Log (selectedAnimaStateInfo.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (selectedAnimaStateInfo.layer).userNameHash); 
//		

						

		
					
						if (isCurrentEqualToSelectedAnimaInfo
								&& normalizedTimeLast != normalizedTimeCurrent) {

								//Debug.Log ("NormalizedTime: " + (currentAnimatorStateInfo.normalizedTime));


							
							
								if (normalizedTimeCurrent > 1f) {
										if (!loop) {
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
								//this.status = Status.Error;
								//return;
								
						}


						
						
						this.status = Status.Running;

				}


		public override void ResetStatus ()
		{
			Debug.Log (selectedAnimaStateInfo.label.text + ">ResetStatus ");
			base.ResetStatus ();
		}

				public override void End ()
				{
						Debug.Log (selectedAnimaStateInfo.label.text + ">" + this.status);

						Debug.Log (selectedAnimaStateInfo.label.text + ">End ");
						base.End ();
			            
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