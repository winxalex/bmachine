using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace ws.winx.editor
{




	#region GameObjectExtensions
	public static class GameObjectExtensions
	{
		public static string GetPath(this GameObject obj)
		{
			string path = "/" + obj.name;
			while (obj.transform.parent != null)
			{
				obj = obj.transform.parent.gameObject;
				path = "/" + obj.name + path;
			}
			return path;
		}

		/// <summary>
		/// </summary>
		public static GameObject InstantiateChild (this GameObject parent, GameObject prototype, bool preserveScale = false)
		{
			var child = UnityEngine.Object.Instantiate (prototype) as GameObject;
			var rotCache = child.transform.rotation;
			var scaleCache = child.transform.localScale;
			child.transform.position = parent.transform.position;
			child.transform.parent = parent.transform;
			if (!preserveScale) child.transform.localScale = scaleCache;
			child.transform.localRotation = rotCache;
			return child;
		}


	}
	#endregion


	
}
