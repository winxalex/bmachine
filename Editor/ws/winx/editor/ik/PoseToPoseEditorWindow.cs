

using UnityEditor;
using UnityEngine;

namespace ws.winx.editor.ik
{

	public class PoseToPose:EditorWindow{

		Transform transformFrom;
		Transform transformTo;

	 void OnGUI(){

			EditorGUILayout.BeginHorizontal ();


			transformFrom = EditorGUILayout.ObjectField (transformFrom, typeof(Transform), true) as Transform;
			transformTo = EditorGUILayout.ObjectField (transformTo, typeof(Transform), true) as Transform;

			if (GUILayout.Button ("Apply")) {

				Undo.RecordObject(transformTo,"Pose Apply to "+transformTo.name);

				int numChildren=transformFrom.childCount;

				if(numChildren!=transformTo.childCount){

					Debug.LogWarning("Transforms should have same number of children!");
				}else{
					for(int i=0;i<numChildren;i++){
						transformTo.GetChild(i).localRotation=transformFrom.GetChild(i).localRotation;
						transformTo.GetChild(i).localPosition=transformFrom.GetChild(i).localPosition;
					}
				}


			}


			EditorGUILayout.EndHorizontal ();


				}

	}
}