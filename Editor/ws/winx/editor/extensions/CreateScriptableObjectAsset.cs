using UnityEngine;
using UnityEditor;
using System.IO;

namespace ws.winx.editor.extensions
{

public static class ScriptableObjectUtility
{
	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		UnityEditor.EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}


	//TODO make editor and use upgrde version 2 form wiki

public class YourClassAsset
{
	[MenuItem("Assets/Create/YourClass")]
	public static void CreateAsset ()
	{
			//ScriptableObjectUtility.CreateAsset<BehaviourMachine.MecanimBlendParameterCustom> ();
	}
}

}
