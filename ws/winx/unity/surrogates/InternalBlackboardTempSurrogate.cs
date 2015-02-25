using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using BehaviourMachine;

namespace ws.winx.unity.surrogates{

	/// <summary>
	/// !!! Just temporary (I"ll remove all FSMEvents, Vars and Blackboard implementation
	/// </summary>
	public class InternalBlackboardTempSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{

		}
		
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{

			return null;
		}
	}
}

