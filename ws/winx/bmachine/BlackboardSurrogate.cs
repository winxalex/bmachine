using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using BehaviourMachine;

namespace ws.winx.bmachine{
public class BlackboardSurrogate : ISerializationSurrogate
{
	public InternalBlackboard blackboard;

	#region ISerializationSurrogate implementation

	public void GetObjectData (object obj, SerializationInfo info, StreamingContext context)
	{
		Debug.Log ("GetObjectData "+obj);
	}

	public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
	{

		//Debug.Log ("SetObjectData "+obj);
		return blackboard;
	}

	#endregion


		
}
}

