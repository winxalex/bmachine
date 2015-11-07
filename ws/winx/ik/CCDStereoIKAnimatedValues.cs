
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
		public class CCDStereoIKAnimatedValues:MonoBehaviour,IAnimatedValues
		{


				public CCDIK ik1;

				[Range(0,1)]
				public float weight1;

				public CCDIK ik2;

				[Range(0,1)]
				public float weight2;

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
						if (ik1 != null) {
								//Debug.Log("CCDAnimatedValues>> Init Solver");
								ik1.solver.Initiate (ik1.transform);
								_isInitated = true;
						}

						if (ik2 != null) {
							//Debug.Log("CCDAnimatedValues>> Init Solver");
							ik2.solver.Initiate (ik1.transform);
							_isInitated = true;
						}
				}

				public void ResetValues ()
				{

						if (ik1 == null)
								return;
									
						if (ik2 == null)
							return;
			
						Debug.Log ("Reset CCDIKAnimatedValues");

						weight2 = 0f;
						weight1 = 0f;
						UpdateValues ();

				}
		
				public void UpdateValues ()
				{
				
						if (ik1 == null)
								return;

						if (ik2 == null)
							return;

						if (!isInitated) 
							this.Initate ();

						IKSolver.Bone[] bones = ik1.solver.bones;
					
						if (bones == null)
								return;

						bones = ik2.solver.bones;
						
						if (bones == null)
							return;

//						int bonesNumber = bones.Length;
//
//
//
//						for (int i=0; i<bonesNumber; i++)
//								if(boneWeights.Length>i)
//								bones [i].weight = boneWeights [i];

						ik1.solver.FixTransforms ();
						ik1.solver.SetIKPositionWeight (weight1);

							ik2.solver.FixTransforms ();
							ik2.solver.SetIKPositionWeight (weight2);
			
						if (!Application.isPlaying) {//only update in Edit mode (In Playmode FullBodyBipedIK component take cares of update)
								ik1.solver.Update ();
								ik2.solver.Update ();
						}
				}


		}

}