using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityAudio{
	[RequireComponent (typeof (AudioSource))]
	[System.Serializable]
	[Category("Audio")]
	public class TweenPitch : TweenAction {
		public float from=1.0f;
		public float to=1.5f;
		
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
				this.cachedAudio.pitch=value;
			}
		}

		private float recValue;
		public override void RecordAction (GameObject target)
		{
			recValue = target.GetComponent<AudioSource>().pitch;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.GetComponent<AudioSource>().pitch = recValue;
		}
	}
}