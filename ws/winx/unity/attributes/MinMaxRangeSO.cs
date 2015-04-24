using UnityEngine;
using System.Collections;


namespace ws.winx.unity.attributes
{
	[System.Serializable]
	public class MinMaxRangeSO:ScriptableObject
	{
		public float rangeStart;
		public float rangeEnd;
		
		public float GetRandomValue()
		{
			return Random.Range( rangeStart, rangeEnd );
		}
	}
}

