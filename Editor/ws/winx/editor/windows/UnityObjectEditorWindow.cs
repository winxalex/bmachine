using UnityEditor;
using System;
using System.Collections;
using ws.winx.unity;
using System.Reflection;
using ws.winx.csharp.extensions;
using ws.winx.csharp.utilities;
using UnityEngine;
using ws.winx.editor.extensions;

namespace ws.winx.editor.windows
{
		public class UnityObjectEditorWindow : EditorWindow
		{
				[NonSerialized]
				private static UnityVariable __variable;

				[NonSerialized]
				private static UnityVariable[] __properties;
				private static System.Func<System.Type, EditorUtilityEx.SwitchDrawerDelegate> __switcher;

				public static void Show (UnityVariable variable)
				{

						if (variable.Value == null || variable.ValueType==typeof(UnityEngine.Object))
								return;

						EditorWindow.GetWindow<UnityObjectEditorWindow> ();

						__variable = variable;



						MemberInfo[] memberInfos = ReflectionUtility.GetPublicMembers (__variable.ValueType, typeof(System.Object), false, true, true, true);

						UnityVariable cvariable;
						int len = memberInfos.Length;

						if (__properties != null)
								Array.Clear (__properties,0,__properties.Length);

						__properties = new UnityVariable[len];

						for (int i=0; i<len; i++) {
								cvariable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
								cvariable.serializable=false;
								cvariable.MemberInfo = memberInfos [i];
								cvariable.reflectedInstance = variable.reflectedInstance;
								__properties [i] = cvariable;
						}


						__switcher = EditorUtilityEx.GetDefaultSwitchDrawer ();
						

				}

				void OnGUI ()
				{

						if (__variable != null && __properties != null && __properties.Length > 0) {// || __variable.ValueType.IsSubclassOf()
								//find property Object Drawer or use default
								Rect combinedPosition = new Rect (10, 10, 10, 25);
								float height = 0f;
							
								EditorUtilityEx.SwitchDrawerDelegate switchDrawerDelegate;

								int len = __properties.Length;
								for (int i=0; i<len; i++) {

										
										switchDrawerDelegate = __switcher (__properties [i].ValueType);



										if (switchDrawerDelegate != null) {
										
												height = switchDrawerDelegate (combinedPosition, __properties [i]).height;
												
												combinedPosition.y += height;
											
				
										}
			
								}
						}
				}
			

		}
}

