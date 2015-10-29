using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;
using ws.winx.editor.utilities;
using ws.winx.editor.extensions;

namespace ws.winx.editor.windows
{
	[CustomEditor(typeof(Sequencer))]
	public class SequencerEditor : Editor
	{

		SequenceNodeEditor nodeEditor;
		SequenceNode selectedNodePrev;
		SequenceEvent selectedEventPrev;
		SerializedProperty wrapSerializedProperty;
		SerializedProperty playOnStartSerializedProperty;
		SerializedProperty OnStartSerializedProperty;
		SerializedProperty OnEndSerializedProperty;
		SerializedProperty eventSelectedSerializedProperty = null;
		SerializedProperty sequenceSerialziedProperty;
		SerializedProperty timeCurrentSerializedProperty;
		Sequence sequence;
		Sequencer sequencer;
		bool showHideSequenceEvents;

		void OnEnable ()
		{

			OnStartSerializedProperty = serializedObject.FindProperty ("OnStart");
			OnEndSerializedProperty = serializedObject.FindProperty ("OnEnd");
			wrapSerializedProperty = serializedObject.FindProperty ("wrap");
			playOnStartSerializedProperty = serializedObject.FindProperty ("playOnStart");
			sequencer = target as Sequencer;
			sequenceSerialziedProperty = serializedObject.FindProperty ("sequence");
			eventSelectedSerializedProperty = serializedObject.FindProperty ("eventSelected");
			timeCurrentSerializedProperty = serializedObject.FindProperty ("timeCurrent");


		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			sequence = sequenceSerialziedProperty.objectReferenceValue as Sequence;

			EditorGUI.BeginChangeCheck ();

			if (sequence != null) {	
				wrapSerializedProperty.enumValueIndex = (int)sequence.wrap;
				EditorGUILayout.PropertyField (wrapSerializedProperty, new GUIContent ("Wrap Mode"));
				sequence.wrap = (Sequence.SequenceWrap)wrapSerializedProperty.enumValueIndex;
			}
			EditorGUILayout.PropertyField (playOnStartSerializedProperty, new GUIContent ("Play On Start"));
			EditorGUILayout.PropertyField (sequenceSerialziedProperty, new GUIContent ("Sequence"));





						




			if (sequence != null) {

						timeCurrentSerializedProperty.floatValue=(float)sequence.timeCurrent;
						timeCurrentSerializedProperty.serializedObject.ApplyModifiedProperties();
						
						EditorGUILayout.Slider(timeCurrentSerializedProperty,(float)sequence.timeStart, (float)sequence.timeEnd);

						sequence.timeCurrent=timeCurrentSerializedProperty.floatValue;



		
				EditorGUILayout.BeginHorizontal ();

				

				if (GUILayout.Button (!Application.isPlaying ? (sequence.isPlaying ? "Pause" : "Play Forward") : "Play Forward")) {
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
						
				if (!Application.isPlaying)
				if (GUILayout.Button ("Open Editor")) {
					SequenceEditorWindow.ShowWindow ();
				}

				EditorGUILayout.EndHorizontal ();


				if (EditorGUI.EndChangeCheck ()) {
					serializedObject.ApplyModifiedProperties ();
							
					SequenceEditorWindow.window.Repaint ();
							
				}

				//EditorUtilityEx.GetDrawer(typeof(UnityEngine.Events.UnityEventBase)).


				///// DRAW SELECTED NODE (inside SequenceEditor) ////////
				SequenceNode selectedNode = sequence.nodeSelected;
				if (selectedNode != null) {



					if (selectedNode != selectedNodePrev){
						EditorGUILayout.Space();
						EditorGUILayout.LabelField ("Selected Node");
						EditorGUILayout.Space();
						nodeEditor = Editor.CreateEditor (selectedNode, typeof(SequenceNodeEditor)) as SequenceNodeEditor;
						EditorGUILayout.Space();
					}


					nodeEditor.OnInspectorGUI ();
				} else {

					if(eventSelectedSerializedProperty!=null && sequencer.eventSelected!=null && sequencer.eventSelected.GetPersistentEventCount()>0)
						EditorGUILayout.PropertyField (eventSelectedSerializedProperty, new GUIContent ("Event Selected"));

					EditorGUILayout.LabelField ("No Node selected");

				}

				selectedNodePrev = selectedNode;

			}


		}

	}
}