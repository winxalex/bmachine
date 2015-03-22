using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ws.winx.editor.drawers
{
	public class UnityDefaultPropertyDrawer:PropertyDrawer
	{

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField (position, property, label,true);

		}	
	}
}

