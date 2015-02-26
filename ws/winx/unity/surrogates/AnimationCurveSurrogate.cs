using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using BehaviourMachine;

namespace ws.winx.unity.surrogates{

	public class AnimationCurveSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var vector = (AnimationCurve)obj;
			int len = vector.keys.Length;
			for (int i=0; i<len; i++) {
				info.AddValue("keyt"+i, vector[i].time);
				info.AddValue("keyv"+i, vector[i].value);
			}

			info.AddValue("keysn", len);

			///!!! 
			//info.AddValue("keys", vector.keys);

		}
		
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			Keyframe[] keyframes; 

			int numKeys=info.GetInt32("keysn");

			keyframes=new Keyframe[numKeys];


			for (int i=0; i<numKeys; i++) {
				keyframes[i]=new Keyframe(info.GetSingle("keyt"+i),
				info.GetSingle("keyv"+i));
			}
		
			// don't know how to make connection between AnimaitonCurver and Keyframes surrogate
			// AnimationCurve surrogate keys are constructed before thoose in Keyframe surrogate resulting in 0,0 Keyframes
			//return new AnimationCurve((Keyframe[])info.GetValue ("keys", typeof(Keyframe[])));

			return new AnimationCurve(keyframes);


		}
	}
}

