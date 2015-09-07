
using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using ws.winx.unity;


namespace ws.winx.ik
{

	public class FBBIKAnimatedValues:MonoBehaviour{


		public FullBodyBipedIK ik;



		/// <summary>
		/// The Left Hand Effector
		/// </summary>
		
		public Transform LHETarget;

		[Range(0,1)]
		
		public float LHEPositionWeight;

		
		public Vector3 LHEPositionOffset;

		
		public Transform positionOffsetSpace;

		
		public float LHERotationWeight;


		/// <summary>
		/// The Left Hand Effector
		/// </summary>
		
		public Transform RHETarget;
		
		[Range(0,1)]
		
		public float RHEPositionWeight;

		
		public Vector3 RHEPositionOffset;
		
		
		public float RHERotationWeight;







		bool _isInitated=false;

		public bool isInitated {
			get {

				return _isInitated && (typeof(RootMotion.FinalIK.IKEffector).GetField("solver",System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(ik.solver.leftHandEffector) as IKSolver)!=null;



			}

		}

		void Start() {

			
			// You can use just LateUpdate, but note that it doesn't work when you have animatePhysics turned on for the character.
			//ik.solver.OnPostUpdate += RotateShoulders;
		}

		public void Initate ()
		{
			ik.solver.Initiate (ik.transform);
			_isInitated = true;
		}


		public void Reset(){

			Debug.Log ("Reset FBBIKAnimatedValues");
			ik.solver.leftHandEffector.target=LHETarget=null;
			

			ik.solver.leftHandEffector.positionWeight=LHEPositionWeight = 0;
			ik.solver.leftHandEffector.position = Vector3.zero;
			ik.solver.leftHandEffector.rotationWeight=LHERotationWeight = 0;

			
			LHEPositionOffset=Vector3.zero;
		
			


			
			RHETarget=null;
			
			

			
			RHEPositionOffset=Vector3.zero;

			//effectors update
			ik.solver.rightHandEffector.positionWeight = RHEPositionWeight = 0;
			//ik.solver.rightHandEffector.positionOffset ???
			ik.solver.rightHandEffector.target = RHETarget = null;
			ik.solver.rightHandEffector.position = Vector3.zero;
			ik.solver.rightHandEffector.rotationWeight = RHERotationWeight = 0;

			


			UpdateSolver ();

		}
		
		
		public void UpdateSolver ()
		{
			if(!isInitated) 
				this.Initate();


			if (LHETarget != null) {

								//effectors update
								ik.solver.leftHandEffector.positionWeight = LHEPositionWeight;

								ik.solver.leftHandEffector.position = LHEPositionOffset + LHETarget.position;

								
								
				
				//ik.solver.leftFootEffector.positionWeight = 0;

												//OffsetX
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.right * LHEPositionOffset.x;
				//
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.up * LHEPositionOffset.y;
				//
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.forward * LHEPositionOffset.z*1;
				//


						
			}

			if (RHETarget != null) {
								//effectors update
								ik.solver.rightHandEffector.positionWeight = RHEPositionWeight;
								//ik.solver.rightHandEffector.positionOffset ???
								ik.solver.rightHandEffector.position = RHEPositionOffset + RHETarget.position;
						}
			
			
			
			if(!Application.isPlaying)//only update in Edit mode (In Playmode FullBodyBipedIK component take cares of update)
			ik.solver.Update ();
		}


	}

}