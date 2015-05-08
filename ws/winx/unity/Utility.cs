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
			AddSurrogate (typeof(UnityEngine.Bounds), new BoundsSurrogate ());



		

		}




		/// <summary>
		/// Returns vector projection on axis multiplied by weight.
		/// </summary>
		public static Vector3 ExtractVertical(Vector3 v, Vector3 verticalAxis, float weight) {
			if (weight == 0f) return Vector3.zero;
			return Vector3.Project(v, verticalAxis) * weight;
		}
		
		/// <summary>
		/// Returns vector projected to a plane and multiplied by weight.
		/// </summary>
		public static Vector3 ExtractHorizontal(Vector3 v, Vector3 normal, float weight) {
			if (weight == 0f) return Vector3.zero;
			
			Vector3 tangent = v;
			Vector3.OrthoNormalize(ref normal, ref tangent);
			return Vector3.Project(v, tangent) * weight;
		}
		
		/// <summary>
		/// Clamps the direction to clampWeight from normalDirection, clampSmoothing is the number of sine smoothing iterations applied on the result.
		/// </summary>
		public static Vector3 ClampDirection(Vector3 direction, Vector3 normalDirection, float clampWeight, int clampSmoothing, out bool changed) {
			changed = false;
			
			if (clampWeight <= 0) return direction;
			
			if (clampWeight >= 1f) {
				changed = true;
				return normalDirection;
			}
			
			// Getting the angle between direction and normalDirection
			float angle = Vector3.Angle(normalDirection, direction);
			float dot = 1f - (angle / 180f);
			
			if (dot > clampWeight) return direction;
			changed = true;
			
			// Clamping the target
			float targetClampMlp = clampWeight > 0? Mathf.Clamp(1f - ((clampWeight - dot) / (1f - dot)), 0f, 1f): 1f;
			
			// Calculating the clamp multiplier
			float clampMlp = clampWeight > 0? Mathf.Clamp(dot / clampWeight, 0f, 1f): 1f;
			
			// Sine smoothing iterations
			for (int i = 0; i < clampSmoothing; i++) {
				float sinF = clampMlp * Mathf.PI * 0.5f;
				clampMlp = Mathf.Sin(sinF);
			}
			
			// Slerping the direction (don't use Lerp here, it breaks it)
			return Vector3.Slerp(normalDirection, direction, clampMlp * targetClampMlp);
		}
		
		/// <summary>
		/// Get the intersection point of line and plane
		/// </summary>
		public static Vector3 LineToPlane(Vector3 origin, Vector3 direction, Vector3 planeNormal, Vector3 planePoint) {
			float dot = Vector3.Dot(planePoint - origin, planeNormal);
			float normalDot = Vector3.Dot(direction, planeNormal);
			
			if (normalDot == 0.0f) return Vector3.zero;
			
			float dist = dot / normalDot;
			return origin + direction.normalized * dist;
		}


		/// <summary>
		/// Gets the closest direction axis to a vector. Input vector must be normalized!
		/// </summary>
		public static Vector3 GetAxis(Vector3 v) {
			Vector3 closest = Vector3.right;
			bool neg = false;
			
			float x = Vector3.Dot(v, Vector3.right);
			float maxAbsDot = Mathf.Abs(x);
			if (x < 0f) neg = true;
			
			float y = Vector3.Dot(v, Vector3.up);
			float absDot = Mathf.Abs(y);
			if (absDot > maxAbsDot) {
				maxAbsDot = absDot;
				closest = Vector3.up;
				neg = y < 0f;
			}
			
			float z = Vector3.Dot(v, Vector3.forward);
			absDot = Mathf.Abs(z);
			if (absDot > maxAbsDot) {
				closest = Vector3.forward;
				neg = z < 0f;
			}
			
			if (neg) closest = -closest;
			return closest;
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
			publicMembers = target.GetPublicMembersOfType (type,false, true, true);
			for (int j = 0; j < publicMembers.Length; j++)
			{
				memberInfo=publicMembers [j];
				
				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));
				instancesList.Add(@object);
				memberInfos.Add (memberInfo);
				
				
			}
			
			//GET properties in COMPONENTS IF GAME OBJECT
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
					publicMembers = compType.GetPublicMembersOfType (type, false, true, true);
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
				MemberInfo[] publicMembers = target.GetPublicMembersOfType (type, true, true, true);
				if (publicMembers.Length > 0)
				{
					for (int i = 0; i < publicMembers.Length; i++)
					{
						memberInfo=publicMembers [i];
						
					guiContentList.Add(new GUIContent (text4 + memberInfo.Name));
					propertyNew = (K)ScriptableObject.CreateInstance<K> ();
					propertyNew.memberName=memberInfo.Name;
					memberInfos.Add (propertyNew);                   

					}
				}
				
				//GET OBJECT NON STATIC PROPERTIES
				publicMembers = target.GetPublicMembersOfType (type, false, true, true);
				for (int j = 0; j < publicMembers.Length; j++)
				{
					memberInfo=publicMembers [j];

				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));

			
				propertyNew = (K)ScriptableObject.CreateInstance<K> ();
				propertyNew.memberName=memberInfo.Name;
				propertyNew.instanceBinded=@object;
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
						publicMembers =compType.GetPublicMembersOfType (type, true, true, true);
						if (publicMembers.Length > 0)
						{
							for (int l = 0; l < publicMembers.Length; l++)
							{
								memberInfo=publicMembers [l];

							guiContentList.Add (new GUIContent (text4 + memberInfo.Name));

							propertyNew = (K)ScriptableObject.CreateInstance<K> ();
							propertyNew.memberName=memberInfo.Name;
							memberInfos.Add (propertyNew);

							}
						}
						
						//NONSTATIC PROPERTIES
						publicMembers = compType.GetPublicMembersOfType (type, false, true, true);
						for (int m = 0; m < publicMembers.Length; m++)
						{
							memberInfo=publicMembers [m];

							guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name));
						propertyNew = (K)ScriptableObject.CreateInstance<K> ();
						propertyNew.memberName=memberInfo.Name;
						propertyNew.instanceBinded=currentComponent;
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

