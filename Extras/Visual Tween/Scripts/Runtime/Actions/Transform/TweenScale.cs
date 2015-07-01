using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityTransform{
	[System.Serializable]
	[Category("Transform")]
	public class TweenScale : TweenAction {
		public Vector3 from;
		public Vector3 to;
		
		private Transform cachedTransform;
		
		public override void OnEnter (GameObject target)
		{
			cachedTransform = target.transform;
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (cachedTransform) {
				cachedTransform.localScale = GetValue (from, to, percentage);
			}
		}

		private Vector3 originalScale;
		public override void RecordAction (GameObject target)
		{
			originalScale = target.transform.localScale;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.transform.localScale = originalScale;
		}
	}
}