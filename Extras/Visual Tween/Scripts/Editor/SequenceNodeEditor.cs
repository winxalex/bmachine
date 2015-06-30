using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VisualTween{
	[CustomEditor(typeof(SequenceNode))]
	public class SequenceNodeEditor : Editor {

		public override void OnInspectorGUI ()
		{
			this.DrawDefaultInspector ();
		}

	}
}