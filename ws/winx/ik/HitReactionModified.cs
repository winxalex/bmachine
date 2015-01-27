using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.FinalIK.Demos;
using ws.winx.unity;

namespace ws.winx.ik {

	/// <summary>
	/// Class for creating procedural FBBIK hit reactions.
	/// </summary>
	public class HitReactionModified: OffsetModifier {


		public HitPoint[] hitPoints; // The array of hit points
	
		/// <summary>
		/// Hit point definition
		/// </summary>
		[System.Serializable]
		public class HitPoint {



			public string name; // The hit point name
			public Transform transform; // Used for debug only
			
			//spring is multipling Time.deltaTime(Time.deltaTime * spring) in 
			//lerping of velocity so bigger value speed up lerp and for big values can even 
			//reach the target velocity in one Update (no springing) 
			//smaller value means more springing (value-limit)
			public float spring = 10f; // The spring force of this hit point
			
			//oscilation speed
			public float speed = 10f; // The speed of this hit point
			
			// The arrays containing all the EffectorLinks and BoneLinks
			public EffectorLink[] effectorLinks = new EffectorLink[0];
			public BoneLink[] boneLinks = new BoneLink[0];
			
			private Vector3 offset, velocity;



			#region EffectorLink

			/// <summary>
			/// Linking a FBBIK effector to this hit point
			/// </summary>
			[System.Serializable]
			public class EffectorLink {

				public FullBodyBipedEffector effector; // The effector type (this is just an enum)
				public float weight; // Weight of following the hit
				public float speed = 10f; // Hit speed
				public float spring = 10f; // Spring force
				public float multiplier = 1f; // Hit force multiplier
				public float gravity; // Gravity of the hit force

				private Vector3 offset;

				// Update and Apply the position offset to the effector
				public void Update(IKSolverFullBodyBiped solver, Vector3 offsetTarget, float weight) {
					// Lerping offset to the offset target
					offset = Vector3.Lerp(offset, offsetTarget * multiplier, Time.deltaTime * speed);

					// Apply gravity
					Vector3 g = (solver.GetRoot().up * gravity) * offset.magnitude;




					// Apply the offset to the effector
					solver.GetEffector(effector).positionOffset += (offset * weight * this.weight) + (g * weight);
				}
			}

			#endregion

			#region BoneLink

			/// <summary>
			/// Linking an individual bone to this hit point
			/// </summary>
			[System.Serializable]
			public class BoneLink {

				public Transform transform; // The bone
				public Transform hitPointInsideBone;//
				public Vector3 swingAxis = -Vector3.right; // the local swing axis of the bone (the axis towards the next bone)
				public Vector3 swingAxis2 = Vector3.left;

				public float weight = 1f; // Weight of rotating the bone with the hit force

			
				public float speed = 10f; // Speed of rotating the bone
				public float spring = 10f; // Spring force
				public float multiplier = 2f; // Hit force multiplier

				private Vector3 offset;

				// Update and Apply the position offset to the effector
				public void Update(Vector3 offsetTarget, float weight) {


					// Lerping offset to the offset target
					offset = Vector3.Lerp(offset, offsetTarget * multiplier, Time.deltaTime * speed);

					DebugEx.ForDebug (transform.position, offset, Color.red);


					DebugEx.ForDebug (transform.position, swingAxis, Color.yellow);
					
					// Calculating the bone rotation offset
					Vector3 axis = transform.rotation * swingAxis;

			//		DebugEx.ForDebug (transform.position, axis, Color.green);

					DebugEx.ForDebug (transform.position, transform.forward, Color.blue);//pointing left
					DebugEx.ForDebug (transform.position, transform.up, Color.green);// pointing front
					DebugEx.ForDebug (transform.position, transform.right, Color.red);// pointing down

					DebugEx.ForDebug (transform.position, axis + offset,Color.black);

					Quaternion rotationOffset = Quaternion.FromToRotation(axis, axis + (offset * weight * this.weight));


					axis = transform.rotation * swingAxis2;

					// Rotating the bone
					//transform.rotation = rotationOffset * transform.rotation;
					//transform.rigidbody.AddForceAtPosition ((transform.position+offset) * 100, transform.position);
					//transform.rigidbody.AddTorque (transform.up * 100);

					
					//		DebugEx.ForDebug (transform.position, axis, Color.green);
					

					
//					DebugEx.ForDebug (transform.position, axis + offset,Color.cyan);
//					
//					rotationOffset = Quaternion.FromToRotation(axis, axis + (offset * weight * this.weight));
//					
//					// Rotating the bone
//					transform.rotation = rotationOffset * transform.rotation;




				}
			}

			#endregion



			/// <summary>
			/// Adds force to the hit point.
			/// </summary>
			public void AddForce(Vector3 force) {
				velocity += force;

//				Debug.Log ("Add force:" + velocity);
			}

			// Update and apply this hit point
			public void Update(IKSolverFullBodyBiped solver, float weight) {

//				Debug.Log ("offest:" + offset + "velocity:" + velocity);

				// Update velocity
				velocity = Vector3.Lerp(velocity, -offset, Time.deltaTime * spring);

				// Update offset
				// s=v*dt;
				offset += velocity * Time.deltaTime * speed;



				// Update Effector Links
				foreach (EffectorLink e in effectorLinks) e.Update(solver, offset, weight);

				// Update Bone Links
				foreach (BoneLink b in boneLinks) b.Update(offset, weight);
			}
		}



		// Called by IKSolverFullBody before updating
		protected override void OnModifyOffset() {
			// Clamp the master weight
			weight = Mathf.Clamp(weight, 0f, weight);

			// Update all the hit points
			foreach (HitPoint hitPoint in hitPoints) hitPoint.Update(ik.solver, weight);
		}

		// For demo only, feel free to delete
		#region Debug

		public Transform debugWeapon;
		public Vector3 debugForce;
		private Transform hitT;

		void Update() {
			if (hitT != null) Debug.DrawLine(debugWeapon.position, hitT.position, Color.red);
		}

		public void OnGUI() {
			foreach (HitPoint hitPoint in hitPoints) {
				if (GUILayout.Button(hitPoint.name)) {
					hitT = hitPoint.transform;
					hitPoint.AddForce((hitT.position - debugWeapon.position).normalized);
				}
			}
		}

		#endregion Debug
	}
}
