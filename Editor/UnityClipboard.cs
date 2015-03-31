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



		public List<int> itemIDs;
		public List<int> assetIDs;
		//public List<int,> properties;

		Dictionary<int,ObjectInfo> __objectInstanceMembers;


		internal struct ObjectInfo
		{
				public int uid;
				public MemberInfo[] members;
				public object[] values;
				


		}

		public void OnReset ()
		{
				itemIDs = new List<int> ();
				assetIDs = new List<int> ();
		}

		public void clear ()
		{

				//AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(
		}

		public bool HasBeenPreseved(int uid){
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

				if (__objectInstanceMembers.ContainsKey (uid))
						objInfo = __objectInstanceMembers [uid];
				else {

						objInfo = new ObjectInfo ();
						
				}

				objInfo.uid = uid;
				

				if (members == null)
						members = obj.GetType ().GetFields ();


		//remove ScriptableObjects
		members = members.Select ((Item) => Item).Where ((item) =>   !item.GetUnderlyingType ().IsSubclassOf(typeof(ScriptableObject)) &&  !(item.GetUnderlyingType().IsArray && item.GetUnderlyingType ().GetElementType().IsSubclassOf(typeof(ScriptableObject)))).ToArray ();

				
				objInfo.members = members;




				objInfo.values = FormatterServices.GetObjectData (obj, objInfo.members);		

				__objectInstanceMembers [uid] = objInfo;

				

		}

		public void remove (int id)
		{




		}

		public object restore (int uid, System.Object obj)
		{
				if (EditorApplication.isPlaying || EditorApplication.isPaused) {
						Debug.LogError ("Restore can be done only in Editor mode");
				}

				if (__objectInstanceMembers != null && __objectInstanceMembers.ContainsKey (uid)) {
						ObjectInfo objInfo = __objectInstanceMembers [uid];
						FormatterServices.PopulateObjectMembers (obj, objInfo.members, objInfo.values);
						__objectInstanceMembers.Remove (uid);
				}

//		if (itemIDs.Contains (id)) {
//			int assetID=assetIDs[itemIDs[id]];
//			UnityVariable variable=(UnityVariable)EditorUtility.InstanceIDToObject(assetID);
//			variable.OnAfterDeserialize();
//			return variable.Value;
//		}

				return null;
		}

//	public void add(int id,System.Object value){
//
//		if (!itemIDs.Contains (id)) {
//						itemIDs.Add (id);
//
//
//						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
//						variable.Value = value;
//
//						EditorUtility.SetDirty (variable);
//						assetIDs.Add (variable.GetInstanceID ());
//
//						AssetDatabase.AddObjectToAsset (variable, this);
//
//						AssetDatabase.SaveAssets ();
//				} else {
//					int assetID=assetIDs[itemIDs[id]];
//					UnityVariable variable=(UnityVariable)EditorUtility.InstanceIDToObject(assetID);
//			variable.Value=value;
//					variable.OnBeforeSerialize();
//			EditorUtility.SetDirty (variable);
//			AssetDatabase.SaveAssets ();
//
//				}
//
//	}
}
