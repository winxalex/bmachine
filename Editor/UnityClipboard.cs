using UnityEngine;
using System.Collections;
using UnityEditor;

using ws.winx.unity;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Unity clipboard. !!! NOT FINISHED
/// </summary>
using System.Runtime.Serialization;
using System.Linq;
using ws.winx.csharp.extensions;
using System;

public class UnityClipboard : ScriptableObject
{


		
		

		private static Dictionary<int,ObjectInfo> __objectInstanceMembers;
		private static Vector3 __positionSaved=Vector3.zero;
		private static Quaternion __rotationSaved=Quaternion.identity;


		internal struct ObjectInfo
		{
				public int uid;
				public MemberInfo[] memberInfosPrimitiveType;
				public object[] valuesPrimitiveTypes;
				public Dictionary<MemberInfo,int[]> membersInstanceIDs;
				public Dictionary<MemberInfo,SerializedObject[]> memberInfosSerializedObject;


			
		}

		public void OnReset ()
		{
				__objectInstanceMembers = new Dictionary<int, ObjectInfo> ();	
		}

		public bool HasBeenPreseved (int uid)
		{
				if (__objectInstanceMembers == null)
						return false;

				return __objectInstanceMembers.ContainsKey (uid);
		}

		public void preserve (int uid, System.Object obj, MemberInfo[] members=null)
		{
				if (!EditorApplication.isPlaying && !EditorApplication.isPaused) {
						Debug.LogError ("Preserve can be done only in Play/Pause mode");
				}


				if (__objectInstanceMembers == null) {

						__objectInstanceMembers = new Dictionary<int, ObjectInfo> ();
						
				}
			
				ObjectInfo objInfo;

				if (__objectInstanceMembers.ContainsKey (uid)) {
						objInfo = __objectInstanceMembers [uid];
						
				} else {

						objInfo = new ObjectInfo ();
						//objInfo.membersInstanceIDs = new Dictionary<MemberInfo, int[]> ();
						objInfo.memberInfosSerializedObject = new Dictionary<MemberInfo, SerializedObject[]> ();
				}

				objInfo.uid = uid;
				

				if (members == null)
						members = obj.GetType ().GetFields ();


				

				Type typeUnityObject = typeof(UnityEngine.Object);

				//remove UnityEngine.Object or Array of UnityEnigine.Object[]
				objInfo.memberInfosPrimitiveType = members.Select ((Item) => Item).Where ((item) => !item.GetUnderlyingType ().IsSubclassOf (typeUnityObject) && !(item.GetUnderlyingType ().IsArray && item.GetUnderlyingType ().GetElementType ().IsSubclassOf (typeUnityObject))).ToArray ();


				//
				MemberInfo[] unityObjectsMemberInfo = members.Select ((Item) => Item).Where ((item) => item.GetUnderlyingType ().IsSubclassOf (typeUnityObject) || (item.GetUnderlyingType ().IsArray && (item.GetUnderlyingType ().GetElementType ().IsSubclassOf (typeUnityObject)))).ToArray ();
				
				MemberInfo memberInfoCurrent;
				

				if (unityObjectsMemberInfo.Length > 0) {
						for (int i=0; i<unityObjectsMemberInfo.Length; i++) {
								memberInfoCurrent = unityObjectsMemberInfo [i];

								//if memberInfoCurrent is info for array of scriptableObject
								if (memberInfoCurrent.GetUnderlyingType ().IsArray) {

										UnityEngine.Object[] unityObjects = (UnityEngine.Object[])memberInfoCurrent.GetValue (obj);

										SerializedObject[] serializedObjects = new SerializedObject[unityObjects.Length];
									
										for (int scriptableObjectsInx=0; scriptableObjectsInx<unityObjects.Length; scriptableObjectsInx++) {	

												serializedObjects [scriptableObjectsInx] = new SerializedObject (unityObjects [scriptableObjectsInx]);
										}

										objInfo.memberInfosSerializedObject [memberInfoCurrent] = serializedObjects;

								} else {
										
										objInfo.memberInfosSerializedObject [memberInfoCurrent] = new SerializedObject[]{ new SerializedObject ((UnityEngine.Object)memberInfoCurrent.GetValue (obj))};

								}
						}
				}



				//get values from Members of Obj
				objInfo.valuesPrimitiveTypes = FormatterServices.GetObjectData (obj, objInfo.memberInfosPrimitiveType);		

				__objectInstanceMembers [uid] = objInfo;

				

		}

