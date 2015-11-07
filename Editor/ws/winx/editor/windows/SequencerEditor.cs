using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.sequence;
using ws.winx.editor.utilities;
using ws.winx.editor.extensions;
using ws.winx.unity;

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
		
		
		SerializedProperty sequenceSerialziedProperty;
		SerializedProperty timeCurrentSerializedProperty;
		Sequence sequence;
		//Sequencer sequencer;
		bool showHideSequenceEvents;
		
		GUIContent cloneSequenceGUIContent;
		GUIContent playOnStartGUIContent;
		
		GUIContent wrapCurrentGUIContent;
		GUIContent sequenceGUIContent;
		
		void OnEnable ()
		{
			
			
			
			wrapSerializedProperty = serializedObject.FindProperty ("wrap");
			playOnStartSerializedProperty = serializedObject.FindProperty ("playOnStart");
			//sequencer = target as Sequencer;
			sequenceSerialziedProperty = serializedObject.FindProperty ("sequence");
			
			timeCurrentSerializedProperty = serializedObject.FindProperty ("timeCurrent");
			
			playOnStartGUIContent = new GUIContent ("PlayOnStart");
			wrapCurrentGUIContent = new GUIContent ("Wrap mode");
			cloneSequenceGUIContent=new GUIContent("Clone");
			sequenceGUIContent = new GUIContent ("Sequence");
			
		}
		
		public override void OnInspectorGUI ()
		{
			//serializedObject.Update ();
			
			sequence = sequenceSerialziedProperty.objectReferenceValue as Sequence;
			
			EditorGUI.BeginChangeCheck ();
			
			if (sequence != null) {	
				wrapSerializedProperty.enumValueIndex = (int)sequence.wrap;
				EditorGUILayout.PropertyField (wrapSerializedProperty, wrapCurrentGUIContent);
				sequence.wrap = (Sequence.SequenceWrap)wrapSerializedProperty.enumValueIndex;
			}
			EditorGUILayout.PropertyField (playOnStartSerializedProperty,playOnStartGUIContent);
			
			
			
			
			EditorGUILayout.BeginHorizontal ();
			
			EditorGUILayout.PropertyField (sequenceSerialziedProperty,sequenceGUIContent );
			
			if (GUILayout.Button (cloneSequenceGUIContent)) {
				if(sequence!=null) {
					Sequence sequenceNew=ScriptableObject.CreateInstance<Sequence>();
					
					
					
					
					EditorUtilityEx.CreateAssetFromInstance(sequenceNew);
					
					foreach(SequenceChannel channel in sequence.channels){
						SequenceChannel channelClone=UnityEngine.Object.Instantiate<SequenceChannel>(channel);
						channelClone.nodes.Clear();
						sequenceNew.channels.Add(channelClone);
						channelClone.sequence=sequenceNew;
						AssetDatabase.AddObjectToAsset(channelClone,sequenceNew);
						
						foreach(SequenceNode node in channel.nodes)
						{
							SequenceNode nodeClone=UnityEngine.Object.Instantiate<SequenceNode>(node);
							nodeClone.channel=channelClone;
							channelClone.nodes.Add(nodeClone);
							AssetDatabase.AddObjectToAsset(nodeClone,channelClone);
							
							EditorClipBinding clipBindingClone=UnityEngine.Object.Instantiate<EditorClipBinding>(node.clipBinding);
							nodeClone.clipBinding=clipBindingClone;
							AssetDatabase.AddObjectToAsset(clipBindingClone,nodeClone);
							
						}
						
						
					}
					
					
					
				}
				
			}
			EditorGUILayout.EndHorizontal ();
			
			
			
			
			
			
			
			
			
			if (sequence != null) {
				
				timeCurrentSerializedProperty.floatValue=(float)sequence.timeCurrent;
				timeCurrentSerializedProperty.serializedObject.ApplyModifiedProperties();
				
				EditorGUILayout.Slider(timeCurrentSerializedProperty,(float)sequence.timeStart, (float)sequence.timeEnd);
				
				sequence.timeCurrent=timeCurrentSerializedProperty.floatValue;
				
				
				
				
				EditorGUILayout.BeginHorizontal ();
				
				if (GUILayout.Button ( "Goto")) {
					sequence.GoTo(timeCurrentSerializedProperty.floatValue);
					
				}
				
				
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
					
					
					
					EditorGUILayout.LabelField ("No Node selected");
					
				}
				
				selectedNodePrev = selectedNode;
				
			}
			
			
		}
		
	}
}