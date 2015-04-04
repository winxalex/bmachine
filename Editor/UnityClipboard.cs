using UnityEngine;
using System.Collections;
using UnityEditor;
using BehaviourMachineEditor;
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



		

		Dictionary<int,ObjectInfo> __objectInstanceMembers;


		internal struct ObjectInfo
		{
				public int uid;
				public MemberInfo[] members;
				public object[] values;
				public Dictionary<MemberInfo,int[]> membersInstanceIDs;
				public Dictionary<MemberInfo,SerializedObject[]> memberInfoSerializedObject;


				
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
						objInfo.membersInstanceIDs = new Dictionary<MemberInfo, int[]> ();
						objInfo.memberInfoSerializedObject = new Dictionary<MemberInfo, SerializedObject[]> ();
				}

				objInfo.uid = uid;
				

				if (members == null)
						members = obj.GetType ().GetFields ();


				//remove ScriptableObjects or Array of ScirptableObjects
				objInfo.members = members.Select ((Item) => Item).Where ((item) => !item.GetUnderlyingType ().IsSubclassOf (typeof(ScriptableObject)) && !(item.GetUnderlyingType ().IsArray && item.GetUnderlyingType ().GetElementType ().IsSubclassOf (typeof(ScriptableObject)))).ToArray ();


				
				MemberInfo[] scriptableObjectsMemberInfo = members.Select ((Item) => Item).Where ((item) => item.GetUnderlyingType ().IsSubclassOf (typeof(ScriptableObject)) || (item.GetUnderlyingType ().IsArray && (item.GetUnderlyingType ().GetElementType ().IsSubclassOf (typeof(ScriptableObject))))).ToArray ();
				ScriptableObject scriptableObjectCurrent;
				MemberInfo memberInfoCurrent;
				ScriptableObject scriptableObjectClone;

				if (scriptableObjectsMemberInfo.Length > 0) {
						for (int i=0; i<scriptableObjectsMemberInfo.Length; i++) {
								memberInfoCurrent = scriptableObjectsMemberInfo [i];

								//if memberInfoCurrent is info for array of scriptableObject
								if (memberInfoCurrent.GetUnderlyingType ().IsArray) {

										UnityEngine.ScriptableObject[] scriptableObjects = (UnityEngine.ScriptableObject[])memberInfoCurrent.GetValue (obj);

										SerializedObject[] serializedObjects = new SerializedObject[scriptableObjects.Length];
									
										for (int scriptableObjectsInx=0; scriptableObjectsInx<scriptableObjects.Length; scriptableObjectsInx++) {	

												serializedObjects [scriptableObjectsInx] = new SerializedObject (scriptableObjects [scriptableObjectsInx]);
										}

										objInfo.memberInfoSerializedObject [memberInfoCurrent] = serializedObjects;

								} else {
										
										objInfo.memberInfoSerializedObject [memberInfoCurrent] = new SerializedObject[]{ new SerializedObject ((ScriptableObject)memberInfoCurrent.GetValue (obj))};

								}
						}
				}



				//clean .asset from any left
				foreach (var memberInfoInstanceIDPair in objInfo.membersInstanceIDs) {
						memberInfoCurrent = memberInfoInstanceIDPair.Key;
						//remove previous clones from .asset
						
						int[] IDs = objInfo.membersInstanceIDs [memberInfoCurrent];
						
						// destroy object and its footprint in .asset
						for (int k=0; k<IDs.Length; k++)
								UnityEngine.Object.DestroyImmediate (EditorUtility.InstanceIDToObject (IDs [k]), true);
						
				}
		
		
