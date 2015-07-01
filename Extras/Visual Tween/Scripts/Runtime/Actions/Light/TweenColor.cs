using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityLight{
	[RequireComponent (typeof (Light))]
	[System.Serializable]
	[Category("Light")]
	public class TweenColor : TweenAction {
		public Color from=Color.white;
		public Color to=Color.black;

		private Light cachedLight;
		
		public override void OnEnter (GameObject target)
		{
			cachedLight = target.GetComponent<Light>();
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedLight)
			{
				Color value = GetValue(from,to,percentage);
				this.cachedLight.color=value;
			}
		}

		private Color recValue;
		public override void RecordAction (GameObject target)
		{
			recValue = target.GetComponent<Light>().color;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.GetComponent<Light>().color = recValue;
		}
	}
}