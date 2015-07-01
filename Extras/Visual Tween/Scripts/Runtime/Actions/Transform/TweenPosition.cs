using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityTransform{
	[System.Serializable]
	[Category("Transform")]
	public class TweenPosition : TweenAction {
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
				cachedTransform.position = GetValue (from, to, percentage);
			}
		}

		private Vector3 originalPosition;
		public override void RecordAction (GameObject target)
		{
			originalPosition = target.transform.position;
		}

		public override void UndoAction (GameObject target)
		{
			target.transform.position = originalPosition;
		}
	}
}