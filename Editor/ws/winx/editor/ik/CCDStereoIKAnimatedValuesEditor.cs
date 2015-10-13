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
	[CustomEditor(typeof(CCDStereoIKAnimatedValues))]
		public class CCDStereoIKAnimatedValuesEditor : Editor
		{

				
				CCDStereoIKAnimatedValues ikAnimatedValues;

				void OnEnable ()
				{
						ikAnimatedValues = target as CCDStereoIKAnimatedValues;

						
						ikAnimatedValues.Initate ();

				}

				
				public override void OnInspectorGUI ()
				{
        
						EditorGUI.BeginChangeCheck ();
						base.OnInspectorGUI ();

						if (EditorGUI.EndChangeCheck ()) {
					
								//Debug.Log ("inspector");
					
					
								if (!EditorApplication.isPlaying && !AnimationMode.InAnimationMode () && ikAnimatedValues.ik1 != null && ikAnimatedValues.ik2!=null) {
									ikAnimatedValues.UpdateValues ();
						
								}

						}
				}
		}
}
