using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;

namespace ws.winx.editor.windows{
	[CustomEditor(typeof(SequenceNode))]
	public class SequenceNodeEditor : Editor {


		SerializedProperty onStartSerializedProperty;
		SerializedProperty onStopSerializedProperty;
		SerializedProperty onPauseSerializedProperty;
		SerializedProperty onUpdateSerializedProperty;

		bool showHideSequenceEvents;

		void OnEnable(){
//			onStartSerializedProperty = serializedObject.FindProperty ("onStart");
//			onStopSerializedProperty = serializedObject.FindProperty ("onStop");
//			onUpdateSerializedProperty = serializedObject.FindProperty ("onUpdate");
//			onPauseSerializedProperty = serializedObject.FindProperty ("onPause");

				}

		public override void OnInspectorGUI ()
		{
//			showHideSequenceEvents = EditorGUILayout.Foldout (showHideSequenceEvents, "Node Events:");
//			
//			if (showHideSequenceEvents) {
//				EditorGUILayout.PropertyField (onStartSerializedProperty, new GUIContent ("OnStart"));
//				EditorGUILayout.PropertyField (onStopSerializedProperty, new GUIContent ("OnStop"));
//				EditorGUILayout.PropertyField (onUpdateSerializedProperty, new GUIContent ("OnUpdate"));
//				EditorGUILayout.PropertyField (onPauseSerializedProperty, new GUIContent ("OnPause"));
//			}

			this.DrawDefaultInspector ();
		}

	}
}