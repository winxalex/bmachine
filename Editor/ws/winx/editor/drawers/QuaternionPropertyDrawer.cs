using UnityEngine;
using UnityEditor;
using System.Collections;
using ws.winx.unity.attributes;

namespace ws.winx.editor.drawers
{
	[CustomPropertyDrawer (typeof(QuaternionPropertyAttribute))]
	[CustomPropertyDrawer (typeof(Quaternion))]
	public class QuaternionPropertyDrawer:PropertyDrawer
	{

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			Quaternion value = property.quaternionValue;
			Vector4 vector = new Vector4 (value.x, value.y, value.z, value.w);

			GUILayoutUtility.GetRect (position.width, 16f);
			EditorGUI.BeginChangeCheck ();
			Vector4 vector2 = EditorGUI.Vector4Field (position, string.Empty, vector);
			if (EditorGUI.EndChangeCheck () && vector != vector2) {
						property.quaternionValue=new Quaternion (vector2.x, vector2.y, vector2.z, vector2.w);
			}


		}	
	}
}

