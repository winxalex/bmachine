using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityLight{
	[RequireComponent (typeof (Light))]
	[System.Serializable]
	[Category("Light")]
	public class TweenIntensity : TweenAction {
		public float from=0.5f;
		public float to=1.0f;
		
		private Light cachedLight;
		
		public override void OnEnter (GameObject target)
		{
			cachedLight = target.GetComponent<Light>();
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedLight)
			{
				float value = GetValue(from,to,percentage);
				this.cachedLight.intensity=value;
			}
		}

		private float recValue;
		public override void RecordAction (GameObject target)
		{
			recValue = target.GetComponent<Light>().intensity;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.GetComponent<Light>().intensity = recValue;
		}
	}
}