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
				GameObject _target;

				public GameObject target {
						get {
								return _target;
						}
						set {
								_target = value;
						}
				}


				public float startTime;
				public float duration = 5;
				public int channel;
				public List<BaseAction> actions;
				private bool isRunning;
				public bool eventNode;
				public int frameRate = 30;
				public AudioClip audioClip;
				public MovieTexture movieTexture;

				[SerializeField]
				public AnimationClip _animationClip;

				public AnimationClip animationClip {
						get {
								return _animationClip;
						}
						set {
								_animationClip = value;
								this.duration=frameRate*value.length;
						}
				}

				int stateNameHash;

				public void StartTween ()
				{

						if (target != null) {
								if (audioClip != null) {
										AudioSource.PlayClipAtPoint (audioClip, target.transform.position);

								} else if (movieTexture != null) {
										Renderer renderer = target.GetComponent<Renderer> ();
										if (renderer != null) {
												renderer.material.mainTexture = movieTexture;
												movieTexture.Play ();
										} else
												Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
								} else if (animationClip != null) {
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