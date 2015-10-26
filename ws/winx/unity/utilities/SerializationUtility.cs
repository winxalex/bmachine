using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using ws.winx.unity.surrogates;
using ws.winx.csharp.utilities;
using ws.winx.csharp.extensions;
using System.Linq;
using System.Threading;



namespace ws.winx.unity.utilities{

	public class SerializationUtility
	{

	
		static SerializationUtility(){

			AddSurrogate(typeof(UnityEngine.Vector3),new Vector3Surrogate());
			AddSurrogate(typeof(UnityEngine.Color),new ColorSurrogate());
			AddSurrogate(typeof(UnityEngine.Rect),new RectSurrogate());
			AddSurrogate(typeof(UnityEngine.Quaternion),new QuaternionSurrogate());

			AddSurrogate (typeof(UnityEngine.Keyframe), new KeyFrameSurrogate ());
			AddSurrogate (typeof(UnityEngine.AnimationCurve), new AnimationCurveSurrogate ());
			AddSurrogate (typeof(UnityEngine.Bounds), new BoundsSurrogate ());
			//AddSurrogate (typeof(UnityEngine.GameObject), new GameObjectSurrogate ());



			UnityObjectSurrogate unityObjectSurrogate = new UnityObjectSurrogate ();
			typeof(UnityEngine.Object).Assembly.GetTypes ()
				.Where (t => typeof(UnityEngine.Object).IsAssignableFrom (t)).ToList<Type> ().ForEach (itm =>
			{
				AddSurrogate (itm, unityObjectSurrogate);});
					
					
					
					
		}
		
		
		
		
		
		
		
		private static BinaryFormatter __binaryFormater;
		private static SurrogateSelector __surrogateSelector;
		private static StreamingContext __streamingContext;
		
		
		public static ISerializationSurrogate GetSurrogate(Type type){
			
			if (__surrogateSelector == null)
				return null;
			
			
			ISurrogateSelector surrogateExist=null;
			
			
			
			return __surrogateSelector.GetSurrogate (type, __streamingContext, out surrogateExist);
			
		}
		
		public static void AddSurrogate(Type type,ISerializationSurrogate surrogate){
			
			if(__surrogateSelector==null){
				
				//__surrogateSelector=new SurrogateSelector();
				__surrogateSelector=new UnitySurrogateSelectorEx();
				
				__streamingContext=new StreamingContext();
				
			}
			
			ISurrogateSelector surrogateExist=null;

			
			
			__surrogateSelector.GetSurrogate (type, __streamingContext,out surrogateExist);
			
			if(surrogateExist==null)
				
				__surrogateSelector.AddSurrogate(type,__streamingContext, surrogate);
			
		}
		
		public static byte[] Serialize(object value){
			
			byte[] result;
			
			if (__binaryFormater == null)
				__binaryFormater = new BinaryFormatter ();
			
			// try to serialize the interface to a string and store the result in our other dictionary
			using (var stream = new System.IO.MemoryStream())
			{
				
				__binaryFormater.SurrogateSelector=__surrogateSelector;
				__binaryFormater.Serialize(stream, value);
				stream.Flush();
				result=stream.ToArray();
			}
			
			
			return result;
		}
		
		public static object Deserialize(byte[] data){
			
			object result;
			
			if (__binaryFormater == null)
				__binaryFormater = new BinaryFormatter ();
			
			using (var stream = new System.IO.MemoryStream())
			{
				stream.Write(data,0,data.Length);
				stream.Seek(0, SeekOrigin.Begin);
				
				if(__surrogateSelector==null)
					//__surrogateSelector=new SurrogateSelector();
					__surrogateSelector=new UnitySurrogateSelectorEx();
				
				__binaryFormater.SurrogateSelector=__surrogateSelector;
				
				result=__binaryFormater.Deserialize(stream);
				
			}
			
			
			return result;
		}





	
		




	}






}

