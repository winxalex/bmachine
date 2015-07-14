using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;

namespace ws.winx.editor.windows{
	[CustomEditor(typeof(SequenceNode))]
	public class SequenceNodeEditor : Editor {

		public override void OnInspectorGUI ()
		{
			this.DrawDefaultInspector ();
		}

	}
}