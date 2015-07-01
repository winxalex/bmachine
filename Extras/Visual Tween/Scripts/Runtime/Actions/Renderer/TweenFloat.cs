using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityRenderer{
	[RequireComponent (typeof (Renderer))]
	[System.Serializable]
	[Category("Renderer")]
	public class TweenFloat : TweenAction {
		public float from=0.0f;
		public float to=1.0f;
		public string property="_Shininess";
		
		private Renderer cachedRenderer;
		
		public override void OnEnter (GameObject target)
		{
			cachedRenderer = target.GetComponent<Renderer>();
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedRenderer)
			{
				float value = GetValue(from,to,percentage);
				this.cachedRenderer.material.SetFloat(property, value);
			}
		}

		private float recValue;
		public override void RecordAction (GameObject target)
		{
			recValue = target.GetComponent<Renderer>().material.GetFloat(property);
		}
		
		public override void UndoAction (GameObject target)
		{
			target.GetComponent<Renderer>().material.SetFloat(property, recValue);
		}
	}
}