using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System;
using UnityEngine.Events;

namespace VisualTween
{
		[System.Serializable]
		public class SequenceNode:ScriptableObject
		{
				public SequenceNodeEvent onStart = new SequenceNodeEvent ();
				public SequenceNodeEvent onStop = new SequenceNodeEvent ();
				public SequenceNodeEvent onPause = new SequenceNodeEvent ();
				public SequenceNodeEvent onUpdate = new SequenceNodeEvent ();
				public int index = -1;
				public float transition = -1f;
				public bool loop;
				public float volume = 1f;

				/// <summary>
				/// The start time in Frames
				/// </summary>
				public float startTime;

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
								if (source is AudioClip) {
											
											
										AudioSource audioSource = target.GetComponent<AudioSource> ();

										if (audioSource == null)
												audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
											
										audioSource.playOnAwake = false;
										audioSource.clip = source as AudioClip;


										//audioSource.clip.LoadAudioData();

										//if(Application.isPlaying)

//					audio.timeSamples = audio.clip.samples - 1;
//					audio.pitch = -1;
//					audio.Play();


//					audioSource.time=20;
//									audioSource.PlayOneShot(audioSource.clip);
//					audioSource.Stop();
//					audioSource.Play();
//
//					audioSource.time=20;
//										audioSource.volume=this.volume;

										//audioSource.timeSamples=audioSource.clip.samples-10000;
					                        


								} else if (source is MovieTexture) {
										MovieTexture movieTexture = (source as MovieTexture);
										if (target == Camera.main) {
												RenderSettings.skybox.mainTexture = (source as MovieTexture);
										} else {

												Renderer renderer = channel.target.GetComponent<Renderer> ();
												if (renderer != null) {
														

														movieTexture.Stop ();

														if (Application.isPlaying)
																renderer.material.mainTexture = movieTexture;
														else
																renderer.sharedMaterial.mainTexture = movieTexture;
												} else
														Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
										}


										AudioSource audioSource = null;
										if (movieTexture.audioClip != null) {
													
												audioSource = target.GetComponent<AudioSource> ();
													
												if (audioSource == null)
														audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
													
												audioSource.clip = movieTexture.audioClip;
												audioSource.PlayOneShot (audioSource.clip);

															
										}



										movieTexture.Play ();


														


					
												 
										
								} else if (source is  AnimationClip) {

										Animator animator = target.GetComponent<Animator> ();
										
										if (animator == null) {
												animator = target.AddComponent<Animator> ();
										}
										animator.runtimeAnimatorController = this.channel.runtimeAnimatorController;
										animator.enabled = true;

										if (transition > 0)
												animator.CrossFade (stateNameHash, transition,0,0f);
										else
												animator.Play (stateNameHash, 0, 0f);
										


										
								}



								onStart.Invoke (this);
				
						}

//TODO put events with drag and drop handlers

						
				}

				public void Stop ()
				{
						GameObject target = channel.target;

						_isRunning = false;

						Debug.Log ("StopNode " + source.name);

						

						if (target != null) {
								if (source is AudioClip) {
										AudioSource audioSource = target.GetComponent<AudioSource> ();
										
										if (audioSource != null)
												audioSource.Stop (); 
								
								} else if (source is MovieTexture) {
										Renderer renderer = target.GetComponent<Renderer> ();
										if (renderer != null) {
												MovieTexture movieTexture = (source as MovieTexture);
								
												movieTexture.Stop ();
//
												AudioSource audioSource = null;
												if (movieTexture.audioClip != null && (audioSource = target.GetComponent<AudioSource> ()) != null)
														audioSource.Stop ();
									
										} else
												Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
								} else if (source is AnimationClip) {
										Animator animator = target.GetComponent<Animator> ();
																				
										if (animator != null)
												animator.enabled = false;

																	
								
							
								
								}
							
							
						}


						onStop.Invoke (this);

				}
		
//				public void End ()
//				{
////						foreach (BaseAction action in actions) {
////								action.OnExit (target);		
////						}
//						isRunning = false;
//
//						Stop ();
//				}

//				public void End (bool forward)
//				{
//						if (!isRunning) {
//								StartNode ();
//						}
//
////						foreach (BaseAction action in actions) {
////								action.OnUpdate (target, forward ? 1.0f : 0.0f);		
////						}	
//
//						End ();
//				}


				public virtual void DoUpdate ()
				{


						GameObject target = channel.target;

			
			
			
//			if (target != null) {
//								if (source is AudioClip) {
//										AudioSource audioSource = target.GetComponent<AudioSource> ();
//
//										audioSource.clip.LoadAudioData ();
//					//audioSource.Play();
//
//					Debug.Log ("DoUpadate" + audioSource.time+" "+audioSource.timeSamples);
//					audioSource.timeSamples=audioSource.timeSamples+1000;
//								}
//						}
					
						onUpdate.Invoke (this);
				}

				public void UpdateNode (double time)
				{

						float timeNormalized = (((float)time - startTime) / _duration);

			              
						// 
						if (_isRunning) {
								if (timeNormalized <= 0.0f || timeNormalized > 1.0f) 
										Stop ();
								else
										DoUpdate ();
						} else {
								if (timeNormalized > 0.0f && timeNormalized <= 1.0f) {
										StartNode ();
								
								
								}
								
						}
						


				}




		}
}