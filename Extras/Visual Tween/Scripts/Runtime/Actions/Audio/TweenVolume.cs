using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityAudio{
	[RequireComponent (typeof (AudioSource))]
	[System.Serializable]
	[Category("Audio")]
	public class TweenVolume : TweenAction {
		public float from=0.0f;
		public float to=1.0f;
		
		private AudioSource cachedAudio;
		
		public override void OnEnter (GameObject target)
		{
			cachedAudio = target.GetComponent<AudioSource>();
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedAudio)
			{
				float value = GetValue(from,to,percentage);
				this.cachedAudio.volume=value;
			}
		}

		private float recValue;
		public override void RecordAction (GameObject target)
		{
			recValue = target.GetComponent<AudioSource>().volume;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.GetComponent<AudioSource>().volume = recValue;
		}
	}
}