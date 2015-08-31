using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;

namespace ws.winx.editor.windows
{
		[CustomEditor(typeof(Sequence))]
		public class SequenceEditor : Editor
		{

				SequenceNodeEditor nodeEditor;
				SequenceNode selectedNodePrev;
				SequenceEvent selectedEventPrev;
				SerializedProperty wrapSerializedProperty;
				SerializedProperty playOnStartSerializedProperty;
				SerializedProperty OnStartSerializedProperty;
				SerializedProperty OnEndSerializedProperty;
				SerializedProperty eventsSerializedProperty = null;
				Sequence sequence;
				bool showHideSequenceEvents;

				void OnEnable ()
				{

						OnStartSerializedProperty = serializedObject.FindProperty ("OnStart");
						OnEndSerializedProperty = serializedObject.FindProperty ("OnEnd");
						wrapSerializedProperty = serializedObject.FindProperty ("wrap");
						playOnStartSerializedProperty = serializedObject.FindProperty ("playOnStart");
						sequence = target as Sequence;
						eventsSerializedProperty = serializedObject.FindProperty ("_events");
				}

				public override void OnInspectorGUI ()
				{
						serializedObject.Update ();

						EditorGUI.BeginChangeCheck ();

						showHideSequenceEvents = EditorGUILayout.Foldout (showHideSequenceEvents, "Events:");

						if (showHideSequenceEvents) {
								EditorGUILayout.PropertyField (OnStartSerializedProperty, new GUIContent ("OnStart"));
								EditorGUILayout.PropertyField (OnEndSerializedProperty, new GUIContent ("OnEnd"));
						}

						EditorGUILayout.PropertyField (wrapSerializedProperty, new GUIContent ("Wrap Mode"));
						EditorGUILayout.PropertyField (playOnStartSerializedProperty, new GUIContent ("Play On Start"));

				

						//
						int eventIndex = sequence.events.FindIndex (itm => itm == sequence.selectedEvent);
						
						if (eventIndex < 0) {
							EditorGUILayout.LabelField ("No Event selected");
						} else {
								SerializedProperty eventAtIndex = eventsSerializedProperty.GetArrayElementAtIndex (eventIndex);
							
							EditorGUILayout.PropertyField (eventAtIndex, new GUIContent ("Selected Event"));
						}

													


		

		



						if (EditorGUI.EndChangeCheck ()) {
								serializedObject.ApplyModifiedProperties ();



						}







		
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Play")) {
								sequence.PlayAt ();

						}

						if (GUILayout.Button ("Stop Forward")) {
								sequence.Stop (true);
						}

						if (GUILayout.Button ("Stop Reset")) {
								//sequence.Stop(false);
								Debug.Log ("Not yet tested, not finished");
						}

						if (GUILayout.Button ("Pause")) {
								//sequence.Pause();
								Debug.Log ("Not yet tested, not finished");
						}
						if (GUILayout.Button ("UnPause")) {
								Debug.Log ("Not yet tested, not finished");
								//sequence.UnPause();
						}
						if (GUILayout.Button ("Restart")) {
								//sequence.Restart();
								Debug.Log ("Not yet tested, not finished");
						}
						if (GUILayout.Button ("Open Editor")) {
								SequenceEditorWindow.ShowWindow ();
						}

						EditorGUILayout.EndHorizontal ();

						SequenceNode selectedNode = sequence.selectedNode;
						if (selectedNode != null) {

								if (selectedNode != selectedNodePrev)
										nodeEditor = Editor.CreateEditor (selectedNode, typeof(SequenceNodeEditor)) as SequenceNodeEditor;

								selectedNodePrev = selectedNode;

								nodeEditor.OnInspectorGUI ();
						}


				}

		}
}