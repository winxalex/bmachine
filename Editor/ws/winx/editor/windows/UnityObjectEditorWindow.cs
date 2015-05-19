using UnityEditor;
using System;
using System.Collections;
using ws.winx.unity;
using System.Reflection;
using ws.winx.csharp.extensions;
using ws.winx.csharp.utilities;
using UnityEngine;
using ws.winx.editor.extensions;
using UnityEngine.Events;
using UnityEditorInternal;
using System.Runtime.Serialization;
using System.CodeDom.Compiler;
using ws.winx.editor.utilities;

namespace ws.winx.editor.windows
{
		public class UnityObjectEditorWindow : EditorWindow
		{
				[NonSerialized]
				private static UnityVariable
						__variable;
				[NonSerialized]
				private static UnityVariable[]
						__properties;

				public static void Show (UnityVariable variable)
				{

						if (variable.Value == null || variable.ValueType == typeof(UnityEngine.Object))
								return;

						EditorWindow.GetWindow<UnityObjectEditorWindow> ();

						__variable = variable;


				}

				void OnGUI ()
				{




						if (__variable != null) {
								PropertyDrawer drawer;


								if (__variable.ValueType == typeof(UnityEvent)) {
										if (__variable.drawer == null)
												__variable.drawer = new UnityEventDrawer ();
										
												drawer = __variable.drawer;
								}
								else
									drawer = EditorUtilityEx.GetDrawer (__variable.ValueType);

				
								Rect pos = new Rect (16, 16, Screen.width - 32, Screen.height - 32);


								drawer.OnGUI (pos, __variable.serializedProperty, new GUIContent (__variable.name));

								__variable.ApplyModifiedProperties ();
						}
			


				}
			

		}
}

