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
using UnityEditor;



namespace ws.winx.unity.utilities{

	public class GameObjectUtilityEx
	{
	
		public static List<T> FindAllContainComponentOfType<T> () where T : Component
		{

			T[] comps = Resources.FindObjectsOfTypeAll (typeof(T)) as T[];
			List<T> list = new List<T> ();
			foreach (T comp in comps) {
				if (comp.gameObject.hideFlags == 0) {
					string path = AssetDatabase.GetAssetPath (comp.gameObject);
					if (string.IsNullOrEmpty (path))
						list.Add (comp);
				}
			}
			return list;
		}




	
		




	}






}

