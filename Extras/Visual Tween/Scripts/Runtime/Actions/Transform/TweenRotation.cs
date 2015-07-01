using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityTransform{
	[System.Serializable]
	[Category("Transform")]
	public class TweenRotation : TweenAction {
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
				cachedTransform.rotation = Quaternion.Euler (GetValue (from, to, percentage));
			}
		}

		private Quaternion originalRotation;
		public override void RecordAction (GameObject target)
		{
			originalRotation = target.transform.rotation;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.transform.rotation = originalRotation;
		}
	}
}