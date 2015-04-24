using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ws.winx.unity.attributes
{
	public class MinMaxRangeSAttribute : PropertyAttribute
	{
		public float minLimit, maxLimit;
		
		public MinMaxRangeSAttribute( float minLimit, float maxLimit )
		{
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
		}
	}
	
	



public class MinMaxRangeAttribute : PropertyAttribute
{
	public float minLimit, maxLimit;
	
	public MinMaxRangeAttribute( float minLimit, float maxLimit )
	{
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
	}
}


	[System.Serializable]
	public class MinMaxRange
	{
		public float rangeStart, rangeEnd;
		
		public float GetRandomValue()
		{
			return Random.Range( rangeStart, rangeEnd );
		}
	}
}