//		if (scriptableObjectsMemberInfo.Length > 0) {
//						for (int i=0; i<scriptableObjectsMemberInfo.Length; i++) {
//								memberInfoCurrent = scriptableObjectsMemberInfo [i];
//								if (memberInfoCurrent.GetUnderlyingType ().IsArray) {
//										IList so = (IList)memberInfoCurrent.GetValue (obj);
//										List<int> instanceIDs = new List<int> ();
//										for (int j = 0; j < so.Count; j++) {
//												scriptableObjectCurrent = so [j] as ScriptableObject;
//												if (scriptableObjectCurrent != null) {
//				
//															
//														scriptableObjectClone = UnityEngine.Object.Instantiate (scriptableObjectCurrent);
//														//save instanceIDs so can be used whild restore fro accesing cloned
//														instanceIDs.Add (scriptableObjectClone.GetInstanceID ());
//							
//														//save to .asset
//														AssetDatabase.AddObjectToAsset (scriptableObjectClone, this);
//												}
//										}
//
//					
//								if (instanceIDs.Count > 0){
//												objInfo.membersInstanceIDs [memberInfoCurrent] = instanceIDs.ToArray ();
//										}
//
//								} else {
//										scriptableObjectCurrent = memberInfoCurrent.GetValue (obj) as ScriptableObject;
//										
//										if (scriptableObjectCurrent != null) {
//
//
//												scriptableObjectClone = UnityEngine.Object.Instantiate (scriptableObjectCurrent);
//
//												//save cloned SO id's
//												objInfo.membersInstanceIDs [memberInfoCurrent] = new int[]{  scriptableObjectClone.GetInstanceID ()};
//											
//												//save to .asset
//												AssetDatabase.AddObjectToAsset (scriptableObjectClone, this);
//					//	AssetDatabase.AddObjectToAsset (((UnityVariable)scriptableObjectClone).__reflectedInstanceUnity, this);
//										}
//					
//
//								}
//						}
//
//						AssetDatabase.SaveAssets ();
//						
//				}


				//get values from Members of Obj
				objInfo.values = FormatterServices.GetObjectData (obj, objInfo.members);		

				__objectInstanceMembers [uid] = objInfo;

				

		}

		public void remove (int id)
		{

			// destroy object and its footprint in .asset
			UnityEngine.Object.DestroyImmediate (EditorUtility.InstanceIDToObject(id), true);
	


		}

		void copySerialized (SerializedObject source, SerializedObject dest)
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
						FormatterServices.PopulateObjectMembers (obj, objInfo.members, objInfo.values);

						SerializedObject serializedObjectCurrent;
						SerializedObject serializedObjectSaved;
						ScriptableObject scriptabledObjectCurrent;
						foreach (var memberInfoInstanceIDPair in objInfo.memberInfoSerializedObject) {
								
								
								if (memberInfoInstanceIDPair.Key.GetUnderlyingType ().IsArray) {

										UnityEngine.ScriptableObject[] scriptableObjects = (UnityEngine.ScriptableObject[])memberInfoInstanceIDPair.Key.GetValue (obj);
									
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

														//create
														scriptabledObjectCurrent = ScriptableObject.CreateInstance (typeElement);
														
												}

												//get current scriptable objecta and serialized it
												serializedObjectCurrent = new SerializedObject (scriptabledObjectCurrent);
													
												//copy saved to current
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


//			ScriptableObject scriptableObjectSaved;
//			ScriptableObject scriptableObjectClone;
//						foreach (var memberInfoInstanceIDPair in objInfo.membersInstanceIDs) {
//								
//								if (memberInfoInstanceIDPair.Key.GetUnderlyingType ().IsArray) {
//										IList scriptableObjectArray = (IList)memberInfoInstanceIDPair.Key.GetValue (obj);
//
//									
//				
//										
//										for (int j = 0; j < scriptableObjectArray.Count; j++) {
//												
//												
//										//get saved object from .asset
//										scriptableObjectSaved = (ScriptableObject)EditorUtility.InstanceIDToObject (memberInfoInstanceIDPair.Value [j]);
//										
//										//clone
//										scriptableObjectClone = UnityEngine.Object.Instantiate (scriptableObjectSaved);
//										
//										//apply
//										scriptableObjectArray[j]=scriptableObjectClone;
//							
//										// destroy object and its footprint in .asset
//										UnityEngine.Object.DestroyImmediate (scriptableObjectSaved, true);
//										
//										
//										
//									}
//								} else {
//										
//										//get saved object
//										scriptableObjectSaved = (ScriptableObject)EditorUtility.InstanceIDToObject (memberInfoInstanceIDPair.Value [0]);
//				
//										//clone
//										 scriptableObjectClone = UnityEngine.Object.Instantiate (scriptableObjectSaved);
//
//										//apply
//										memberInfoInstanceIDPair.Key.SetValue (obj, scriptableObjectClone);
//
//										// destroy object and its footprint in .asset
//										UnityEngine.Object.DestroyImmediate (scriptableObjectSaved, true);
//
//	
//
//								}
//
//							
//						}
//
//						AssetDatabase.SaveAssets ();
//
//						//reimport
//						AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (this.GetInstanceID ()));

						objInfo.membersInstanceIDs.Clear ();

						__objectInstanceMembers.Remove (uid);


						
				}



				
		}


}
