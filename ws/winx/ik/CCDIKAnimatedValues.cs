
using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using ws.winx.unity;

namespace ws.winx.ik
{
		/// <summary>
		/// CCDIK animated values.
		/// CCDIK should have set Fix Transforms
		/// </summary>
		public class CCDIKAnimatedValues:MonoBehaviour,IAnimatedValues
		{


				public CCDIK ik;

				[Range(0,1)]
				public float weight;

				//public float[] boneWeights;
			
				bool _isInitated = false;

				public bool isInitated {
						get {

								//return _isInitated && (typeof(RootMotion.FinalIK.IKEffector).GetField ("solver", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue (ik.solver.leftHandEffector) as IKSolver) != null;
								return _isInitated;


						}

				}

				void Start ()
				{

				}

				public void Initate ()
				{
						if (ik != null) {
								Debug.Log("CCDAnimatedValues>> Init Solver");
								ik.solver.Initiate (ik.transform);
								_isInitated = true;
						}
				}

				public void ResetValues ()
				{

						if (ik == null)
								return;
						
			
						Debug.Log ("Reset CCDIKAnimatedValues");

						weight = 0f;
						UpdateValues ();

				}
		
				public void UpdateValues ()
				{
				
						if (ik == null)
								return;

						if (!isInitated) 
							this.Initate ();

					IKSolver.Bone[] bones = ik.solver.bones;
					
					if (bones == null)
								return;

//						int bonesNumber = bones.Length;
//
//
//
//						for (int i=0; i<bonesNumber; i++)
//								if(boneWeights.Length>i)
//								bones [i].weight = boneWeights [i];

			ik.solver.FixTransforms ();
						ik.solver.SetIKPositionWeight (weight);
			
						if (!Application.isPlaying)//only update in Edit mode (In Playmode FullBodyBipedIK component take cares of update)
								ik.solver.Update ();
				}


		}

}