		public void remove (int id)
		{

				// destroy object and its footprint in .asset
				UnityEngine.Object.DestroyImmediate (EditorUtility.InstanceIDToObject (id), true);
	


		}

		public static void copySerialized (SerializedObject source, SerializedObject dest)
		{

				SerializedProperty serializedPropertyCurrent;
			
				serializedPropertyCurrent = source.GetIterator ();
			
			
			
				while (serializedPropertyCurrent.Next(true)) {
				
						dest.CopyFromSerializedProperty (serializedPropertyCurrent);
				}
			
				dest.ApplyModifiedProperties ();

		}

		public void restore (int uid, System.Object obj)
		{
				if (EditorApplication.isPlaying || EditorApplication.isPaused) {
						Debug.LogError ("Restore can be done only in Editor mode");
				}

				if (__objectInstanceMembers != null && __objectInstanceMembers.ContainsKey (uid)) {
						ObjectInfo objInfo = __objectInstanceMembers [uid];

						//restore values of normal members(not ScriptableObjects)
						FormatterServices.PopulateObjectMembers (obj, objInfo.memberInfosPrimitiveType, objInfo.valuesPrimitiveTypes);

						SerializedObject serializedObjectCurrent;
						SerializedObject serializedObjectSaved;
						UnityEngine.Object scriptabledObjectCurrent;
						foreach (var memberInfoInstanceIDPair in objInfo.memberInfosSerializedObject) {
								
								
								if (memberInfoInstanceIDPair.Key.GetUnderlyingType ().IsArray) {

										UnityEngine.Object[] scriptableObjects = (UnityEngine.Object[])memberInfoInstanceIDPair.Key.GetValue (obj);
									
										SerializedObject[] serializedObjects = memberInfoInstanceIDPair.Value;
										
										var listType = typeof(List<>);
										var typeElement = scriptableObjects.GetType ().GetElementType ();
										var concreteType = listType.MakeGenericType (typeElement);
										IList scriptableObjectList = (IList)Activator.CreateInstance (concreteType);

									
										for (int serializedObjectsInx=0; serializedObjectsInx<serializedObjects.Length; serializedObjectsInx++) {	

												serializedObjectSaved = serializedObjects [serializedObjectsInx];
										
												if (serializedObjectsInx < scriptableObjects.Length) {
														scriptabledObjectCurrent = scriptableObjects [serializedObjectsInx];

														
												} else {//if new ScriptableObject is added during Preserve
														if (typeElement is ScriptableObject)
														//create
																scriptabledObjectCurrent = ScriptableObject.CreateInstance (typeElement);
														else
																scriptabledObjectCurrent = (UnityEngine.Object)Activator.CreateInstance (typeElement);//Some types should be handled manually
														
														
												}

												//get current scriptable objecta and serialized it
												serializedObjectCurrent = new SerializedObject (scriptabledObjectCurrent);
													
												//copy saved to current
												//TODO try
												//EditorUtility.CopySerialized(
												copySerialized (serializedObjectSaved, serializedObjectCurrent);
													
												//keep current in list
												scriptableObjectList.Add (scriptabledObjectCurrent);

									
					
										}


										//create and copy list into array
										Array scriptableObjectArray = Array.CreateInstance (typeElement, scriptableObjectList.Count);
										scriptableObjectList.CopyTo (scriptableObjectArray, 0);

										//set new array
										memberInfoInstanceIDPair.Key.SetValue (obj, scriptableObjectArray);

								} else {
										serializedObjectSaved = memberInfoInstanceIDPair.Value [0];
										serializedObjectCurrent = new SerializedObject ((UnityEngine.Object)memberInfoInstanceIDPair.Key.GetValue (obj));
								
										copySerialized (serializedObjectSaved, serializedObjectCurrent);
								}
								
						}



						objInfo.memberInfosSerializedObject.Clear ();

						__objectInstanceMembers.Remove (uid);


						
				}



				
		}

		public static void PasteTransform (ref Transform transformRoot)
		{
			transformRoot.position = __positionSaved;
			transformRoot.rotation = __rotationSaved;
		}

		public static void CopyTransform (Transform transformRoot)
		{
			__positionSaved = transformRoot.position;
			__rotationSaved = transformRoot.rotation;
		}

}
