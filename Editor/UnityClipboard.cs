using UnityEngine;
using System.Collections;
using UnityEditor;
using BehaviourMachineEditor;
using ws.winx.unity;
using System.Collections.Generic;

public class UnityClipboard : ScriptableObject {



	public List<int> itemIDs;
	public List<int> assetIDs;

	public void OnReset(){
		itemIDs=new List<int> ();
		 assetIDs=new List<int>();
	}

	public object restore (int id)
	{
		if(EditorApplication.isPlaying || EditorApplication.isPaused){
			Debug.LogError("resore can be done only in Editor mode");
		}

		if (itemIDs.Contains (id)) {
			int assetID=assetIDs[itemIDs[id]];
			UnityVariable variable=(UnityVariable)EditorUtility.InstanceIDToObject(assetID);
			variable.OnAfterDeserialize();
			return variable.Value;
		}

		return null;
	}

	public void add(int id,System.Object value){

		if (!itemIDs.Contains (id)) {
						itemIDs.Add (id);


						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						variable.Value = value;

						EditorUtility.SetDirty (variable);
						assetIDs.Add (variable.GetInstanceID ());

						AssetDatabase.AddObjectToAsset (variable, this);

						AssetDatabase.SaveAssets ();
				} else {
					int assetID=assetIDs[itemIDs[id]];
					UnityVariable variable=(UnityVariable)EditorUtility.InstanceIDToObject(assetID);
			variable.Value=value;
					variable.OnBeforeSerialize();
			EditorUtility.SetDirty (variable);
			AssetDatabase.SaveAssets ();

				}

	}
}
