using UnityEngine;
using System.Collections;


namespace ws.winx.unity
{

	[System.Serializable]
		public class QuaternionEx
	{
		public Quaternion q;
		
		public QuaternionEx(Quaternion q){
			this.q = q;
			
		}
		
		public Vector3 a;
		
		public float x;
		
		public float X {
			get {
				return q.x;
			}
			set {
				q.x = value;
			}
		}
		
		public float y;
		public float z;
		public float w;
		
		
		public static implicit operator QuaternionEx(Quaternion q){
			return new QuaternionEx(q);
			
		}
		
		
	}


		
}
