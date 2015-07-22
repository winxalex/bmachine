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
		SerializedProperty wrapSerializedProperty;
		SerializedProperty playOnStartSerializedProperty;
		SerializedProperty OnStartSerializedProperty;
		SerializedProperty OnEndSerializedProperty;

		void OnEnable(){
			selectedNodeSerializedProperty = serializedObject.FindProperty ("selectedNode");
			OnStartSerializedProperty=serializedObject.FindProperty ("OnStart");
			OnEndSerializedProperty=serializedObject.FindProperty ("OnEnd");
			wrapSerializedProperty=serializedObject.FindProperty ("wrap");
			playOnStartSerializedProperty=serializedObject.FindProperty ("playOnStart");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();


			EditorGUILayout.PropertyField (OnStartSerializedProperty, new GUIContent("OnStart"));
			EditorGUILayout.PropertyField (OnEndSerializedProperty, new GUIContent("OnEnd"));


			EditorGUILayout.PropertyField (wrapSerializedProperty, new GUIContent("Wrap Mode"));
			EditorGUILayout.PropertyField (playOnStartSerializedProperty, new GUIContent("Play On Start"));

			SequenceNode selectedNode=selectedNodeSerializedProperty.objectReferenceValue as SequenceNode;
			serializedObject.ApplyModifiedProperties ();
			Sequence sequence = target as Sequence;

		
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Play")) {
				sequence.PlayAt();

			}

			if (GUILayout.Button ("Stop Forward")) {
				sequence.Stop(true);
			}

			if (GUILayout.Button ("Stop Reset")) {
				//sequence.Stop(false);
				Debug.Log("Not yet tested, not finished");
			}

			if (GUILayout.Button ("Pause")) {
				//sequence.Pause();
				Debug.Log("Not yet tested, not finished");
			}
			if (GUILayout.Button ("UnPause")) {
				Debug.Log("Not yet tested, not finished");
				//sequence.UnPause();
			}
			if (GUILayout.Button ("Restart")) {
				//sequence.Restart();
				Debug.Log("Not yet tested, not finished");
			}
			if (GUILayout.Button ("Open Editor")) {
				SequenceEditorWindow.ShowWindow();
			}

			EditorGUILayout.EndHorizontal ();

			if (selectedNode != null){

				if(selectedNode!=selectedNodePrev)
								  	 nodeEditor=Editor.CreateEditor (selectedNode, typeof(SequenceNodeEditor)) as SequenceNodeEditor;

				selectedNodePrev=selectedNode;

				nodeEditor.OnInspectorGUI();
			}


		}

	}
}