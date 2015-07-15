using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;

namespace ws.winx.editor.windows{
	[CustomEditor(typeof(Sequence))]
	public class SequenceEditor : Editor {

		SequenceNodeEditor nodeEditor;



		SequenceNode selectedNodePrev;

		SerializedProperty selectedNodeSerializedProperty;

		void OnEnable(){
			selectedNodeSerializedProperty = serializedObject.FindProperty ("selectedNode");
			
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("wrap"), new GUIContent("Wrap Mode"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("playOnStart"), new GUIContent("Play On Start"));

			SequenceNode selectedNode=selectedNodeSerializedProperty.objectReferenceValue as SequenceNode;
			serializedObject.ApplyModifiedProperties ();
			Sequence sequence = target as Sequence;

		

			if (GUILayout.Button ("Play")) {
				sequence.PlayAt();

			}

			if (GUILayout.Button ("Stop Forward")) {
				sequence.Stop(true);
			}

			if (GUILayout.Button ("Stop Reset")) {
				sequence.Stop(false);
			}

			if (GUILayout.Button ("Pause")) {
				sequence.Pause();
			}
			if (GUILayout.Button ("UnPause")) {
				sequence.UnPause();
			}
			if (GUILayout.Button ("Restart")) {
				sequence.Restart();
			}
			if (GUILayout.Button ("Open Editor")) {
				SequenceEditorWindow.ShowWindow();
			}

			if (selectedNode != null){

				if(selectedNode!=selectedNodePrev)
								  	 nodeEditor=Editor.CreateEditor (selectedNode, typeof(SequenceNodeEditor)) as SequenceNodeEditor;

				selectedNodePrev=selectedNode;

				nodeEditor.OnInspectorGUI();
			}


		}

	}
}