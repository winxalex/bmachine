using UnityEngine;
using System.Collections;
using UnityEditor;
using ws.winx.ik;
using ws.winx.editor.extensions;
using ws.winx.editor.windows;

namespace ws.winx.editor.ik
{
		[CustomEditor(typeof(FBBIKAnimatedValues))]
		public class FBBIKAnimatedValuesEditor : Editor
		{

				SerializedProperty lhePosition;
				SerializedProperty lhePositionWeight;
				SerializedProperty lhePostionOffset;
				FBBIKAnimatedValues ikAnimatedValues;
				

				void OnEnable ()
				{
						ikAnimatedValues = target as FBBIKAnimatedValues;
						ikAnimatedValues.Initate ();
				}

				public override void OnInspectorGUI ()
				{

						
								EditorGUI.BeginChangeCheck ();
								base.OnInspectorGUI ();

			//
								
		

								if (EditorGUI.EndChangeCheck ()) {

										Debug.Log ("inspector");


										if (!EditorApplication.isPlaying && !AnimationMode.InAnimationMode ()){
										//Reset
										Vector3 position = ikAnimatedValues.ik.gameObject.transform.position;
										Quaternion rotation = ikAnimatedValues.ik.gameObject.transform.rotation;
										ikAnimatedValues.ik.gameObject.ResetPropertyModification<Transform> ();
										ikAnimatedValues.ik.gameObject.transform.position = position;
										ikAnimatedValues.ik.gameObject.transform.rotation = rotation;

							
										ikAnimatedValues.UpdateSolver ();

										

								}



						}




				}
		}
}
