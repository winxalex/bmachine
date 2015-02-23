using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using BehaviourMachine;

namespace ws.winx.unity.surrogates{

	public class Vector3Surrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var vector = (Vector3)obj;
			info.AddValue("x", vector.x);
			info.AddValue("y", vector.y);
			info.AddValue("z", vector.z);
		}
		
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{

			return new Vector3((float)info.GetValue("x", typeof(float)), (float)info.GetValue("y", typeof(float)),(float) info.GetValue("z", typeof(float)));
		}
	}
}

