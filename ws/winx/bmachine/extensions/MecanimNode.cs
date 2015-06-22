//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
//----------------------------------------------
//           MecanimNode
// 			by Alex Winx
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BehaviourMachine;
using Motion=UnityEngine.Motion;
using ws.winx.unity;
using ws.winx.unity.attributes;
using System.Runtime.Serialization;

namespace ws.winx.bmachine.extensions
{

		/// <summary>
		/// !!! MecaniNode goes KABOOM on new compilation if added not saved
		/// </summary>
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Mecanima inside BTree")]
		public class MecanimNode:CompositeNode
		{
				
		
				public new BlackboardCustom blackboard {
						get{ return (BlackboardCustom)base.blackboard; }
				}

				[HideInInspector]
				public EditorClipBinding[]
						clipBindings;
				[HideInInspector]
				public AnimationCurve[]
						curves;
				[HideInInspector]
				public Color[]
						curvesColors;
				[HideInInspector]
				public UnityVariable[]
						variablesBindedToCurves;
				[HideInInspector]
				public Animator
						animator;
				[AnimatorStateAttribute("animator","layer")]
				public AnimatorState
						animatorStateSelected;
				[HideInInspector]
				public int
						layer;
				[UnityVariablePropertyAttribute(typeof(AnimationClip),"Motion Override")]
				public UnityVariable
						motionOverride;
				public bool loop = false;
				[UnityVariableProperty(typeof(float),"blendX")]
				public UnityVariable
						blendX;
				[UnityVariableProperty(typeof(float),"blendY")]
				public UnityVariable
						blendY;

				//The duration of the transition. Value is in source state normalized time.
				//if this value is 0.3 as normalized time, so 0.3 * source.length=timeInSeconds would
				// be used in source and destination state for transition
				[RangeAttribute(0f,1f)]
				public float
						transitionDuration = 0.1f;
				[MinMaxRangeSAttribute(0f,1f)]
				public MinMaxRangeSO
						range;

			
				//[RangeAttribute(0f,1f)]
				//[HideInInspector]
				//public float
				//timeNormalizedStart = 0f;
				[HideInInspector]
				public float
						timeNormalizedCurrent = 0f;
				[HideInInspector]
				public float
						animatorStateRunTimeControl = 0.5f;
				public bool
						animationRunTimeControlEnabled = false;
				public bool mirror = false;
				public float speed = 1f;
				public float weight = 1f;

				/// <summary>
				/// Mecanim AnimatorStateRuntimeControl is in conflict with Rigidbody
				/// number of MecanimNodes with set AnimatorStateRuntimeControl
				/// </summary>
				public static List<int> _nAnimatorStateRuntimeControlHaveEnabled;

				public static List<int> nAnimatorStateRuntimeControlHaveEnabled {
						get {
								if (_nAnimatorStateRuntimeControlHaveEnabled == null)
										_nAnimatorStateRuntimeControlHaveEnabled = new List<int> ();
								return _nAnimatorStateRuntimeControlHaveEnabled;
						}
				}
				
				//public static SerializedObject rigidbodySerializedObject;

		
				float normalizedTimeLast = 0f;
				int numBlendParamters;
				AnimatorStateInfo animatorStateInfoCurrent;
				AnimatorStateInfo animatorStateInfoNext;
				bool isCurrentEqualToSelectedAnimaInfo;
				bool isSelectedAnimaInfoInTransition;
				List<int> _treeInx;
				int _LastTickedChildren = -1;
				AnimatorOverrideController _animatorOverrideController;

				public AnimatorOverrideController animatorOverrideController {
						get {
								if (_animatorOverrideController == null) {
										if (animator.runtimeAnimatorController is AnimatorOverrideController) {
												//animator.runtimeAnimatorController is already overrided just take reference
												_animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
										} else {
												_animatorOverrideController = new AnimatorOverrideController ();

												//bind all clips from animator.runtimeAnimatorController to overrider
												_animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
										}												
				

								}

								return _animatorOverrideController;
						}
				}

				public override void OnEnable ()
				{
						//Debug.Log ("OnEnable");
						base.OnEnable ();
						//	animator = self.GetComponent<Animator> ();
					
				}

				public override void Remove (ActionNode child)
				{
						base.Remove (child);
				}

