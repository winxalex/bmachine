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
						int eventIndex = sequence.events.FindIndex (itm => itm == sequence.eventSelected);
						
						if (eventIndex < 0) {
								EditorGUILayout.LabelField ("No Event selected");
						} else {
								SerializedProperty eventAtIndex = eventsSerializedProperty.GetArrayElementAtIndex (eventIndex);
							
								EditorGUILayout.PropertyField (eventAtIndex, new GUIContent ("Selected Event"));
						}

													


		

		



						



				if(!sequence.isPlaying && !sequence.isRecording)
			sequence.timeCurrent = EditorGUILayout.Slider ("Time Current", (float)sequence.timeCurrent,(float)sequence.timeStart,(float)sequence.timeEnd);




		
						EditorGUILayout.BeginHorizontal ();

				

						if (GUILayout.Button (!Application.isPlaying ? (sequence.isPlaying ? "Pause" :"Play Forward") : "Play Forward")) {
								if (Application.isPlaying)
										sequence.Play (sequence.timeCurrent);
								else
										SequenceEditorWindow.Play ();

						}

						if (GUILayout.Button ("Play Backward")) {
								if (Application.isPlaying)
										sequence.Play (sequence.timeCurrent, false);
								else
									SequenceEditorWindow.Play (false);	
							
						}

						if (GUILayout.Button ("Stop")) {
								if (Application.isPlaying)
										sequence.Stop ();
								else
										SequenceEditorWindow.Stop ();


						}

						if (Application.isPlaying)
							if (GUILayout.Button ("Pause")) {
									Debug.LogWarning ("Not yet tested, not finished");
									
											sequence.Pause ();
								
									
							}

						if (Application.isPlaying)
							if (GUILayout.Button ("UnPause")) {
									Debug.LogWarning ("Not yet tested, not finished");
											sequence.UnPause ();
									
							}
						
						if(!Application.isPlaying)
						if (GUILayout.Button ("Open Editor")) {
								SequenceEditorWindow.ShowWindow ();
						}

						EditorGUILayout.EndHorizontal ();


						if (EditorGUI.EndChangeCheck ()) {
							serializedObject.ApplyModifiedProperties ();
							
							SequenceEditorWindow.window.Repaint();
							
						}



						///// DRAW SELECTED NODE (inside SequenceEditor) ////////
						SequenceNode selectedNode = sequence.nodeSelected;
						if (selectedNode != null) {

								EditorGUILayout.LabelField ("Selected Node");

								if (selectedNode != selectedNodePrev)
										nodeEditor = Editor.CreateEditor (selectedNode, typeof(SequenceNodeEditor)) as SequenceNodeEditor;

								selectedNodePrev = selectedNode;

								nodeEditor.OnInspectorGUI ();
						} else {
								EditorGUILayout.LabelField ("No Node selected");

						}


				}

		}
}