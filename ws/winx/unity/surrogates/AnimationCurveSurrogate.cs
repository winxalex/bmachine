using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;


namespace ws.winx.unity.surrogates{

	public class AnimationCurveSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var curve = (AnimationCurve)obj;
			int len = curve.keys.Length;
			for (int i=0; i<len; i++) {
				info.AddValue("keyt"+i, curve[i].time);
				info.AddValue("keyv"+i, curve[i].value);
				info.AddValue("keyin"+i, curve[i].inTangent);
				info.AddValue("keyout"+i, curve[i].outTangent);
				info.AddValue("keymod"+i, curve[i].tangentMode);



			}

			info.AddValue("keysn", len);


			//info.AddValue ("postWrapMode", vector.postWrapMode);
			//info.AddValue ("preWrapMode", vector.preWrapMode);


			///!!! 
			//info.AddValue("keys", vector.keys);

		}
		
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			Keyframe[] keyframes; 

			AnimationCurve curve=new AnimationCurve();

		//	curve.preWrapMode=(WrapMode)info.GetValue("preWrapMode",typeof(WrapMode));
		//	curve.postWrapMode=(WrapMode)info.GetValue("postWrapMode",typeof(WrapMode));

			int numKeys=info.GetInt32("keysn");

			keyframes=new Keyframe[numKeys];

			Keyframe keyframeCurrent;
			for (int i=0; i<numKeys; i++) {
				keyframeCurrent=keyframes[i]=new Keyframe(info.GetSingle("keyt"+i),
				info.GetSingle("keyv"+i));
				keyframeCurrent.tangentMode=info.GetInt32("keymod"+i);
				keyframeCurrent.inTangent=info.GetSingle("keyin"+i);
				keyframeCurrent.outTangent=info.GetSingle("keyout"+i);
			}


			curve.keys = keyframes;

		
			// don't know how to make connection between AnimaitonCurver and Keyframes surrogate
			// AnimationCurve surrogate keys are constructed before thoose in Keyframe surrogate resulting in 0,0 Keyframes
			//return new AnimationCurve((Keyframe[])info.GetValue ("keys", typeof(Keyframe[])));

			return curve;


		}
	}
}

