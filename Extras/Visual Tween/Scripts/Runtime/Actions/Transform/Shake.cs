using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityTransform{
	[System.Serializable]
	[Category("Transform")]
	public class Shake : TweenAction {
		private Transform cachedTransform;
		private Vector3 originalPosition;
		private Quaternion originalRotation;
		public float intensity=0.3f;
		private float shakeIntensity;
		private bool shake;

		public override void OnEnter (GameObject target)
		{
			if (!shake) {
				cachedTransform = target.transform;
				originalPosition = cachedTransform.position;
				originalRotation = cachedTransform.rotation;
				shakeIntensity = intensity;
				shake = true;
			}
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedTransform)
			{
				if(shakeIntensity > 0.0f  && shake)
				{
					cachedTransform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
					cachedTransform.rotation = new Quaternion(originalRotation.x + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
				                                         	originalRotation.y + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
				                                          	originalRotation.z + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
				                                          	originalRotation.w + Random.Range(-shakeIntensity, shakeIntensity)*.2f);
					
					shakeIntensity=GetValue(intensity,0.0f,percentage);
				}
			}
		}

		public override void OnExit (GameObject target)
		{
			if (shake) {
				shakeIntensity = intensity;
				cachedTransform.position = originalPosition;
				cachedTransform.rotation = originalRotation;
				shake = false;
			}
		}

		private Vector3 recPosition;
		private Quaternion recRotation;
		private float recIntensity;
		public override void RecordAction (GameObject target)
		{
			recPosition = target.transform.position;
			recRotation = target.transform.rotation;
		}
		
		public override void UndoAction (GameObject target)
		{
			target.transform.position = recPosition;
			target.transform.rotation = recRotation;
		}


	}
}