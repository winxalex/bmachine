using BehaviourMachine;
using System;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using BehaviourMachineEditor;
using UnityEditorInternal;
using System.Linq;
using ws.winx.unity;

namespace ws.winx.editor.bmachine.extensions
{
		[CustomPropertyDrawer (typeof(UnityVariablePropertyAttribute))]
		public class UnityVariableNodePropertyDrawer : PropertyDrawer
		{

			

				public new UnityVariablePropertyAttribute attribute { 
						get{ return (UnityVariablePropertyAttribute)attribute;}
				}


				private void OnEnable() {

						
				}
				


				void onDrawElement(Rect rect, int index, bool isActive, bool isFocused){
					EditorGUI.LabelField (rect, "List is Empty");
				}
		
		

		
				//
				// Methods
				//
//				private void DrawVariables ()
//				{
//					float height = BlackboardGUIUtility.GetHeight (this.m_Blackboard);
//					bool flag = height == 0f;
//					Rect rect = GUILayoutUtility.GetRect (10f, ((!flag) ? height : 21f) + 7f, new GUILayoutOption[]
//					                                      {
//						GUILayout.ExpandWidth (true)
//					});
//					rect.set_xMin (rect.get_xMin () + 4f);
//					rect.set_xMax (rect.get_xMax () - 4f);
//					if (Event.get_current ().get_type () == 7)
//					{
//						VariableEditor.s_Styles.boxBackground.Draw (rect, false, false, false, false);
//					}
//					rect.set_yMin (rect.get_yMin () + 2f);
//					rect.set_yMax (rect.get_yMax () - 3f);
//					Rect rect2 = rect;
//					if (flag)
//					{
//						rect2.set_height (21f);
//						rect2.set_x (rect2.get_x () + 6f);
//						EditorGUI.LabelField (rect2, "List is Empty");
//					}
//					else
//					{
//						BlackboardGUIUtility.DrawVariables (rect2, this.m_Blackboard);
//					}
//				}





		       public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
					base.OnGUI (position, property, label);
				}


//				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
//				{
//
//						attribute.serializedObject = property.serializedNode;
//					
//
//
//					
//
//		
//						property.ApplyModifiedValue ();
//
//				}
		

		}
}