				public override bool Add (ActionNode child)
				{
						bool result = false;

						if (!(child is SendEventNormalizedNode)) {
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
				
						animator = self.GetComponent<Animator> ();

						
						transitionDuration = 0f;

						
						clipBindings = new EditorClipBinding[0];

						curvesColors = new Color[0];
						curves = new AnimationCurve[0];
						variablesBindedToCurves = new UnityVariable[0];

						animatorStateSelected = (AnimatorState)ScriptableObject.CreateInstance<AnimatorState> ();
						
						blendX = UnityVariable.CreateInstanceOf (typeof(float));
						
						

						blendY = UnityVariable.CreateInstanceOf (typeof(float));
						
					
						motionOverride = UnityVariable.CreateInstanceOf (typeof(AnimationClip));
						
						range = (ws.winx.unity.attributes.MinMaxRangeSO)ScriptableObject.CreateInstance<ws.winx.unity.attributes.MinMaxRangeSO> ();
				
						Array.ForEach (this.children, (itm) => {
			
								//remove localy from node parent
								this.Remove (itm);
								//remove from internal behaviour tree
								this.tree.RemoveNode (itm, false);
						});


						
			
				}
		
				public override void Awake ()
				{
						//	Debug.Log ("Awake");
						//animaStateInfoSelected1 = ((AnimatorController)animator.runtimeAnimatorController).
						//layers [0].stateMachine.states [0].state;

						if (animator == null)
								animator = self.GetComponent<Animator> ();

						
			
				}


				//
//								public override void OnEnable ()
//								{
//							
//									//	Debug.Log (animaStateInfoSelected.label.text + ">Enable");
//				
//								}
				//		
				//				public override void OnDisable ()
				//				{
				//			
				//						Debug.Log (selectedAnimaStateInfo.label.text + ">Disable");
				//			
				//			
				//			
				//				}
		
//				public override void Start ()
//				{
//
////						Debug.Log (selectedAnimaStateInfo.label.text + ">Start MecanimNode");
//
//						_LastTickedChildren = -1;
//
//						PlayAnimaState ();
//
//				}



				/// <summary>
				/// Plays the state of the mecanima.
				/// </summary>
				void PlayAnimaState ()
				{
			
						//if we are already in that AnimaState
						//cos normalied time doesn't stop but continue to increase
						if (isCurrentEqualToSelectedAnimaInfo) {
								//if(isSelectedAnimaInfoInTransition)
								//normalizedTimeLast =animatorStateInfoNext.normalizedTime
								//else
								normalizedTimeLast = animatorStateInfoCurrent.normalizedTime;
				
				
						} else {
				
				
				
								normalizedTimeLast = 0f;
				
				
						}
			
			
						//						Debug.Log (animaStateInfoSelected.label.text + ">Crosfade() "); 
						//						Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
						//						Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
						//						Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
						//			
			
			
						//		animator.Play (selectedAnimaStateInfo.hash, selectedAnimaStateInfo.layer, normalizedTimeStart);
						animator.CrossFade (animatorStateSelected.nameHash, transitionDuration, layer, range.rangeStart);

						//loop thru binded animation and play them
						EditorClipBinding clipBindingCurrent;
						Animator animatorComponent;

						int clipBindingsNum = this.clipBindings.Length;

						for (int i=0; i<clipBindingsNum; i++) {

								clipBindingCurrent = clipBindings [i];

								if (clipBindingCurrent.gameObject != null && clipBindingCurrent.clip != null) {

										if ((animatorComponent = clipBindingCurrent.gameObject.GetComponent<Animator> ()) == null) 
												animatorComponent = clipBindingCurrent.gameObject.AddComponent<Animator> ();

										GameObject gameObjectBinded = clipBindingCurrent.gameObject;
										gameObjectBinded.transform.rotation = self.transform.rotation * clipBindingCurrent.rotationOffset;
										gameObjectBinded.transform.position = self.transform.position + self.transform.rotation * clipBindingCurrent.positionOffset;

										//speed of bindind with main animation should be same (also same framerate)
										animatorComponent.speed = animator.speed;

										if (clipBindingCurrent.clip.frameRate != (animatorStateSelected.motion as AnimationClip).frameRate)
												Debug.LogWarning (clipBindingCurrent.clip.name + " should have frameRate of " + (animatorStateSelected.motion as AnimationClip).frameRate + " instead of " + clipBindingCurrent.clip.frameRate);
					   
										//clipBindingCurrent
										animatorComponent.enabled = true;
										animatorComponent.Play (clipBindingCurrent.clip, range.rangeStart);
											

								}
						}
						
			
				}

				public override Status Update ()
				{
					
						

						if (animatorStateSelected == null)
								return Status.Failure;


						
						animatorStateInfoCurrent = animator.GetCurrentAnimatorStateInfo (layer);
			
						animatorStateInfoNext = animator.GetNextAnimatorStateInfo (layer);
			
						isSelectedAnimaInfoInTransition = animator.IsInTransition (layer) && (animatorStateInfoNext.shortNameHash == animatorStateSelected.nameHash);
			
						isCurrentEqualToSelectedAnimaInfo = animatorStateInfoCurrent.shortNameHash == animatorStateSelected.nameHash;

						
						//Debug.Log (this.name + " isTransition:" + isSelectedAnimaInfoInTransition+" isCurrent:"+isCurrentEqualToSelectedAnimaInfo+" t:"+animatorStateInfoCurrent.normalizedTime);


						///////////////////////  START  //////////////////////////

						//The second check ins't nessery if I could reset Status when this node is switched
						if (this.status != Status.Running) {
				
								// 
								if (motionOverride != null && this.motionOverride.Value != null && animatorOverrideController [(AnimationClip)animatorStateSelected.motion] != (AnimationClip)motionOverride.Value) {
					
					
										//	Debug.Log (this.name + ">Selected state Motion " + animaStateInfoSelected.motion + "to be overrided with " + motionOverride);
					
										animatorOverrideController [(AnimationClip)animatorStateSelected.motion] = (AnimationClip)motionOverride.Value;
					
										//	Debug.Log (this.name + ">Override result:" + animatorOverrideController [(AnimationClip)animaStateInfoSelected.motion] );
					
					
										//to avoid nesting 
										if (animator.runtimeAnimatorController is AnimatorOverrideController) {
												animator.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
										}
					
										//rebind back												
										animator.runtimeAnimatorController = animatorOverrideController;
										
					
								}
				
								//TODO investigate AnimatorState access in U5.0
								//animatorStateSelected.mirror=this.mirror;
								//animatorStateSelected.speed=this.speed;
				
								animator.speed = this.speed;

				
								animator.SetLayerWeight (layer, this.weight);
				
								//this.Start ();	

								_LastTickedChildren = -1;
								
								PlayAnimaState ();
								////////////////////
								
								return Status.Running;
						}

						//////////////////////////////////////////////////////////////////////
					


				

						///////////////////  UPDATE  /////////////////////

						if (isCurrentEqualToSelectedAnimaInfo) {	




								if (loop)
										timeNormalizedCurrent = animatorStateInfoCurrent.normalizedTime - (int)animatorStateInfoCurrent.normalizedTime;
								else
										timeNormalizedCurrent = animatorStateInfoCurrent.normalizedTime;




//						Debug.Log (animaStateInfoSelected.label.text + ">Update() "); 
//						Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
//						Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
//						Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
//		

						

		
					
								if (isCurrentEqualToSelectedAnimaInfo
										&& normalizedTimeLast != timeNormalizedCurrent) {

										//Debug.Log ("NormalizedTime: " + (animatorStateInfoCurrent.normalizedTime));


							
										//1f
										if (timeNormalizedCurrent > range.rangeEnd) {
												if (!loop) {
														

														return Status.Success;
												}

										} else {

												//send event if its between previous and current time
												//Test only, event sending is done try SentEventNormalized
												//if (normalizedTimeLast < 0.67f && normalizedTimeCurrent >= 0.67f)
												//Debug.Log ("Event sent designated at 0.67 sent at:" + normalizedTimeCurrent);




												if (this.speed < 0f && timeNormalizedCurrent < 0f) {
														if (!loop) {
																

																return Status.Success;
												
														} else {
																animator.Play (animatorStateSelected.nameHash, layer, range.rangeStart);
																return Status.Running;
														}

														
												}

												////////   TICK CHILDREN ///////
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

							
										
										int[] blendParamsHashes;

										//Debug.Log("animatorStateSelected.blendParamsHashes="+animatorStateSelected.blendParamsHashes);

										//!!! Blend Tree although is seraialized in Editor in StandAlonePlayer is NULL and condition animatorStateSelected.motion != null fail
										if (animatorStateSelected != null && (blendParamsHashes = animatorStateSelected.blendParamsHashes) != null && blendParamsHashes.Length > 0) {

												
												
												
												animator.SetFloat (blendParamsHashes [0], (float)blendX.Value);

												if (blendParamsHashes.Length > 1) {
														animator.SetFloat (blendParamsHashes [1], (float)blendY.Value);

												}
							
												

							
						
						
										}
					
					
										//Debug.Log (animaStateInfoSelected.label.text + ">Update at: " + timeNormalizedCurrent);	



										if (animationRunTimeControlEnabled) {
												animator.Update (animatorStateRunTimeControl - animatorStateInfoCurrent.normalizedTime);

										}

										//Calculate curves values
										int numCurves = curves.Length;
										for (int j=0; j<numCurves; j++) 
												variablesBindedToCurves [j].Value = curves [j].Evaluate (timeNormalizedCurrent);

										normalizedTimeLast = timeNormalizedCurrent;



			
								} else {

//										Debug.LogWarning ("MecanimNode Update on wrong  AnimaState");
										//this.status = Status.Error;
										//return;
								
								}


						
						
								

						}


						return Status.Running;
						

				}

				public override void End ()
				{
						if (animatorStateSelected != null)
			   			//restore layer weight
								animator.SetLayerWeight (layer, 0);
				}





		}
}