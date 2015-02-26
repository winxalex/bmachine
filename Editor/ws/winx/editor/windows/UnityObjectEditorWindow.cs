using UnityEditor;
using System.Collections;
using ws.winx.unity;
using System.Reflection;
using ws.winx.csharp.extensions;
using ws.winx.csharp.utilities;
using UnityEngine;

namespace ws.winx.editor.windows
{
		public class UnityObjectEditorWindow : EditorWindow
		{
				private static UnityVariable __variable;
				
				private static UnityVariable[] __properties;

				public static void Show (UnityVariable variable)
				{
						EditorWindow.GetWindow<UnityObjectEditorWindow> ();

						__variable = variable;

						MemberInfo[] memberInfos = ReflectionUtility.GetPublicMembers (__variable.ValueType, typeof(System.Object), false, true, true, true);

			UnityVariable cvariable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();

						int len = memberInfos.Length;
						__properties=new UnityVariable[len];
						for (int i=0; i<len; i++) {
								cvariable.MemberInfo=memberInfos[i];
								cvariable.reflectedInstance=variable.reflectedInstance;
							
						}
						

				}

				void OnGUI ()
				{

					if (__variable != null && __properties!=null && __properties.Length) {// || __variable.ValueType.IsSubclassOf()
								//find property Object Drawer or use default
								Rect combinedPosition = new Rect (10, 10, 10, 25);
								float height = combinedPosition.height = 21f;
							
								

				
								for (int i=0; i<__properties.Length; i++) {
					BlackboardCustomEditor
										switchDrawerDelegate = switchDrawer (__properties [i].ValueType);
										if (switchDrawerDelegate != null) {
																
				
												
												height = switchDrawerDelegate (position, __properties[i]).height;
												combinedPosition.height += height;
												position.y += height;
												position.x = combinedPosition.x;
												//	position.height=21f;
				
										}
			
								}
						}
				}
			

		}
}

