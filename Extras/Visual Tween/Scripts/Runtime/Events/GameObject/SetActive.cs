using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityGameObject{
	[System.Serializable]
	[Category("GameObject")]
	public class SetActive : SequenceEvent {
		public bool active;
		public override void OnEnter (GameObject target)
		{
			target.SetActive (active);
		}

		private bool isActive;
		public override void RecordAction (GameObject target)
		{
			isActive = target.activeSelf;
		}

		public override void UndoAction (GameObject target)
		{
			target.SetActive (isActive);
		}
	}
}