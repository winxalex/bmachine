using UnityEngine;
using System.Collections;
using UnityEditor;
using ws.winx.ik;
using ws.winx.editor.extensions;
using ws.winx.editor.windows;
using ws.winx.unity;
using RootMotion.FinalIK;

namespace ws.winx.editor.ik
{
		[CustomEditor(typeof(CCDIKAnimatedValues))]
		public class CCDIKAnimatedValuesEditor : Editor
		{

				
				CCDIKAnimatedValues ikAnimatedValues;

				void OnEnable ()
				{
						ikAnimatedValues = target as CCDIKAnimatedValues;

						
						ikAnimatedValues.Initate ();


						//copyBoneWeights ();
						


			
			


				}

//				void copyBoneWeights ()
//				{
//
//						IKSolver.Bone[] bones = ikAnimatedValues.ik.solver.bones;
//						if (bones != null) {
//								if (ikAnimatedValues.boneWeights == null || bones.Length != ikAnimatedValues.boneWeights.Length) {
//										ikAnimatedValues.boneWeights = new float[bones.Length];
//				
//										int bonesNumber = bones.Length;
//				
//				
//				
//										for (int i=0; i<bonesNumber; i++)
//					
//												ikAnimatedValues.boneWeights [i] = bones [i].weight;
//				
//								}
//						}
//
//				}

				public override void OnInspectorGUI ()
				{

						//copyBoneWeights ();

						        
						EditorGUI.BeginChangeCheck ();
						base.OnInspectorGUI ();

						if (EditorGUI.EndChangeCheck ()) {
					
								//Debug.Log ("inspector");
					
					
								if (!EditorApplication.isPlaying && !AnimationMode.InAnimationMode () && ikAnimatedValues.ik != null) {
										//Reset
//						Vector3 position = ikAnimatedValues.ik.gameObject.transform.position;
//						Quaternion rotation = ikAnimatedValues.ik.gameObject.transform.rotation;
//						ikAnimatedValues.ik.gameObject.ResetPropertyModification<Transform> ();
//						ikAnimatedValues.ik.gameObject.transform.position = position;
//						ikAnimatedValues.ik.gameObject.transform.rotation = rotation;
										//ikAnimatedValues.ik.solver.FixTransforms ();
						
										ikAnimatedValues.UpdateValues ();
						
						
						
								}

						}
				}
		}
}
