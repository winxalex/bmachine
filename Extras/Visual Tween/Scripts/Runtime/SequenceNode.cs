using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System;

namespace VisualTween
{
		[System.Serializable]
		public class SequenceNode:ScriptableObject
		{
				[SerializeField]
				GameObject
						_target;

				public GameObject target {
						get {
								return _target;
						}
						set {
								_target = value;
						}
				}

				/// <summary>
				/// The start time in Frames
				/// </summary>
				public float startTime;

				/// <summary>
				/// The duration in Frames (default=5Frames)
				/// </summary>
				public float duration = 5;
				public int channel;
				public List<BaseAction> actions;
				private bool isRunning;
			
			
				

				public UnityEngine.Object source;

				

				int stateNameHash;

				public void StartTween ()
				{

						if (target != null) {
								if (source is AudioClip) {
										AudioSource.PlayClipAtPoint (source as AudioClip, target.transform.position);

								} else if (source is MovieTexture) {
										Renderer renderer = target.GetComponent<Renderer> ();
										if (renderer != null) {
												MovieTexture movieTexture=(source as MovieTexture);
												renderer.material.mainTexture = movieTexture;
												if(movieTexture.audioClip!=null)
												AudioSource.PlayClipAtPoint(movieTexture.audioClip,target.transform.position);
												movieTexture.Play ();
										} else
												Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
								} else if (source is AnimationClip) {
										Animator animator = target.GetComponent<Animator> ();
										
										if (animator)
												animator.CrossFade (stateNameHash, 0f, 0, 0f);
								}
						
				     
						}


						foreach (BaseAction action in actions) {
								action.OnEnter (target);		
						}
						isRunning = true;
				}
		
				public void CompleteTween ()
				{
						foreach (BaseAction action in actions) {
								action.OnExit (target);		
						}
						isRunning = false;
				}

				public void CompleteTween (bool forward)
				{
						if (!isRunning) {
								StartTween ();
						}

						foreach (BaseAction action in actions) {
								action.OnUpdate (target, forward ? 1.0f : 0.0f);		
						}	

						CompleteTween ();
				}

				public void UpdateTween (float time)
				{
						float percentage = ((time - startTime) / duration);
	
						if (percentage > 0.0f && percentage <= 1.0f) {
								StartTween ();
						}
			
						if (isRunning) {
								foreach (BaseAction action in actions) {
										action.OnUpdate (target, percentage);		
								}
						}
			
						if (percentage <= 0.0f || percentage > 1.0f) {
								CompleteTween ();
						}
				}

				public void DoOnGUI ()
				{
						foreach (BaseAction action in actions) {
								action.OnGUI ();		
						}
				}

				private bool recorded;

				public void RecordAction ()
				{
						if (!recorded && target != null) {
								if (actions == null) {
										actions = new List<BaseAction> ();
								}
								foreach (BaseAction action in actions) {
										action.RecordAction (target);		
								}
								recorded = true;
						}
				}

				public void UndoAction ()
				{
						if (recorded && target != null) {
								foreach (BaseAction action in actions) {
										action.UndoAction (target);		
								}
								recorded = false;
						}
				}
		}
}