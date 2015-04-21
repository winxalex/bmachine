using UnityEngine;

namespace ws.winx.unity
{
		public sealed class AnimatorState : ScriptableObject
		{
				//
				// Fields
				//
	
	
				//
				// Properties
				//
	
	
				public  bool iKOnFeet {
		
						get;
		
						set;
				}
	
				public  bool mirror {
		
						get;
	
						set;
				}
	
				public Motion motion {
		
						get;
	
						set;
				}
	
				public  int nameHash {
						get;
						set;
				}
	
				public  float speed {
		
						get;
	
						set;
				}
	
				public string tag {
		
						get;
		
						set;
				}
	
				public bool writeDefaultValues {

						get;

						set;
				}

				public int[] blendParamsHashes;


				
	
				//
				// Constructors
				//
				
		}
}
