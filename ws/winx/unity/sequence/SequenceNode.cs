using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System;
using UnityEngine.Events;

namespace ws.winx.unity.sequence
{
		[System.Serializable]
		public class SequenceNode:ScriptableObject
		{

				public SequenceNodeEvent onStart = new SequenceNodeEvent ();
				public SequenceNodeEvent onStop = new SequenceNodeEvent ();
				public SequenceNodeEvent onPause = new SequenceNodeEvent ();
				public SequenceNodeEvent onUpdate = new SequenceNodeEvent ();
				public int index = -1;
				public float transition = 0f;
				public bool loop;
				public float volume = 1f;

				/// <summary>
				/// The start time in Frames
				/// </summary>
				public float startTime;
				float _timeLocal;
				float _timeNormalized;


				/// <summary>
				/// The duration in [s] 
				/// </summary>
				[SerializeField]
				float
						_duration;
				float _durationInv = 0.2f;// 1/5

				public float duration {
						get {
								return _duration;
						}
						set {
								_duration = value;
								_durationInv = 1 / _duration;
						}
				}

				//public int channelOrd;
				public SequenceChannel channel;
				bool _isRunning;

				public bool isRunning {
						get {
								return _isRunning;
						}
				}

				public UnityEngine.Object source;
				public int stateNameHash;

				public void StartNode ()
				{

						Debug.Log ("StartNode " + source.name);

						

						GameObject target = channel.target;

						_isRunning = true;

						if (target != null) {
								if (Application.isPlaying) {
										if (source is AudioClip) {
											
											
												AudioSource audioSource = target.GetComponent<AudioSource> ();

												if (audioSource == null)
														audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
											
												audioSource.playOnAwake = false;
												audioSource.clip = source as AudioClip;

												audioSource.time = _timeLocal;
												audioSource.volume = volume;
												audioSource.Play ();
					                        


										} else if (source is MovieTexture) {
												MovieTexture movieTexture = (source as MovieTexture);
												if (target.tag == "MainCamera") {//==Camera.main
														RenderSettings.skybox.mainTexture = movieTexture;
												} else {

														Renderer renderer = channel.target.GetComponent<Renderer> ();
														if (renderer != null) {
														

																movieTexture.Stop ();

														
																renderer.material.mainTexture = movieTexture;
														
														} else
																Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
												}


												AudioSource audioSource = null;
												if (movieTexture.audioClip != null) {
													
														audioSource = target.GetComponent<AudioSource> ();
													
														if (audioSource == null)
																audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
													
														audioSource.clip = movieTexture.audioClip;
														audioSource.playOnAwake = false;
														//audioSource.PlayOneShot (audioSource.clip);
														audioSource.Play ();

															
												}



												movieTexture.Play ();


														


					
												 
										
										} else if (source is  AnimationClip) {

												Animator animator = target.GetComponent<Animator> ();
										
												if (animator == null) {
														animator = target.AddComponent<Animator> ();
												}
												animator.runtimeAnimatorController = this.channel.runtimeAnimatorController;
												animator.enabled = true;

									   
												animator.Update (_timeNormalized);

												if (transition > 0) {
														Debug.Log ("Crossfade " + this.name);
														animator.CrossFade (stateNameHash, transition, 0, 0f);
												} else {
														Debug.Log ("Play " + this.name);
														animator.Play (stateNameHash, 0, 0f);
												}
										


										
										}

								}

								onStart.Invoke (this);
				
						} else {
								Debug.LogWarning ("No target set on channel " + this.channel.name);
						}

//TODO put events with drag and drop handlers

						
				}

				public void Stop ()
				{
						GameObject target = channel.target;

						_isRunning = false;

						Debug.Log ("StopNode " + source.name);

						

						if (target != null) {
								if (Application.isPlaying) {
										if (source is AudioClip) {
												AudioSource audioSource = target.GetComponent<AudioSource> ();
										
												if (audioSource != null)
														audioSource.Stop (); 
								
										} else if (source is MovieTexture) {
												Renderer renderer = target.GetComponent<Renderer> ();
												if (renderer != null) {
														MovieTexture movieTexture = (source as MovieTexture);
								
														if (_timeLocal > 0) {
																movieTexture.Pause ();
																Debug.Log ("SequenceNode>Video paused");
														} else {
																movieTexture.Stop ();
																Debug.Log ("SequenceNode>Video stopped");
														}
//
														AudioSource audioSource = null;
														if (movieTexture.audioClip != null && (audioSource = target.GetComponent<AudioSource> ()) != null)
																audioSource.Stop ();
									
												} else
														Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
										} else if (source is AnimationClip) {

												//hard stop
//										Animator animator = target.GetComponent<Animator> ();
//															
//										//
//										if (animator != null && this.index+1==this.channel.nodes.Count)
//												animator.enabled = false;

																	
								
							
								
										}
							
							
								}
						}


						onStop.Invoke (this);

				}

				public virtual void DoUpdate ()
				{
						if (onUpdate != null)
								onUpdate.Invoke (this);
				}

				public void UpdateNode (double time)
				{
						_timeLocal = ((float)time - startTime);
						_timeNormalized = (_timeLocal / _duration);

			              
						// 
						if (_isRunning) {
								if (_timeNormalized <= 0.0f || _timeNormalized > 1.0f) 
										Stop ();
								else
										DoUpdate ();
						} else {
								if (_timeNormalized > 0.0f && _timeNormalized <= 1.0f) {
										StartNode ();
								
								
								}
								
						}
						


				}




		}
}