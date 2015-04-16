using UnityEngine;
using System.Collections;
using UnityEditor;
using ws.winx.unity;
using ws.winx.editor.extensions;
using ws.winx.editor;
using ws.winx.unity.attributes;

[CustomEditor(typeof(TestFunctionContainer))]
public class TestFunctionContainerEditor : Editor {

	public SerializedProperty prop;


	UnityVariable vari;


	//Enable is called once in Editor and Play mode
	void OnEnable(){

		//vari=(UnityVariable)ScriptableObject.CreateInstance<UnityVariable>();
		//vari.Value = new AnimationCurve ();
//		Debug.Log ("Enable Editor"+this.GetInstanceID ());

		EditorApplicationEventDispatcher.PlayModeChanged += onModeChange;

		}

	void OnDisable(){
		EditorApplicationEventDispatcher.PlayModeChanged -= onModeChange;

		}

		void onModeChange(EditorApplicationEventDispatcher.Mode modeOld,EditorApplicationEventDispatcher.Mode modeNew){

//				Debug.Log (modeOld + " " + modeNew);
//
//				if (modeNew == EditorApplicationEventDispatcher.Mode.Playing) {
//						Debug.Log ("Try to save");
//						EditorUtilityEx.Clipboard.add (0, this.serializedObject.FindProperty ("curve"));
//
//						EditorUtility.SetDirty (EditorUtilityEx.Clipboard);
//
//						AssetDatabase.SaveAssets ();
//				} else if (modeOld == EditorApplicationEventDispatcher.Mode.Playing && modeNew == EditorApplicationEventDispatcher.Mode.Editing) {
//
//						Debug.Log ("Check saved");
//
//				}
		}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();
		base.OnInspectorGUI ();

//		Debug.Log ("Target ID:"+ target.GetInstanceID ());
		Debug.Log(((UnityVariable)this.serializedObject.FindProperty("variable2").objectReferenceValue).Value);

		if (EditorGUI.EndChangeCheck ()) {


//					Debug.Log("changed curver");
					if(Application.isPlaying){
//						//prop=this.serializedObject.FindProperty("curve").Copy();
//				vari=(UnityVariable)this.serializedObject.FindProperty("variable").objectReferenceValue;
//
//				vari.Value=this.serializedObject.FindProperty("curve").animationCurveValue;
//				EditorUtility.SetDirty(vari);

							//UnityEditorInternal.ComponentUtility.CopyComponent(serializedObject.targetObject as Component);

//				Debug.Log ("Try to save");
//				EditorUtilityEx.Clipboard.add (0, this.serializedObject.FindProperty ("curve"));
//				
//				EditorUtility.SetDirty (EditorUtilityEx.Clipboard);
//
//				AssetDatabase.SaveAssets ();
					}

			}

		if (!Application.isPlaying) {


			//UnityEditorInternal.ComponentUtility.PasteComponentValues(serializedObject.targetObject as Component);

			//var obj=EditorUtility.InstanceIDToObject(-27930);

				//AssetDatabase.AddObjectToAsset((UnityVariable)ScriptableObject.CreateInstance<UnityVariable> (),
				                               //EditorUtilityEx.Clipboard);
				//EditorUtilityEx.Clipboard.variables.Add();
				//EditorUtility.SetDirty (EditorUtilityEx.Clipboard);
				
				//AssetDatabase.SaveAssets ();
		
			//UnityVariable var=EditorUtilityEx.Clipboard.variables[0];
		}


			//this.serializedObject.FindProperty("curve").animationCurveValue=((TestFunctionContainer)this.serializedObject.targetObject).curve;

			//this.serializedObject.CopyFromSerializedProperty(this.serializedObject.FindProperty("curve"));
			//this.serializedObject.ApplyModifiedProperties();
			//EditorUtility.SetDirty(this.serializedObject.targetObject);

	}
}
