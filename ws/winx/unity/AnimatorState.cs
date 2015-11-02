using UnityEngine;

namespace ws.winx.unity
{
		public class AnimatorState : ScriptableObject
		{
				//
				// Fields
				//
	
	
				//
				// Properties
				//
				public string name;
				
				public  bool iKOnFeet ;
	
				public  bool mirror ;
	
				public Motion motion ;
	
				public  int nameHash;
	
				public  float speed;
	
				public string tag ;
	
				public bool writeDefaultValues ;

				public int[] blendParamsHashes;

				public int layer;
				
	
				//
				// Constructors
				//
				
		}
}
