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



namespace ws.winx.unity{

	public class Utility
	{
	
		static Utility(){

			AddSurrogate(typeof(UnityEngine.Vector3),new Vector3Surrogate());
			AddSurrogate(typeof(UnityEngine.Color),new ColorSurrogate());
			AddSurrogate(typeof(UnityEngine.Rect),new RectSurrogate());
			AddSurrogate(typeof(UnityEngine.Quaternion),new QuaternionSurrogate());
			AddSurrogate (typeof(BehaviourMachine.InternalBlackboard), new InternalBlackboardTempSurrogate ());
			AddSurrogate (typeof(UnityEngine.Keyframe), new KeyFrameSurrogate ());
			AddSurrogate (typeof(UnityEngine.AnimationCurve), new AnimationCurveSurrogate ());



		

		}




		public static void ObjectToDisplayOptionsValues (UnityEngine.Object @object,Type type,out GUIContent[] displayOptions,out MemberInfo[] memeberInfoValues,out object[] instances)

		{
//			displayOptions = null;
//			memeberInfoValues = null;
//			instances = null;

			
			Type target = null;
			List<GUIContent> guiContentList = new List<GUIContent> ();
			List<MemberInfo> memberInfos = new List<MemberInfo> ();
			List<object> instancesList = new List<object> ();
			MemberInfo memberInfo;

			
			
			target = @object.GetType ();
			
			
			
			
			
			List<string> list = new List<string> ();
			
			
			
			
			

			MemberInfo[] publicMembers ;

			
			//GET OBJECT NON STATIC PROPERTIES
			publicMembers = ReflectionUtility.GetPublicMembers (target, type, false, true, true);
			for (int j = 0; j < publicMembers.Length; j++)
			{
				memberInfo=publicMembers [j];
				
				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));
				instancesList.Add(@object);
				memberInfos.Add (memberInfo);
				
				
			}
			
			//GET COMPONENTS IF GAME OBJECT
			GameObject gameObject = @object as GameObject;
			if (gameObject != null)
			{
				Component currentComponent=null;
				Component[] components = gameObject.GetComponents<Component> ();
				for (int k = 0; k < components.Length; k++)
				{
					currentComponent = components [k];
					Type compType = currentComponent.GetType ();
					string uniqueNameInList = StringUtility.GetUniqueNameInList (list, compType.Name);
					list.Add (uniqueNameInList);
					
					

					
					//NONSTATIC PROPERTIES
					publicMembers = ReflectionUtility.GetPublicMembers (compType, type, false, true, true);
					for (int m = 0; m < publicMembers.Length; m++)
					{
						memberInfo=publicMembers [m];
						
						guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name));
						instancesList.Add(currentComponent);
						memberInfos.Add(memberInfo);
					}
				}
			}
			
			
			displayOptions=guiContentList.ToArray();
			memeberInfoValues=memberInfos.ToArray();
			instances=instancesList.ToArray();
			
			
			
		}//end function


	

		public static void ObjectToDisplayOptionsValues<T,K> (UnityEngine.Object @object,out GUIContent[] displayOptions,out K[] values)
			where K:UnityVariable where T:Type
		{
			GUIContent[] tempDisplayOptions;
			K[] tempValues;

			ObjectToDisplayOptionsValues<K> (@object, typeof(T),out tempDisplayOptions,out tempValues);

			displayOptions=tempDisplayOptions;
			values=tempValues;

		}


		public static void ObjectToDisplayOptionsValues<K> (UnityEngine.Object @object,Type type,out GUIContent[] displayOptions,out K[] values)
				where K:UnityVariable
		{
			displayOptions = null;
			values = null;

		
			if (@object == null) return;

			Type target = null;
			List<GUIContent> guiContentList = new List<GUIContent> ();
			List<K> memberInfos = new List<K> ();
			MemberInfo memberInfo;
			K propertyNew;
				
					
			 target = @object.GetType ();
			
			
				


				List<string> list = new List<string> ();

				
				
			    
				
				//GET OBJECT STATIC PROPERTIES
				string text4 = target.Name + "/Static Properties/";
				MemberInfo[] publicMembers = ReflectionUtility.GetPublicMembers (target, type, true, true, true);
				if (publicMembers.Length > 0)
				{
					for (int i = 0; i < publicMembers.Length; i++)
					{
						memberInfo=publicMembers [i];
						
					guiContentList.Add(new GUIContent (text4 + memberInfo.Name));
					propertyNew = (K)ScriptableObject.CreateInstance<K> ();
					propertyNew.MemberInfo=memberInfo;
					memberInfos.Add (propertyNew);                   

					}
				}
				
				//GET OBJECT NON STATIC PROPERTIES
				publicMembers = ReflectionUtility.GetPublicMembers (target, type, false, true, true);
				for (int j = 0; j < publicMembers.Length; j++)
				{
					memberInfo=publicMembers [j];

				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));

			
				propertyNew = (K)ScriptableObject.CreateInstance<K> ();
				propertyNew.MemberInfo=memberInfo;
				propertyNew.reflectedInstance=@object;
				memberInfos.Add (propertyNew);


				}
				
				//GET COMPONENTS IF GAME OBJECT
				GameObject gameObject = @object as GameObject;
				if (gameObject != null)
				{
					Component currentComponent=null;
					Component[] components = gameObject.GetComponents<Component> ();
					for (int k = 0; k < components.Length; k++)
					{
						currentComponent = components [k];
						Type compType = currentComponent.GetType ();
						string uniqueNameInList = StringUtility.GetUniqueNameInList (list, compType.Name);
						list.Add (uniqueNameInList);
						
						
						//STATIC PROPERTIES
						text4 = uniqueNameInList + "/Static Properties/";
						publicMembers =ReflectionUtility.GetPublicMembers (compType, type, true, true, true);
						if (publicMembers.Length > 0)
						{
							for (int l = 0; l < publicMembers.Length; l++)
							{
								memberInfo=publicMembers [l];

							guiContentList.Add (new GUIContent (text4 + memberInfo.Name));

							propertyNew = (K)ScriptableObject.CreateInstance<K> ();
							propertyNew.MemberInfo=memberInfo;
							memberInfos.Add (propertyNew);

							}
						}
						
						//NONSTATIC PROPERTIES
						publicMembers = ReflectionUtility.GetPublicMembers (compType, type, false, true, true);
						for (int m = 0; m < publicMembers.Length; m++)
						{
							memberInfo=publicMembers [m];

							guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name));
						propertyNew = (K)ScriptableObject.CreateInstance<K> ();
						propertyNew.MemberInfo=memberInfo;
						propertyNew.reflectedInstance=currentComponent;
							memberInfos.Add(propertyNew);
						}
					}
				}


			displayOptions=guiContentList.ToArray();
			values=memberInfos.ToArray();
				
				
				

		}//end function

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
				
				__surrogateSelector=new SurrogateSelector();
				
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
					__surrogateSelector=new SurrogateSelector();
				
				__binaryFormater.SurrogateSelector=__surrogateSelector;
				
				result=__binaryFormater.Deserialize(stream);
				
			}
			
			
			return result;
		}





	
		




	}
}

