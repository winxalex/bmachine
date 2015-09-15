
using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using ws.winx.unity;


namespace ws.winx.ik
{

	public class FBBIKAnimatedValues:MonoBehaviour,IAnimatedValues{


		public FullBodyBipedIK ik;

		public bool useLHETarget;

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
		/// 
		public bool useRHETarget;
		
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
			if (ik != null) {
								ik.solver.Initiate (ik.transform);
								_isInitated = true;
						}
		}


		public void ResetValues(){

			Debug.Log ("Reset FBBIKAnimatedValues");

			

			ik.solver.leftHandEffector.positionWeight=LHEPositionWeight = 0;
			ik.solver.leftHandEffector.rotationWeight=LHERotationWeight = 0;
			useLHETarget = false;
			//ik.solver.leftHandEffector.target=LHETarget=null;
			ik.solver.leftHandEffector.positionOffset=LHEPositionOffset=Vector3.zero;
			
			
		
			



			ik.solver.rightHandEffector.rotationWeight=RHERotationWeight = 0;
			ik.solver.rightHandEffector.positionWeight = RHEPositionWeight = 0;
			useRHETarget = false;
			//ik.solver.rightHandEffector.target = RHETarget = null;
			ik.solver.rightHandEffector.positionOffset = RHEPositionOffset=Vector3.zero;
			ik.solver.rightHandEffector.rotationWeight = RHERotationWeight = 0;

			


			UpdateValues ();

		}
		
		
		public void UpdateValues ()
		{
			if(!isInitated) 
				this.Initate();




								//effectors update
								ik.solver.leftHandEffector.positionWeight = LHEPositionWeight;

								ik.solver.leftHandEffector.positionOffset = LHEPositionOffset;
								ik.solver.leftHandEffector.rotationWeight = RHERotationWeight;

								if(useLHETarget)
									ik.solver.leftHandEffector.target = LHETarget;
								else
									ik.solver.leftHandEffector.target = null;
			
												//OffsetX
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.right * LHEPositionOffset.x;
				//
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.up * LHEPositionOffset.y;
				//
				//			ik.solver.leftHandEffector.position += (positionOffsetSpace != null? positionOffsetSpace.rotation: ik.solver.GetRoot().rotation) * Vector3.forward * LHEPositionOffset.z*1;
				//


						



								//effectors update
								ik.solver.rightHandEffector.positionWeight = RHEPositionWeight;
								ik.solver.rightHandEffector.positionOffset = RHEPositionOffset;
								ik.solver.rightHandEffector.target = RHETarget;
								ik.solver.rightHandEffector.rotationWeight = RHERotationWeight;
						
			
			
			
			if(!Application.isPlaying)//only update in Edit mode (In Playmode FullBodyBipedIK component take cares of update)
			ik.solver.Update ();
		}


	}

}